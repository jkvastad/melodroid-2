using Melodroid_2.LCMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melodroid_2.MidiMakers.TonalCoverComposer;

/// <summary>
/// A tonal cover consists of Tonal Sets which are sets of notes...
/// ... whose union span all sounding notes
/// ... have a common LCM factor for some fundamental
/// </summary>
public class TonalCover
{
    List<TonalSet> TonalSets { get; set; }
    public TonalCover(List<TonalSet> tonalSets)
    {
        TonalSets = tonalSets;
    }
}

/// <summary>
/// A TonalSet is a set of notes which sounds consonant when played as a block. 
/// This is all subsets of the LCM8 isomorphism ring (8,9,10,12), as well as 
/// </summary>
public class TonalSet
{
    public virtual Bit12Int ChromaMask { get; set; } // e.g. major triad is new(0b10010001)        
    public TonalSet(Bit12Int chromaMask)
    {
        //TODO: map chroma mask to full match LCM factors
        // perhaps a dictionary taking chroma masks and outputing full match lcm factors?

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
