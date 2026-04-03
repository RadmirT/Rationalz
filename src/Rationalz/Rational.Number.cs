using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Rationalz;

/// <summary>
/// Реализация контракта <see cref="INumber{TSelf}"/> для <see cref="Rational{T}"/>.
/// </summary>
public readonly partial struct Rational<T> : INumber<Rational<T>>
    where T : struct, IBinaryInteger<T>
{
    /// <summary>
    /// Разбирает строку в рациональное число.
    /// </summary>
    public static Rational<T> Parse(string s, IFormatProvider? provider)
    {
        return Parse(s, NumberStyles.Integer, provider);
    }

    /// <summary>
    /// Пытается разобрать строку в рациональное число.
    /// </summary>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Rational<T> result)
    {
        return TryParse(s, NumberStyles.Integer, provider, out result);
    }

    /// <summary>
    /// Разбирает текстовый диапазон в рациональное число.
    /// </summary>
    public static Rational<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        return Parse(s, NumberStyles.Integer, provider);
    }

    /// <summary>
    /// Пытается разобрать текстовый диапазон в рациональное число.
    /// </summary>
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Rational<T> result)
    {
        return TryParse(s, NumberStyles.Integer, provider, out result);
    }

    /// <summary>
    /// Возвращает аддитивный нейтральный элемент.
    /// </summary>
    public static Rational<T> AdditiveIdentity => Zero;

    /// <summary>
    /// Возвращает мультипликативный нейтральный элемент.
    /// </summary>
    public static Rational<T> MultiplicativeIdentity => One;

    /// <summary>
    /// Возвращает абсолютное значение дроби.
    /// </summary>
    public static Rational<T> Abs(Rational<T> value)
    {
        return value.Numerator >= T.Zero
            ? value
            : new Rational<T>(T.Abs(value.Numerator), value.Denominator);
    }

    /// <summary>
    /// Проверяет, что дробь находится в каноническом виде.
    /// </summary>
    public static bool IsCanonical(Rational<T> value)
    {
        if (value.Denominator <= T.Zero)
        {
            return false;
        }

        if (value.Numerator == T.Zero)
        {
            return value.Denominator == T.One;
        }

        return GreatestCommonDivisor(T.Abs(value.Numerator), value.Denominator) == T.One;
    }

    /// <summary>
    /// Возвращает <see langword="false"/>, так как рациональное число не имеет мнимой части.
    /// </summary>
    public static bool IsComplexNumber(Rational<T> value)
    {
        return false;
    }

    /// <summary>
    /// Проверяет, что дробь является чётным целым числом.
    /// </summary>
    public static bool IsEvenInteger(Rational<T> value)
    {
        return value.Denominator == T.One && T.IsEvenInteger(value.Numerator);
    }

    /// <summary>
    /// Возвращает <see langword="true"/>, так как рациональные числа конечны.
    /// </summary>
    public static bool IsFinite(Rational<T> value)
    {
        return true;
    }

    /// <summary>
    /// Возвращает <see langword="false"/>, так как рациональное число не является мнимым.
    /// </summary>
    public static bool IsImaginaryNumber(Rational<T> value)
    {
        return false;
    }

    /// <summary>
    /// Возвращает <see langword="false"/>, так как бесконечность не представляется этим типом.
    /// </summary>
    public static bool IsInfinity(Rational<T> value)
    {
        return false;
    }

    /// <summary>
    /// Проверяет, что дробь является целым числом.
    /// </summary>
    public static bool IsInteger(Rational<T> value)
    {
        return value.Denominator == T.One;
    }

    /// <summary>
    /// Возвращает <see langword="false"/>, так как NaN не представляется этим типом.
    /// </summary>
    public static bool IsNaN(Rational<T> value)
    {
        return false;
    }

    /// <summary>
    /// Проверяет, что дробь отрицательна.
    /// </summary>
    public static bool IsNegative(Rational<T> value)
    {
        return value.Numerator < T.Zero;
    }

    /// <summary>
    /// Возвращает <see langword="false"/>, так как отрицательная бесконечность не представляется этим типом.
    /// </summary>
    public static bool IsNegativeInfinity(Rational<T> value)
    {
        return false;
    }

    /// <summary>
    /// Проверяет, что дробь не равна нулю.
    /// </summary>
    public static bool IsNormal(Rational<T> value)
    {
        return value.Numerator != T.Zero;
    }

    /// <summary>
    /// Проверяет, что дробь является нечётным целым числом.
    /// </summary>
    public static bool IsOddInteger(Rational<T> value)
    {
        return value.Denominator == T.One && T.IsOddInteger(value.Numerator);
    }

    /// <summary>
    /// Проверяет, что дробь положительна.
    /// </summary>
    public static bool IsPositive(Rational<T> value)
    {
        return value.Numerator > T.Zero;
    }

    /// <summary>
    /// Возвращает <see langword="false"/>, так как положительная бесконечность не представляется этим типом.
    /// </summary>
    public static bool IsPositiveInfinity(Rational<T> value)
    {
        return false;
    }

    /// <summary>
    /// Возвращает <see langword="true"/>, так как дробь представляет действительное число.
    /// </summary>
    public static bool IsRealNumber(Rational<T> value)
    {
        return true;
    }

    /// <summary>
    /// Возвращает <see langword="false"/>, так как субнормальные значения не применимы к дробям.
    /// </summary>
    public static bool IsSubnormal(Rational<T> value)
    {
        return false;
    }

    /// <summary>
    /// Проверяет, что дробь равна нулю.
    /// </summary>
    public static bool IsZero(Rational<T> value)
    {
        return value.Numerator == T.Zero;
    }

    /// <summary>
    /// Возвращает дробь с большим абсолютным значением.
    /// </summary>
    public static Rational<T> MaxMagnitude(Rational<T> x, Rational<T> y)
    {
        Rational<T> ax = Abs(x);
        Rational<T> ay = Abs(y);

        return ax >= ay ? x : y;
    }

    /// <summary>
    /// Возвращает дробь с большим абсолютным значением.
    /// </summary>
    public static Rational<T> MaxMagnitudeNumber(Rational<T> x, Rational<T> y)
    {
        return MaxMagnitude(x, y);
    }

    /// <summary>
    /// Возвращает дробь с меньшим абсолютным значением.
    /// </summary>
    public static Rational<T> MinMagnitude(Rational<T> x, Rational<T> y)
    {
        Rational<T> ax = Abs(x);
        Rational<T> ay = Abs(y);

        return ax <= ay ? x : y;
    }

    /// <summary>
    /// Возвращает дробь с меньшим абсолютным значением.
    /// </summary>
    public static Rational<T> MinMagnitudeNumber(Rational<T> x, Rational<T> y)
    {
        return MinMagnitude(x, y);
    }

    /// <summary>
    /// Разбирает текстовый диапазон в рациональное число с заданным числовым стилем.
    /// </summary>
    public static Rational<T> Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        if (!TryParse(s, style, provider, out Rational<T> result))
        {
            throw new FormatException("The input string was not in a correct rational format.");
        }

        return result;
    }

    /// <summary>
    /// Разбирает строку в рациональное число с заданным числовым стилем.
    /// </summary>
    public static Rational<T> Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        if (!TryParse(s, style, provider, out Rational<T> result))
        {
            throw new FormatException("The input string was not in a correct rational format.");
        }

        return result;
    }

    /// <summary>
    /// Пытается преобразовать другое числовое значение в дробь с проверкой переполнения.
    /// </summary>
    public static bool TryConvertFromChecked<TOther>(TOther value, out Rational<T> result)
        where TOther : INumberBase<TOther>
    {
        if (typeof(TOther) == typeof(T))
        {
            result = new Rational<T>((T)(object)value, T.One);
            return true;
        }

        if (TOther.TryConvertToChecked(value, out T converted))
        {
            result = new Rational<T>(converted, T.One);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Пытается преобразовать другое числовое значение в дробь с насыщением.
    /// </summary>
    public static bool TryConvertFromSaturating<TOther>(TOther value, out Rational<T> result)
        where TOther : INumberBase<TOther>
    {
        if (typeof(TOther) == typeof(T))
        {
            result = new Rational<T>((T)(object)value, T.One);
            return true;
        }

        if (TOther.TryConvertToSaturating(value, out T converted))
        {
            result = new Rational<T>(converted, T.One);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Пытается преобразовать другое числовое значение в дробь с усечением.
    /// </summary>
    public static bool TryConvertFromTruncating<TOther>(TOther value, out Rational<T> result)
        where TOther : INumberBase<TOther>
    {
        if (typeof(TOther) == typeof(T))
        {
            result = new Rational<T>((T)(object)value, T.One);
            return true;
        }

        if (TOther.TryConvertToTruncating(value, out T converted))
        {
            result = new Rational<T>(converted, T.One);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Пытается преобразовать дробь в другой числовой тип с проверкой переполнения.
    /// </summary>
    public static bool TryConvertToChecked<TOther>(Rational<T> value, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        if (value.Denominator != T.One)
        {
            result = default;
            return false;
        }

        if (typeof(TOther) == typeof(T))
        {
            result = (TOther)(object)value.Numerator;
            return true;
        }

        return TOther.TryConvertFromChecked(value.Numerator, out result);
    }

    /// <summary>
    /// Пытается преобразовать дробь в другой числовой тип с насыщением.
    /// </summary>
    public static bool TryConvertToSaturating<TOther>(Rational<T> value, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        T truncated = value.Numerator / value.Denominator;

        if (typeof(TOther) == typeof(T))
        {
            result = (TOther)(object)truncated;
            return true;
        }

        return TOther.TryConvertFromSaturating(truncated, out result);
    }

    /// <summary>
    /// Пытается преобразовать дробь в другой числовой тип с усечением.
    /// </summary>
    public static bool TryConvertToTruncating<TOther>(Rational<T> value, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        T truncated = value.Numerator / value.Denominator;

        if (typeof(TOther) == typeof(T))
        {
            result = (TOther)(object)truncated;
            return true;
        }

        return TOther.TryConvertFromTruncating(truncated, out result);
    }

    /// <summary>
    /// Пытается разобрать текстовый диапазон в рациональное число с заданным числовым стилем.
    /// </summary>
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Rational<T> result)
    {
        s = s.Trim();

        if (s.IsEmpty)
        {
            result = default;
            return false;
        }

        int slashIndex = s.IndexOf('/');

        if (slashIndex < 0)
        {
            if (T.TryParse(s, style, provider, out T integer))
            {
                result = new Rational<T>(integer, T.One);
                return true;
            }

            result = default;
            return false;
        }

        ReadOnlySpan<char> left = s[..slashIndex].Trim();
        ReadOnlySpan<char> right = s[(slashIndex + 1)..].Trim();

        if (!T.TryParse(left, style, provider, out T numerator) ||
            !T.TryParse(right, style, provider, out T denominator) ||
            denominator == T.Zero)
        {
            result = default;
            return false;
        }

        try
        {
            result = new Rational<T>(numerator, denominator);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Пытается разобрать строку в рациональное число с заданным числовым стилем.
    /// </summary>
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Rational<T> result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), style, provider, out result);
    }

    /// <summary>
    /// Возвращает единицу.
    /// </summary>
    public static Rational<T> One { get; } = new(T.One, T.One);

    /// <summary>
    /// Возвращает основание системы счисления типа <typeparamref name="T"/>.
    /// </summary>
    public static int Radix { get; } = T.Radix;

    /// <summary>
    /// Возвращает ноль.
    /// </summary>
    public static Rational<T> Zero { get; } = new(T.Zero, T.One);
}
