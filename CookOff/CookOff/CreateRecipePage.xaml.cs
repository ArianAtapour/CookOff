using CookOff.ViewModels;

namespace CookOff;

public partial class CreateRecipePage : ContentPage
{
    public CreateRecipePage()
    {
        InitializeComponent();
        BindingContext = new CreateRecipeVM();
    }

    private bool IsNumeric(string text)
    {
        return text.All(char.IsDigit);
    }

    private void OnQuantityTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            // Check if the entered value is numeric
            if (!IsNumeric(e.NewTextValue))
            {
                // If not numeric, remove the last character from the text
                ((Entry)sender).Text = e.OldTextValue;
            }
        }
    }

    private void HandleNumericText(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            if (!IsNumeric(e.NewTextValue))
            {
                ((Entry)sender).Text = e.OldTextValue;
            }
            else
            {
                var entry = (Entry)sender;
                var newValue = e.NewTextValue.Length <= 2 ? e.NewTextValue : e.NewTextValue.Substring(0, 2);
                if (int.TryParse(newValue, out int value) && value >= 0 && value < 60)
                {
                    entry.Text = newValue;
                }
                else
                {
                    entry.Text = e.OldTextValue;
                }
            }
        }
    }

    private void OnHoursTextChanged(object sender, TextChangedEventArgs e)
    {
        HandleNumericText(sender, e);
    }

    private void OnMinutesTextChanged(object sender, TextChangedEventArgs e)
    {
        HandleNumericText(sender, e);
    }

    private void OnSecondsTextChanged(object sender, TextChangedEventArgs e)
    {
        HandleNumericText(sender, e);
    }
}
