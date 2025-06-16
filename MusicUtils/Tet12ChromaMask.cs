using Fractions;
using Melanchall.DryWetMidi.Interaction;
using Melodroid_2.LCMs;

namespace Melodroid_2.MusicUtils;

public class Tet12ChromaMask
{
    public Bit12Int Mask { get; set; }
    public Tet12ChromaMask(Bit12Int mask)
    {
        Mask = mask;
    }

    public static implicit operator Tet12ChromaMask(Bit12Int mask)
    {
        return new Tet12ChromaMask(mask);
    }

    public static Dictionary<int, List<Fraction>> FractionsAtMaskPosition = new()
    {
        [0] = new() { new(1) },
        [1] = new() { new(16, 15) },
        [2] = new() { new(10, 9), new(9, 8) },
        [3] = new() { new(6, 5) },
        [4] = new() { new(5, 4) },
        [5] = new() { new(4, 3) },
        [6] = new(),
        [7] = new() { new(3, 2) },
        [8] = new() { new(8, 5) },
        [9] = new() { new(5, 3) },
        [10] = new() { new(16, 9), new(9, 5) },
        [11] = new() { new(15, 8) }
    };

    public static Tet12ChromaMask LCM8 = new(new(0b100010010101));

    /// <summary>
    /// Calculates all lcms for all rotations of the chroma mask
    /// </summary>
    /// <param name="chromaMask"></param>
    /// <returns> Dictionary with keys being root position, e.g. 1 for lcm after rotating mask right once,
    /// and values being all lcms for that root. Lcm 0 signifies undefined lcm (e.g. tritone inclusion) </returns>
    public static Dictionary<int, List<int>> GetAllMaskLCMs(Bit12Int chromaMask, int maxLcm = 12)
    {
        Dictionary<int, List<int>> lcmsAtRoot = new();
        for (int root = 0; root < 12; root++)
        {
            Tet12ChromaMask rotatedMask = new(chromaMask >> root);
            lcmsAtRoot[root] = GetMaskRootLCMs(rotatedMask).Where(lcm => lcm <= 12).ToList();
        }
        return lcmsAtRoot;
    }

    public static Dictionary<int, List<int>> GetAllMaskLCMs(Tet12ChromaMask chromaMask, int maxLcm = 12)
        => GetAllMaskLCMs(chromaMask.Mask, maxLcm);

    public Dictionary<int, List<int>> GetAllMaskLCMs(int maxLcm = 12)
        => GetAllMaskLCMs(this, maxLcm);


    /// <summary>
    /// Gets all LCMs relative the masks 0 position
    /// </summary>
    /// <param name="chromaMask"></param>
    /// <returns></returns>
    public static List<int> GetMaskRootLCMs(Tet12ChromaMask chromaMask)
    {
        List<int> positions = [];

        // add all chroma mask positions
        for (int i = 0; i < 12; i++)
            if (((chromaMask.Mask >> i) & 1) == 1)
                positions.Add(i);

        // 0 value LCM for tritone - lcm undefined.
        // 0 value for empty mask - lcm undefined.
        if (positions.Contains(6) || positions.Count == 0)
            return new(0);

        // get all fractions at positions
        List<List<Fraction>> fractions = new();
        foreach (var position in positions)
            fractions.Add(FractionsAtMaskPosition[position]);

        // create all combinations from one fraction per position
        List<List<Fraction>> combinations = GetCombinations(fractions);
        List<int> combinationLCMs = new();

        // calculate LCM for each combination
        foreach (var combination in combinations)
            combinationLCMs.Add(Utils.LCM(combination.Select(fraction => (int)fraction.Denominator).ToArray()));

        return combinationLCMs;
    }

    public List<int> GetMaskRootLCMs() => GetMaskRootLCMs(this);

    public static List<Tet12ChromaMask> GetAllMaskSubsets(Tet12ChromaMask mask)
    {
        var allMaskBitCombinations = GetSetBitCombinations(mask.Mask);
        return allMaskBitCombinations.Select(bits => new Tet12ChromaMask(bits)).ToList();
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

    public List<Bit12Int> GetSetBitCombinations() => GetSetBitCombinations(Mask);

    public static List<List<T>> GetCombinations<T>(List<List<T>> jaggedArray)
    {
        var results = new List<List<T>>();
        Generate(jaggedArray, 0, new List<T>(), results);
        return results;
    }

    private static void Generate<T>(List<List<T>> jaggedArray, int depth, List<T> current, List<List<T>> results)
    {
        if (depth == jaggedArray.Count)
        {
            results.Add(new List<T>(current));
            return;
        }

        foreach (var item in jaggedArray[depth])
        {
            current.Add(item);
            Generate(jaggedArray, depth + 1, current, results);
            current.RemoveAt(current.Count - 1);
        }
    }

    public List<int> ChromaToIntervals()
    {
        List<int> intervals = [];
        for (int i = 0; i < 12; i++)
        {
            if (((Mask >> i) & 1) == 1)
                intervals.Add(i);
        }
        return intervals;
    }

    /// <summary>
    /// Tries to voice chroma mask as triads with fundamental as root
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="maskFundamental"></param>
    /// <returns></returns>
    public static Dictionary<int, bool> ChromaToTriadMidi(Tet12ChromaMask mask, int maskFundamental)
    {
        // no chroma no midi
        if (mask.Mask == 0)
            return [];
        // Go through all intervals
        var intervals = mask.ChromaToIntervals();
        List<int> shiftedIntervals = [];
        int previousInterval = intervals.First();
        shiftedIntervals.Add(previousInterval);

        for (int i = 1; i < intervals.Count; i++)
        {
            // any interval closer than 3 steps gets pushed up an octave - mostly good enough
            if (intervals[i] - previousInterval < 3)
            {
                int shiftedInterval = intervals[i] + 12;
                shiftedIntervals.Add(shiftedInterval);
            }
            else
            {
                shiftedIntervals.Add(intervals[i]);
                previousInterval = intervals[i];
            }
        }

        Dictionary<int, bool> midi = [];
        foreach (var interval in shiftedIntervals)
            midi[interval + maskFundamental] = true;

        return midi;
    }    

    public override string ToString()
    {
        return Mask.ToString();
    }
}
