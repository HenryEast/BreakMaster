using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace BreakMaster.Converters
{
    public class ColourMatchConverter : IValueConverter
    {
        // ===== Check if this button's colour matches the visible final colour =====
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string visibleFinalColour = value.ToString();
            string thisButtonColour = parameter.ToString();

            // Show all colours temporarily during "any colour allowed" phase
            if (visibleFinalColour == "Any")
                return true;

            return visibleFinalColour == thisButtonColour;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
