using Fractions;
using Melodroid_2.LCMs;
using System.Linq;

namespace Melodroid_2.MusicUtils;

public class Tet12ChromaMask
{
    public Bit12Int Mask { get; set; }
    public Tet12ChromaMask(Bit12Int mask)
    {
        Mask = mask;
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

    // Gets all LCMs relative the masks 0 position
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
}
