using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CookOff.Models;
using CookOff.Utils;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace CookOff.ViewModels
{
    public class CreateRecipeVM : INotifyPropertyChanged
    {
        private static int RecipeCounter = 1;

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

        private string _ingredientName;
        public string IngredientName
        {
            get { return _ingredientName; }
            set
            {
                _ingredientName = value;
                OnPropertyChanged();
            }
        }

        private string _ingredientQuantity;
        public string IngredientQuantity
        {
            get { return _ingredientQuantity; }
            set
            {
                _ingredientQuantity = value;
                OnPropertyChanged();
            }
        }

        private string _ingredientUnit;
        public string IngredientUnit
        {
            get { return _ingredientUnit; }
            set
            {
                _ingredientUnit = value;
                OnPropertyChanged();
            }
        }

        private string _validationMessage;
        public string ValidationMessage
        {
            get { return _validationMessage; }
            set
            {
                _validationMessage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> Ratings { get; } = new ObservableCollection<int> { 1, 2, 3, 4, 5 };

        private ObservableCollection<string> _ingredientUnits;
        public ObservableCollection<string> IngredientUnits { get; } = new ObservableCollection<string>
        {
            "Pieces (pcs)",
            "Teaspoons (tsp)",
            "Tablespoons (tbsp)",
            "Milliliters (ml)",
            "Liters (l)",
            "Grams (g)",
            "Kilograms (kg)"
        };

        private ObservableCollection<StepVM> _steps;
        public ObservableCollection<StepVM> Steps
        {
            get { return _steps; }
            set
            {
                _steps = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IngredientVM> _ingredients;
        public ObservableCollection<IngredientVM> Ingredients
        {
            get { return _ingredients; }
            set
            {
                _ingredients = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddStepCommand { get; private set; }
        public ICommand AddIngredientsCommand { get; private set; }
        public ICommand SubmitCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand UploadImageCommand { get; private set; }

        private int stepCount = 1;

        public CreateRecipeVM()
        {
            Steps = new ObservableCollection<StepVM>();
            Ingredients = new ObservableCollection<IngredientVM>();
            AddStepCommand = new Command(OnAddStep);
            AddIngredientsCommand = new Command(OnAddIngredient);
            SubmitCommand = new Command(OnSubmit);
            BackCommand = new Command(OnBack);
            UploadImageCommand = new Command(async () => await OnUploadImage());

            // Ensure RecipeCounter starts from the maximum RecipeID
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
                    if (recipeRecords.Any())
                    {
                        RecipeCounter = recipeRecords.Max(r => r.RecipeID) + 1;
                    }
                    else
                    {
                        RecipeCounter = 1; // Initialize to 1 if no recipes exist
                    }
                }
            }
            else
            {
                RecipeCounter = 1; // Initialize to 1 if the file does not exist
            }
        }



        private void OnAddStep()
        {
            Steps.Add(new StepVM($"Step {stepCount++}"));
        }

        private void OnAddIngredient()
        {
            if (!string.IsNullOrWhiteSpace(IngredientName) &&
                !string.IsNullOrWhiteSpace(IngredientQuantity) &&
                !string.IsNullOrWhiteSpace(IngredientUnit))
            {
                Ingredients.Add(new IngredientVM(
                    IngredientName, IngredientUnit, IngredientQuantity));

                // Clear input fields
                IngredientName = string.Empty;
                IngredientQuantity = string.Empty;
                IngredientUnit = string.Empty;
            }
        }

        private bool ValidateRecipe()
        {
            if (string.IsNullOrWhiteSpace(RecipeName))
            {
                ValidationMessage = "Recipe Name is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(ImagePath))
            {
                ValidationMessage = "Image upload is required.";
                return false;
            }

            if (Rating <= 0)
            {
                ValidationMessage = "Deliciousness Rating is required.";
                return false;
            }

            if (Ingredients.Count == 0)
            {
                ValidationMessage = "At least one ingredient is required.";
                return false;
            }

            if (Steps.Count == 0)
            {
                ValidationMessage = "At least one step is required.";
                return false;
            }

            foreach (var step in Steps)
            {
                if (string.IsNullOrWhiteSpace(step.Description))
                {
                    ValidationMessage = "Step description is required.";
                    return false;
                }

                if (step.TimerRequired && step.Hours == 0 && step.Minutes == 0 && step.Seconds == 0)
                {
                    ValidationMessage = "All timer values cannot be 0 if the baking timer option is selected.";
                    return false;
                }
            }

            ValidationMessage = string.Empty;
            return true;
        }

        private async void OnSubmit()
        {
            if (!ValidateRecipe())
            {
                return;
            }

            var newRecipe = new Recipe(RecipeCounter++, RecipeName, ImagePath, Rating);

            foreach (var stepVM in Steps)
            {
                var timer = new TimeSpan(stepVM.Hours, stepVM.Minutes, stepVM.Seconds);
                newRecipe.AddStep(new Step(newRecipe.RecipeID, stepVM.Description, stepVM.TimerRequired, timer));
            }

            foreach (var ingredient in Ingredients)
            {
                newRecipe.AddIngredient(new Ingredient(newRecipe.RecipeID, ingredient.Name, ingredient.Unit, double.Parse(ingredient.Quantity)));
            }

            // Save the newRecipe object to CSV files
            string projectDir = GetProjectDirectory();
            string recipesFilePath = Path.Combine(projectDir, "recipes.csv");
            string ingredientsFilePath = Path.Combine(projectDir, "ingredients.csv");
            string stepsFilePath = Path.Combine(projectDir, "steps.csv");

            CookOff.Utils.CsvHelper.AppendRecipeToCsv(recipesFilePath, ingredientsFilePath, stepsFilePath, newRecipe);

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
                    // Ensure the images directory exists in the project directory
                    string projectDirectory = GetProjectDirectory();
                    string imagesDirectory = Path.Combine(projectDirectory, "images");

                    if (!Directory.Exists(imagesDirectory))
                    {
                        Directory.CreateDirectory(imagesDirectory);
                    }

                    // Create a unique file name for the image
                    string fileName = Path.GetFileName(result.FullPath);
                    string newFilePath = Path.Combine(imagesDirectory, fileName);

                    // Copy the selected file to the new directory
                    using (var stream = await result.OpenReadAsync())
                    using (var newStream = File.Create(newFilePath))
                    {
                        await stream.CopyToAsync(newStream);
                    }

                    // Save the relative path of the image
                    string relativeImagePath = Path.GetRelativePath(projectDirectory, newFilePath);
                    ImagePath = relativeImagePath ?? throw new ArgumentNullException(nameof(relativeImagePath), "Image path cannot be null");
                    Debug.WriteLine($"ImagePath set to: {ImagePath}");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions, such as the user cancelling the file picker
                Debug.WriteLine($"File picking error: {ex.Message}");
            }
        }


        private string GetProjectDirectory()
        {
            // Assuming the application runs from the bin directory, we can navigate up to the project directory
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectDir = Directory.GetParent(currentDir).Parent.Parent.Parent.Parent.Parent.FullName;
            return projectDir;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

