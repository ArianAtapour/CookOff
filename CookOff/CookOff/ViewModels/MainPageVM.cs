using System;
using Android.SE.Omapi;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CookOff
{
    //Main Page View Model
    public class MainPageVM : INotifyPropertyChanged
    {
        private ObservableCollection<Recipe> _recipes;
        public ObservableCollection<Recipe> Recipes
        {
            get { return _recipes; }
            set
            {
                _recipes = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateToCreateRecipeCommand { get; private set; }

        public MainPageVM()
        {
            Recipes = new ObservableCollection<Recipe>();
            LoadRecipes();
            NavigateToCreateRecipeCommand = new Command(OnNavigateToCreateRecipe);
        }

        private void LoadRecipes()
        {
            var recipes = CsvDependecy.readRecipe();
            foreach (var recipe in recipes)
            {
                Recipes.Add(recipe);
            }
        }

        private async void OnNavigateToCreateRecipe()
        {
            await Shell.Current.GoToAsync("CreateRecipePage");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

