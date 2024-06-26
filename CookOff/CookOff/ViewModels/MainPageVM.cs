﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CookOff.Models;
using Microsoft.Maui.Controls;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System;

namespace CookOff.ViewModels
{
    public class MainPageVM : INotifyPropertyChanged
    {
        // Class for the main page view model


        // Fields 
        private FileSystemWatcher fileWatcher;
        private DateTime lastRead = DateTime.MinValue;
        private readonly TimeSpan debounceTime = TimeSpan.FromMilliseconds(500);

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
        public ObservableCollection<string> SortOptions { get; set; }
        private string _selectedSortOption;

        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (_selectedSortOption != value)
                {
                    _selectedSortOption = value;
                    OnPropertyChanged(nameof(SelectedSortOption));
                    SortRecipes();
                }
            }
        }

        // Command fields
        public ICommand NavigateToCreateRecipeCommand { get; private set; }
        public ICommand DeleteSelectedRecipesCommand { get; private set; }
        public ICommand NavigateToRecipePageCommand { get; private set; }
        public ICommand OnMainPageShowHelpCommand { get; private set; }

        // Constructor
        public MainPageVM()
        {
            Recipes = new ObservableCollection<Recipe>();
            SortOptions = new ObservableCollection<string>
            {
                "Name",
                "Average User Rating",
                "Deliciousness Rating"
            };
            LoadRecipesFromCsv();
            LoadIngredientsFromCsv();
            LoadStepsFromCsv();
            LoadRatingsFromCsv();
            NavigateToCreateRecipeCommand = new Command(OnNavigateToCreateRecipe);
            DeleteSelectedRecipesCommand = new Command(OnDeleteSelectedRecipes);
            NavigateToRecipePageCommand = new Command<Recipe>(OnNavigateToRecipePage);
            OnMainPageShowHelpCommand = new Command(OnMainPageShowHelp);

            InitializeFileWatcher();
        }


        // Method to monitor changes in the csv
        private void InitializeFileWatcher()
        {
            string projectDir = GetProjectDirectory();
            string recipesFilePath = Path.Combine(projectDir, "recipes.csv");

            fileWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(recipesFilePath),
                Filter = Path.GetFileName(recipesFilePath),
                NotifyFilter = NotifyFilters.LastWrite
            };

            fileWatcher.Changed += OnCsvFileChanged;
            fileWatcher.EnableRaisingEvents = true;
        }

        // Sorts recipes according to name, average user rating and deliciousness rating 
        private void SortRecipes()
        {
            switch (SelectedSortOption)
            {
                case "Name":
                    Recipes = new ObservableCollection<Recipe>(Recipes.OrderBy(r => r.Name));
                    break;
                case "Average User Rating":
                    Recipes = new ObservableCollection<Recipe>(Recipes.OrderByDescending(r => r.AverageRating));
                    break;
                case "Deliciousness Rating":
                    Recipes = new ObservableCollection<Recipe>(Recipes.OrderByDescending(r => r.Rating));
                    break;
            }
            OnPropertyChanged(nameof(Recipes));
        }

        // Method that laods data from the csv when changes are made
        private async void OnCsvFileChanged(object sender, FileSystemEventArgs e)
        {
            var now = DateTime.Now;
            if (now - lastRead < debounceTime)
                return;

            lastRead = now;
            await Task.Delay(100); // Slight delay to ensure file write is complete

            Device.BeginInvokeOnMainThread(() =>
            {
                LoadRecipesFromCsv();
                LoadIngredientsFromCsv();
                LoadStepsFromCsv();
                LoadRatingsFromCsv();
            });
        }

        // Navigation to the recipe creation page
        private async void OnNavigateToCreateRecipe()
        {
            await Shell.Current.GoToAsync("CreateRecipePage");
        }

        // Navigation to the recipe view page
        private async void OnNavigateToRecipePage(Recipe selectedRecipe)
        {
            if (selectedRecipe == null)
                return;

            var navigationParameter = new Dictionary<string, object>
            {
                { "SelectedRecipe", selectedRecipe }
            };

            await Shell.Current.GoToAsync("RecipePage", navigationParameter);
        }

        // Loads all the recipes from the csv to main page
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

                    // Clear existing recipes before loading new ones
                    Recipes.Clear();

                    // Ensure unique RecipeID
                    var existingIds = new HashSet<int>();
                    int maxId = recipeRecords.DefaultIfEmpty().Max(r => r?.RecipeID ?? 0);
                    foreach (var recipe in recipeRecords)
                    {
                        // Add project directory to the image path
                        if (!string.IsNullOrWhiteSpace(recipe.ImagePath))
                        {
                            recipe.DisplayImagePath = Path.Combine(projectDir, recipe.ImagePath);
                        }

                        if (!existingIds.Add(recipe.RecipeID))
                        {
                            recipe.RecipeID = ++maxId;
                        }
                        Recipes.Add(recipe);
                        Debug.WriteLine($"Loaded recipe: {recipe.Name}, ImagePath: {recipe.ImagePath}");
                    }

                    Debug.WriteLine($"Loaded {Recipes.Count} recipes from CSV.");
                }
            }
            else
            {
                Debug.WriteLine("Recipes CSV file not found.");
            }
        }

        // Loads all the ingredients from the csv to recipe page
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
                    csv.Context.RegisterClassMap<IngredientMap>();

                    var ingredientRecords = csv.GetRecords<Ingredient>().ToList();

                    // Clear existing ingredients for each recipe before loading new ones
                    foreach (var recipe in Recipes)
                    {
                        recipe.Ingredients.Clear();
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

        // Loads all the steps from the csv to recipe page
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
                    csv.Context.RegisterClassMap<StepMap>();

                    var stepRecords = csv.GetRecords<Step>().ToList();

                    // Clear existing steps for each recipe before loading new ones
                    foreach (var recipe in Recipes)
                    {
                        recipe.Steps.Clear();
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

        // Loads all the ratings from the csv to recipe page
        public void LoadRatingsFromCsv()
        {
            Debug.WriteLine("Loading ratings");
            string projectDir = GetProjectDirectory();
            string ratingsFilePath = Path.Combine(projectDir, "ratings.csv");
            var ratingRecords = new List<(int RecipeID, int UserRating)>();

            if (File.Exists(ratingsFilePath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    HeaderValidated = null
                };

                using (var reader = new StreamReader(ratingsFilePath))
                using (var csv = new CsvReader(reader, config))
                {
                    try
                    {
                        csv.Read();
                        csv.ReadHeader();

                        while (csv.Read())
                        {
                            int recipeId = csv.GetField<int>("RecipeID");
                            int userRating = csv.GetField<int>("User Rating");

                            ratingRecords.Add((recipeId, userRating));
                        }
                        foreach (var recipe in Recipes)
                        {
                            recipe.UserRatings.Clear();
                            var ratingsForRecipe = ratingRecords.Where(r => r.RecipeID == recipe.RecipeID)
                                                                .Select(r => r.UserRating)
                                                                .ToList();

                            recipe.UserRatings.AddRange(ratingsForRecipe);
                        }
                        Debug.WriteLine($"Loaded ratings from CSV: {ratingRecords.Count} records.");
                    }
                    catch (CsvHelperException ex)
                    {
                        Debug.WriteLine($"Error reading CSV record: {ex.Message}");
                    }
                }
            }
            else
            {
                Debug.WriteLine("Ratings CSV file not found.");
            }
        }

        // Method that shows a 'Are you sure?' pop up and deletes the selected data from the csv when the delete button is pressed
        private async void OnDeleteSelectedRecipes()
        {
            bool isConfirmed = await App.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                "Are you sure you want to delete the selected recipes?",
                "Yes",
                "No"
            );

            if (!isConfirmed)
            {
                await Shell.Current.GoToAsync("MainPage");
            }

            var selectedRecipes = Recipes.Where(r => r.IsSelected).ToList();
            var selectedRecipeIds = selectedRecipes.Select(r => r.RecipeID).ToHashSet();

            foreach (var recipe in selectedRecipes)
            {
                Recipes.Remove(recipe);
            }

            SaveRecipesToCsv();

            // Remove ingredients and steps related to the selected recipes
            RemoveIngredientsByRecipeIds(selectedRecipeIds);
            RemoveStepsByRecipeIds(selectedRecipeIds);
            RemoveRatingsByRecipeIds(selectedRecipeIds);

            await Shell.Current.GoToAsync("MainPage");
        }

        // Saves the recipe data to the csv
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
                csv.Context.RegisterClassMap<RecipeMap>();
                csv.WriteRecords(Recipes);
            }
        }

        // Removes the recipe rating by the ID of the recipe
        private void RemoveRatingsByRecipeIds(HashSet<int> recipeIds)
        {
            string projectDir = GetProjectDirectory();
            string ratingsFilePath = Path.Combine(projectDir, "ratings.csv");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true
            };

            var remainingRows = new List<string>();

            // Read the CSV file and filter out ratings with the specified RecipeIDs
            using (var reader = new StreamReader(ratingsFilePath))
            using (var csv = new CsvParser(reader, config))
            {
                bool isFirstRow = true;
                while (csv.Read())
                {
                    if (isFirstRow)
                    {
                        // Add header row to the remaining rows
                        remainingRows.Add(string.Join(",", csv.Record));
                        isFirstRow = false;
                        continue;
                    }

                    int recipeID = int.Parse(csv.Record[0]);
                    if (!recipeIds.Contains(recipeID))
                    {
                        remainingRows.Add(string.Join(",", csv.Record));
                    }
                }
            }

            // Write the remaining rows back to the CSV file
            using (var writer = new StreamWriter(ratingsFilePath))
            {
                foreach (var row in remainingRows)
                {
                    writer.WriteLine(row);
                }
            }
        }

        // Removes the recipe ingredients by the ID of the recipe
        private void RemoveIngredientsByRecipeIds(HashSet<int> recipeIds)
        {
            string projectDir = GetProjectDirectory();
            string ingredientsFilePath = Path.Combine(projectDir, "ingredients.csv");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true
            };

            var remainingIngredients = new List<Ingredient>();

            using (var reader = new StreamReader(ingredientsFilePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<IngredientMap>();
                remainingIngredients = csv.GetRecords<Ingredient>().Where(i => !recipeIds.Contains(i.RecipeID)).ToList();
            }

            using (var writer = new StreamWriter(ingredientsFilePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<IngredientMap>();
                csv.WriteRecords(remainingIngredients);
            }
        }

        // Removes the recipe steps by the ID of the recipe
        private void RemoveStepsByRecipeIds(HashSet<int> recipeIds)
        {
            string projectDir = GetProjectDirectory();
            string stepsFilePath = Path.Combine(projectDir, "steps.csv");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true
            };

            var remainingSteps = new List<Step>();

            using (var reader = new StreamReader(stepsFilePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<StepMap>();
                remainingSteps = csv.GetRecords<Step>().Where(s => !recipeIds.Contains(s.RecipeID)).ToList();
            }

            using (var writer = new StreamWriter(stepsFilePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<StepMap>();
                csv.WriteRecords(remainingSteps);
            }
        }

        // Gets the local project path
        private string GetProjectDirectory()
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectDir = Directory.GetParent(currentDir).Parent.Parent.Parent.Parent.Parent.FullName;
            return projectDir;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Method to get the 'Help.jpg' image
        private string GetImagePath()
        {
            // Ensure the images directory exists in the project directory
            string projectDirectory = GetProjectDirectory();
            string imagesDirectory = Path.Combine(projectDirectory, "images");
            string imageName = "Help.jpg"; // Change this to match your image file name
            return Path.Combine(imagesDirectory, imageName);
        }

        public string HelpImageSource => GetImagePath();

        // Method to show a help pop up for the main page
        private async void OnMainPageShowHelp()
        {
            // Display a message indicating the purpose of the picker
            await App.Current.MainPage.DisplayAlert("Help", "Welcome to the Recipe Collection! \n\nThis page lists all available recipes. Click on any recipe to view its detailed instructions and ingredients.", "OK");
        }
    }
}
