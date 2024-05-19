using System;
namespace CookOff.Models
{
    //Step model
	public class Step
	{
        private string name;
        private bool timeReq;
        private TimeSpan timer;
        private List<Ingredient> ingredients;

        public Step(string name, bool timeReq, TimeSpan timer)
        {
            this.ingredients = new List<Ingredient>();
            setName(name);
            isTimeReq(timeReq);
            setTimer(timer);
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

        public void isTimeReq(bool timeReq)
        {
            this.timeReq = timeReq;
        }

        public bool getTimeReq()
        {
            return this.timeReq;
        }

        public void setTimer(TimeSpan timer)
        {
            this.timer = timer;
        }

        public TimeSpan getTimer()
        {
            return this.timer;
        }

        public void addIngredient(Ingredient ingredient)
        {
            this.ingredients.Add(ingredient);
        }

        public List<Ingredient> GetIngredients()
        {
            return this.ingredients;
        }
    }
}


