using Microsoft.Maui.Controls;
using CookOff.ViewModels;

namespace CookOff
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageVM();
        }
    }
}
