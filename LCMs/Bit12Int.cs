using System.Text;

namespace Melodroid_2.LCMs;

public struct Bit12Int
{
    private const int _bitSize = 12;
    private const int _maxValue = (1 << _bitSize) - 1;

    private int _value;

    public Bit12Int(int initialValue)
    {
        _value = initialValue & _maxValue;
    }

    public static Bit12Int operator <<(Bit12Int left, int rotations)
    {
        rotations %= _bitSize;
        return new((left._value << rotations | left._value >> (_bitSize - rotations)) & _maxValue);
    }
    public static Bit12Int operator >>(Bit12Int left, int rotations)
    {
        rotations %= _bitSize;
        return new((left._value >> rotations | left._value << (_bitSize - rotations)) & _maxValue);
    }

    public int GetValue()
    {
        return _value;
    }

    public static string Bit12IntToIntervalString(Bit12Int binaryKeySet)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 12; i++)
        {
            if (((binaryKeySet >> i) & 1) == 1)
                sb.Append($"{i} ");
        }
        if (sb.Length > 1)
            sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }

    public static HashSet<int> Bit12IntToIntervals(Bit12Int binaryKeySet)
    {
        HashSet<int> intervals = new();
        for (int i = 0; i < 12; i++)
        {
            if (((binaryKeySet >> i) & 1) == 1)
                intervals.Add(i);
        }

        return intervals;
    }

    public static int operator |(Bit12Int left, int right)
    {
        return left._value | right;
    }

    public static Bit12Int operator |(Bit12Int left, Bit12Int right)
    {
        return new Bit12Int(left._value | (int)right);
    }
    public static Bit12Int operator ^(Bit12Int left, Bit12Int right)
    {
        return new Bit12Int(left._value ^ (int)right);
    }

    public static int operator &(Bit12Int left, int right)
    {
        return left._value & right;
    }

    public static Bit12Int operator &(Bit12Int left, Bit12Int right)
    {
        return new Bit12Int(left._value & (int)right);
    }

    public static implicit operator Bit12Int(int value)
    {
        return new Bit12Int(value);
    }

    public static explicit operator int(Bit12Int value)
    {
        return value._value;
    }

    public static bool operator ==(Bit12Int left, Bit12Int right)
    {
        return left._value == right._value;
    }

    public static bool operator ==(Bit12Int left, int right)
    {
        return left._value == right;
    }

    public static bool operator !=(Bit12Int left, Bit12Int right)
    {
        return left._value != right._value;
    }

    public static bool operator !=(Bit12Int left, int right)
    {
        return left._value != right;
    }

    public override string ToString()
    {
        return Convert.ToString(_value, 2).PadLeft(12, '0');
    }
}