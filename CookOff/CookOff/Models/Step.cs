using System;
namespace CookOff
{
    //Step model
	public class Step
	{
        private string name;
        private bool timeReq;
        private TimeSpan timer;

        public Step(string name, bool timeReq, TimeSpan timer)
        {
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
    }
}


