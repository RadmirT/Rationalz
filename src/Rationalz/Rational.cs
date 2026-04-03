using System.Globalization;
using System.Numerics;

namespace Rationalz;

/// <summary>
/// Представляет рациональное число в виде несократимой дроби <c>Numerator / Denominator</c>.
/// </summary>
/// <typeparam name="T">Целочисленный тип числителя и знаменателя.</typeparam>
public readonly partial struct Rational<T>
    where T : struct, IBinaryInteger<T>
{
    /// <summary>
    /// Создаёт рациональное число и приводит дробь к каноническому виду.
    /// </summary>
    /// <param name="numerator">Числитель.</param>
    /// <param name="denominator">Знаменатель.</param>
    /// <exception cref="DivideByZeroException">Если <paramref name="denominator"/> равен нулю.</exception>
    public Rational(T numerator, T denominator)
    {
        if (denominator == T.Zero)
        {
            throw new DivideByZeroException();
        }

        if (numerator == T.Zero)
        {
            Numerator = T.Zero;
            Denominator = T.One;
            return;
        }

        if (denominator < T.Zero)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        T gcd = GreatestCommonDivisor(T.Abs(numerator), denominator);

        Numerator = numerator / gcd;
        Denominator = denominator / gcd;
    }

    /// <summary>
    /// Создаёт рациональное число и инициализирует его значением.
    /// </summary>
    /// <param name="value">Значение.</param>
    public Rational(T value)
        : this(value, T.One)
    {
    }
        

    /// <summary>
    /// Возвращает числитель рационального числа.
    /// </summary>
    public T Numerator { get; }

    /// <summary>
    /// Возвращает положительный знаменатель рационального числа.
    /// </summary>
    public T Denominator { get; }

    /// <summary>
    /// Возвращает строковое представление рационального числа.
    /// </summary>
    public override string? ToString()
    {
        return Denominator == T.One ? Numerator.ToString() : $"{Numerator.ToString()}/{Denominator.ToString()}";
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Numerator, Denominator);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Rational<T> other && Equals(other);
    }

    /// <summary>
    /// Сравнивает текущую дробь с другим объектом.
    /// </summary>
    /// <param name="obj">Объект для сравнения.</param>
    /// <returns>Отрицательное число, ноль или положительное число.</returns>
    /// <exception cref="ArgumentException">Если <paramref name="obj"/> не является <see cref="Rational{T}"/>.</exception>
    public int CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }

        if (obj is not Rational<T> other)
        {
            throw new ArgumentException($"Object must be of type {typeof(Rational<T>)}.", nameof(obj));
        }

        return CompareTo(other);
    }

    /// <summary>
    /// Сравнивает текущую дробь с другой дробью.
    /// </summary>
    /// <param name="other">Другая дробь.</param>
    /// <returns>Отрицательное число, ноль или положительное число.</returns>
    public int CompareTo(Rational<T> other)
    {
        if (Equals(other))
        {
            return 0;
        }

        int sign1 = T.Sign(Numerator);
        int sign2 = T.Sign(other.Numerator);

        if (sign1 != sign2)
        {
            return sign1.CompareTo(sign2);
        }

        T left = Numerator * other.Denominator;
        T right = other.Numerator * Denominator;

        return left.CompareTo(right);
    }

    /// <summary>
    /// Проверяет равенство текущей дроби другой дроби.
    /// </summary>
    /// <param name="other">Другая дробь.</param>
    /// <returns><see langword="true"/>, если дроби равны.</returns>
    public bool Equals(Rational<T> other)
    {
        return Numerator == other.Numerator && Denominator == other.Denominator;
    }

    /// <summary>
    /// Форматирует дробь с учётом указанного формата и провайдера.
    /// </summary>
    /// <param name="format">Формат числителя и знаменателя.</param>
    /// <param name="formatProvider">Провайдер форматирования.</param>
    /// <returns>Строковое представление дроби.</returns>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Denominator == T.One
            ? Numerator.ToString(format, formatProvider)
            : $"{Numerator.ToString(format, formatProvider)}/{Denominator.ToString(format, formatProvider)}";
    }

    /// <summary>
    /// Пытается записать строковое представление дроби в указанный буфер.
    /// </summary>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        var text = ToString(format.Length == 0 ? null : new string(format), provider);

        if (text.AsSpan().TryCopyTo(destination))
        {
            charsWritten = text.Length;
            return true;
        }

        charsWritten = 0;
        return false;
    }

    /// <summary>
    /// Складывает две дроби.
    /// </summary>
    public static Rational<T> operator +(Rational<T> left, Rational<T> right)
    {
        if (left.Numerator == T.Zero)
        {
            return right;
        }

        if (right.Numerator == T.Zero)
        {
            return left;
        }

        T gcd = GreatestCommonDivisor(left.Denominator, right.Denominator);

        T leftMultiplier = right.Denominator / gcd;
        T rightMultiplier = left.Denominator / gcd;

        T numerator = (left.Numerator * leftMultiplier) + (right.Numerator * rightMultiplier);
        T denominator = left.Denominator * leftMultiplier;

        return new Rational<T>(numerator, denominator);
    }

    /// <summary>
    /// Проверяет равенство двух дробей.
    /// </summary>
    public static bool operator ==(Rational<T> left, Rational<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Проверяет неравенство двух дробей.
    /// </summary>
    public static bool operator !=(Rational<T> left, Rational<T> right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Проверяет, что левая дробь больше правой.
    /// </summary>
    public static bool operator >(Rational<T> left, Rational<T> right)
    {
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Проверяет, что левая дробь больше или равна правой.
    /// </summary>
    public static bool operator >=(Rational<T> left, Rational<T> right)
    {
        return left.CompareTo(right) >= 0;
    }

    /// <summary>
    /// Проверяет, что левая дробь меньше правой.
    /// </summary>
    public static bool operator <(Rational<T> left, Rational<T> right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Проверяет, что левая дробь меньше или равна правой.
    /// </summary>
    public static bool operator <=(Rational<T> left, Rational<T> right)
    {
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Уменьшает дробь на единицу.
    /// </summary>
    public static Rational<T> operator --(Rational<T> value)
    {
        return value - One;
    }

    /// <summary>
    /// Делит одну дробь на другую.
    /// </summary>
    /// <exception cref="DivideByZeroException">Если правая дробь равна нулю.</exception>
    public static Rational<T> operator /(Rational<T> left, Rational<T> right)
    {
        if (right.Numerator == T.Zero)
        {
            throw new DivideByZeroException();
        }

        if (left.Numerator == T.Zero)
        {
            return Zero;
        }

        T gcd1 = GreatestCommonDivisor(T.Abs(left.Numerator), T.Abs(right.Numerator));
        T gcd2 = GreatestCommonDivisor(left.Denominator, right.Denominator);

        T leftNumerator = left.Numerator / gcd1;
        T rightNumerator = right.Numerator / gcd1;
        T leftDenominator = left.Denominator / gcd2;
        T rightDenominator = right.Denominator / gcd2;

        T numerator = leftNumerator * rightDenominator;
        T denominator = leftDenominator * rightNumerator;

        return new Rational<T>(numerator, denominator);
    }

    /// <summary>
    /// Увеличивает дробь на единицу.
    /// </summary>
    public static Rational<T> operator ++(Rational<T> value)
    {
        return value + One;
    }

    /// <summary>
    /// Возвращает остаток от деления двух дробей.
    /// </summary>
    /// <exception cref="DivideByZeroException">Если правая дробь равна нулю.</exception>
    public static Rational<T> operator %(Rational<T> left, Rational<T> right)
    {
        if (right.Numerator == T.Zero)
        {
            throw new DivideByZeroException();
        }

        Rational<T> q = Truncate(left / right);
        return left - (q * right);
    }

    /// <summary>
    /// Перемножает две дроби.
    /// </summary>
    public static Rational<T> operator *(Rational<T> left, Rational<T> right)
    {
        if (left.Numerator == T.Zero || right.Numerator == T.Zero)
        {
            return Zero;
        }

        T gcd1 = GreatestCommonDivisor(T.Abs(left.Numerator), right.Denominator);
        T gcd2 = GreatestCommonDivisor(T.Abs(right.Numerator), left.Denominator);

        T leftNumerator = left.Numerator / gcd1;
        T rightNumerator = right.Numerator / gcd2;
        T leftDenominator = left.Denominator / gcd2;
        T rightDenominator = right.Denominator / gcd1;

        return new Rational<T>(
            leftNumerator * rightNumerator,
            leftDenominator * rightDenominator);
    }

    /// <summary>
    /// Вычитает правую дробь из левой.
    /// </summary>
    public static Rational<T> operator -(Rational<T> left, Rational<T> right)
    {
        if (right.Numerator == T.Zero)
        {
            return left;
        }

        if (left.Numerator == T.Zero)
        {
            return -right;
        }

        T gcd = GreatestCommonDivisor(left.Denominator, right.Denominator);

        T leftMultiplier = right.Denominator / gcd;
        T rightMultiplier = left.Denominator / gcd;

        T numerator = (left.Numerator * leftMultiplier) - (right.Numerator * rightMultiplier);
        T denominator = left.Denominator * leftMultiplier;

        return new Rational<T>(numerator, denominator);
    }

    /// <summary>
    /// Меняет знак дроби.
    /// </summary>
    public static Rational<T> operator -(Rational<T> value)
    {
        return new Rational<T>(-value.Numerator, value.Denominator);
    }

    /// <summary>
    /// Возвращает дробь без изменения знака.
    /// </summary>
    public static Rational<T> operator +(Rational<T> value)
    {
        return value;
    }

    private static Rational<T> Truncate(Rational<T> value)
    {
        return new Rational<T>(value.Numerator / value.Denominator, T.One);
    }

    private static T GreatestCommonDivisor(T a, T b)
    {
        a = T.Abs(a);
        b = T.Abs(b);

        while (b != T.Zero)
        {
            T t = a % b;
            a = b;
            b = t;
        }

        return a == T.Zero ? T.One : a;
    }
}
