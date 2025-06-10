using Fractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melodroid_2.MusicUtils;

public static class Utils
{
    //Thanks stack overflow https://stackoverflow.com/questions/147515/least-common-multiple-for-3-or-more-numbers/29717490#29717490
    public static int LCM(int[] numbers)
    {
        int LcmResult = numbers.Aggregate(lcm);
        if (LcmResult > short.MaxValue)
            throw new ArgumentException($"Lcm result {LcmResult} is larger than short.MaxValue: This exceeds maximum midi time division");
        return LcmResult;
    }
    public static int lcm(int a, int b)
    {
        return Math.Abs(a * b) / GCD(a, b);
    }
    public static int GCD(int a, int b)
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

    // Compares sets, useful when calculating e.g. tonal coverages
    public class SetComparer<T> : IEqualityComparer<HashSet<T>>
    {
        public bool Equals(HashSet<T> x, HashSet<T> y)
        {
            return x.SetEquals(y);
        }

        public int GetHashCode(HashSet<T> obj)
        {
            int hash = 0;
            foreach (var item in obj)
                hash ^= item.GetHashCode(); // or use a better combination strategy
            return hash;
        }
    }

    // Comparers for sets of sets, useful when calculating tonal coverages
    public class SetOfSetComparer<T> : IEqualityComparer<HashSet<HashSet<T>>>
    {
        private readonly SetComparer<T> _innerComparer = new();

        public bool Equals(HashSet<HashSet<T>> x, HashSet<HashSet<T>> y)
        {
            if (x.Count != y.Count)
                return false;

            return x.All(xs => y.Any(ys => _innerComparer.Equals(xs, ys)));
        }

        public int GetHashCode(HashSet<HashSet<T>> obj)
        {
            int hash = 0;
            foreach (var innerSet in obj)
                hash ^= _innerComparer.GetHashCode(innerSet);
            return hash;
        }
    }

    /// <summary>
    /// Returns the common factor of two sets of primes. 0 if no common factor.
    /// </summary>
    /// <param name="primes1"></param>
    /// <param name="primes2"></param>
    /// <returns></returns>
    public static int CommonFactors(List<int> primes1, List<int> primes2)
    {
        int CommonFactor = 0;
        List<int> commonPrimes = [];
        foreach (var prime1 in primes1)
            if (primes2.Contains(prime1))
                commonPrimes.Add(prime1);
        if (commonPrimes.Count > 0)
            CommonFactor = commonPrimes.Aggregate((a, b) => a * b);
        
        return CommonFactor;
    }
}
