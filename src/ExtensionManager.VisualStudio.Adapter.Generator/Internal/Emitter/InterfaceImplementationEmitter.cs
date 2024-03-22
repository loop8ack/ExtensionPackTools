using System.Linq.Expressions;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Internal.Emitter;

internal sealed class InterfaceImplementationEmitter<TInterface>
{
    private readonly ClassEmitter _emitter;

    public InterfaceImplementationEmitter(ClassEmitter emitter)
        => _emitter = emitter;

    public void Property<TProperty>(Expression<Func<TInterface, TProperty>> getPropertyExpression, Action<PropertyEmitter, PropertyInfo> emit)
    {
        var propertyInfo = GetPropertyInfo(getPropertyExpression);

        _emitter.Property(propertyInfo.Name, propertyInfo.PropertyType, emitter => emit(emitter, propertyInfo));
    }

    public MethodBuilder Method(Expression<Func<TInterface>> callMethodExpression) => EmiMethod(callMethodExpression);
    public MethodBuilder Method<TResult>(Expression<Func<TInterface, TResult>> callMethodExpression) => EmiMethod(callMethodExpression);
    private MethodBuilder EmiMethod(LambdaExpression callMethodExpression)
        => _emitter.ImplementMethod(GetMethodInfo(callMethodExpression));

    private PropertyInfo GetPropertyInfo(LambdaExpression getPropertyExpression)
    {
        if (getPropertyExpression.Body is MemberExpression { Member: PropertyInfo propertyInfo })
            return propertyInfo;

        throw new ArgumentException("Expression is not a valid property expression.", nameof(getPropertyExpression));
    }

    private MethodInfo GetMethodInfo(LambdaExpression callMethodExpression)
    {
        if (callMethodExpression.Body is MethodCallExpression { Method: var method })
            return method;

        throw new ArgumentException("Expression is not a valid method expression.", nameof(callMethodExpression));
    }
}
