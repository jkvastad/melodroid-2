using Fractions;
using Melodroid_2.LCMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

    private static readonly Random rng = new();

    public static T RandomElement<T>(this IList<T> list)
    {
        return list[rng.Next(list.Count)];
    }

    /// <summary>
    /// Calculates all chroma masks having an lcm factor at root position, mapped by factor
    /// </summary>
    /// <returns></returns>
    public static Dictionary<int, HashSet<Bit12Int>> CalculateAllChordTargets()
    {
        int[] targetFactors = [2, 3, 4, 5, 6, 8, 9, 10, 12, 15];
        Dictionary<int, HashSet<Bit12Int>> chordTargetsByFactor = [];
        foreach (var factor in targetFactors)
            chordTargetsByFactor[factor] = [];

        // Go through all keys, if root LCM matches then map mask to factor
        for (int i = 0; i < BigInteger.Pow(2, 12); i++)
        {
            Bit12Int chromaMask = new(i);
            var lcms = Tet12ChromaMask.GetMaskRootLCMs(chromaMask);
            foreach (var lcm in lcms)
                if (targetFactors.Contains(lcm))
                    chordTargetsByFactor[lcm].Add(chromaMask);
        }
        return chordTargetsByFactor;
    }

    /// <summary>
    /// Calculates all root position chroma masks having a legal full match lcm, mapped by cardinality
    /// </summary>
    /// <returns></returns>
    public static Dictionary<int, HashSet<Bit12Int>> CalculateAllChordOrigins()
    {
        int[] legalLcms = [2, 3, 4, 5, 6, 8, 9, 10, 12, 15];
        Dictionary<int, HashSet<Bit12Int>> chordOriginsByCardinality = [];
        for (int i = 0; i < 12; i++)
            chordOriginsByCardinality[i] = [];

        // Go through all keys, if root LCM matches legal lcm then map mask to cardinality
        for (int i = 0; i < BigInteger.Pow(2, 12); i++)
        {
            Bit12Int chromaMask = new(i);
            
            // only consider root positions
            if ((chromaMask & 1) != 1)
                continue;

            // check if lcm is legal
            var lcms = Tet12ChromaMask.GetMaskRootLCMs(chromaMask);
            if (!lcms.Any(legalLcms.Contains))
                continue;

            // check if rotation is already present
            List<Bit12Int> rotatedMasks = [];
            for (int j = 0; j < 12; j++)
                rotatedMasks.Add(chromaMask << j);

            int cardinality = BitOperations.PopCount((uint)(int)chromaMask);
            var cardinalMasks = chordOriginsByCardinality[cardinality];
            if (rotatedMasks.Any(cardinalMasks.Contains))
                continue;

            chordOriginsByCardinality[cardinality].Add(chromaMask);
        }
        return chordOriginsByCardinality;
    }
}
