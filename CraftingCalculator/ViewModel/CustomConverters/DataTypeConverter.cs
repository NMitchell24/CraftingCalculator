using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CraftingCalculator.ViewModel.CustomConverters
{
    class DataTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is Enum enumValue) ? DependencyProperty.UnsetValue : enumValue.GetDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
