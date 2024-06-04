using System;
using System.Collections.Generic;

namespace CookOff.Models
{
    // Recipe model
    public class Recipe
    {
        private string name;
        private string imagePath;
        private int rating;
        private List<Step> steps;
        private List<Ingredient> ingredients;

        public Recipe(string name, string imagePath, int rating)
        {
            setName(name);
            setImagePath(imagePath);
            setRating(rating);
            this.steps = new List<Step>();
            this.ingredients = new List<Ingredient>();
        }

        public void setName(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty!");
            }
            else
            {
                this.name = name;
            }
        }

        public string getName()
        {
            return this.name;
        }

        public void setImagePath(string imagePath)
        {
            if (String.IsNullOrEmpty(imagePath))
            {
                throw new ArgumentException("Image path cannot be null or empty!");
            }
            else
            {
                this.imagePath = imagePath;
            }
        }

        public string getImagePath()
        {
            return this.imagePath;
        }

        public void setRating(int rating)
        {
            if (rating < 1 || rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5!");
            }
            else
            {
                this.rating = rating;
            }
        }

        public int getRating()
        {
            return this.rating;
        }

        public void addStep(Step step)
        {
            if (step == null)
            {
                throw new ArgumentException("Step cannot be null!");
            }
            else
            {
                this.steps.Add(step);
            }
        }

        public List<Step> getSteps()
        {
            return this.steps;
        }

        public void addIngredient(Ingredient ingredient)
        {
            if (ingredient == null)
            {
                throw new ArgumentException("Ingredient cannot be null!");
            }
            else
            {
                this.ingredients.Add(ingredient);
            }
        }

        public List<Ingredient> getIngredients()
        {
            return this.ingredients;
        }
    }
}
