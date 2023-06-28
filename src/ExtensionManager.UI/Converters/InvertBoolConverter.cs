using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ExtensionManager.UI.Converters;

[ValueConversion(typeof(bool), typeof(bool))]
[MarkupExtensionReturnType(typeof(InvertBoolConverter))]
internal sealed class InvertBoolConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Invert(value);
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Invert(value);

    private object Invert(object value)
    {
        if (value is not bool boolValue)
            return value;

        boolValue = !boolValue;

        return boolValue;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
