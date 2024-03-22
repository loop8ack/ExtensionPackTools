namespace ExtensionManager.VisualStudio.Adapter.Generator.Internal.Utils;

internal static class LinqExtensions
{
    public static TResult[] SelectArray<TSource, TResult>(this IReadOnlyList<TSource> source, Func<TSource, TResult> selector)
    {
        var result = new TResult[source.Count];

        for (var i = 0; i < source.Count; i++)
            result[i] = selector(source[i]);

        return result;
    }
}
