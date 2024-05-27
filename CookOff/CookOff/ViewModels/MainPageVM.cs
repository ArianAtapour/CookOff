using CookOff.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CookOff.ViewModels
{
    public class MainPageVM : INotifyPropertyChanged
    {
        private ObservableCollection<Recipe> _recipes;
        public ObservableCollection<Recipe> Recipes
        {
            get => _recipes;
            set
            {
                _recipes = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateToCreateRecipeCommand { get; }
        public ICommand NavigateToViewRecipeCommand { get; }

        public MainPageVM()
        {
            Recipes = new ObservableCollection<Recipe>();
            LoadRecipes();
            NavigateToCreateRecipeCommand = new Command(OnNavigateToCreateRecipe);
            NavigateToViewRecipeCommand = new Command(OnNavigateToViewRecipe);
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

        private async void OnNavigateToViewRecipe()
        {
            await Shell.Current.GoToAsync("ViewRecipe");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
