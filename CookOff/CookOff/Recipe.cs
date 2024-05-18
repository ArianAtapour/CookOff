using System;
namespace CookOff
{
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
                throw new ArgumentException("Name cannot be null or empty !");
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
                throw new ArgumentException("Image path is null or empty !");
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
            if (rating < 1)
            {
                throw new ArgumentException("Rating can not be lower than 1");
            }
            else
            {
                this.rating = rating;
            }
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


