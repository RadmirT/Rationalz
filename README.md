# Rationalz

`Rationalz` - библиотека для работы с рациональными числами в виде несократимой дроби `Numerator / Denominator`.

Основной тип библиотеки - `Rational<T>`, где `T` должен быть целочисленным типом, реализующим `IBinaryInteger<T>` (`int`, `long`, `BigInteger` и т.д.).

## Возможности

- автоматическая нормализация дроби и сокращение по НОД;
- хранение знаменателя в положительном виде;
- арифметические операции `+`, `-`, `*`, `/`, `%`, unary `+`, unary `-`, `++`, `--`;
- сравнение `==`, `!=`, `<`, `<=`, `>`, `>=`;
- парсинг из строк формата `"3/4"` и `"42"`;
- форматирование через `ToString(...)` и `TryFormat(...)`;
- преобразования в `double`, `float`, `decimal`;
- интеграция с generic math через `INumber<Rational<T>>`.

## Подключение

Добавьте ссылку на проект `Rationalz`:

```xml
<ProjectReference Include="src\Rationalz\Rationalz.csproj" />
```

Целевые платформы библиотеки: `.NET 7`, `.NET 8`, `.NET 9`, `.NET 10`.

## Примеры

### Создание дробей

```csharp
using Rationalz;

Rational<int> a = new(6, -8);   // -3/4
Rational<int> b = new(5);       // 5
Rational<int> c = 42;           // implicit conversion from int

Console.WriteLine(a); // -3/4
Console.WriteLine(b); // 5
Console.WriteLine(c); // 42
```

### Арифметика

```csharp
using Rationalz;

Rational<int> left = new(1, 2);
Rational<int> right = new(1, 3);

Rational<int> sum = left + right;         // 5/6
Rational<int> difference = left - right;  // 1/6
Rational<int> product = left * right;     // 1/6
Rational<int> quotient = left / right;    // 3/2
Rational<int> remainder = new Rational<int>(7, 3) % new Rational<int>(2, 3); // 1/3
```

### Сравнение

```csharp
using Rationalz;

Rational<int> x = new(1, 2);
Rational<int> y = new(2, 3);

bool less = x < y;        // true
bool equals = x == y;     // false
int order = x.CompareTo(y); // < 0
```

### Парсинг и форматирование

```csharp
using System.Globalization;
using Rationalz;

Rational<long> x = Rational<long>.Parse(" -10 / 20 ", CultureInfo.InvariantCulture);
bool parsed = Rational<int>.TryParse("7/8", CultureInfo.InvariantCulture, out Rational<int> y);

Console.WriteLine(x); // -1/2
Console.WriteLine(y.ToString("X", CultureInfo.InvariantCulture)); // 7/8
```

### Преобразование к вещественным типам

```csharp
using Rationalz;

Rational<int> value = new(3, 4);

double asDouble = (double)value;    // 0.75
float asFloat = (float)value;       // 0.75
decimal asDecimal = (decimal)value; // 0.75
```

### Generic Math

```csharp
using Rationalz;

static T Sum<T>(T left, T right)
    where T : System.Numerics.INumber<T>
{
    return left + right;
}

Rational<int> result = Sum(new Rational<int>(1, 2), new Rational<int>(1, 6));
Console.WriteLine(result); // 2/3
```

## Поведение и ограничения

- `new Rational<T>(numerator, 0)` выбрасывает `DivideByZeroException`;
- если числитель равен нулю, дробь нормализуется к `0/1`;
- если знаменатель отрицательный, знак переносится в числитель;
- `TryConvertToChecked` возвращает `false` для нецелых дробей;
- тип `T` должен быть целочисленным, дробные типы в качестве `T` не поддерживаются.

## Тесты

Запуск unit-тестов:

```bash
dotnet test .\tests\Rationalz.UnitTests\Rationalz.UnitTests.csproj
```
