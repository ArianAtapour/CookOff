using CsvHelper.Configuration;
using CookOff.Models;

public sealed class RecipeMap : ClassMap<Recipe>
{
    public RecipeMap()
    {
        Map(m => m.RecipeID).Name("RecipeID");
        Map(m => m.Name).Name("Name");
        Map(m => m.ImagePath).Name("ImagePath");
        Map(m => m.Rating).Name("Rating");
        Map(m => m.IsSelected).Ignore(); // Ignore this property as it's not in the CSV
    }
}
