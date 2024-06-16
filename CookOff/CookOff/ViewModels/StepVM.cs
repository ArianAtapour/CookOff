using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CookOff.ViewModels
{
    public class StepVM : INotifyPropertyChanged
    {
        //Step View Model

        //Fields
        private string name;
        private string description;
        private bool timerRequired;
        private string hours;
        private string minutes;
        private string seconds;
        public ICommand StepShowHelpCommand { get; private set; }

        //Constructor
        public StepVM(string name)
        {
            this.name = name;
            Description = string.Empty;
            TimerRequired = false;
            Hours = string.Empty;
            Minutes = string.Empty;
            Seconds = string.Empty;
            StepShowHelpCommand = new Command(OnStepShowHelp);
        }

        //Methods
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

        public string Hours
        {
            get => hours;
            set
            {
                hours = value;
                OnPropertyChanged();
            }
        }

        public string Minutes
        {
            get => minutes;
            set
            {
                minutes = value;
                OnPropertyChanged();
            }
        }

        public string Seconds
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

        private async void OnStepShowHelp()
        {
            //Display a message indicating the purpose of each step
            await App.Current.MainPage.DisplayAlert("Help", "This is where you can describe the current step in detail.", "OK");
        }

        private string GetProjectDirectory()
        {
            //Assuming the application runs from the bin directory, we can navigate up to the project directory
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectDir = Directory.GetParent(currentDir).Parent.Parent.Parent.Parent.Parent.FullName;
            return projectDir;
        }

        private string GetImagePath()
        {
            //Ensure the images directory exists in the project directory
            string projectDirectory = GetProjectDirectory();
            string imagesDirectory = Path.Combine(projectDirectory, "images");
            string imageName = "Help.jpg"; // Change this to match your image file name
            return Path.Combine(imagesDirectory, imageName);
        }

        public string HelpImageSource => GetImagePath();
    }
}
