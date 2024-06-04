using System.Collections.Generic;

namespace CookOff.Models
{
    public class Recipe
    {
        public int RecipeID { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int Rating { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public List<Step> Steps { get; set; } = new List<Step>();

        public Recipe(string name, string imagePath, int rating)
        {
            Name = name;
            ImagePath = imagePath;
            Rating = rating;
        }

        public void AddIngredient(Ingredient ingredient)
        {
            Ingredients.Add(ingredient);
        }

        public void AddStep(Step step)
        {
            Steps.Add(step);
        }
    }
}
