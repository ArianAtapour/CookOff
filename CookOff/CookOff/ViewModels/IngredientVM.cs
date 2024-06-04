using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CookOff.ViewModels
{
    public class IngredientVM : INotifyPropertyChanged
    {
        private string name;
        private string unit;
        private string quantity;

        public IngredientVM(string name, string unit, string quantity)
        {
            setName(name);
            setUnit(unit);
            setQuantity(quantity);
        }

        public void setName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty!");
            }
            else
            {
                this.name = name;
                OnPropertyChanged();
            }
        }

        public string getName()
        {
            return this.name;
        }

        public void setUnit(string unit)
        {
            if (string.IsNullOrEmpty(unit))
            {
                throw new ArgumentException("Unit cannot be null or empty!");
            }
            else
            {
                this.unit = unit;
                OnPropertyChanged();
            }
        }

        public string getUnit()
        {
            return this.unit;
        }

        public void setQuantity(string quantity)
        {
            if (string.IsNullOrEmpty(quantity) || !double.TryParse(quantity, out double qty) || qty <= 0)
            {
                throw new ArgumentException("Quantity must be a positive number and greater than zero!");
            }
            else
            {
                this.quantity = quantity;
                OnPropertyChanged();
            }
        }

        public string getQuantity()
        {
            return this.quantity;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
