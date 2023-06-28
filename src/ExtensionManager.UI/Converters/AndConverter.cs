using System.Windows.Markup;

namespace ExtensionManager.UI.Converters;

[MarkupExtensionReturnType(typeof(AndConverter))]
internal sealed class AndConverter : CombineBoolConverterBase
{
    protected override bool Combine(bool value1, bool value2) => value1 && value2;
}
