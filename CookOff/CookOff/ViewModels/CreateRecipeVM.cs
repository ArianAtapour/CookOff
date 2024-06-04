using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
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

        private ObservableCollection<StepViewModel> _steps;
        public ObservableCollection<StepViewModel> Steps
        {
            get { return _steps; }
            set
            {
                _steps = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<IngredientViewModel> _ingredients;
        public ObservableCollection<IngredientViewModel> Ingredients
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
            Steps = new ObservableCollection<StepViewModel>();
            Ingredients = new ObservableCollection<IngredientViewModel>();
            AddStepCommand = new Command(OnAddStep);
            AddIngredientsCommand = new Command(OnAddIngredient);
            SubmitCommand = new Command(OnSubmit);
            BackCommand = new Command(OnBack);
            UploadImageCommand = new Command(async () => await OnUploadImage());
        }

        private void OnAddStep()
        {
            Steps.Add(new StepViewModel($"Step {stepCount++}"));
        }

        private void OnAddIngredient()
        {
            Ingredients.Add(new IngredientViewModel
            {
                Name = IngredientName,
                Quantity = IngredientQuantity,
                Unit = IngredientUnit
            });

            // Clear input fields
            IngredientName = string.Empty;
            IngredientQuantity = string.Empty;
            IngredientUnit = string.Empty;
        }

        private async void OnSubmit()
        {
            // Here you can implement the logic to handle the submission of the recipe
            // For example, saving the recipe data to a file or a database

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

                    Console.WriteLine($"Project Directory: {projectDirectory}");
                    Console.WriteLine($"Images Directory: {imagesDirectory}");

                    if (!Directory.Exists(imagesDirectory))
                    {
                        Directory.CreateDirectory(imagesDirectory);
                        Console.WriteLine("Created images directory.");
                    }

                    // Create a unique file name for the image
                    string fileName = Path.GetFileName(result.FullPath);
                    string newFilePath = Path.Combine(imagesDirectory, fileName);

                    Console.WriteLine($"New File Path: {newFilePath}");

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
            Console.WriteLine($"Current Directory: {currentDir}");
            // Navigate to the project directory by going up one more level
            var projectDir = Directory.GetParent(currentDir).Parent.Parent.Parent.Parent.Parent.FullName;
            Console.WriteLine($"Determined Project Directory: {projectDir}");
            return projectDir;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class StepViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        private bool _timerRequired;
        private int _hours;
        private int _minutes;
        private int _seconds;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public bool TimerRequired
        {
            get { return _timerRequired; }
            set
            {
                _timerRequired = value;
                OnPropertyChanged();
            }
        }

        public int Hours
        {
            get { return _hours; }
            set
            {
                _hours = value;
                OnPropertyChanged();
            }
        }

        public int Minutes
        {
            get { return _minutes; }
            set
            {
                _minutes = value;
                OnPropertyChanged();
            }
        }

        public int Seconds
        {
            get { return _seconds; }
            set
            {
                _seconds = value;
                OnPropertyChanged();
            }
        }

        public StepViewModel(string name)
        {
            Name = name;
            Description = string.Empty;
            TimerRequired = false;
            Hours = 0;
            Minutes = 0;
            Seconds = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class IngredientViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _quantity;
        private string _unit;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }

        public string Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
