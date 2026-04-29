using Melanchall.DryWetMidi.MusicTheory;
using System.ComponentModel;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Melodroid_2.MusicUtils;

public class ChordCalculator
{
    /// <summary>
    /// <para>Calculates all TET12 chords fulfilling some pragmatic boundary conditions. The conditions are:</para>
    ///    <list type = "bullet">    
    ///    <item><description>
    ///    A chord should consist of at least 2 notes (to contrast it with melody)
    ///    </description></item>
    ///    <item><description>
    ///    A chord should have at most two notes in a row (three is too dissonant)
    ///    </description></item>
    ///    <item><description>
    ///    A chord should have at most two pairs of adjacent notes (three is too dissonant)
    ///    </description></item>
    ///    <item><description>
    ///    Chords are in root position
    ///    </description></item>
    ///    <item><description>
    ///    Chords are octave equivalent
    ///    </description></item>    
    ///    </list>    
    /// </summary>
    /// <returns>
    /// <para>
    /// Outer dictionary with key being number of keys in the chord.
    /// </para>
    /// <para>
    /// Inner dictionary with key being a chord in 12bit representation, with value being the first discovered octave equivalent chord (possibly itself).
    /// </para>
    /// <para>
    /// This is useful when studying unique chords, ignoring voicings and fundamentals
    /// </para>
    /// </returns>
    public static Dictionary<int, Dictionary<Bit12Int, Bit12Int>> CalculateAllPragmaticTET12Chords()
    {
        Dictionary<int, Dictionary<Bit12Int, Bit12Int>> allPragmaticTET12Chords = [];
        // Possible keys in chord including 0
        for (int i = 0; i <= 12; i++)
            allPragmaticTET12Chords[i] = [];
        // Create all chords
        for (int i = 0; i < 4096; i++)
        {
            Bit12Int chord = i;
            // Skip non-root position chords
            if (!chord.IsBitSet(0))
                continue;
            // Skip chords with less than two notes
            int bitsInChord = NumberOfBitmaskMatches(chord, new Bit12Int(1));
            if (bitsInChord < 2)
                continue;
            // Skip chords with three or more consecutive bits. 7 is 111.
            if (NumberOfBitmaskMatches(chord, new Bit12Int(7)) > 0)
                continue;
            // Skip chords with two or more pairs of consecutive bits. 3 is 11.
            // Note that this also matches e.g. triplets as multiple pairs - this is fine as we already sorted those out.
            if (NumberOfBitmaskMatches(chord, new Bit12Int(3)) > 2)
                continue;

            // If chord is mapped then all its rotations are mapped, skip it
            if (allPragmaticTET12Chords[bitsInChord].ContainsKey(chord))
                continue;

            // New chord, add it to mapping
            // Create all rotations of chord
            List<Bit12Int> rotatedChords = [];
            for (int j = 0; j < 12; j++)
            {
                Bit12Int rotatedChord = chord << j;
                // Skip non-root rotations                
                if (!rotatedChord.IsBitSet(0))
                    continue;
                rotatedChords.Add(rotatedChord);
            }
            // Map rotations to first discovered chord
            foreach (var rotatedChord in rotatedChords)
                allPragmaticTET12Chords[bitsInChord][rotatedChord] = chord;
        }
        return allPragmaticTET12Chords;
    }

    public static void PrintAllPragmaticTET12Chords()
    {
        Dictionary<int, Dictionary<Bit12Int, Bit12Int>> chordMappings = CalculateAllPragmaticTET12Chords();
        foreach (var numberOfKeys in chordMappings.Keys)
        {
            Console.WriteLine("Chords with " + numberOfKeys.ToString() + " keys:");
            var chordMappingsPerKeyAmount = chordMappings[numberOfKeys];
            // Get chords mapping onto themselves and print their rotations
            foreach (var chord in chordMappingsPerKeyAmount.Keys)
            {
                if (chordMappingsPerKeyAmount[chord] == chord)
                {
                    // print all rotations
                    List<Bit12Int> chordRotations = chord.GetAllRotations();
                    foreach (var rotatedChord in chordRotations)
                    {
                        // only print root positions
                        if(rotatedChord.IsBitSet(0))
                            Console.Write($"{rotatedChord.ToIntervalString()} - ");
                    }
                    Console.WriteLine();
                }
            }
        }
    }

    /// <summary>
    /// Returns the number of times the bitmask matched the chord under rotation
    /// </summary>
    /// <param name="chord"></param>
    /// <param name="bitmask"></param>
    /// <returns></returns>
    public static int NumberOfBitmaskMatches(Bit12Int chord, Bit12Int bitmask)
    {
        int matches = 0;
        for (int j = 0; j < 12; j++)
        {
            Bit12Int rotatedMask = bitmask << j;
            if ((chord & rotatedMask) == rotatedMask)
                matches++;
        }
        return matches;
    }

}