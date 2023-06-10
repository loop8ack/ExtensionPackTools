using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ExtensionManager.UI.Converters;

[ValueConversion(typeof(object), typeof(bool))]
[MarkupExtensionReturnType(typeof(IsNullOrEmptyConverter))]
internal sealed class IsNullOrEmptyConverter : MarkupExtension, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return true;

        if (value is string s)
            return string.IsNullOrEmpty(s);

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
