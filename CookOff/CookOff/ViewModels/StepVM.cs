using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace CookOff.ViewModels
{
    public class StepVM : INotifyPropertyChanged
    {
        private string description;
        private bool timerRequired;
        private int hours;
        private int minutes;
        private int seconds;

        public StepVM(string description)
        {
            setDescription(description);
            TimerRequired = false;
            Hours = 0;
            Minutes = 0;
            Seconds = 0;
        }

        public void setDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("Description cannot be null or empty!");
            }
            else
            {
                this.description = description;
                OnPropertyChanged();
            }
        }

        public string getDescription()
        {
            return this.description;
        }

        public bool TimerRequired
        {
            get { return timerRequired; }
            set
            {
                timerRequired = value;
                OnPropertyChanged();
            }
        }

        public int Hours
        {
            get { return hours; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Hours cannot be negative!");
                }
                else
                {
                    hours = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Minutes
        {
            get { return minutes; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Minutes cannot be negative!");
                }
                else
                {
                    minutes = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Seconds
        {
            get { return seconds; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Seconds cannot be negative!");
                }
                else
                {
                    seconds = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

