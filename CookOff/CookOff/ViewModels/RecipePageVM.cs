using System.ComponentModel;
using System.Runtime.CompilerServices;
using CookOff.Models;
using Microsoft.Maui.Controls;

namespace CookOff.ViewModels
{
    [QueryProperty(nameof(Recipe), "SelectedRecipe")]
    public class RecipePageVM : INotifyPropertyChanged
    {
        private Recipe recipe;
        public Recipe Recipe
        {
            get { return recipe; }
            set
            {
                recipe = value;
                OnPropertyChanged();
            }
        }

        public RecipePageVM() { }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
