using System;

namespace CookOff.Models
{
    public class Step
    {
        public int RecipeID { get; set; }
        public string Description { get; set; }
        public bool TimerRequired { get; set; }
        public TimeSpan Timer { get; set; }

        public Step() { }

        public Step(int recipeID, string description, bool timerRequired, TimeSpan timer)
        {
            RecipeID = recipeID;
            Description = description;
            TimerRequired = timerRequired;
            Timer = timer;
        }

        public void SetDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("Description cannot be null or empty!");
            }
            this.Description = description;
        }

        public void SetTimerRequired(bool timerRequired)
        {
            this.TimerRequired = timerRequired;
        }

        public void SetTimer(TimeSpan timer)
        {
            if (timer < TimeSpan.Zero)
            {
                throw new ArgumentException("Timer cannot be negative!");
            }
            this.Timer = timer;
        }
    }
}
