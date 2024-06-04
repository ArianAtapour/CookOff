using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CookOff.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace CookOff.ViewModels
{
    public class CreateRecipeVM : INotifyPropertyChanged
    {
        private string _recipeName;
        public string RecipeName
        {
            get { return _recipeName; }
            set
            {
                _recipeName = value;
                OnPropertyChanged();
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        private int _rating;
        public int Rating
        {
            get { return _rating; }
            set
            {
                if (value >= 1)
                {
                    _rating = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ingredientName;
        public string IngredientName
        {
            get { return _ingredientName; }
            set
            {
                _ingredientName = value;
                OnPropertyChanged();
            }
        }

        private string _ingredientQuantity;
        public string IngredientQuantity
        {
            get { return _ingredientQuantity; }
            set
            {
                _ingredientQuantity = value;
                OnPropertyChanged();
            }
        }

        private string _ingredientUnit;
        public string IngredientUnit
        {
            get { return _ingredientUnit; }
            set
            {
                _ingredientUnit = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> Ratings { get; } = new ObservableCollection<int> { 1, 2, 3, 4, 5 };

        private ObservableCollection<StepVM> _steps;
        public ObservableCollection<StepVM> Steps
        {
            get { return _steps; }
            set
            {
                _steps = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IngredientVM> _ingredients;
        public ObservableCollection<IngredientVM> Ingredients
        {
            get { return _ingredients; }
            set
            {
                _ingredients = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddStepCommand { get; private set; }
        public ICommand AddIngredientsCommand { get; private set; }
        public ICommand SubmitCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand UploadImageCommand { get; private set; }

        private int stepCount = 1;

        public CreateRecipeVM()
        {
            Steps = new ObservableCollection<StepVM>();
            Ingredients = new ObservableCollection<IngredientVM>();
            AddStepCommand = new Command(OnAddStep);
            AddIngredientsCommand = new Command(OnAddIngredient);
            SubmitCommand = new Command(OnSubmit);
            BackCommand = new Command(OnBack);
            UploadImageCommand = new Command(async () => await OnUploadImage());
        }

        private void OnAddStep()
        {
            Steps.Add(new StepVM($"Step {stepCount++}"));
        }

        private void OnAddIngredient()
        {
            Ingredients.Add(new IngredientVM(
                IngredientName, IngredientUnit, IngredientQuantity));

            // Clear input fields
            IngredientName = string.Empty;
            IngredientQuantity = string.Empty;
            IngredientUnit = string.Empty;
        }

        private async void OnSubmit()
        {
            var newRecipe = new Recipe(RecipeName, ImagePath, Rating);
            foreach (var StepVM in Steps)
            {
                var timer = new TimeSpan(StepVM.Hours, StepVM.Minutes, StepVM.Seconds);
                newRecipe.addStep(new Step(StepVM.getDescription(), StepVM.TimerRequired, timer));
            }

            foreach (var ingredient in Ingredients)
            {
                newRecipe.addIngredient(new Ingredient(ingredient.getName(), ingredient.getUnit(), double.Parse(ingredient.getQuantity())));
            }

            // Save or process newRecipe object as needed
            // For example, you could write it to a file or send it to a backend service

            await Shell.Current.GoToAsync("..");
        }

        private async void OnBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async Task OnUploadImage()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Please select an image file",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    // Ensure the images directory exists in the project directory
                    string projectDirectory = GetProjectDirectory();
                    string imagesDirectory = Path.Combine(projectDirectory, "images");

                    if (!Directory.Exists(imagesDirectory))
                    {
                        Directory.CreateDirectory(imagesDirectory);
                    }

                    // Create a unique file name for the image
                    string fileName = Path.GetFileName(result.FullPath);
                    string newFilePath = Path.Combine(imagesDirectory, fileName);

                    // Copy the selected file to the new directory
                    using (var stream = await result.OpenReadAsync())
                    using (var newStream = File.Create(newFilePath))
                    {
                        await stream.CopyToAsync(newStream);
                    }

                    // Update the image path
                    ImagePath = newFilePath;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions, such as the user cancelling the file picker
                Console.WriteLine($"File picking error: {ex.Message}");
            }
        }

        private string GetProjectDirectory()
        {
            // Assuming the application runs from the bin directory, we can navigate up to the project directory
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectDir = Directory.GetParent(currentDir).Parent.Parent.Parent.Parent.Parent.FullName;
            return projectDir;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
