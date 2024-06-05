using System.Collections.Generic;
using System.ComponentModel;

namespace CookOff.Models
{
    public class Recipe : INotifyPropertyChanged
    {
        public int RecipeID { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int Rating { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public List<Step> Steps { get; set; } = new List<Step>();

        public bool IsSelected { get; set; }

        // Parameterless constructor for deserialization
        public Recipe() { }

        // Constructor with parameters for creating new recipes
        public Recipe(int recipeID, string name, string imagePath, int rating)
        {
            RecipeID = recipeID;
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
