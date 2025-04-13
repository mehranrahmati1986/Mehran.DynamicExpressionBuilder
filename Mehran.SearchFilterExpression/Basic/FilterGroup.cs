using Mehran.SearchFilterExpression.Enums;

namespace Mehran.SearchFilterExpression.Basic;

/// <summary>
/// فیلتر گروهی
/// </summary>
public sealed record class FilterGroup
{
    public List<Filter> Filters { get; set; } = [];
    public AndOr GroupAndOr { get; set; }
}