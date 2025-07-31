using Melodroid_2.LCMs;
using System.Numerics;

namespace Melodroid_2.MusicUtils;

/// <summary>
/// Calculates tonal cover for a tet12 key set 
/// A tonal cover is a:
///     1. Superposition of two sets whose union is the original set
///     2. These sets share a full matching factor at a fundamental
///     3. Possibly using upscaling
///         3.1 Upscaling refers to substituting a set with a superset - e,g, a full match 3@0 can be a subset to 24@0 or 15@0
///             3@0 may then function as e.g. 6@0 or 15@0
/// The subsets are called origin and complement
/// </summary>
public class Tet12TonalCoverCalculator
{
    public static readonly List<int> LegalSubsetLcms = [2, 3, 4, 5, 6, 8, 9, 10, 12, 15, 24];

    // permutates all possible combinations
    public static Dictionary<Bit12Int, List<(int fundamental, Bit12Int complement, int originLcm, int complementLcm)>>
        CalculateTet12TonalCoverage(
        Bit12Int tet12Keys,
        int minOriginSetSize = 3,
        int minComplementSetSize = 3,
        bool upscale = true,
        bool no15Collapse = true // if any lcm is 15, then (fundamental - 4) is not a legal key
        )
    {
        // Get all origin and complement subsets
        Dictionary<Bit12Int, List<(int fundamental, Bit12Int complement, int originLcm, int complementLcm)>> subsets = [];
        HashSet<Bit12Int> allCombinations = [.. Bit12Int.GetSetBitCombinations(tet12Keys)];
        HashSet<Bit12Int> allOrigins = allCombinations.Where(
            keys => BitOperations.PopCount((uint)keys.GetValue()) >= minOriginSetSize).ToHashSet();
        HashSet<Bit12Int> allComplements = allCombinations.Where(
            keys => BitOperations.PopCount((uint)keys.GetValue()) >= minComplementSetSize).ToHashSet();

        // keep only subsets which have a full match
        int maxLcm = LegalSubsetLcms.Max();
        List<(Bit12Int origin, Dictionary<int, List<int>> fullMatches)> legalOrigins = [];
        foreach (var origin in allOrigins)
        {
            var originFullMatches = Tet12ChromaMask.GetAllMaskLCMs(origin, maxLcm);
            // origin must have at least one full match
            if (originFullMatches.Any(kv => kv.Value.Any(lcm => LegalSubsetLcms.Contains(lcm))))
                legalOrigins.Add((origin, originFullMatches));
        }
        List<(Bit12Int complement, Dictionary<int, List<int>> fullMatches)> legalComplements = [];
        foreach (var complement in allComplements)
        {
            var complementFullMatches = Tet12ChromaMask.GetAllMaskLCMs(complement, maxLcm);
            // complement must have at least one full match
            if (complementFullMatches.Any(kv => kv.Value.Any(lcm => LegalSubsetLcms.Contains(lcm))))
                legalComplements.Add((complement, complementFullMatches));
        }

        // find all unions of origin and complement where there is an overlapping full match fundamental and a (upscaled) factor is shared
        foreach (var (origin, originMatches) in legalOrigins)
        {
            subsets[origin] = [];
            foreach (var (complement, complementMatches) in legalComplements)
            {
                // check if union is a full set
                if (!((origin | complement) == tet12Keys))
                    continue;
                // check if origin and complement share any fundamentals
                for (int fundamental = 0; fundamental < 12; fundamental++)
                {
                    if (originMatches[fundamental].Count > 0 && complementMatches[fundamental].Count > 0)
                    {
                        // Create all lcm combinations
                        List<List<int>> lcmCombinations = [];
                        foreach (var originLcm in originMatches[fundamental])
                        {
                            foreach (var complementLcm in complementMatches[fundamental])
                            {
                                // only add combinations with legal lcms
                                if (LegalSubsetLcms.Contains(originLcm) && LegalSubsetLcms.Contains(complementLcm))
                                    lcmCombinations.Add([originLcm, complementLcm]);
                            }
                        }
                        foreach (var lcmCombination in lcmCombinations)
                        {
                            //check if origin and complement share factor
                            int originLcm = lcmCombination[0];
                            int complementLcm = lcmCombination[1];
                            List<int> originFactors = Utils.Factorise(originLcm);
                            List<int> complementFactors = Utils.Factorise(complementLcm);
                            if (originFactors.Any(complementFactors.Contains))
                            {
                                // add data for origin set, showing complement set, fundamental and origin/complement lcms
                                subsets[origin].Add((fundamental, complement, originLcm, complementLcm));
                            }
                            else if (upscale)
                            {
                                // try for upscale
                                foreach (var lcm in LegalSubsetLcms)
                                {
                                    // legal lcm contains factor from both complement and origin
                                    if (lcm % originLcm == 0 && lcm % complementLcm == 0)
                                    {
                                        // add data for origin set, showing complement set, fundamental and origin/complement lcms
                                        subsets[origin].Add((fundamental, complement, lcm, lcm));
                                    }
                                }
                            }
                            // if origin or complement uses fundamental - 4 when base is 15, exclude it
                            if (no15Collapse && subsets[origin].Count > 0)
                            {
                                (int fundamental, Bit12Int complement, int originLcm, int complementLcm) currentCover = subsets[origin][^1];
                                if (currentCover.originLcm == 15 || currentCover.complementLcm == 15)
                                {
                                    HashSet<int> activeKeys = [.. currentCover.complement.ToIntervals(), .. origin.ToIntervals()];
                                    if (activeKeys.Contains((fundamental + 8) % 12))
                                    {
                                        subsets[origin].RemoveAt(subsets[origin].Count - 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return subsets;
    }
}