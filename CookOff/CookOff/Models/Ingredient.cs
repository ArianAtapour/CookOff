using System;

namespace CookOff.Models
{
    // Ingredient model
    public class Ingredient
    {
        private string name;
        private string unit;
        private double quantity;

        public Ingredient(string name, string unit, double quantity)
        {
            setName(name);
            setUnit(unit);
            setQuantity(quantity);
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

        public void setUnit(string unit)
        {
            if (String.IsNullOrEmpty(unit))
            {
                throw new ArgumentException("Unit cannot be null or empty!");
            }
            else
            {
                this.unit = unit;
            }
        }

        public string getUnit()
        {
            return this.unit;
        }

        public void setQuantity(double quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero!");
            }
            else
            {
                this.quantity = quantity;
            }
        }

        public double getQuantity()
        {
            return this.quantity;
        }
    }
}
