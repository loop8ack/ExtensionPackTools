using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Utils.Reflection;

internal static class Reflect
{
    public static FieldInfo Field(Expression<Func<object>> fieldLambdaExpression)
    {
        return ExpressionReflector.Member<FieldInfo>(fieldLambdaExpression);
    }
    public static FieldInfo Field<TField>(Expression<Func<TField>> fieldLambdaExpression)
    {
        return ExpressionReflector.Member<FieldInfo>(fieldLambdaExpression);
    }

    public static PropertyInfo Property(Expression<Func<object>> propertyLambdaExpression)
    {
        return ExpressionReflector.Member<PropertyInfo>(propertyLambdaExpression);
    }
    public static PropertyInfo Property<TProperty>(Expression<Func<TProperty>> propertyLambdaExpression)
    {
        return ExpressionReflector.Member<PropertyInfo>(propertyLambdaExpression);
    }

    public static MethodInfo Method(Expression<Action> methodLambdaExpression)
    {
        return ExpressionReflector.Member<MethodInfo>(methodLambdaExpression);
    }
    public static MethodInfo Method(Expression<Func<object>> methodLambdaExpression)
    {
        return ExpressionReflector.Member<MethodInfo>(methodLambdaExpression);
    }
    public static MethodInfo Method<TResult>(Expression<Func<TResult>> methodLambdaExpression)
    {
        return ExpressionReflector.Member<MethodInfo>(methodLambdaExpression);
    }

    public static FieldInfo Field(Expression<Func<object>> fieldLambdaExpression, out object instance)
    {
        return ExpressionReflector.Member<FieldInfo>(fieldLambdaExpression, out instance);
    }
    public static FieldInfo Field<TField>(Expression<Func<TField>> fieldLambdaExpression, out object instance)
    {
        return ExpressionReflector.Member<FieldInfo>(fieldLambdaExpression, out instance);
    }

    public static PropertyInfo Property(Expression<Func<object>> propertyLambdaExpression, out object instance)
    {
        return ExpressionReflector.Member<PropertyInfo>(propertyLambdaExpression, out instance);
    }
    public static PropertyInfo Property<TProperty>(Expression<Func<TProperty>> propertyLambdaExpression, out object instance)
    {
        return ExpressionReflector.Member<PropertyInfo>(propertyLambdaExpression, out instance);
    }

    public static MethodInfo Method(Expression<Action> methodLambdaExpression, out object instance)
    {
        return ExpressionReflector.Member<MethodInfo>(methodLambdaExpression, out instance);
    }
    public static MethodInfo Method(Expression<Func<object>> methodLambdaExpression, out object instance)
    {
        return ExpressionReflector.Member<MethodInfo>(methodLambdaExpression, out instance);
    }
    public static MethodInfo Method<TResult>(Expression<Func<TResult>> methodLambdaExpression, out object instance)
    {
        return ExpressionReflector.Member<MethodInfo>(methodLambdaExpression, out instance);
    }

    public static IEnumerable<PropertyInfo> PropertyChain(Expression<Func<object>> propertyLambdaExpression)
    {
        return ExpressionReflector.MemberChain<PropertyInfo>(propertyLambdaExpression);
    }
    public static IEnumerable<PropertyInfo> PropertyChain<TLastProperty>(Expression<Func<TLastProperty>> propertyLambdaExpression)
    {
        return ExpressionReflector.MemberChain<PropertyInfo>(propertyLambdaExpression);
    }

    public static IEnumerable<FieldInfo> FieldChain(Expression<Func<object>> propertyLambdaExpression)
    {
        return ExpressionReflector.MemberChain<FieldInfo>(propertyLambdaExpression);
    }
    public static IEnumerable<FieldInfo> FieldChain<TLastProperty>(Expression<Func<TLastProperty>> propertyLambdaExpression)
    {
        return ExpressionReflector.MemberChain<FieldInfo>(propertyLambdaExpression);
    }

    public static IEnumerable<MethodInfo> MethodChain(Expression<Action> methodLambdaExpression)
    {
        return ExpressionReflector.MemberChain<MethodInfo>(methodLambdaExpression);
    }
    public static IEnumerable<MethodInfo> MethodChain(Expression<Func<object>> methodLambdaExpression)
    {
        return ExpressionReflector.MemberChain<MethodInfo>(methodLambdaExpression);
    }
    public static IEnumerable<MethodInfo> MethodChain<TLastReslt>(Expression<Func<TLastReslt>> methodLambdaExpression)
    {
        return ExpressionReflector.MemberChain<MethodInfo>(methodLambdaExpression);
    }

    public static IEnumerable<PropertyInfo> PropertyChain(Expression<Func<object>> propertyLambdaExpression, out object instance)
    {
        return ExpressionReflector.MemberChain<PropertyInfo>(propertyLambdaExpression, out instance);
    }
    public static IEnumerable<PropertyInfo> PropertyChain<TLastProperty>(Expression<Func<TLastProperty>> propertyLambdaExpression, out object instance)
    {
        return ExpressionReflector.MemberChain<PropertyInfo>(propertyLambdaExpression, out instance);
    }

    public static IEnumerable<FieldInfo> FieldChain(Expression<Func<object>> propertyLambdaExpression, out object instance)
    {
        return ExpressionReflector.MemberChain<FieldInfo>(propertyLambdaExpression, out instance);
    }
    public static IEnumerable<FieldInfo> FieldChain<TLastProperty>(Expression<Func<TLastProperty>> propertyLambdaExpression, out object instance)
    {
        return ExpressionReflector.MemberChain<FieldInfo>(propertyLambdaExpression, out instance);
    }

    public static IEnumerable<MethodInfo> MethodChain(Expression<Func<Action>> methodLambdaExpression, out object instance)
    {
        return ExpressionReflector.MemberChain<MethodInfo>(methodLambdaExpression, out instance);
    }
    public static IEnumerable<MethodInfo> MethodChain(Expression<Func<Func<object>>> methodLambdaExpression, out object instance)
    {
        return ExpressionReflector.MemberChain<MethodInfo>(methodLambdaExpression, out instance);
    }
    public static IEnumerable<MethodInfo> MethodChain<TLastReslt>(Expression<Func<TLastReslt>> methodLambdaExpression, out object instance)
    {
        return ExpressionReflector.MemberChain<MethodInfo>(methodLambdaExpression, out instance);
    }
}

internal static class Reflect<TOwner>
{
    public static FieldInfo Field(Expression<Func<TOwner, object>> fieldLambdaExpression)
    {
        return ExpressionReflector.Member<FieldInfo>(fieldLambdaExpression);
    }
    public static FieldInfo Field<TField>(Expression<Func<TOwner, TField>> fieldLambdaExpression)
    {
        return ExpressionReflector.Member<FieldInfo>(fieldLambdaExpression);
    }

    public static PropertyInfo Property(Expression<Func<TOwner, object>> propertyLambdaExpression)
    {
        return ExpressionReflector.Member<PropertyInfo>(propertyLambdaExpression);
    }
    public static PropertyInfo Property<TProperty>(Expression<Func<TOwner, TProperty>> propertyLambdaExpression)
    {
        return ExpressionReflector.Member<PropertyInfo>(propertyLambdaExpression);
    }

    public static MethodInfo Method(Expression<Action<TOwner>> methodLambdaExpression)
    {
        return ExpressionReflector.Member<MethodInfo>(methodLambdaExpression);
    }
    public static MethodInfo Method(Expression<Func<TOwner, object>> methodLambdaExpression)
    {
        return ExpressionReflector.Member<MethodInfo>(methodLambdaExpression);
    }
    public static MethodInfo Method<TResult>(Expression<Func<TOwner, TResult>> methodLambdaExpression)
    {
        return ExpressionReflector.Member<MethodInfo>(methodLambdaExpression);
    }

    public static IEnumerable<PropertyInfo> PropertyChain(Expression<Func<TOwner, object>> propertyLambdaExpression)
    {
        return ExpressionReflector.MemberChain<PropertyInfo>(propertyLambdaExpression);
    }
    public static IEnumerable<PropertyInfo> PropertyChain<TLastProperty>(Expression<Func<TOwner, TLastProperty>> propertyLambdaExpression)
    {
        return ExpressionReflector.MemberChain<PropertyInfo>(propertyLambdaExpression);
    }

    public static IEnumerable<FieldInfo> FieldChain(Expression<Func<TOwner, object>> propertyLambdaExpression)
    {
        return ExpressionReflector.MemberChain<FieldInfo>(propertyLambdaExpression);
    }
    public static IEnumerable<FieldInfo> FieldChain<TLastField>(Expression<Func<TOwner, TLastField>> propertyLambdaExpression)
    {
        return ExpressionReflector.MemberChain<FieldInfo>(propertyLambdaExpression);
    }

    public static IEnumerable<MethodInfo> MethodChain(Expression<Action<TOwner>> methodLambdaExpression)
    {
        return ExpressionReflector.MemberChain<MethodInfo>(methodLambdaExpression);
    }
    public static IEnumerable<MethodInfo> MethodChain(Expression<Func<TOwner, object>> methodLambdaExpression)
    {
        return ExpressionReflector.MemberChain<MethodInfo>(methodLambdaExpression);
    }
    public static IEnumerable<MethodInfo> MethodChain<TLastMethodResult>(Expression<Func<TOwner, TLastMethodResult>> methodLambdaExpression)
    {
        return ExpressionReflector.MemberChain<MethodInfo>(methodLambdaExpression);
    }
}
