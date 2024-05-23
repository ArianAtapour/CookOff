using CookOff.Models;
namespace CookOff
{
	//CSV Dependency
	public class CsvDependecy
	{
        private static readonly string filePath = Path.Combine(FileSystem.AppDataDirectory, "recipes.csv");

        private static void createCsvIfNotFound()
        {
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }

        public static List<Recipe> readRecipe()
        {
            createCsvIfNotFound();

            var recipes = new List<Recipe>();
            var lines = File.ReadAllLines(filePath);

            Recipe currentRecipe = null;
            foreach (var line in lines)
            {
                var values = line.Split(',');

                if (values[0] == "Recipe")
                {
                    if (currentRecipe != null)
                    {
                        recipes.Add(currentRecipe);
                    }
                    currentRecipe = new Recipe(
                        values[1],
                        values[2],
                        int.Parse(values[3])
                    );
                }
                else if (values[0] == "Step" && currentRecipe != null)
                {
                    var step = new Step(
                        values[1],
                        bool.Parse(values[2]),
                        TimeSpan.Parse(values[3])
                    );
                    currentRecipe.addStep(step);
                }
            }

            if (currentRecipe != null)
            {
                recipes.Add(currentRecipe);
            }

            return recipes;
        }

        public static void writeRecipe(List<Recipe> recipes)
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var recipe in recipes)
                {
                    writer.WriteLine($"Recipe,{recipe.getName()},{recipe.getImagePath()},{recipe.getRating()}");
                    foreach (var step in recipe.getSteps())
                    {
                        writer.WriteLine($"Step,{step.getName()},{step.getTimeReq()},{step.getTimer()}");
                    }
                }
            }
        }
    }
}


