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

        if (TryConvertFromFloatingPoint(value, IntegerConversion.Checked, out result))
        {
            return true;
        }

        if (TOther.IsInteger(value) && T.TryConvertFromChecked(value, out var integer))
        {
            result = new Rational<T>(integer, T.One);
            return true;
        }
        if (TOther.IsInteger(value) && TOther.TryConvertToChecked(value, out T converted))
        {
            result = new Rational<T>(converted, T.One);
            return true;
        }

        result = Zero;
        return false;
    }

    /// <summary>
    /// Creates a rational number from another numeric value with overflow checking.
    /// </summary>
    public static Rational<T> CreateChecked<TOther>(TOther value)
        where TOther : INumberBase<TOther>
    {
        if (TryConvertFromChecked(value, out Rational<T> result))
        {
            return result;
        }

        throw new OverflowException($"Value of type {typeof(TOther)} cannot be represented as {typeof(Rational<T>)}.");
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

        if (TryConvertFromFloatingPoint(value, IntegerConversion.Saturating, out result))
        {
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

        if (TryConvertFromFloatingPoint(value, IntegerConversion.Truncating, out result))
        {
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

    private static bool TryConvertFromFloatingPoint<TOther>(TOther value, IntegerConversion conversion, out Rational<T> result)
        where TOther : INumberBase<TOther>
    {
        if (!TOther.IsRealNumber(value) || !TryGetComponents(value, out BigInteger significand, out BigInteger exponentBase, out int exponent))
        {
            result = default;
            return false;
        }

        BigInteger numerator = significand;
        BigInteger denominator = BigInteger.One;

        if (exponent > 0)
        {
            numerator *= BigInteger.Pow(exponentBase, exponent);
        }
        else if (exponent < 0)
        {
            denominator = BigInteger.Pow(exponentBase, -exponent);
        }

        try
        {
            BigInteger gcd = BigInteger.GreatestCommonDivisor(BigInteger.Abs(numerator), denominator);
            if (gcd > BigInteger.One)
            {
                numerator /= gcd;
                denominator /= gcd;
            }

            T convertedNumerator = ConvertInteger(numerator, conversion);
            T convertedDenominator = T.CreateChecked(denominator);

            result = new Rational<T>(convertedNumerator, convertedDenominator);
            return true;
        }
        catch
        {
            if (conversion == IntegerConversion.Checked &&
                exponentBase == 10 &&
                exponent < 0 &&
                IsBinaryFloatingPoint(value) &&
                TryApproximate(numerator, denominator, -exponent, out result))
            {
                return true;
            }

            result = default;
            return false;
        }
    }

    private static bool TryApproximate(BigInteger numerator, BigInteger denominator, int decimalPlaces, out Rational<T> result)
    {
        BigInteger toleranceDenominator = BigInteger.Pow(10, decimalPlaces);
        BigInteger n0 = BigInteger.Zero;
        BigInteger d0 = BigInteger.One;
        BigInteger n1 = BigInteger.One;
        BigInteger d1 = BigInteger.Zero;
        BigInteger remainingNumerator = numerator;
        BigInteger remainingDenominator = denominator;

        while (remainingDenominator != BigInteger.Zero)
        {
            BigInteger quotient = BigInteger.DivRem(remainingNumerator, remainingDenominator, out BigInteger remainder);
            BigInteger nextNumerator = quotient * n1 + n0;
            BigInteger nextDenominator = quotient * d1 + d0;

            if (!CanRepresent(nextNumerator) || !CanRepresent(nextDenominator))
            {
                break;
            }

            BigInteger differenceNumerator = BigInteger.Abs(numerator * nextDenominator - nextNumerator * denominator);
            BigInteger differenceDenominator = denominator * nextDenominator;

            if (differenceNumerator * toleranceDenominator <= differenceDenominator)
            {
                result = new Rational<T>(T.CreateChecked(nextNumerator), T.CreateChecked(nextDenominator));
                return true;
            }

            n0 = n1;
            d0 = d1;
            n1 = nextNumerator;
            d1 = nextDenominator;
            remainingNumerator = remainingDenominator;
            remainingDenominator = remainder;
        }

        result = default;
        return false;
    }

    private static bool CanRepresent(BigInteger value)
    {
        try
        {
            T.CreateChecked(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsBinaryFloatingPoint<TOther>(TOther value)
        where TOther : INumberBase<TOther>
    {
        return value is double or float or Half;
    }

    private static T ConvertInteger(BigInteger value, IntegerConversion conversion)
    {
        return conversion switch
        {
            IntegerConversion.Checked => T.CreateChecked(value),
            IntegerConversion.Saturating => T.CreateSaturating(value),
            IntegerConversion.Truncating => T.CreateTruncating(value),
            _ => throw new ArgumentOutOfRangeException(nameof(conversion), conversion, null)
        };
    }

    private static bool TryGetComponents<TOther>(TOther value, out BigInteger significand, out BigInteger exponentBase, out int exponent)
        where TOther : INumberBase<TOther>
    {
        if (value is decimal decimalValue)
        {
            int[] bits = decimal.GetBits(decimalValue);
            uint lo = (uint)bits[0];
            uint mid = (uint)bits[1];
            uint hi = (uint)bits[2];
            bool isNegative = (bits[3] & unchecked((int)0x80000000)) != 0;
            int scale = (bits[3] >> 16) & 0xFF;

            significand = ((BigInteger)hi << 64) | ((BigInteger)mid << 32) | lo;
            if (isNegative)
            {
                significand = -significand;
            }

            exponentBase = 10;
            exponent = -scale;
            return true;
        }

        if (value is double doubleValue)
        {
            if (TryGetDecimalFloatingComponents(doubleValue, out significand, out exponentBase, out exponent))
            {
                return true;
            }

            return TryGetBinaryFloatingComponents(doubleValue, 11, 52, 1023, out significand, out exponentBase, out exponent);
        }

        if (value is float floatValue)
        {
            if (TryGetDecimalFloatingComponents(floatValue, out significand, out exponentBase, out exponent))
            {
                return true;
            }

            return TryGetBinaryFloatingComponents(floatValue, 8, 23, 127, out significand, out exponentBase, out exponent);
        }

        if (value is Half halfValue)
        {
            if (TryGetDecimalFloatingComponents(halfValue, out significand, out exponentBase, out exponent))
            {
                return true;
            }

            return TryGetBinaryFloatingComponents(halfValue, 5, 10, 15, out significand, out exponentBase, out exponent);
        }

        significand = default;
        exponentBase = default;
        exponent = default;
        return false;
    }

    private static bool TryGetDecimalFloatingComponents<TFloat>(
        TFloat value,
        out BigInteger significand,
        out BigInteger exponentBase,
        out int exponent)
        where TFloat : unmanaged
    {
        string? text = value switch
        {
            double doubleValue when double.IsFinite(doubleValue) => doubleValue.ToString(null, CultureInfo.InvariantCulture),
            float floatValue when float.IsFinite(floatValue) => floatValue.ToString(null, CultureInfo.InvariantCulture),
            Half halfValue when Half.IsFinite(halfValue) => halfValue.ToString(null, CultureInfo.InvariantCulture),
            _ => null
        };

        if (text is null)
        {
            significand = default;
            exponentBase = default;
            exponent = default;
            return false;
        }

        int scientificExponent = 0;
        int exponentIndex = text.IndexOfAny("eE".ToCharArray());
        if (exponentIndex >= 0)
        {
            if (!int.TryParse(text.AsSpan(exponentIndex + 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out scientificExponent))
            {
                significand = default;
                exponentBase = default;
                exponent = default;
                return false;
            }

            text = text[..exponentIndex];
        }

        bool isNegative = text.StartsWith("-", StringComparison.Ordinal);
        if (isNegative || text.StartsWith("+", StringComparison.Ordinal))
        {
            text = text[1..];
        }

        int decimalPointIndex = text.IndexOf('.');
        int fractionalDigits = decimalPointIndex >= 0 ? text.Length - decimalPointIndex - 1 : 0;
        string digits = decimalPointIndex >= 0 ? text.Remove(decimalPointIndex, 1) : text;

        digits = digits.TrimStart('0');
        significand = digits.Length == 0
            ? BigInteger.Zero
            : BigInteger.Parse(digits, NumberStyles.None, CultureInfo.InvariantCulture);

        if (isNegative)
        {
            significand = -significand;
        }

        exponentBase = 10;
        exponent = scientificExponent - fractionalDigits;
        return true;
    }

    private static bool TryGetBinaryFloatingComponents<TFloat>(
        TFloat value,
        int exponentBitCount,
        int significandBitCount,
        int exponentBias,
        out BigInteger significand,
        out BigInteger exponentBase,
        out int exponent)
        where TFloat : unmanaged
    {
        ulong bits = value switch
        {
            double doubleValue when !double.IsFinite(doubleValue) => 0,
            double doubleValue => (ulong)BitConverter.DoubleToInt64Bits(doubleValue),
            float floatValue when !float.IsFinite(floatValue) => 0,
            float floatValue => BitConverter.SingleToUInt32Bits(floatValue),
            Half halfValue when !Half.IsFinite(halfValue) => 0,
            Half halfValue => BitConverter.HalfToUInt16Bits(halfValue),
            _ => 0
        };

        if (bits == 0 && !IsZero(value))
        {
            significand = default;
            exponentBase = default;
            exponent = default;
            return false;
        }

        int signShift = significandBitCount + exponentBitCount;
        ulong exponentMask = (1UL << exponentBitCount) - 1;
        ulong significandMask = (1UL << significandBitCount) - 1;
        bool isNegative = ((bits >> signShift) & 1UL) != 0;
        int rawExponent = (int)((bits >> significandBitCount) & exponentMask);
        ulong rawSignificand = bits & significandMask;

        if (rawExponent == 0)
        {
            significand = rawSignificand;
            exponent = 1 - exponentBias - significandBitCount;
        }
        else
        {
            significand = (BigInteger.One << significandBitCount) | rawSignificand;
            exponent = rawExponent - exponentBias - significandBitCount;
        }

        if (isNegative)
        {
            significand = -significand;
        }

        exponentBase = 2;
        return true;
    }

    private static bool IsZero<TFloat>(TFloat value)
        where TFloat : unmanaged
    {
        return value switch
        {
            double doubleValue => doubleValue == 0d,
            float floatValue => floatValue == 0f,
            Half halfValue => halfValue == (Half)0,
            _ => false
        };
    }

    private enum IntegerConversion
    {
        Checked,
        Saturating,
        Truncating
    }
}
