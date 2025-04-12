using System.ComponentModel.DataAnnotations;

namespace Mehran.SearchFilterExpression.Enums;

/// <summary>
/// نوع مقایسه بین مقدار فیلد و مقدار ورودی
/// </summary>
public enum Operator : byte
{
    [Display(Name = "مساوی")]
    Equal = 1,

    [Display(Name = "نامساوی")]
    NotEqual = 2,

    [Display(Name = "بزرگتر")]
    Greater = 3,

    [Display(Name = "بزرگتر یا مساوی")]
    GreaterOrEqual = 4,

    [Display(Name = "کوچکتر")]
    Less = 5,

    [Display(Name = "کوچکتر یا مساوی")]
    LessOrEqual = 6,

    [Display(Name = "حاوی")]
    Contains = 7,

    [Display(Name = "شروع با")]
    StartsWith = 8,

    [Display(Name = "پایان با")]
    EndsWith = 9,
}

