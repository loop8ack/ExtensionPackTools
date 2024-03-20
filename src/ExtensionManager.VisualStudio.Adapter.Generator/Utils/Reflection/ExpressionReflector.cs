using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable disable

namespace ExtensionManager.VisualStudio.Adapter.Generator.Utils.Reflection;

internal static class ExpressionReflector
{
    public static Type[] SupportedMemberTypes { get; } = new[]
            {
                typeof(FieldInfo),
                typeof(PropertyInfo),
                typeof(MethodInfo),
            };

    public static TMemberInfo Member<TMemberInfo>(LambdaExpression memberLambdaExpression)
        where TMemberInfo : MemberInfo
    {
        object _;
        return Member<TMemberInfo>(memberLambdaExpression, false, out _);
    }
    public static TMemberInfo Member<TMemberInfo>(LambdaExpression memberLambdaExpression, out object instance)
        where TMemberInfo : MemberInfo
    {
        return Member<TMemberInfo>(memberLambdaExpression, true, out instance);
    }
    private static TMemberInfo Member<TMemberInfo>(LambdaExpression memberLambdaExpression, bool getInstance, out object instance)
        where TMemberInfo : MemberInfo
    {
        if (SupportedMemberTypes.Contains(typeof(TMemberInfo)))
            return MemberChain<TMemberInfo>(memberLambdaExpression, getInstance, out instance).Last();
        else
            throw CreateNotSupportedMemberInfoType<TMemberInfo>();
    }

    public static IEnumerable<TMemberInfo> MemberChain<TMemberInfo>(LambdaExpression memberLambdaExpression)
        where TMemberInfo : MemberInfo
    {
        object _;
        return MemberChain<TMemberInfo>(memberLambdaExpression, false, out _);
    }
    public static IEnumerable<TMemberInfo> MemberChain<TMemberInfo>(LambdaExpression memberLambdaExpression, out object instance)
        where TMemberInfo : MemberInfo
    {
        return MemberChain<TMemberInfo>(memberLambdaExpression, true, out instance);
    }
    private static IEnumerable<TMemberInfo> MemberChain<TMemberInfo>(LambdaExpression memberLambdaExpression, bool getInstance, out object instance)
        where TMemberInfo : MemberInfo
    {
        if (SupportedMemberTypes.Contains(typeof(TMemberInfo)))
        {
            var bodyExpression = GetPureExpressionBody(memberLambdaExpression);

            var isIndexerChain = typeof(TMemberInfo) == typeof(PropertyInfo)
                && bodyExpression is MethodCallExpression
                && IsIndexerGetter((bodyExpression as MethodCallExpression).Method);

            if (typeof(TMemberInfo) == typeof(MethodInfo) || isIndexerChain)
            {
                // Methoden-Ketten können nur aufgebaut werden, wenn die Methode in der Expression aufgerufen wird
                // Die resultierenden MethodCallExpressions verlangen eine gesonderte Behandlung
                var methodChain = GetMethodChain(memberLambdaExpression, getInstance, out instance);

                return isIndexerChain
                    ? methodChain.Select(GetIndexer) as IEnumerable<TMemberInfo>
                    : methodChain as IEnumerable<TMemberInfo>;
            }
            else if (bodyExpression is MemberExpression)
            {
                // Property/Field-Ketten werden in Form von MemberExpressions aufgebaut
                return GetMemberChain<TMemberInfo>(bodyExpression, getInstance, out instance);
            }
            else
                throw CreateNotSupportedMemberChainExpression<TMemberInfo>(getInstance);

        }
        else
            throw CreateNotSupportedMemberInfoType<TMemberInfo>();
    }

    private static IEnumerable<TMemberInfo> GetMemberChain<TMemberInfo>(Expression bodyExpression, bool getInstance, out object instance)
        where TMemberInfo : MemberInfo
    {
        // LambdaExpressions bauen Verkettungen von hinten auf, daher muss sie umgedreht werden
        var expressionChain = BuildExpressionChain(bodyExpression as MemberExpression, exp => exp.Expression).Reverse();

        instance = getInstance
            ? ExtractMemberInstance(ref expressionChain)
            : null;

        var memberChain = expressionChain
            .SkipWhile(exp => !(exp is MemberExpression) || !((exp as MemberExpression).Member is TMemberInfo))
            .Cast<MemberExpression>()
            .Select(m => (TMemberInfo)m.Member);

        // Wenn die Quelle eine lokale Variable ist, dann lagert der Compiler die in eine Klasse aus
        // Die zweite Expression ist daher eine FieldExpression, die vorher als korrekt erachtet wurde
        // Fix: Wenn der erste Member in einer vom Compiler generierten Klasse liegt, wird dieser ignoriert

        var isFirstMemberLocalInstanceField = memberChain
            .First()
            .DeclaringType.GetTypeInfo()
            .GetCustomAttribute<CompilerGeneratedAttribute>() != null;

        if (isFirstMemberLocalInstanceField)
            memberChain = memberChain.Skip(1);

        // Die Member-Kette muss immer einen Member haben, das verlangt die Syntax von Lambda-Expressions
        if (!memberChain.Any())
            CreateNotSupportedMemberExpression<TMemberInfo>(getInstance);

        return memberChain;
    }
    private static IEnumerable<MethodInfo> GetMethodChain(LambdaExpression memberLambdaExpression, bool getInstance, out object instance)
    {
        var bodyExpression = GetPureExpressionBody(memberLambdaExpression);
        var callExpression = bodyExpression as MethodCallExpression;

        if (callExpression != null)
        {
            // LambdaExpressions bauen Verkettungen von hinten auf, daher muss sie umgedreht werden
            var callChain = BuildExpressionChain(callExpression, exp => exp.Object).Reverse();

            instance = getInstance
                ? ExtractMethodInstance(ref callChain)
                : null;

            var methods = callChain
                .SkipWhile(exp => !(exp is MethodCallExpression))
                .Cast<MethodCallExpression>()
                .Select(exp => exp.Method);

            // Die Methoden-Kette muss immer einen Methode haben, das verlangt die Syntax von Lambda-Expressions
            if (!methods.Any())
                CreateNotSupportedMemberExpression<MethodInfo>(getInstance);

            return methods;
        }
        else
            throw CreateNotSupportedMemberChainExpression<MethodInfo>(getInstance);
    }

    private static object ExtractMemberInstance(ref IEnumerable<Expression> expressionChain)
    {
        object instance;

        var instanceMemberExpression = expressionChain.First();

        if (instanceMemberExpression is MemberExpression)
        {
            // Ist die Quelle statisch, dann gibt es keine Expression, die die Instanz beinhaltet
            // Die Methoden-Kette beginnt dann direkt mit dem ersten Methoden-Aufruf
            instance = null;
        }
        else if (instanceMemberExpression is ConstantExpression)
        {
            // Die Konstante enthält die Quell-Instanz
            expressionChain = expressionChain.Skip(1);

            var sourceExpression = instanceMemberExpression as ConstantExpression;

            if (sourceExpression.Type.GetTypeInfo().GetCustomAttribute<CompilerGeneratedAttribute>() != null)
            {
                // Bei einer lokalen Variable wird die Instanz in einer vom Compiler generierten Klasse ausgelagert
                // Die "neue" Quell-Instanz befindet sich in einem Feld der vom Compiler generierten Klasse

                instanceMemberExpression = expressionChain.FirstOrDefault() as MemberExpression;
                expressionChain = expressionChain.Skip(1);

                if (instanceMemberExpression is MemberExpression)
                {
                    var staticInstanceMember = (instanceMemberExpression as MemberExpression).Member;

                    if (staticInstanceMember is FieldInfo)
                        instance = (staticInstanceMember as FieldInfo).GetValue(sourceExpression.Value);
                    else
                        throw CreateNotSupportedInstanceMemberExpression();
                }
                else
                    throw CreateNotSupportedInstanceMemberExpression();
            }
            else
                instance = sourceExpression.Value;
        }
        else
            throw CreateNotSupportedInstanceMemberExpression();

        return instance;
    }
    private static object ExtractMethodInstance(ref IEnumerable<Expression> callChain)
    {
        object instance;
        var first = callChain.First();

        if (first is MethodCallExpression)
        {
            // Ist die Quelle statisch, dann gibt es keine Expression, die die Instanz beinhaltet
            // Die Method-Kette beginnt dann direkt mit dem ersten Member
            instance = null;
        }
        else
        {
            if (first is ConstantExpression)
            {
                // Die Konstante enthält die Quell-Instanz
                instance = (first as ConstantExpression).Value;
            }
            else if (first is MemberExpression)
            {
                // Bei einer lokalen Variable wird die Instanz in einer vom Compiler generierten Klasse ausgelagert
                // Die "neue" Quell-Instanz befindet sich in einem Feld der vom Compiler generierten Klasse

                var instanceMemberExpression = first as MemberExpression;

                if (instanceMemberExpression.Member is FieldInfo)
                {
                    var instanceMemberInstance = (instanceMemberExpression.Expression as ConstantExpression).Value;
                    instance = (instanceMemberExpression.Member as FieldInfo).GetValue(instanceMemberInstance);
                }
                else
                    throw CreateNotSupportedInstanceMemberExpression();
            }
            else
                throw CreateNotSupportedInstanceMemberExpression();

            callChain = callChain.Skip(1);
        }

        return instance;
    }

    private static Expression GetPureExpressionBody(LambdaExpression memberLambdaExpression)
    {
        var bodyExpression = null as Expression;

        if (memberLambdaExpression.Body is MemberExpression)
            bodyExpression = memberLambdaExpression.Body;
        else if (memberLambdaExpression.Body is MethodCallExpression)
            bodyExpression = memberLambdaExpression.Body;

        return bodyExpression;
    }

    private static NotSupportedException CreateNotSupportedInstanceMemberExpression()
    {
        return new NotSupportedException($"Not supported expression. The instance source must be a field, a property or a local variable");
    }
    private static NotSupportedException CreateNotSupportedMemberInfoType<TMemberInfo>()
        where TMemberInfo : MemberInfo
    {
        return new NotSupportedException($"Not supported MemberInfo-Type: {typeof(TMemberInfo)}");
    }
    private static NotSupportedException CreateNotSupportedMemberExpression<TMemberInfo>(bool withInstance)
    {
        var instanceMember = withInstance ? "obj." : "";
        var memberName = typeof(TMemberInfo).Name.Replace("Info", "");

        if (typeof(TMemberInfo) == typeof(MemberInfo))
            memberName += "()";

        return new NotSupportedException($"Not supported expression. Expression musst be like: () => {instanceMember}{memberName}");
    }
    private static NotSupportedException CreateNotSupportedMemberChainExpression<TMemberInfo>(bool withInstance)
    {
        var instanceMember = withInstance ? "obj." : "";
        var memberName = typeof(TMemberInfo).Name.Replace("Info", "");

        if (typeof(TMemberInfo) == typeof(MemberInfo))
            memberName += "()";

        return new NotSupportedException($"Not supported expression. Expression musst be like: () => {instanceMember}{memberName}1.{memberName}2.{memberName}3");
    }

    private static IEnumerable<Expression> BuildExpressionChain<TExpression>(TExpression expression, Func<TExpression, Expression> getPreviousExpression)
        where TExpression : Expression
    {
        yield return expression;

        var previousExpression = getPreviousExpression(expression);

        if (previousExpression is UnaryExpression)
        {
            // Wenn Konvertierung notwendig ist, dann sieht die Expression so aus: Convert(obj.Member)
            var unaryExpression = previousExpression as UnaryExpression;

            if (unaryExpression.NodeType == ExpressionType.Convert)
                previousExpression = unaryExpression.Operand;
            else
                throw new Exception();
        }

        if (previousExpression is TExpression)
        {
            foreach (var previous in BuildExpressionChain((TExpression)previousExpression, getPreviousExpression))
                yield return previous;
        }
        else if (previousExpression != null)
            yield return previousExpression;
    }

    private static bool IsIndexerGetter(MethodInfo getItemMethod)
    {
        return getItemMethod.DeclaringType.GetRuntimeProperties().Any(p => p.GetMethod == getItemMethod);
    }
    private static PropertyInfo GetIndexer(MethodInfo getItemMethod)
    {
        return getItemMethod.DeclaringType.GetRuntimeProperties().Single(p => p.GetMethod == getItemMethod);
    }
}
