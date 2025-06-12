using Melodroid_2.LCMs;
using Melodroid_2.MusicUtils;

namespace Melodroid_2.MidiMakers.TonalCoverComposer;

/// <summary>
/// A tonal cover consists of Tonal Sets which are sets of notes...
/// ... whose union span all sounding notes
/// ... have a common LCM factor for some fundamental
/// </summary>
public class TonalCover
{
    public List<TonalSet> TonalSets { get; set; }
    public TonalCover(List<TonalSet> tonalSets)
    {
        TonalSets = tonalSets;
    }

    internal Tet12ChromaMask GetChromaMask()
    {
        Bit12Int coverMask = 0;
        foreach (var tonalSet in TonalSets)
            coverMask |= tonalSet.ChromaMask.Mask;
        return coverMask;
    }
}

// TODO: Handle higher voicings, for now just go with lcm 8 ring
/// <summary>
/// A TonalSet is a set of notes which sounds consonant when played as a block. 
/// This is all subsets of the LCM8 isomorphism ring (8,9,10,12), as well as some higher voicings: 
///     Proper subsets of lcm 15 
///     G13 for 24@0
/// </summary>
public class TonalSet
{
    public Tet12ChromaMask ChromaMask { get; set; } // e.g. major triad is new(0b10010001)

    public TonalSet(Tet12ChromaMask chromaMask)
    {
        ChromaMask = chromaMask;
    }


    // TODO: Currently assuming tonal sets are subsets of lcm 8 isomorphic ring
    // Only need factors at some root, this can then be rotated to desired root
    public static List<TonalSet> GetTonalSetsWithFactor(int factor, int maxLCM = 12)
    {
        List<TonalSet> tonalSetsWithFactor = new();
        Dictionary<int, List<int>> maskLcms = Tet12ChromaMask.LCM8.GetAllMaskLCMs(maxLCM);
        return tonalSetsWithFactor;
    }

    public static Dictionary<int, List<int>> FullMatchFactorsLcm8 = new()
    {
        [0] = { 2, 2, 2 },
        [2] = { 3, 3 },
        [4] = { 2, 5 },
        [7] = { 2, 2, 3 },
        [11] = { 3, 5 }
    };
    public static Dictionary<int, List<int>> FullMatchFactorsLcm15 = new()
    {
        [0] = { 2, 2, 2 },
        [2] = { 3, 3 },
        [4] = { 2, 5 },
        [7] = { 2, 2, 3 },
        [11] = { 3, 5 }
    };


}
