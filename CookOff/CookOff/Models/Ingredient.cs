using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CookOff.Models
{
    public class Ingredient : INotifyPropertyChanged
    {
        public int RecipeID { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Quantity { get; set; }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public Ingredient() { }

        public Ingredient(int recipeID, string name, string unit, double quantity)
        {
            RecipeID = recipeID;
            Name = name;
            Unit = unit;
            Quantity = quantity;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty!");
            }
            this.Name = name;
        }

        public void SetUnit(string unit)
        {
            if (string.IsNullOrEmpty(unit))
            {
                throw new ArgumentException("Unit cannot be null or empty!");
            }
            this.Unit = unit;
        }

        public void SetQuantity(double quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero!");
            }
            this.Quantity = quantity;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
