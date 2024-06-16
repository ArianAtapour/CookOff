using Microsoft.Maui.Controls;
using CookOff.ViewModels;
using CookOff.Models;

namespace CookOff
{
    public partial class RecipePage : ContentPage
    {
        //Constructor
        public RecipePage()
        {
            InitializeComponent();
        }

        //Ingredient and Step handling for updating the list of ingredients or steps.
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
