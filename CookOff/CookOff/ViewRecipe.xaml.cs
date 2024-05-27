using CookOff.ViewModels;
using Microsoft.Maui.Controls;

namespace CookOff;

public partial class ViewRecipe : ContentPage
{
    public ViewRecipe()
    {
        InitializeComponent();
        BindingContext = new ViewModels.ViewRecipe();
    }
}
