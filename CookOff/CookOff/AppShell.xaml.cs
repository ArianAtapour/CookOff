namespace CookOff;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("CreateRecipePage", typeof(CreateRecipePage));
        Routing.RegisterRoute("RecipePage", typeof(RecipePage));
    }
}

