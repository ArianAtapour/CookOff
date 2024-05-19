using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace CookOff.ViewModels
{
    public class CreateRecipeVM : INotifyPropertyChanged
    {
        private string _recipeName;
        public string RecipeName
        {
            get { return _recipeName; }
            set
            {
                _recipeName = value;
                OnPropertyChanged();
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        private int _rating;
        public int Rating
        {
            get { return _rating; }
            set
            {
                if (value >= 1)
                {
                    _rating = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<int> Ratings { get; } = new ObservableCollection<int> { 1, 2, 3, 4, 5 };

        private ObservableCollection<StepViewModel> _steps;
        public ObservableCollection<StepViewModel> Steps
        {
            get { return _steps; }
            set
            {
                _steps = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddStepCommand { get; private set; }
        public ICommand SubmitCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand UploadImageCommand { get; private set; }

        private int stepCount = 1;

        public CreateRecipeVM()
        {
            Steps = new ObservableCollection<StepViewModel>();
            AddStepCommand = new Command(OnAddStep);
            SubmitCommand = new Command(OnSubmit);
            BackCommand = new Command(OnBack);
            UploadImageCommand = new Command(async () => await OnUploadImage());
        }

        private void OnAddStep()
        {
            Steps.Add(new StepViewModel($"Step {stepCount++}"));
        }

        private async void OnSubmit()
        {
            var newRecipe = new Recipe(RecipeName, ImagePath, Rating);
            foreach (var stepViewModel in Steps)
            {
                var timer = new TimeSpan(stepViewModel.Hours, stepViewModel.Minutes, stepViewModel.Seconds);
                newRecipe.addStep(new Step(stepViewModel.Name, stepViewModel.TimerRequired, timer));
            }

            var recipes = CsvDependecy.readRecipe();
            recipes.Add(newRecipe);
            CsvDependecy.writeRecipe(recipes);

            await Shell.Current.GoToAsync("..");
        }

        private async void OnBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async Task OnUploadImage()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Please select an image file",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    ImagePath = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions, such as the user cancelling the file picker
                Console.WriteLine($"File picking error: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class StepViewModel : INotifyPropertyChanged
    {
        private string _name;
        private bool _timerRequired;
        private int _hours;
        private int _minutes;
        private int _seconds;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public bool TimerRequired
        {
            get { return _timerRequired; }
            set
            {
                _timerRequired = value;
                OnPropertyChanged();
            }
        }

        public int Hours
        {
            get { return _hours; }
            set
            {
                _hours = value;
                OnPropertyChanged();
            }
        }

        public int Minutes
        {
            get { return _minutes; }
            set
            {
                _minutes = value;
                OnPropertyChanged();
            }
        }

        public int Seconds
        {
            get { return _seconds; }
            set
            {
                _seconds = value;
                OnPropertyChanged();
            }
        }

        public StepViewModel(string name)
        {
            Name = name;
            TimerRequired = false;
            Hours = 0;
            Minutes = 0;
            Seconds = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
