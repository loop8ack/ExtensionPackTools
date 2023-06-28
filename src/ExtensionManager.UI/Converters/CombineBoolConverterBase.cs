using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ExtensionManager.UI.Converters;

internal abstract class CombineBoolConverterBase : MarkupExtension, IMultiValueConverter
{
    public bool Empty { get; set; }
    public bool CastFallback { get; set; }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        bool result;

        if (values.Length > 0)
        {
            result = true;

            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i] as bool? ?? CastFallback;

                result = Combine(result, value);
            }
        }
        else
            result = Empty;

        return result;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;

    protected abstract bool Combine(bool value1, bool value2);
}
