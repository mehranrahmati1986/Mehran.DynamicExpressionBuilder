using Mehran.SearchFilterExpression.Enums;

namespace Mehran.SearchFilterExpression.Basic;

/// <summary>
/// فیلتر گروهی
/// </summary>
public class FilterGroup
{
    public List<Filter> Filters { get; set; }
    public AndOr GroupAndOr { get; set; }
    public bool UseParentheses { get; set; }  // آیا پرانتز باید استفاده شود؟
}