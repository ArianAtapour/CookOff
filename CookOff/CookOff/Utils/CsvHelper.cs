using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CookOff.Models;

namespace CookOff.Utils
{
    public static class CsvHelper
    {
        public static void AppendRecipeToCsv(string recipesFilePath, string ingredientsFilePath, string stepsFilePath, Recipe recipe)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = false
            };

            bool recipesFileExists = File.Exists(recipesFilePath);
            bool ingredientsFileExists = File.Exists(ingredientsFilePath);
            bool stepsFileExists = File.Exists(stepsFilePath);

            // Save recipe metadata
            using (var writer = new StreamWriter(recipesFilePath, append: true))
            using (var csv = new CsvWriter(writer, config))
            {
                if (!recipesFileExists)
                {
                    csv.WriteField("RecipeID");
                    csv.WriteField("Recipe Name");
                    csv.WriteField("Image Path");
                    csv.WriteField("Rating");
                    csv.NextRecord();
                }

                csv.WriteField(recipe.RecipeID);
                csv.WriteField(recipe.Name);
                csv.WriteField(recipe.ImagePath);
                csv.WriteField(recipe.Rating);
                csv.NextRecord();
            }

            // Save ingredients
            using (var writer = new StreamWriter(ingredientsFilePath, append: true))
            using (var csv = new CsvWriter(writer, config))
            {
                if (!ingredientsFileExists)
                {
                    csv.WriteField("RecipeID");
                    csv.WriteField("Ingredient Name");
                    csv.WriteField("Ingredient Unit");
                    csv.WriteField("Ingredient Quantity");
                    csv.NextRecord();
                }

                foreach (var ingredient in recipe.Ingredients)
                {
                    csv.WriteField(recipe.RecipeID);
                    csv.WriteField(ingredient.Name);
                    csv.WriteField(ingredient.Unit);
                    csv.WriteField(ingredient.Quantity);
                    csv.NextRecord();
                }
            }

            // Save steps
            using (var writer = new StreamWriter(stepsFilePath, append: true))
            using (var csv = new CsvWriter(writer, config))
            {
                if (!stepsFileExists)
                {
                    csv.WriteField("RecipeID");
                    csv.WriteField("Step Description");
                    csv.WriteField("Timer Required");
                    csv.WriteField("Timer");
                    csv.NextRecord();
                }

                foreach (var step in recipe.Steps)
                {
                    csv.WriteField(recipe.RecipeID);
                    csv.WriteField(step.Description);
                    csv.WriteField(step.TimerRequired);
                    csv.WriteField(step.Timer);
                    csv.NextRecord();
                }
            }
        }
    }
}
