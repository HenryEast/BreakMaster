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

            // If "AreColorsVisible" is true (e.g. after red): show all
            if (areColorsVisible)
                return true;

            // If in final sequence: only show if colour matches current required one
            return visibleFinalColour == thisButtonColour;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
