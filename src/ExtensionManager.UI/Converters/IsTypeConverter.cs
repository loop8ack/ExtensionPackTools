using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ExtensionManager.UI.Converters;

[ValueConversion(typeof(object), typeof(bool))]
[MarkupExtensionReturnType(typeof(IsTypeConverter))]
internal sealed class IsTypeConverter : MarkupExtension, IValueConverter
{
    public Type? Type { get; set; }

    public object? Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (Type is null || value is null)
            return false;

        var valueType = value.GetType();

        return Type.IsAssignableFrom(valueType);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
