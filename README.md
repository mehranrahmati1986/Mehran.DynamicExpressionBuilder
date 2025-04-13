راهنما به 3 زبان انگلیسی فارسی و عربی

---

# Dynamic Expression Builder

A powerful and flexible library for building dynamic filter expressions in C#. It supports:
- Complex filtering scenarios like `Equal`, `Contains`, `StartsWith`, and more.
- Handling different data types (`string`, `int`, `enum`, etc.).
- Deep property support, including nested properties and collections (with `Any`).
- Seamless integration with Entity Framework Core (EF Core) to generate SQL-compatible expressions.

---

## 📦 Installation

Add the `DynamicExpressionBuilder` class to your project. You can either clone the repository or copy the code directly.

---

## 🚀 Quick Start

To use the `DynamicExpressionBuilder`, create a list of `FilterGroup` objects and pass them to the `Build` method to generate a dynamic `Expression<Func<T, bool>>`.

### Example:

```csharp
var filters = new List<FilterGroup>
{
    new()
    {
        Filters = new List<Filter>
        {
            new()
            {
                FieldName = "FullName",
                Operator = Operator.Contains,
                Value = "john",
                AndOr = AndOr.And
            }
        },
        GroupAndOr = AndOr.And
    }
};

var expression = DynamicExpressionBuilder.Build<User>(filters);
var results = dbContext.Users.Where(expression).ToList();
```

In the example above:
- We filter the `Users` collection where `FullName` contains the string "john".
- The resulting expression is used in a LINQ query with EF Core.

---

## 🔧 Filtering Model

The filter model is structured like this:

```json
{
  "filterGroups": [
    {
      "filters": [
        {
          "fieldName": "fullName",
          "operator": 1,
          "value": "john",
          "andOr": 1
        }
      ],
      "groupAndOr": 1
    }
  ],
  "pageIndex": 1,
  "pageSize": 5,
  "sortOptions": []
}
```

Where:
- `filterGroups`: A list of filter groups.
- `filters`: Each filter specifies the property, operator, value, and logical operator (AND/OR).
- `groupAndOr`: Defines whether to combine filters in this group using AND or OR.

---

## 🛠 Enums

### `Operator`
Defines the supported operators for filtering.

```csharp
public enum Operator : byte
{
    Equal = 1,
    NotEqual = 2,
    Greater = 3,
    GreaterOrEqual = 4,
    Less = 5,
    LessOrEqual = 6,
    Contains = 7,
    StartsWith = 8,
    EndsWith = 9
}
```

### `AndOr`
Defines the logical operators to combine multiple filters.

```csharp
public enum AndOr : byte
{
    And = 1,
    Or = 2
}
```

---

##  Type Conversion

- **Automatic Conversion**: Filter values (passed as strings) are automatically converted to the correct property type (e.g., `int`, `DateTime`, `enum`).
- **Enum Parsing**: Enum values are parsed in a case-insensitive manner.
- **String Matching**: String filters (like `Contains`, `StartsWith`) are case-insensitive.

---

## 🧳 Nested Properties & Collections Support

You can filter nested properties or collections (like `Any`) with ease:

```csharp
new Filter
{
    FieldName = "Orders.TotalAmount",
    Operator = Operator.GreaterOrEqual,
    Value = "100",
    AndOr = AndOr.And
}
```

This generates a LINQ expression like:

```csharp
x => x.Orders.Any(o => o.TotalAmount >= 100)
```

---

## 🔄 Using with Entity Framework Core

This library generates valid LINQ expressions that can be used directly with EF Core, enabling dynamic filtering in database queries:

```csharp
var filteredResults = dbContext.Users.Where(expression).ToList();
```

The `expression` returned from `DynamicExpressionBuilder.Build<T>()` is used to filter the data in the database at runtime.

---

## 📚 Additional Features

- **Performance**: The expression is built dynamically and efficiently.
- **Flexible**: Can handle complex filtering, including combining multiple filters and groups.
- **Customizable**: Easy to extend with additional operators or types.

---

## ⚙️ Configuration & Extensions

Feel free to extend the expression builder to support additional scenarios, such as:
- Adding custom operators.
- Handling more complex type conversions.

---

## 🏗 Contributing

If you find any issues or want to improve the library, feel free to submit a pull request!

---

## 📝 License

This project is licensed under the MIT License.

---

### 🔑 Key Points:
- **Seamless**: Automatically converts filter values to the correct types.
- **Nested Support**: Filter nested properties and collections (e.g., `Any`).
- **EF Core**: Fully works with EF Core for dynamic queries in SQL.

---


---

## 🌍 راهنمای فارسی

# **Dynamic Expression Builder**

کتابیخانه‌ای قدرتمند و انعطاف‌پذیر برای ساخت عبارات فیلتر دینامیک در C#. این کتابخانه از ویژگی‌های زیر پشتیبانی می‌کند:
- سناریوهای پیچیده فیلترینگ مانند `Equal`، `Contains`، `StartsWith` و بیشتر.
- پشتیبانی از انواع مختلف داده‌ها (`string`، `int`، `enum` و غیره).
- پشتیبانی از ویژگی‌های تو در تو، از جمله فیلترهای مجموعه‌ها (با استفاده از `Any`).
- سازگاری کامل با Entity Framework Core (EF Core) برای تولید عبارات سازگار با SQL.

---

### 📦 نصب

کلاس `DynamicExpressionBuilder` را به پروژه خود اضافه کنید. می‌توانید مخزن را کلون کرده یا کد را مستقیماً کپی کنید.

---

### 🚀 شروع سریع

برای استفاده از `DynamicExpressionBuilder`، یک لیست از `FilterGroup` ها بسازید و آن را به متد `Build` ارسال کنید تا یک `Expression<Func<T, bool>>` دینامیک تولید کنید.

#### مثال:

```csharp
var filters = new List<FilterGroup>
{
    new()
    {
        Filters = new List<Filter>
        {
            new()
            {
                FieldName = "FullName",
                Operator = Operator.Contains,
                Value = "john",
                AndOr = AndOr.And
            }
        },
        GroupAndOr = AndOr.And
    }
};

var expression = DynamicExpressionBuilder.Build<User>(filters);
var results = dbContext.Users.Where(expression).ToList();
```

در این مثال:
- ما مجموعه کاربران را فیلتر می‌کنیم که `FullName` حاوی رشته "john" باشد.
- عبارت تولیدشده در یک پرس‌وجوی LINQ با EF Core استفاده می‌شود.

---

### 🔧 مدل فیلترینگ

مدل فیلتر به این صورت ساختاربندی شده است:

```json
{
  "filterGroups": [
    {
      "filters": [
        {
          "fieldName": "fullName",
          "operator": 1,
          "value": "john",
          "andOr": 1
        }
      ],
      "groupAndOr": 1
    }
  ],
  "pageIndex": 1,
  "pageSize": 5,
  "sortOptions": []
}
```

جایی که:
- `filterGroups`: یک لیست از گروه‌های فیلتر.
- `filters`: هر فیلتر مشخص می‌کند که چه ویژگی، اپراتور و مقدار را فیلتر کند.
- `groupAndOr`: مشخص می‌کند که فیلترها در این گروه با AND یا OR ترکیب شوند.

---

### 🛠 انواع داده‌ها

### `Operator`
اپراتورهای پشتیبانی‌شده برای فیلترینگ.

```csharp
public enum Operator : byte
{
    Equal = 1,
    NotEqual = 2,
    Greater = 3,
    GreaterOrEqual = 4,
    Less = 5,
    LessOrEqual = 6,
    Contains = 7,
    StartsWith = 8,
    EndsWith = 9
}
```

### `AndOr`
اپراتورهای منطقی برای ترکیب چندین فیلتر.

```csharp
public enum AndOr : byte
{
    And = 1,
    Or = 2
}
```

---

###  تبدیل انواع داده‌ها

- **تبدیل خودکار**: مقادیر فیلتر (که به صورت رشته ارسال می‌شوند) به طور خودکار به نوع صحیح ویژگی‌ها تبدیل می‌شوند (مثل `int`، `DateTime`، `enum`).
- **تجزیه Enum**: مقادیر Enum به صورت بی‌تفاوت به حروف بزرگ/کوچک تجزیه می‌شوند.
- **مقایسه رشته‌ای**: فیلترهای رشته‌ای (مانند `Contains`، `StartsWith`) به صورت بی‌تفاوت به حروف بزرگ/کوچک انجام می‌شوند.

---

### 🧳 پشتیبانی از ویژگی‌های تو در تو و مجموعه‌ها

شما می‌توانید فیلترهایی برای ویژگی‌های تو در تو یا مجموعه‌ها (مثل `Any`) ایجاد کنید:

```csharp
new Filter
{
    FieldName = "Orders.TotalAmount",
    Operator = Operator.GreaterOrEqual,
    Value = "100",
    AndOr = AndOr.And
}
```

این یک عبارت LINQ مانند این تولید می‌کند:

```csharp
x => x.Orders.Any(o => o.TotalAmount >= 100)
```

---

### 🔄 استفاده با Entity Framework Core

این کتابخانه عبارات LINQ معتبر تولید می‌کند که می‌توانند به راحتی با EF Core استفاده شوند و فیلترهای دینامیک را در پرس‌وجوهای پایگاه‌داده انجام دهند:

```csharp
var filteredResults = dbContext.Users.Where(expression).ToList();
```

عبارتی که از `DynamicExpressionBuilder.Build<T>()` تولید می‌شود برای فیلتر کردن داده‌ها در پایگاه‌داده استفاده می‌شود.

---

### 📚 ویژگی‌های اضافی

- **عملکرد**: عبارت به طور دینامیک و کارآمد ساخته می‌شود.
- **انعطاف‌پذیر**: قابلیت پشتیبانی از فیلترینگ پیچیده، از جمله ترکیب چندین فیلتر و گروه.
- **قابل تنظیم**: به راحتی قابل گسترش با اپراتورهای یا انواع اضافی.

---

### 🧳 پیکربندی و افزونه‌ها

می‌توانید کتابخانه را برای پشتیبانی از سناریوهای اضافی گسترش دهید، مانند:
- افزودن اپراتورهای سفارشی.
- پشتیبانی از تبدیل‌های پیچیده‌تر انواع.

---

### 🏗 مشارکت

اگر مشکلی پیدا کردید یا می‌خواهید کتابخانه را بهبود بخشید، خوشحال می‌شویم که درخواست Pull Request ارسال کنید!

---

### 📝 مجوز

این پروژه تحت مجوز MIT منتشر شده است.

---

## 🌍 النسخة العربية

# **Dynamic Expression Builder**

مكتبة قوية ومرنة لبناء تعبيرات الفلترة الديناميكية في C#. تدعم:
- سيناريوهات الفلترة المعقدة مثل `Equal`، `Contains`، `StartsWith` وأكثر.
- دعم أنواع البيانات المختلفة (`string`، `int`، `enum`، إلخ).
- دعم الخصائص المتداخلة، بما في ذلك فلاتر المجموعات (باستخدام `Any`).
- تكامل كامل مع Entity Framework Core (EF Core) لإنشاء تعبيرات متوافقة مع SQL.

---

### 📦 التثبيت

أضف فئة `DynamicExpressionBuilder` إلى مشروعك. يمكنك إما استنساخ المستودع أو نسخ الكود مباشرة.

---

### 🚀 البدء السريع

لاستخدام `DynamicExpressionBuilder`، أنشئ قائمة من `FilterGroup` وقم بتمريرها إلى طريقة `Build` لإنشاء تعبير ديناميكي من النوع `Expression<Func<T, bool>>`.

#### مثال:

```csharp
var filters = new List<FilterGroup>
{
    new()
    {
        Filters = new List<Filter>
        {
            new()
            {
                FieldName = "FullName",
                Operator = Operator.Contains,
                Value = "john",
                AndOr = AndOr.And
            }
        },
        GroupAndOr = AndOr.And
    }
};

var expression = DynamicExpressionBuilder.Build<User>(filters);
var results = dbContext.Users.Where(expression).ToList();
```

في هذا المثال:
- نقوم بتصفية مجموعة المستخدمين حيث يحتوي `FullName` على السلسلة "john".
- يتم استخدام التعبير الناتج في استعلام LINQ باستخدام EF Core.

---

### 🔧 نموذج الفلترة

نموذج الفلتر مُهيأ كما يلي:

```json
{
  "filterGroups": [
    {
      "filters": [
        {
          "fieldName": "fullName",
          "operator": 1,
          "value": "john",
          "andOr": 1
        }
      ],
      "groupAndOr": 1
    }
  ],
  "pageIndex": 1,
  "pageSize": 5,
  "sortOptions": []
}
```

حيث:
- `filterGroups`: قائمة بمجموعات الفلاتر.
- `filters`: كل فلتر يحدد الخاصية والعملية والقيمة التي سيتم تصفيتها.
- `groupAndOr`: يحدد كيفية دمج الفلاتر في هذه المجموعة باستخدام AND أو OR.

---

### 🛠 الأنواع

### `Operator`
يحدد العمليات المدعومة للفلترة.

```csharp
public enum Operator : byte
{
    Equal = 1,
    NotEqual = 2,
    Greater = 3,
    GreaterOrEqual = 4,
    Less = 5,
    LessOrEqual = 6,
    Contains = 7,
    StartsWith = 8,
    EndsWith = 9
}
```

### `AndOr`
يحدد العمليات المنطقية لدمج عدة فلاتر.

```csharp
public enum AndOr : byte
{
    And = 1,
    Or = 2
}
```

---

###  تحويل الأنواع

- **التحويل التلقائي**: القيم المفلترة (المعطاة كسلسلة) يتم تحويلها تلقائيًا إلى النوع الصحيح (مثل `int`، `DateTime`، `enum`).
- **تحليل الـ Enum**: يتم تحليل القيم من نوع Enum بغض النظر عن حالة الأحرف.
- **مطابقة السلاسل**: تتم عمليات الفلترة على السلاسل بشكل غير حساس لحالة الأحرف.

---

### 🧳 دعم الخصائص المتداخلة والمجموعات

يمكنك إنشاء فلاتر للخصائص المتداخلة أو المجموعات (مثل `Any`):

```csharp
new Filter
{
    FieldName = "Orders.TotalAmount",
    Operator = Operator.GreaterOrEqual,
    Value = "100",
    AndOr = AndOr.And
}
```

يتم إنتاج تعبير LINQ مشابه لهذا:

```csharp
x => x.Orders.Any(o => o.TotalAmount >= 100)
```

---

### 🔄 الاستخدام مع Entity Framework Core

يُنتج هذا المكتبة تعبيرات LINQ صحيحة يمكن استخدامها مع EF Core، مما يتيح لك إنشاء استعلامات ديناميكية في قاعدة البيانات:

```csharp
var filteredResults = dbContext.Users.Where(expression).ToList();
```

يتم استخدام التعبير الناتج من `DynamicExpressionBuilder.Build<T>()` لتصفية البيانات في قاعدة البيانات.

---

### 📚 المزيد من الميزات

- **أداء عالي**: يتم بناء التعبير ديناميكيًا وفعالًا.
- **مرونة**: يدعم الفلترة المعقدة بما في ذلك دمج عدة فلاتر ومجموعات.
- **قابل للتخصيص**: سهل التوسيع مع عمليات أو أنواع إضافية.

---

### 🧳 التكوين والإضافات

يمكنك توسيع المكتبة لدعم سيناريوهات إضافية مثل:
- إضافة عمليات مخصصة.
- دعم تحويلات الأنواع المعقدة.

---

### 🏗 المساهمة

إذا وجدت أي مشاكل أو ترغب في تحسين المكتبة، يمكنك تقديم طلب سحب!

---

### 📝 الترخيص

يتم ترخيص هذا المشروع بموجب ترخيص MIT.

---
Made with ❤️ by Mehran Ghaederahmat
