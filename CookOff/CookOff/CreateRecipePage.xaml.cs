using CookOff.ViewModels;

namespace CookOff;

public partial class CreateRecipePage : ContentPage
{
    public CreateRecipePage()
    {
        InitializeComponent();
        BindingContext = new CreateRecipeVM();
    }
}
