using System;
using System.Collections.Generic;

namespace CookOff.Models
{
    // Recipe model
    public class Recipe
    {
        private string name;
        private string imagePath;
        private double rating;
        private List<Step> steps;

        public Recipe(string name, string imagePath, double rating)
        {
            this.steps = new List<Step>();
            setName(name);
            setImagePath(imagePath);
            setRating(rating);
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

        public void setRating(double rating)
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

        public double getRating()
        {
            return this.rating;
        }

        public void addStep(Step step)
        {
            this.steps.Add(step);
        }

        public List<Step> getSteps()
        {
            return this.steps;
        }
    }
}
