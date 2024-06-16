using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
                AssignStepNumbers();
            }
        }

        public string IngredientsCountLabel => $"Ingredients: {selectedIngredientsCount}/{Recipe?.Ingredients.Count}";
        public string StepsCountLabel => $"Steps: {selectedStepsCount}/{Recipe?.Steps.Count}";
        public string UserRatingsLabel => $"User ratings:  {Recipe?.UserRatings.Count}";
        public string UserRatingAverageLabel
        {
            get
            {
                if (Recipe != null && Recipe?.AverageRating != 0)
                {
                    // Round AverageRating to 2 decimal places and ensure it always shows .00
                    string formattedRating = Recipe.AverageRating.ToString("0.00");

                    return $"Average rating: {formattedRating}";
                }
                else
                {
                    return "Average rating: N/A"; // Handle the case where Recipe or AverageRating is null
                }
            }
        }
        public Color IngredientsCountColor => selectedIngredientsCount == Recipe?.Ingredients.Count ? Colors.Green : Colors.Black;
        public Color StepsCountColor => selectedStepsCount == Recipe?.Steps.Count ? Colors.Green : Colors.Black;

        public ICommand BackCommand { get; }
        public ICommand UpdateIngredientsCountCommand { get; }
        public ICommand UpdateStepsCountCommand { get; }
        public ICommand RecipeViewShowHelpCommand { get; private set; }
        public ICommand AverageRatingShowHelpCommand { get; private set; }

        public RecipePageVM()
        {
            BackCommand = new Command(OnBack);
            UpdateIngredientsCountCommand = new Command(UpdateIngredientsCount);
            UpdateStepsCountCommand = new Command(UpdateStepsCount);
            RecipeViewShowHelpCommand = new Command(OnRecipeViewShowHelp);
            AverageRatingShowHelpCommand = new Command(OnAverageRatingShowHelp);
        }

        private void OnBack()
        {
            Shell.Current.GoToAsync("..");
        }

        private async void UpdateCounts()
        {
            selectedIngredientsCount = Recipe.Ingredients.Count(i => i.IsSelected);
            selectedStepsCount = Recipe.Steps.Count(s => s.IsSelected);
            OnPropertyChanged(nameof(IngredientsCountLabel));
            OnPropertyChanged(nameof(StepsCountLabel));
            OnPropertyChanged(nameof(IngredientsCountColor));
            OnPropertyChanged(nameof(StepsCountColor));
            OnPropertyChanged(nameof(UserRatingsLabel));
            OnPropertyChanged(nameof(UserRatingAverageLabel));

            if (selectedIngredientsCount == Recipe.Ingredients.Count && selectedStepsCount == Recipe.Steps.Count)
            {
                await ShowRatingMessageBox();
            }
        }
         
        private void AssignStepNumbers()
        {
            var stepNumber = 0;
            foreach (var step in Recipe.Steps)
            {
                step.StepNumber = "Step " + (stepNumber + 1).ToString();
                stepNumber++;
            }
        }
        private async Task ShowRatingMessageBox()
        {
            ResetCheckBoxes();
            string result = await Shell.Current.DisplayPromptAsync("Rate Recipe", "Please rate the recipe from 1 to 5:", "OK", "Cancel", keyboard: Keyboard.Numeric);

            if (int.TryParse(result, out int userRating) && userRating >= 1 && userRating <= 5)
            {
                Recipe.UserRatings.Add(userRating);
                OnPropertyChanged(nameof(Recipe.UserRatings));
                OnPropertyChanged(nameof(Recipe.AverageRating));
                UpdateCounts();
                // Save the newRecipe object to CSV files
                string projectDir = GetProjectDirectory();

                string userRatingFilePath = Path.Combine(projectDir, "ratings.csv");

                CookOff.Utils.CsvHelper.AppendRatingToCsv(userRatingFilePath, Recipe.RecipeID, userRating);
            }
            else if (result == null)
            {
            }
            else
            {
                await Shell.Current.DisplayAlert("Invalid Input", "Please enter a valid rating between 1 and 5.", "OK");
            }
        }

        private void ResetCheckBoxes()
        {
            foreach (var ingredient in Recipe.Ingredients)
            {
                ingredient.IsSelected = false;
            }

            foreach (var step in Recipe.Steps)
            {
                step.IsSelected = false;
            }

            UpdateCounts();
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
        private string GetProjectDirectory()
        {
            // Assuming the application runs from the bin directory, we can navigate up to the project directory
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectDir = Directory.GetParent(currentDir).Parent.Parent.Parent.Parent.Parent.FullName;
            return projectDir;
        }

        private string GetImagePath()
        {
            // Ensure the images directory exists in the project directory
            string projectDirectory = GetProjectDirectory();
            string imagesDirectory = Path.Combine(projectDirectory, "images");
            string imageName = "Help.jpg"; // Change this to match your image file name
            return Path.Combine(imagesDirectory, imageName);
        }

        public string HelpImageSource => GetImagePath();

        private async void OnRecipeViewShowHelp()
        {
            // Display a message indicating the purpose of the recipe details page
            await App.Current.MainPage.DisplayAlert("Help", "Welcome to the Recipe Details Page! \n\nHere you can find detailed instructions and ingredients for the selected recipe. \n\nBrowse through the steps and gather all necessary ingredients to start cooking your dish!", "OK");
        }

        private async void OnAverageRatingShowHelp()
        {
            // Display a message indicating the purpose of the recipe details page
            await App.Current.MainPage.DisplayAlert("Help", "The average rating represents the overall user satisfaction with this recipe, based on all the collected ratings.", "OK");

        }
    }
}
