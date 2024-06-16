using CsvHelper.Configuration;
using CookOff.Models;

public sealed class StepMap : ClassMap<Step>
{
    //CSV headers for the Step CSV file
    public StepMap()
    {
        Map(m => m.RecipeID).Name("RecipeID");
        Map(m => m.Description).Name("Step Description");
        Map(m => m.TimerRequired).Name("Timer Required");
        Map(m => m.Timer).Name("Timer");
    }
}
