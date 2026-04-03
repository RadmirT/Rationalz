using System.Globalization;

namespace Rationalz.UnitTests;

public class RationalTests
{
    [Fact]
    public void Constructor_ReducesFractionAndKeepsPositiveDenominator()
    {
        Rational<int> rational = new(6, -8);

        Assert.Equal(-3, rational.Numerator);
        Assert.Equal(4, rational.Denominator);
    }

    [Fact]
    public void Constructor_ZeroNumerator_NormalizesToZeroOverOne()
    {
        Rational<int> rational = new(0, 15);

        Assert.Equal(0, rational.Numerator);
        Assert.Equal(1, rational.Denominator);
    }

    [Fact]
    public void Constructor_ZeroDenominator_ThrowsDivideByZeroException()
    {
        Assert.Throws<DivideByZeroException>(() => new Rational<int>(1, 0));
    }

    [Fact]
    public void Add_Subtract_Multiply_Divide_ReturnExpectedValues()
    {
        Rational<int> left = new(1, 2);
        Rational<int> right = new(1, 3);

        Assert.Equal(new Rational<int>(5, 6), left + right);
        Assert.Equal(new Rational<int>(1, 6), left - right);
        Assert.Equal(new Rational<int>(1, 6), left * right);
        Assert.Equal(new Rational<int>(3, 2), left / right);
    }

    [Fact]
    public void Divide_ByZero_ThrowsDivideByZeroException()
    {
        Assert.Throws<DivideByZeroException>(() => new Rational<int>(1, 2) / Rational<int>.Zero);
    }

    [Fact]
    public void Remainder_ReturnsExpectedValue()
    {
        Rational<int> result = new Rational<int>(7, 3) % new Rational<int>(2, 3);

        Assert.Equal(new Rational<int>(1, 3), result);
    }

    [Fact]
    public void UnaryOperators_AndIncrementDecrement_ReturnExpectedValues()
    {
        Rational<int> value = new(2, 3);

        Assert.Equal(new Rational<int>(2, 3), +value);
        Assert.Equal(new Rational<int>(-2, 3), -value);
        Assert.Equal(new Rational<int>(5, 3), ++value);
        Assert.Equal(new Rational<int>(2, 3), --value);
    }

    [Fact]
    public void ComparisonOperators_UseNumericOrder()
    {
        Rational<int> smaller = new(1, 2);
        Rational<int> greater = new(2, 3);

        Assert.True(smaller < greater);
        Assert.True(smaller <= greater);
        Assert.True(greater > smaller);
        Assert.True(greater >= smaller);
        Assert.Equal(0, new Rational<int>(2, 4).CompareTo(new Rational<int>(1, 2)));
    }

    [Fact]
    public void CompareToObject_NullReturnsOne_AndInvalidTypeThrows()
    {
        Rational<int> value = new(1, 2);

        Assert.Equal(1, value.CompareTo(null));
        Assert.Throws<ArgumentException>(() => value.CompareTo("1/2"));
    }

    [Fact]
    public void Equals_AndHashCode_UseCanonicalValue()
    {
        Rational<int> left = new(2, 4);
        Rational<int> right = new(1, 2);

        Assert.True(left.Equals(right));
        Assert.True(left == right);
        Assert.False(left != right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Theory]
    [InlineData("7/8", 7, 8)]
    [InlineData("  -10 / 20  ", -1, 2)]
    [InlineData("42", 42, 1)]
    public void Parse_ValidInput_ReturnsExpectedRational(string text, int expectedNumerator, int expectedDenominator)
    {
        Rational<long> rational = Rational<long>.Parse(text, CultureInfo.InvariantCulture);

        Assert.Equal(new Rational<long>(expectedNumerator, expectedDenominator), rational);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("1/0")]
    [InlineData("1/")]
    public void TryParse_InvalidInput_ReturnsFalse(string? text)
    {
        bool parsed = Rational<int>.TryParse(text, CultureInfo.InvariantCulture, out Rational<int> result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("1/0")]
    [InlineData("1/")]
    [InlineData("not-a-rational")]
    public void Parse_InvalidInput_ThrowsFormatException(string text)
    {
        Assert.Throws<FormatException>(() => Rational<int>.Parse(text, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ToString_FormatsIntegerAndFraction()
    {
        Assert.Equal("5", new Rational<int>(5, 1).ToString());
        Assert.Equal("3/4", new Rational<int>(3, 4).ToString());
    }

    [Fact]
    public void ToString_WithFormatProvider_FormatsNumeratorAndDenominator()
    {
        Rational<int> rational = new(15, 16);

        Assert.Equal("000F/0010", rational.ToString("X4", CultureInfo.InvariantCulture));
    }

    [Fact]
    public void TryFormat_WritesToDestination_WhenBufferIsLargeEnough()
    {
        Span<char> destination = stackalloc char[8];

        bool success = new Rational<int>(3, 4).TryFormat(destination, out int charsWritten, default, CultureInfo.InvariantCulture);

        Assert.True(success);
        Assert.Equal(3, charsWritten);
        Assert.Equal("3/4", destination[..charsWritten].ToString());
    }

    [Fact]
    public void TryFormat_ReturnsFalse_WhenBufferIsTooSmall()
    {
        Span<char> destination = stackalloc char[2];

        bool success = new Rational<int>(3, 4).TryFormat(destination, out int charsWritten, default, CultureInfo.InvariantCulture);

        Assert.False(success);
        Assert.Equal(0, charsWritten);
    }

    [Fact]
    public void NumericPredicates_ReturnExpectedValues()
    {
        Rational<int> negativeInteger = new(-3, 1);
        Rational<int> positiveFraction = new(3, 2);

        Assert.True(Rational<int>.IsFinite(positiveFraction));
        Assert.True(Rational<int>.IsInteger(negativeInteger));
        Assert.True(Rational<int>.IsOddInteger(negativeInteger));
        Assert.True(Rational<int>.IsNegative(negativeInteger));
        Assert.True(Rational<int>.IsPositive(positiveFraction));
        Assert.True(Rational<int>.IsRealNumber(positiveFraction));
        Assert.True(Rational<int>.IsNormal(positiveFraction));
        Assert.True(Rational<int>.IsZero(Rational<int>.Zero));
        Assert.True(Rational<int>.IsCanonical(new Rational<int>(2, 3)));
        Assert.False(Rational<int>.IsEvenInteger(positiveFraction));
        Assert.False(Rational<int>.IsNaN(positiveFraction));
        Assert.False(Rational<int>.IsInfinity(positiveFraction));
        Assert.False(Rational<int>.IsNegativeInfinity(positiveFraction));
        Assert.False(Rational<int>.IsPositiveInfinity(positiveFraction));
        Assert.False(Rational<int>.IsImaginaryNumber(positiveFraction));
        Assert.False(Rational<int>.IsComplexNumber(positiveFraction));
        Assert.False(Rational<int>.IsSubnormal(positiveFraction));
    }

    [Fact]
    public void Abs_MaxMagnitude_MinMagnitude_ReturnExpectedValues()
    {
        Rational<int> left = new(-3, 4);
        Rational<int> right = new(2, 3);

        Assert.Equal(new Rational<int>(3, 4), Rational<int>.Abs(left));
        Assert.Equal(left, Rational<int>.MaxMagnitude(left, right));
        Assert.Equal(left, Rational<int>.MaxMagnitudeNumber(left, right));
        Assert.Equal(right, Rational<int>.MinMagnitude(left, right));
        Assert.Equal(right, Rational<int>.MinMagnitudeNumber(left, right));
    }

    [Fact]
    public void Constants_ReturnExpectedValues()
    {
        Assert.Equal(new Rational<int>(0, 1), Rational<int>.Zero);
        Assert.Equal(new Rational<int>(1, 1), Rational<int>.One);
        Assert.Equal(Rational<int>.Zero, Rational<int>.AdditiveIdentity);
        Assert.Equal(Rational<int>.One, Rational<int>.MultiplicativeIdentity);
        Assert.Equal(2, Rational<int>.Radix);
    }

    [Fact]
    public void TryConvertFromChecked_IntegerValue_ReturnsRational()
    {
        bool converted = Rational<int>.TryConvertFromChecked(42, out Rational<int> result);

        Assert.True(converted);
        Assert.Equal(new Rational<int>(42, 1), result);
    }

    [Fact]
    public void TryConvertToChecked_IntegerRational_ReturnsTargetValue()
    {
        bool converted = Rational<int>.TryConvertToChecked(new Rational<int>(42, 1), out int result);

        Assert.True(converted);
        Assert.Equal(42, result);
    }

    [Fact]
    public void TryConvertToChecked_NonIntegerRational_ReturnsFalse()
    {
        bool converted = Rational<int>.TryConvertToChecked(new Rational<int>(3, 2), out int result);

        Assert.False(converted);
        Assert.Equal(0, result);
    }

    [Fact]
    public void TryConvertToTruncating_AndSaturating_TruncateFractionalPart()
    {
        Rational<int> value = new(7, 3);

        Assert.True(Rational<int>.TryConvertToTruncating(value, out int truncated));
        Assert.True(Rational<int>.TryConvertToSaturating(value, out int saturated));
        Assert.Equal(2, truncated);
        Assert.Equal(2, saturated);
    }

    [Fact]
    public void ImplicitConversion_FromInteger_CreatesRationalWithUnitDenominator()
    {
        Rational<int> rational = 42;

        Assert.Equal(new Rational<int>(42, 1), rational);
    }

    [Fact]
    public void ExplicitConversion_ToDouble_ReturnsFractionValue()
    {
        Rational<int> rational = new(-3, 4);

        double value = (double)rational;

        Assert.Equal(-0.75d, value);
    }

    [Fact]
    public void ExplicitConversion_ToFloat_ReturnsFractionValue()
    {
        Rational<int> rational = new(1, 8);

        float value = (float)rational;

        Assert.Equal(0.125f, value);
    }

    [Fact]
    public void ExplicitConversion_ToDecimal_ReturnsFractionValue()
    {
        Rational<int> rational = new(5, 8);

        decimal value = (decimal)rational;

        Assert.Equal(0.625m, value);
    }

}
