using Microsoft.Maui.Controls;
using CookOff.ViewModels;
using System.Diagnostics;

namespace CookOff
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageVM();
            Debug.WriteLine($"BindingContext set to MainPageVM with {((MainPageVM)BindingContext).Recipes.Count} recipes.");
        }
    }
}
