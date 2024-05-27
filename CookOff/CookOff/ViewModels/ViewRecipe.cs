using System;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace CookOff.ViewModels
{
    public class ViewRecipe : BindableObject
    {
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand CloseCommand { get; }

        public ViewRecipe()
        {
            StartCommand = new Command(OnStartClicked);
            StopCommand = new Command(OnStopClicked);
            CloseCommand = new Command(OnCloseClicked);
        }

        private void OnStartClicked()
        {
            // Start timer logic
        }

        private void OnStopClicked()
        {
            // Stop timer logic
        }

        private void OnCloseClicked()
        {
            // Close the page or navigate back
            Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
