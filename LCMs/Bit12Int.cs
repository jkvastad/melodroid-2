using System.Text;
using System.Threading.Tasks;

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

    public readonly string ToIntervalString() => Bit12IntToIntervalString(this);

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

    public readonly HashSet<int> ToIntervals() => Bit12IntToIntervals(_value);


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

    public static explicit operator uint(Bit12Int v)
    {
        return (uint)v._value;
    }

    /// <summary>
    /// Get all non-zero bit combinations
    /// </summary>
    /// <param name="mask"></param>    
    /// <returns></returns>
    public static List<Bit12Int> GetSetBitCombinations(Bit12Int mask)
    {
        List<int> setBitPositions = new List<int>();

        // Find positions of set bits
        for (int i = 0; i < 12; i++)
        {
            if ((mask & (1 << i)) != 0)
                setBitPositions.Add(i);
        }

        List<Bit12Int> combinations = new();
        int totalSubsets = 1 << setBitPositions.Count;

        // Generate all non-zero subsets - a subset is some combination of set bit positions
        for (int subset = 1; subset < totalSubsets; subset++)
        {
            int combination = 0;
            // convert subset bits to (absolute) bit positions
            for (int bit = 0; bit < setBitPositions.Count; bit++)
            {
                if ((subset & (1 << bit)) != 0)
                    combination |= (1 << setBitPositions[bit]);
            }
            combinations.Add(combination);
        }

        return combinations;
    }

    public readonly List<Bit12Int> GetSetBitCombinations() => GetSetBitCombinations(_value);

    public override string ToString()
    {
        return Convert.ToString(_value, 2).PadLeft(12, '0');
    }    
}