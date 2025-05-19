using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melodroid_2.MusicUtils;

public static class Utils
{
    //Thanks stack overflow https://stackoverflow.com/questions/147515/least-common-multiple-for-3-or-more-numbers/29717490#29717490
    public static long LCM(long[] numbers)
    {
        long LcmResult = numbers.Aggregate(lcm);
        if (LcmResult > short.MaxValue)
            throw new ArgumentException($"Lcm result {LcmResult} is larger than short.MaxValue: This exceeds maximum midi time division");
        return LcmResult;
    }
    public static long lcm(long a, long b)
    {
        return Math.Abs(a * b) / GCD(a, b);
    }
    public static long GCD(long a, long b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }

    public static List<List<T>> GetPowerSet<T>(T[] array)
    {
        List<List<T>> powerSet = [[]]; // Start with the empty set

        foreach (T element in array)
        {
            int count = powerSet.Count;
            for (int i = 0; i < count; i++)
            {
                List<T> subset = new(powerSet[i]) { element };
                powerSet.Add(subset);
            }
        }

        return powerSet;
    }

    public static List<int> Factorise(int integer, int maxLoops = 100)
    {
        List<int> factors = new();
        int factor = 2;
        int loops = 0;
        while (integer != 1)
        {
            if (integer % factor == 0)
            {
                factors.Add(factor);
                integer /= factor;
            }
            else
            {
                factor += 1;
            }
            loops++;
            if (loops > maxLoops) throw new ArgumentException($"Factorisation failed - exceeded maxLoops {maxLoops}");
        }
        return factors.Count == 0 ? new() { 1 } : factors;
    }

}
