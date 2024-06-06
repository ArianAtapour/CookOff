using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CookOff.Models;
using Microsoft.Maui.Controls;

namespace CookOff.ViewModels
{
    [QueryProperty(nameof(Recipe), "SelectedRecipe")]
    public class RecipePageVM : INotifyPropertyChanged
    {
        private Recipe recipe;
        private int selectedIngredientsCount;
        private int selectedStepsCount;

        public Recipe Recipe
        {
            get => recipe;
            set
            {
                recipe = value;
                OnPropertyChanged();
                UpdateCounts();
            }
        }

        public string IngredientsCountLabel => $"Ingredients: {selectedIngredientsCount}/{Recipe?.Ingredients.Count}";
        public string StepsCountLabel => $"Steps: {selectedStepsCount}/{Recipe?.Steps.Count}";
        public Color IngredientsCountColor => selectedIngredientsCount == Recipe?.Ingredients.Count ? Colors.Green : Colors.Black;
        public Color StepsCountColor => selectedStepsCount == Recipe?.Steps.Count ? Colors.Green : Colors.Black;

        public ICommand BackCommand { get; }
        public ICommand UpdateIngredientsCountCommand { get; }
        public ICommand UpdateStepsCountCommand { get; }

        public RecipePageVM()
        {
            BackCommand = new Command(OnBack);
            UpdateIngredientsCountCommand = new Command(UpdateIngredientsCount);
            UpdateStepsCountCommand = new Command(UpdateStepsCount);
        }

        private void OnBack()
        {
            Shell.Current.GoToAsync("..");
        }

        private void UpdateCounts()
        {
            selectedIngredientsCount = Recipe.Ingredients.Count(i => i.IsSelected);
            selectedStepsCount = Recipe.Steps.Count(s => s.IsSelected);
            OnPropertyChanged(nameof(IngredientsCountLabel));
            OnPropertyChanged(nameof(StepsCountLabel));
            OnPropertyChanged(nameof(IngredientsCountColor));
            OnPropertyChanged(nameof(StepsCountColor));
        }

        public void UpdateIngredientsCount()
        {
            UpdateCounts();
        }

        public void UpdateStepsCount()
        {
            UpdateCounts();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
