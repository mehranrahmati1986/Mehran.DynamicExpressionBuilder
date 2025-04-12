using Mehran.SearchFilterExpression.Enums;

namespace Mehran.SearchFilterExpression.Basic;

public sealed record class Filter
{
    /// <summary>
    /// نام فیلد
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// عملگر مقایسه
    /// </summary>
    public Operator Operator { get; set; }

    /// <summary>
    /// مقدار فیلد
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// انتخاب شرط بین فیلتر ها
    /// </summary>
    public AndOr AndOr { get; set; } = AndOr.And;
}