using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace BreakMaster.Converters
{
    public class ColourVisibilityMultiConverter : IMultiValueConverter
    {
        // ===== Multibinding: [AreColorsVisible, VisibleFinalColour], parameter = this button's colour =====
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || parameter == null)
                return false;

            bool areColorsVisible = values[0] is bool flag && flag;
            string visibleFinalColour = values[1]?.ToString();
            string thisButtonColour = parameter.ToString();

            // If not allowed to show colours at all
            if (!areColorsVisible)
                return false;

            // If we're allowing any colour (e.g. after potting a red)
            if (visibleFinalColour == "Any")
                return true;

            // Otherwise, only show the exact required colour
            return visibleFinalColour == thisButtonColour;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
