using System;
using System.Globalization;
using System.Windows.Data;

namespace CraftingCalculator.ViewModel.CustomConverters
{
    class NestedControlHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                       System.Globalization.CultureInfo culture)
        {
            double height = (double)value;

            if (value != null)
            {
                height = height * 0.75;
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
