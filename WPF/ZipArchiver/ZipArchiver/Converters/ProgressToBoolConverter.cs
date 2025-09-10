using System;
using System.Globalization;
using System.Windows.Data;

namespace ZipArchiver.Converters;

public class ProgressToBoolConverter : IValueConverter {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is double progress) {
            return progress >= 100;
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}