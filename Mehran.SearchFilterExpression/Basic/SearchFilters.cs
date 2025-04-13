namespace Mehran.SearchFilterExpression.Basic;

/// <summary>
/// یک شرط فیلتر شامل نام فیلد، عملگر و مقدار
/// </summary>
public class SearchFilter
{
    /// <summary>
    /// لیستی از فیلتر ها
    /// </summary>
    public List<FilterGroup> FilterGroups { get; set; } = [];
}