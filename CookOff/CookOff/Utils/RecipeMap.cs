using CsvHelper.Configuration;
using CookOff.Models;

public sealed class RecipeMap : ClassMap<Recipe>
{
    //CSV headers for the Recipe CSV file
    public RecipeMap()
    {
        Map(m => m.RecipeID).Name("RecipeID");
        Map(m => m.Name).Name("Recipe Name");
        Map(m => m.ImagePath).Name("Image Path");
        Map(m => m.Rating).Name("Rating");
    }
}
