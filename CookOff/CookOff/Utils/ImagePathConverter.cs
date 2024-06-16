using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using System.IO;

namespace CookOff.Utils
{
    public class ImagePathConverter : IValueConverter
    {
        // Class for converting relative image path to the project directory


        // Converts the image paths of images folder to the local project directory
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            string relativePath = value.ToString();
            string projectDirectory = GetProjectDirectory();
            string absolutePath = Path.Combine(projectDirectory, relativePath);

            return absolutePath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        // Gets the local project directory path
        private string GetProjectDirectory()
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectDir = Directory.GetParent(currentDir).Parent.Parent.Parent.Parent.Parent.FullName;
            return projectDir;
        }
    }
}
