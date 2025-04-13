using MehranSmartPaginatedList.Core.Sort;

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

    /// <summary>
    /// شماره صفحه
    /// </summary>
    public int PageIndex { get; init; } = 1;

    /// <summary>
    /// اندازه صفحه
    /// </summary>
    public int PageSize { get; init; } = 5;

    /// <summary>
    /// لیست مرتب سازی
    /// </summary>
    public IEnumerable<SortOption> SortOptions { get; init; } = [];
}