using System;
using System.Globalization;
using System.Windows.Data;

namespace CraftingCalculator.Views.CustomConverters
{
    public class ControlHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                               System.Globalization.CultureInfo culture)
        {
            double height = (double)value;

            if (value != null)
            {
                height = height * 0.97;
            }
            else
            {
                height = double.NaN;
            }

            return height;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double height = (double)value;

            if (value != null)
            {
                height = height / 0.97;
            }
            else
            {
                height = double.NaN;
            }

            return height;
        }
    }
}
