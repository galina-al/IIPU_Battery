using System;
using System.Windows.Forms;

namespace Battery
{
    public partial class Battery : Form
    {
        private readonly BatteryManager _manager = new BatteryManager();

        public Battery()
        {
            InitializeComponent();
        }

        //Method for initialization of states of app
        private void BatteryLoad(object sender, EventArgs e)
        {
            //Getting the state of time of the battery
            _manager.Init();
            if (_manager.charging == "Online") timeoutBox.Enabled = false;
            UpdateBattery(null, null);
            //Setting the event of timer
            UpdateTimer.Tick += UpdateBattery;
            UpdateTimer.Interval = 2000;
            UpdateTimer.Start();
            if (_manager.previousScreenTime > 300)
                _manager.previousScreenTime = 300;
            if (_manager.previousScreenTime == 0)
                _manager.previousScreenTime = 1;
            timeoutBox.SelectedIndex = timeoutBox.FindString(_manager.previousScreenTime.ToString());
            timeoutSeconds.Text = (Int32.Parse(_manager.previousScreenTime.ToString()) * 60).ToString();
            timeoutLabel.Text = "Время отключения дисплея " + timeoutBox.SelectedItem.ToString() + " минут.";
        }

        private void UpdateBattery(object sender, EventArgs e)
        {
            _manager.UpdateData();
            State.Text = _manager.charging;
            Percentage.Text = _manager.percentBattery;
            timeLeft.Text = _manager.workTime;
            //Checking if we need to unlock the dropdown
            if (_manager.previousCharging != State.Text)
            {
                if (_manager.previousCharging == "Offline")
                {
                    timeoutBox.Enabled = false;
                    timeoutSeconds.ReadOnly = true;
                }
                else
                {
                    timeoutBox.Enabled = true;
                    timeoutSeconds.ReadOnly = false;
                }
                _manager.previousCharging = State.Text;
            }
        }

        private void AppClosing(object sender, FormClosingEventArgs e)
        {
            timeoutBox.SelectedIndex = timeoutBox.FindString(_manager.previousScreenTime.ToString());
        }

        private void timeoutBox_ValueChanged(object sender, EventArgs e)
        {
            timeoutLabel.Text = "Время отключения дисплея " + timeoutBox.SelectedItem.ToString() + " минут.";
            timeoutSeconds.Text = (Int32.Parse(timeoutBox.SelectedItem.ToString()) * 60).ToString();
            _manager.SetDisplayOff(Int32.Parse(timeoutBox.SelectedItem.ToString()));
        }

        private void timeoutSeconds_ValueChanged(object sender, EventArgs e)
        {
            if (Int32.Parse(timeoutSeconds.Text) >= 60 && Int32.Parse(timeoutSeconds.Text) <= 18000)
            {
                timeoutLabel.Text = "Время отключения дисплея " + ((int)(Int32.Parse(timeoutSeconds.Text) / 60)).ToString() + " минут.";
                _manager.SetDisplayOff((int)(Int32.Parse(timeoutSeconds.Text) / 60));
            }
            else
            {
                timeoutLabel.Text = "Время отключения дисплея 1 минут.";
                timeoutSeconds.Text = "60";
                _manager.SetDisplayOff(1);
            }
        }
    }
}