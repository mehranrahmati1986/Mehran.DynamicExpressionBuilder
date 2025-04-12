namespace Mehran.SearchFilterExpression.Basic;

public class SortOption(string propertyName, bool descending, int priority) : ISortOption
{
    public string PropertyName { get; set; } = propertyName;

    public bool Descending { get; set; } = descending;

    public int Priority { get; set; } = priority;
}