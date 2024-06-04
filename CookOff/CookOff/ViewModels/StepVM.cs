using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CookOff.ViewModels
{
    public class StepVM : INotifyPropertyChanged
    {
        private string name;
        private string description;
        private bool timerRequired;
        private int hours;
        private int minutes;
        private int seconds;

        public StepVM(string name)
        {
            this.name = name;
            Description = string.Empty;
            TimerRequired = false;
            Hours = 0;
            Minutes = 0;
            Seconds = 0;
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }

        public bool TimerRequired
        {
            get => timerRequired;
            set
            {
                timerRequired = value;
                OnPropertyChanged();
            }
        }

        public int Hours
        {
            get => hours;
            set
            {
                hours = value;
                OnPropertyChanged();
            }
        }

        public int Minutes
        {
            get => minutes;
            set
            {
                minutes = value;
                OnPropertyChanged();
            }
        }

        public int Seconds
        {
            get => seconds;
            set
            {
                seconds = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
