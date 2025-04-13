using Mehran.SearchFilterExpression.Basic;
using Mehran.SearchFilterExpression.Enums;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Mehran.SearchFilterExpression;

/// <summary>
/// تولید اکسپرشن داینامیک برای فیلتر کردن داده‌ها
/// </summary>
public static class DynamicExpressionBuilder
{
    public static Expression<Func<T, bool>> Build<T>(IEnumerable<FilterGroup> filterGroups)
    {
        if (filterGroups == null || !filterGroups.Any())
            return x => true;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression finalExpr = null;

        foreach (var group in filterGroups)
        {
            Expression groupExpr = null;

            foreach (var filter in group.Filters)
            {
                var expr = GetExpression(parameter, filter);
                groupExpr = groupExpr == null ? expr : Combine(groupExpr, expr, filter.AndOr);
            }

            finalExpr = finalExpr == null ? groupExpr : Combine(finalExpr, groupExpr, group.GroupAndOr);
        }

        return Expression.Lambda<Func<T, bool>>(finalExpr!, parameter);
    }

    private static Expression GetExpression(Expression parameter, Filter filter)
    {
        var parts = filter.FieldName.Split('.');
        Expression current = parameter;

        for (int i = 0; i < parts.Length; i++)
        {
            var prop = current.Type
                .GetProperty(parts[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) 
                ?? throw new InvalidOperationException($"Property '{parts[i]}' not found on '{current.Type.Name}'");

            bool isEnumerable = prop.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType);

            if (isEnumerable && prop.PropertyType.IsGenericType)
            {
                var itemType = prop.PropertyType.GetGenericArguments()[0];
                var innerParam = Expression.Parameter(itemType, "y");
                var remainingPath = string.Join('.', parts.Skip(i + 1));
                var innerExpr = GetExpression(innerParam, filter with { FieldName = remainingPath });

                var anyMethod = typeof(Enumerable).GetMethods()
                    .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(itemType);

                return Expression.Call(anyMethod, Expression.PropertyOrField(current, parts[i]), Expression.Lambda(innerExpr, innerParam));
            }

            current = Expression.PropertyOrField(current, parts[i]);
        }

        var targetType = Nullable.GetUnderlyingType(current.Type) ?? current.Type;
        object convertedValue = null;

        try
        {
            if (filter.Value is null)
                convertedValue = null;
            else if (targetType.IsEnum)
                convertedValue = Enum.Parse(targetType, filter.Value.ToString()!, ignoreCase: true);
            else
            {
                var converter = TypeDescriptor.GetConverter(targetType);
                if (converter.CanConvertFrom(filter.Value.GetType()))
                    convertedValue = converter.ConvertFrom(filter.Value);
                else
                    convertedValue = converter.ConvertFromInvariantString(filter.Value.ToString());
            }
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Cannot convert '{filter.Value}' to type '{targetType.Name}'", ex);
        }

        var constant = Expression.Constant(convertedValue, current.Type);

        if (current.Type == typeof(string))
        {
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
            current = Expression.Call(current, toLowerMethod);
            constant = Expression.Constant(convertedValue?.ToString()?.ToLower(), typeof(string));
        }

        return filter.Operator switch
        {
            Operator.Equal => Expression.Equal(current, constant),
            Operator.NotEqual => Expression.NotEqual(current, constant),
            Operator.Greater => Expression.GreaterThan(current, constant),
            Operator.GreaterOrEqual => Expression.GreaterThanOrEqual(current, constant),
            Operator.Less => Expression.LessThan(current, constant),
            Operator.LessOrEqual => Expression.LessThanOrEqual(current, constant),
            Operator.Contains => Expression.Call(current, nameof(string.Contains), null, constant),
            Operator.StartsWith => Expression.Call(current, nameof(string.StartsWith), null, constant),
            Operator.EndsWith => Expression.Call(current, nameof(string.EndsWith), null, constant),
            _ => throw new NotSupportedException($"Operator '{filter.Operator}' is not supported.")
        };
    }

    private static Expression Combine(Expression left, Expression right, AndOr op) 
        => op == AndOr.And ? Expression.AndAlso(left, right) : Expression.OrElse(left, right);
}




