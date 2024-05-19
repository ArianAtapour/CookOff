using System;
using Android.SE.Omapi;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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
                _rating = value;
                OnPropertyChanged();
            }
        }

        private int _hours;
        public int Hours
        {
            get { return _hours; }
            set
            {
                _hours = value;
                OnPropertyChanged();
            }
        }

        private int _minutes;
        public int Minutes
        {
            get { return _minutes; }
            set
            {
                _minutes = value;
                OnPropertyChanged();
            }
        }

        private int _seconds;
        public int Seconds
        {
            get { return _seconds; }
            set
            {
                _seconds = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Step> _steps;
        public ObservableCollection<Step> Steps
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

        private int stepCount = 1;

        public CreateRecipeVM()
        {
            Steps = new ObservableCollection<Step>();
            AddStepCommand = new Command(OnAddStep);
            SubmitCommand = new Command(OnSubmit);
            BackCommand = new Command(OnBack);
        }

        private void OnAddStep()
        {
            var timer = new TimeSpan(Hours, Minutes, Seconds);
            Steps.Add(new Step($"Step {stepCount++}", timer != TimeSpan.Zero, timer));
            Hours = 0;
            Minutes = 0;
            Seconds = 0;
        }

        private async void OnSubmit()
        {
            var newRecipe = new Recipe(RecipeName, ImagePath, Rating);
            foreach (var step in Steps)
            {
                newRecipe.addStep(step);
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
