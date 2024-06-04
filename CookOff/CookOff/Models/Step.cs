using System;
namespace CookOff.Models
{
    //Step model
    public class Step
    {
        private string description;
        private bool timerRequired;
        private TimeSpan timer;

        public Step(string description, bool timerRequired, TimeSpan timer)
        {
            setDescription(description);
            setTimerRequired(timerRequired);
            setTimer(timer);
        }

        public void setDescription(string description)
        {
            if (String.IsNullOrEmpty(description))
            {
                throw new ArgumentException("Description cannot be null or empty!");
            }
            else
            {
                this.description = description;
            }
        }

        public string getDescription()
        {
            return this.description;
        }

        public void setTimerRequired(bool timerRequired)
        {
            this.timerRequired = timerRequired;
        }

        public bool getTimerRequired()
        {
            return this.timerRequired;
        }

        public void setTimer(TimeSpan timer)
        {
            if (timer < TimeSpan.Zero)
            {
                throw new ArgumentException("Timer cannot be negative!");
            }
            else
            {
                this.timer = timer;
            }
        }

        public TimeSpan getTimer()
        {
            return this.timer;
        }
    }
}


