using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ZipArchiver.Converters;

public class ActiveButtonConverter : IValueConverter {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is bool isActive) {
            return isActive ? Brushes.Red : Brushes.Gray;
        }
        return Brushes.Red;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}