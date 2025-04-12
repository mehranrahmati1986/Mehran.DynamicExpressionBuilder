using System.ComponentModel.DataAnnotations;

namespace Mehran.SearchFilterExpression.Enums;

public enum AndOr : byte
{
    [Display(Name = "و")]
    And = 1,

    [Display(Name = "یا")]
    Or = 2,
}