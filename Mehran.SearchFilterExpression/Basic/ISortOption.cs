namespace Mehran.SearchFilterExpression.Basic;

public interface ISortOption
{
    string PropertyName { get; }

    bool Descending { get; }

    int Priority { get; }
}
