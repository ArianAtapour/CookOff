using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using CookOff.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Maui.Controls;
using System.Globalization;
using System.Diagnostics;

namespace CookOff.ViewModels
{
    public class MainPageVM : INotifyPropertyChanged
    {
        private ObservableCollection<Recipe> recipes;
        public ObservableCollection<Recipe> Recipes
        {
            get { return recipes; }
            set
            {
                recipes = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateToCreateRecipeCommand { get; private set; }
        public ICommand DeleteSelectedRecipesCommand { get; private set; }

        public MainPageVM()
        {
            Recipes = new ObservableCollection<Recipe>();
            LoadRecipesFromCsv();
            LoadIngredientsFromCsv();
            LoadStepsFromCsv();
            NavigateToCreateRecipeCommand = new Command(OnNavigateToCreateRecipe);
            DeleteSelectedRecipesCommand = new Command(OnDeleteSelectedRecipes);
        }

        private async void OnNavigateToCreateRecipe()
        {
            await Shell.Current.GoToAsync("CreateRecipePage");
        }

        private void LoadRecipesFromCsv()
        {
            string projectDir = GetProjectDirectory();
            string recipesFilePath = Path.Combine(projectDir, "recipes.csv");

            if (File.Exists(recipesFilePath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    HeaderValidated = null
                };

                using (var reader = new StreamReader(recipesFilePath))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.RegisterClassMap<RecipeMap>();

                    var recipeRecords = csv.GetRecords<Recipe>().ToList();

                    // Ensure unique RecipeID
                    var existingIds = new HashSet<int>();
                    int maxId = recipeRecords.DefaultIfEmpty().Max(r => r?.RecipeID ?? 0);
                    foreach (var recipe in recipeRecords)
                    {
                        if (!existingIds.Add(recipe.RecipeID))
                        {
                            recipe.RecipeID = ++maxId;
                        }
                        Recipes.Add(recipe);
                        Debug.WriteLine($"Loaded recipe: {recipe.Name}, ImagePath: {recipe.ImagePath}, RecipeID: {recipe.RecipeID}");
                    }

                    Debug.WriteLine($"Loaded {Recipes.Count} recipes from CSV.");
                }
            }
            else
            {
                Debug.WriteLine("Recipes CSV file not found.");
            }
        }


        private void LoadIngredientsFromCsv()
        {
            string projectDir = GetProjectDirectory();
            string ingredientsFilePath = Path.Combine(projectDir, "ingredients.csv");

            if (File.Exists(ingredientsFilePath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    HeaderValidated = null
                };

                using (var reader = new StreamReader(ingredientsFilePath))
                using (var csv = new CsvReader(reader, config))
                {
                    var ingredientRecords = csv.GetRecords<Ingredient>().ToList();
                    foreach (var recipe in Recipes)
                    {
                        recipe.Ingredients.AddRange(ingredientRecords.Where(i => i.RecipeID == recipe.RecipeID));
                    }

                    Debug.WriteLine($"Loaded ingredients from CSV.");
                }
            }
            else
            {
                Debug.WriteLine("Ingredients CSV file not found.");
            }
        }

        private void LoadStepsFromCsv()
        {
            string projectDir = GetProjectDirectory();
            string stepsFilePath = Path.Combine(projectDir, "steps.csv");

            if (File.Exists(stepsFilePath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    HeaderValidated = null
                };

                using (var reader = new StreamReader(stepsFilePath))
                using (var csv = new CsvReader(reader, config))
                {
                    var stepRecords = csv.GetRecords<Step>().ToList();
                    foreach (var recipe in Recipes)
                    {
                        recipe.Steps.AddRange(stepRecords.Where(s => s.RecipeID == recipe.RecipeID));
                    }

                    Debug.WriteLine($"Loaded steps from CSV.");
                }
            }
            else
            {
                Debug.WriteLine("Steps CSV file not found.");
            }
        }

        private void OnDeleteSelectedRecipes()
        {
            var selectedRecipes = Recipes.Where(r => r.IsSelected).ToList();
            foreach (var recipe in selectedRecipes)
            {
                Recipes.Remove(recipe);
            }

            SaveRecipesToCsv();
            SaveIngredientsToCsv();
            SaveStepsToCsv();
        }

        private void SaveRecipesToCsv()
        {
            string projectDir = GetProjectDirectory();
            string recipesFilePath = Path.Combine(projectDir, "recipes.csv");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true
            };

            using (var writer = new StreamWriter(recipesFilePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(Recipes);
            }
        }

        private void SaveIngredientsToCsv()
        {
            string projectDir = GetProjectDirectory();
            string ingredientsFilePath = Path.Combine(projectDir, "ingredients.csv");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true
            };

            var remainingIngredients = Recipes.SelectMany(r => r.Ingredients).ToList();

            using (var writer = new StreamWriter(ingredientsFilePath))
            using (var csv = new CsvWriter(writer, config))
            {
                // Write header manually since we are not using the auto-generated header
                writer.WriteLine("RecipeID,Ingredient Name,Ingredient Unit,Ingredient Quantity");
                foreach (var ingredient in remainingIngredients)
                {
                    csv.WriteField(ingredient.RecipeID);
                    csv.WriteField(ingredient.Name);
                    csv.WriteField(ingredient.Unit);
                    csv.WriteField(ingredient.Quantity);
                    csv.NextRecord();
                }
            }
        }

        private void SaveStepsToCsv()
        {
            string projectDir = GetProjectDirectory();
            string stepsFilePath = Path.Combine(projectDir, "steps.csv");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true
            };

            var remainingSteps = Recipes.SelectMany(r => r.Steps).ToList();

            using (var writer = new StreamWriter(stepsFilePath))
            using (var csv = new CsvWriter(writer, config))
            {
                // Write header manually since we are not using the auto-generated header
                writer.WriteLine("RecipeID,Step Description,Timer Required,Timer");
                foreach (var step in remainingSteps)
                {
                    csv.WriteField(step.RecipeID);
                    csv.WriteField(step.Description);
                    csv.WriteField(step.TimerRequired);
                    csv.WriteField(step.Timer);
                    csv.NextRecord();
                }
            }
        }


        private string GetProjectDirectory()
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectDir = Directory.GetParent(currentDir).Parent.Parent.Parent.Parent.Parent.FullName;
            return projectDir;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
