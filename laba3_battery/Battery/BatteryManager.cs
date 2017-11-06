using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Battery
{
    public class BatteryManager
    {

        public int previousScreenTime { get; set; }
        public string charging { get; set; }
        public string percentBattery { get; set; }
        public string workTime { get; set; }
        public string previousCharging { get; set; }
        public bool startApp { get; set; }

        public void Init()
        {
            //Starting our app and get the state of time of the battery
            startApp = true;
            previousScreenTime = GetScreenTime();
            //Updating app
            UpdateData();
        }
        //method for getting current state of time
        public int GetScreenTime()
        {
            //Describing process for cmd
            var procCmd = new Process();
            procCmd.StartInfo.UseShellExecute = false;
            procCmd.StartInfo.RedirectStandardOutput = true;
            procCmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            procCmd.StartInfo.FileName = "cmd.exe";
            procCmd.StartInfo.Arguments = "/c powercfg /q";
            procCmd.Start();
            //From all output we need only VIDEOIDLE and last string
            var powerConfig = procCmd.StandardOutput.ReadToEnd();
            var lastString = new Regex("VIDEOIDLE.*\\n.*\\n.*\\n.*\\n.*\\n.*\\n.*");
            var videoIdle = lastString.Match(powerConfig).Value;
            //After that we need only 16 value
            var batteryState = videoIdle.Substring(videoIdle.Length - 11).TrimEnd();
            //Convert string value to Int
            return Convert.ToInt32(batteryState, 16) / 60;
        }
        public void UpdateData()
        {
            //Getting the current state of power
            charging = SystemInformation.PowerStatus.PowerLineStatus.ToString();
            //If we start our app, then initialize it one time
            if (startApp)
            {
                previousCharging = charging;
                startApp = false;
            }
            //Getting the current percent of battery
            percentBattery = SystemInformation.PowerStatus.BatteryLifePercent * 100 + "%";
            //If we dont use AC adapter
            if (charging == "Offline")
            {
                //Then we can start calculating lifetime
                var calcLife = SystemInformation.PowerStatus.BatteryLifeRemaining;
                //If we dont get the lifetime, we continue calculating
                if (calcLife != -1)
                {
                    //Or initialize variable for storing it in formatted way
                    var span = new TimeSpan(0, 0, calcLife);
                    workTime = span.ToString("g");
                }
                else workTime = "Подсчет времени...";
            }
            else
            {
                workTime = "Устройство работает от сети.";
            }
        }

        //Method for changing time in system directly
        public void SetDisplayOff(int value)
        {
            //Describing process for cmd
            var p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.Arguments = "/c powercfg /x -monitor-timeout-dc " + value;
            p.Start();
        }
    }
}