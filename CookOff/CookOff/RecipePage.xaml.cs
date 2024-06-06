using Microsoft.Maui.Controls;
using CookOff.ViewModels;
using CookOff.Models;

namespace CookOff
{
    public partial class RecipePage : ContentPage
    {
        public RecipePage()
        {
            InitializeComponent();
        }

        private void OnIngredientCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            ((RecipePageVM)BindingContext).UpdateIngredientsCount();
        }

        private void OnStepCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            ((RecipePageVM)BindingContext).UpdateStepsCount();
        }
    }
}
