﻿using CsvHelper.Configuration;
using CookOff.Models;

public sealed class IngredientMap : ClassMap<Ingredient>
{
    //CSV headers for the Ingredients CSV file
    public IngredientMap()
    {
        Map(m => m.RecipeID).Name("RecipeID");
        Map(m => m.Name).Name("Ingredient Name");
        Map(m => m.Unit).Name("Ingredient Unit");
        Map(m => m.Quantity).Name("Ingredient Quantity");
    }
}
