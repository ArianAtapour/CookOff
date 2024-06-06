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

        public ICommand NavigateToCreateRecipeCommand { get; private set; }
        public ICommand DeleteSelectedRecipesCommand { get; private set; }
        public ICommand NavigateToRecipePageCommand { get; private set; }

        public MainPageVM()
        {
            Recipes = new ObservableCollection<Recipe>();
            LoadRecipesFromCsv();
            LoadIngredientsFromCsv();
            LoadStepsFromCsv();
            NavigateToCreateRecipeCommand = new Command(OnNavigateToCreateRecipe);
            DeleteSelectedRecipesCommand = new Command(OnDeleteSelectedRecipes);
            NavigateToRecipePageCommand = new Command<Recipe>(OnNavigateToRecipePage);

            InitializeFileWatcher();
        }

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
            });
        }

        private async void OnNavigateToCreateRecipe()
        {
            await Shell.Current.GoToAsync("CreateRecipePage");
        }

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

        private void OnDeleteSelectedRecipes()
        {
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
                csv.Context.RegisterClassMap<RecipeMap>();
                csv.WriteRecords(Recipes);
            }
        }

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
    }
}
