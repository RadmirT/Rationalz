namespace Rationalz;

readonly partial struct Rational<T>
{
    public static implicit operator Rational<T> (T value) => new(value);

    public static explicit operator double(Rational<T> rational)
    {
        return double.CreateChecked(rational.Numerator) / double.CreateChecked(rational.Denominator);
    }
    
    public static explicit operator float(Rational<T> rational)
    {
        return float.CreateChecked(rational.Numerator) / float.CreateChecked(rational.Denominator);
    }

    public static explicit operator decimal(Rational<T> rational)
    {
        return decimal.CreateChecked(rational.Numerator) / decimal.CreateChecked(rational.Denominator);
    }
}