using System.ComponentModel;
using System.Runtime.CompilerServices;

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

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public string Unit
        {
            get => unit;
            set
            {
                unit = value;
                OnPropertyChanged();
            }
        }

        public string Quantity
        {
            get => quantity;
            set
            {
                quantity = value;
                OnPropertyChanged();
            }
        }

        public void setName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty!");
            }
            this.name = name;
            OnPropertyChanged(nameof(Name));
        }

        public string getName() => name;

        public void setUnit(string unit)
        {
            if (string.IsNullOrEmpty(unit))
            {
                throw new ArgumentException("Unit cannot be null or empty!");
            }
            this.unit = unit;
            OnPropertyChanged(nameof(Unit));
        }

        public string getUnit() => unit;

        public void setQuantity(string quantity)
        {
            if (string.IsNullOrEmpty(quantity) || !double.TryParse(quantity, out double qty) || qty <= 0)
            {
                throw new ArgumentException("Quantity must be a positive number and greater than zero!");
            }
            this.quantity = quantity;
            OnPropertyChanged(nameof(Quantity));
        }

        public string getQuantity() => quantity;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
