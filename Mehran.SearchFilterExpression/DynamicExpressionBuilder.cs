using Mehran.SearchFilterExpression.Basic;
using Mehran.SearchFilterExpression.Enums;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Mehran.SearchFilterExpression;

/// <summary>
/// تولید اکسپرشن داینامیک برای فیلتر کردن داده‌ها
/// </summary>
public class DynamicExpressionBuilder
{
    public static Expression<Func<T, bool>> Build<T>(IEnumerable<FilterGroup> filterGroups)
    {
        if (filterGroups == null || !filterGroups.Any())
            return x => true;

        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        Expression combined = null;

        foreach (FilterGroup group in filterGroups)
        {
            Expression groupExpr = null;

            // اگر فیلترها به صورت یک مجموعه ساده (بدون گروه‌بندی تو در تو) باشند.
            if (IsFlatCollectionFilterGroup<T>(group.Filters, out Expression collectionExpr, out ParameterExpression itemParam, out Expression conditionExpr))
            {
                MethodInfo anyMethod = typeof(Enumerable).GetMethods()
                    .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(itemParam.Type);

                MethodCallExpression anyExpr = Expression.Call(anyMethod, collectionExpr, Expression.Lambda(conditionExpr, itemParam));
                groupExpr = anyExpr;
            }
            else
            {
                // برای هر فیلتر در گروه
                foreach (Filter filter in group.Filters)
                {
                    Expression binary = GetNestedPropertyExpression(parameter, filter.Key, filter.Value, filter.Operator);
                    groupExpr = groupExpr == null ? binary
                        : filter.AndOr == AndOr.Or
                            ? Expression.OrElse(groupExpr, binary)
                            : Expression.AndAlso(groupExpr, binary);
                }
            }

            // اگر در گروه نیاز به پرانتز باشد
            if (group.UseParentheses)
            {
                groupExpr = Expression.Call(
                    typeof(Expression).GetMethod("Quote", [typeof(Expression)]),
                    groupExpr
                );
            }

            // ترکیب گروه‌ها با عملیات AND/OR
            combined = combined == null ? groupExpr
                : group.GroupAndOr == AndOr.Or
                    ? Expression.OrElse(combined, groupExpr)
                    : Expression.AndAlso(combined, groupExpr);
        }

        return Expression.Lambda<Func<T, bool>>(combined!, parameter);
    }

    private static bool IsFlatCollectionFilterGroup<T>(List<Filter> filters, out Expression collectionExpr, out ParameterExpression itemParam, out Expression conditionExpr)
    {
        collectionExpr = null!;
        itemParam = null!;
        conditionExpr = null!;

        if (!filters.All(f => f.Key.Contains('.') && f.Key.Split('.').Length == 2)) return false;

        string rootName = filters.First().Key.Split('.')[0];
        PropertyInfo propInfo = typeof(T).GetProperties()
            .FirstOrDefault(p => string.Equals(p.Name, rootName, StringComparison.OrdinalIgnoreCase));

        if (propInfo == null) return false;
        if (!typeof(IEnumerable).IsAssignableFrom(propInfo.PropertyType) || propInfo.PropertyType == typeof(string)) return false;

        Type itemType = propInfo.PropertyType.GetGenericArguments().FirstOrDefault();
        if (itemType == null) return false;

        ParameterExpression param = Expression.Parameter(itemType, "c");
        Expression innerCombined = null;

        foreach (Filter filter in filters)
        {
            string innerPropName = filter.Key.Split('.')[1];
            PropertyInfo innerPropInfo = itemType.GetProperty(innerPropName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (innerPropInfo == null) return false;

            MemberExpression member = Expression.PropertyOrField(param, innerPropName);
            Type targetType = Nullable.GetUnderlyingType(member.Type) ?? member.Type;

            object convertedValue = targetType.IsEnum
                ? Enum.Parse(targetType, filter.Value.ToString()!)
                : Convert.ChangeType(filter.Value, targetType);

            ConstantExpression constant = Expression.Constant(convertedValue, member.Type);

            Expression binary = filter.Operator switch
            {
                Operator.Equal => Expression.Equal(member, constant),
                Operator.NotEqual => Expression.NotEqual(member, constant),
                Operator.Greater => Expression.GreaterThan(member, constant),
                Operator.GreaterOrEqual => Expression.GreaterThanOrEqual(member, constant),
                Operator.Less => Expression.LessThan(member, constant),
                Operator.LessOrEqual => Expression.LessThanOrEqual(member, constant),
                Operator.Contains => Expression.Call(member, typeof(string).GetMethod("Contains", [typeof(string)])!, constant),
                Operator.StartsWith => Expression.Call(member, typeof(string).GetMethod("StartsWith", [typeof(string)])!, constant),
                Operator.EndsWith => Expression.Call(member, typeof(string).GetMethod("EndsWith", [typeof(string)])!, constant),
                _ => throw new NotSupportedException($"Operator '{filter.Operator}' is not supported.")
            };


            innerCombined = innerCombined == null ? binary
                : filter.AndOr == AndOr.Or
                    ? Expression.OrElse(innerCombined, binary)
                    : Expression.AndAlso(innerCombined, binary);
        }

        collectionExpr = Expression.PropertyOrField(Expression.Parameter(typeof(T), "x"), rootName);
        itemParam = param;
        conditionExpr = innerCombined!;
        return true;
    }

    public static Expression GetNestedPropertyExpression(Expression parameter, string propertyPath, object value, Operator op)
    {
        string[] parts = propertyPath.Split('.');
        Expression current = parameter;

        for (int i = 0; i < parts.Length; i++)
        {
            PropertyInfo propInfo = current.Type
                .GetProperties()
                .FirstOrDefault(p => string.Equals(p.Name, parts[i], StringComparison.OrdinalIgnoreCase));

            if (propInfo == null)
            {
                string availableProps = string.Join(", ", current.Type.GetProperties().Select(p => p.Name));
                throw new InvalidOperationException($"Property '{parts[i]}' not found on type '{current.Type.Name}'. Available properties: {availableProps}");
            }

            bool isEnumerable = propInfo.PropertyType != typeof(string)
                                && typeof(IEnumerable).IsAssignableFrom(propInfo.PropertyType);

            if (isEnumerable && propInfo.PropertyType.IsGenericType)
            {
                Type itemType = propInfo.PropertyType.GetGenericArguments()[0];
                ParameterExpression innerParam = Expression.Parameter(itemType, "y");
                string remainingPath = string.Join('.', parts.Skip(i + 1));
                Expression innerExpr = GetNestedPropertyExpression(innerParam, remainingPath, value, op);

                MethodInfo anyMethod = typeof(Enumerable).GetMethods()
                    .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(itemType);

                LambdaExpression lambda = Expression.Lambda(innerExpr, innerParam);
                return Expression.Call(anyMethod, Expression.PropertyOrField(current, parts[i]), lambda);
            }

            current = Expression.PropertyOrField(current, parts[i]);
        }

        Type targetTypeFinal = Nullable.GetUnderlyingType(current.Type) ?? current.Type;

        // بررسی و تبدیل ایمن به نوع مناسب
        object convertedValueFinal = null;
        try
        {
            if (value == null)
            {
                convertedValueFinal = null;
            }
            else
            {
                if (targetTypeFinal == typeof(string))
                {
                    // اگر نوع مقصد string باشد، به طور مستقیم تبدیل می‌کنیم.
                    convertedValueFinal = value.ToString();
                }
                else if (targetTypeFinal.IsEnum)
                {
                    convertedValueFinal = Enum.Parse(targetTypeFinal, value.ToString()!);
                }
                else
                {
                    convertedValueFinal = Convert.ChangeType(value, targetTypeFinal);
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Error converting value '{value}' to type '{targetTypeFinal.Name}'.", ex);
        }

        ConstantExpression constantFinal = Expression.Constant(convertedValueFinal, current.Type);

        return op switch
        {
            Operator.Equal => Expression.Equal(current, constantFinal),
            Operator.NotEqual => Expression.NotEqual(current, constantFinal),
            Operator.Greater => Expression.GreaterThan(current, constantFinal),
            Operator.GreaterOrEqual => Expression.GreaterThanOrEqual(current, constantFinal),
            Operator.Less => Expression.LessThan(current, constantFinal),
            Operator.LessOrEqual => Expression.LessThanOrEqual(current, constantFinal),
            Operator.Contains => Expression.Call(current, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, constantFinal),
            Operator.StartsWith => Expression.Call(current, typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!, constantFinal),
            Operator.EndsWith => Expression.Call(current, typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!, constantFinal),
            _ => throw new NotSupportedException($"Operator '{op}' is not supported.")
        };
    }


}

