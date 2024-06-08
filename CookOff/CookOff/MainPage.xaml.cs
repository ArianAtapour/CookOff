using Microsoft.Maui.Controls;
using CookOff.ViewModels;
using CookOff.Models;
using System.Collections.Generic;

namespace CookOff
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageVM();
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                var selectedRecipe = e.CurrentSelection[0] as Recipe;
                if (selectedRecipe != null)
                {
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "SelectedRecipe", selectedRecipe }
                    };
                    await Shell.Current.GoToAsync("RecipePage", navigationParameter);

                    // Deselect item
                    ((CollectionView)sender).SelectedItem = null;
                }
            }
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {

        }
    }
}
