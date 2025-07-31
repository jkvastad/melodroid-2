using Fractions;
using Melodroid_2.LCMs;
using Melodroid_2.MidiMakers;
using Melodroid_2.MidiMakers.ChromaComposer;
using Melodroid_2.MidiMakers.RandomComposer;
using Melodroid_2.MidiMakers.TonalCoverComposer;
using Melodroid_2.MusicUtils;
using Serilog;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using static Melodroid_2.MusicUtils.MusicTheory;
using static Melodroid_2.MusicUtils.Utils;

namespace Melodroid_2;

public class Program
{
    public static readonly List<Fraction> LCM15_No_Major_Sixth = [
        Unison,
        MinorSecond,
        MinorThird,
        PerfectFourth,
        MinorSixth,
        MinorSeventh
       ];
    public static readonly List<Fraction> LCM15_No_Minor_Sixth = [
        Unison,
        MinorSecond,
        MinorThird,
        PerfectFourth,
        MajorSixth,
        MinorSeventh
       ];
    public static readonly List<Fraction> LCM15_No_Minor_Seventh = [
        Unison,
        MinorSecond,
        MinorThird,
        PerfectFourth,
        MinorSixth,
        MajorSixth,
    ];
    public static readonly List<double> MinorChord_Tritone = [
        (double)Unison,
        (double)MinorThird,
        Tritone,
        (double)PerfectFifth,
    ];
    public static readonly List<double> MinorChord_PerfectFourth = [
        (double)Unison,
        (double)MinorThird,
        (double)PerfectFourth,
        (double)PerfectFifth,
    ];
    public static readonly List<double> DimChord_PerfectFourth = [
        (double)Unison,
        (double)MinorThird,
        (double)PerfectFourth,
        Tritone,
    ];

    public static readonly List<double> MajorScaleAtNote8 = [
        (double)Unison,
        (double)MinorSecond,
        (double)MinorThird,
        (double)PerfectFourth,
        (double)PerfectFifth,
        (double)PerfectFifth,
        (double)MinorSixth,
        (double)MinorSeventh
    ];
    public static readonly List<double> MyNotes = [
        (double)Unison,
        (double)MinorSecond,
        (double)MinorThird,
        (double)PerfectFourth,
        (double)MajorSixth,
        (double)MinorSeventh
    ];
    public static readonly List<double> MyNotes2 = [
        (double)Unison,
        (double)MinorSecond,
        (double)MinorThird,
        (double)PerfectFourth,
        (double)MinorSixth,
        (double)MajorSixth,
        (double)MinorSeventh
    ];
    public static readonly List<double> MyNotes3 = [
        (double)Unison,
        (double)MinorThird,
        (double)PerfectFourth,
        (double)MajorSixth,
        (double)Tritone,
        //(double)MinorSeventh
    ];
    // augmented chord perception? LCM 20?
    public static readonly List<double> MyNotes4 = [
        (double)MinorThird,
        (double)PerfectFourth,
        (double)MajorSeventh
    ];


    static void Main(string[] args)
    {
        // Chord Crogression Test         
        //TODO: Fix error - missing 0 4 7 (10@4) -> 1 4 7 (15@4). Perhaps some error relating to cluster width
        // - result shows up at (large) cluster width 0.2
        //var originRatios = MajorChordFractions.Select(fraction => (double)(fraction)).ToHashSet();
        //var keyOffset = (double)MinorSixth;
        ////var targetRatios = new HashSet<double>() { 1.0666666666666667, 1.25, 1.50 };
        //var targetRatios = MajorChordFractions.Select(fraction => ((double)fraction * keyOffset)).ToHashSet();
        ////var keyOffset = PerfectFourth;
        ////var targetRatios = MajorChordFractions.Select(fraction => (double)(fraction * keyOffset)).ToHashSet();        
        //ChordProgression chordProgression = new(originRatios, targetRatios, GoodFractions, clusterWidth: 0.02);
        //foreach (var consoleRow in chordProgression.GetConsoleOutput(3, 3))
        //    Console.WriteLine(consoleRow);

        //// Tonal Coverage Test                
        //var FractionsToSweep = new List<double>()
        //{
        //(double)Unison,
        //(double)MinorSecond,
        ////(double)MajorSecond,
        //(double)MinorThird,
        ////(double)MajorThird,
        ////(double)PerfectFourth,
        ////Tritone,
        //(double)PerfectFifth,
        ////(double)MinorSixth,
        ////(double)MajorSixth,
        ////(double)MinorSeventh,
        ////(double)MajorSeventh
        //};
        //var ratiosToSweep = FractionsToSweep.Select(fraction => (double)fraction).ToList();
        //var tonalCoverageCalculator = new TonalCoverageCalculator(
        //    ratiosToSweep,
        //    clusterWidth: 0.022);
        //foreach (var consoleRow in tonalCoverageCalculator.GetConsoleOutput(3, 2, maxSubsetLcm: 24))
        //    Console.WriteLine(consoleRow);

        // Tet12 Tonal Coverage Test
        // 0b000010010001 for major chord
        // 0b000010001001 for minor chord
        // 0b000001001001 for dim chord
        Bit12Int tet12CoverKeys = 0b010010011001;
        var tonalCoverage = Tet12TonalCoverCalculator.CalculateTet12TonalCoverage(tet12CoverKeys, no15Collapse: true);
        foreach (var origin in tonalCoverage.Keys)
        {
            Console.WriteLine(origin.ToIntervalString());
            var sortedCoverage = tonalCoverage[origin].OrderBy(data => data.fundamental);
            foreach (var (fundamental, complement, originLcm, complementLcm) in sortedCoverage)
            {
                Console.Write($"{fundamental,-2}: ");
                Console.Write($"{originLcm,-2} + ");
                Console.Write($"{complementLcm,-2} - ");
                Console.Write($"{complement.ToIntervalString()} ");
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        //// Octave Sweep Test                
        //var FractionsToSweep = new List<double>()
        //{
        //(double)Unison,
        ////(double)MinorSecond,
        //(double)MajorSecond,
        ////(double)MinorThird,
        //(double)MajorThird,
        //(double)PerfectFourth,
        ////Tritone,
        //(double)PerfectFifth,
        ////(double)MinorSixth,
        //(double)MajorSixth,
        ////(double)MinorSeventh,
        //(double)MajorSeventh
        //};
        //List<Fraction> ClusterTargets = GoodFractions;
        //double clusterWidth = 0.013;
        //double sweepStep = 0.001;
        //var ratiosToSweep = FractionsToSweep.Select(fraction => (double)fraction).ToHashSet();
        //OctaveSweep sweep = new(ratiosToSweep, ClusterTargets, clusterWidth, sweepStep);
        //foreach (var consoleRow in sweep.GetConsoleOutput(fullMatchOnly: false, skipDuplicateResults: true))
        //    Console.WriteLine(consoleRow);

        //Console.WriteLine("---");
        //var fractionsToSweep2 = new List<double>()
        //{
        //    (double) MinorSeventh,
        //    (double) MinorSecond,
        //    (double) PerfectFourth,
        //    (double) MinorSixth,
        //    (double) Unison,
        //    (double) MinorThird,
        //};
        //var ratiosToSweep2 = fractionsToSweep2.Select(fraction => (double)fraction).ToHashSet();
        //OctaveSweep sweep2 = new(ratiosToSweep2, ClusterTargets, clusterWidth, sweepStep);
        //foreach (var consoleRow2 in sweep2.GetConsoleOutput())
        //    Console.WriteLine(consoleRow2);

        //Log.Logger = new LoggerConfiguration()
        //.WriteTo.File(@"D:\Projects\Code\Melodroid 2\logs\log.txt", rollingInterval: RollingInterval.Infinite)
        //.CreateLogger();

        //TonalCoverComposer composer = new();
        //string folderPath = @"E:\Documents\Reaper Projects\Melodroid\MIDI_write_testing\TonalCoverComposer";
        //composer.Compose();
        //string fileName = "tonal_composer_test";
        //var midiNotes = NotesToMidi.TimeEventsToNotes(composer.TimeEvents);
        //NotesToMidi.WriteNotesToMidi(composer.Notes, folderPath, fileName, bpm: 60, overWrite: true);

        //ChromaComposer composer = new();
        //composer.Compose();
        //string folderPath = @"E:\Documents\Reaper Projects\Melodroid\MIDI_write_testing\ChromaComposer";
        //string fileName = "chroma_composer_test";
        //NotesToMidi.WriteNotesToMidi(composer.Notes, folderPath, fileName, bpm: 60, overWrite: true);

        //RandomComposer composer = new();
        //composer.Compose();
        //string folderPath = @"E:\Documents\Reaper Projects\Melodroid\MIDI_write_testing\ChromaComposer";
        //string fileName = "random_composer_test";
        //NotesToMidi.WriteNotesToMidi(composer.Notes, folderPath, fileName, bpm: 60, overWrite: true);


        //var targets = CalculateAllChordTargets();
        //foreach (var target in targets)
        //{
        //    Console.WriteLine($"--- Factor: {target.Key} ---");
        //    foreach (var mask in target.Value)
        //    {
        //        Console.WriteLine($"{mask.ToIntervalString()}");
        //    }
        //    Console.WriteLine();
        //}

        //Bit12Int minorC = new Bit12Int(0b000010001001);
        //Dictionary<int, List<int>> targets = Tet12ChromaMask.GetAllMaskLCMs(minorC);
        //foreach (var target in targets)
        //{
        //    if (target.Value.Count == 0)
        //        continue;
        //    Console.WriteLine($"--- Key: {target.Key} ---");
        //    foreach (var lcm in target.Value)
        //    {
        //        Console.WriteLine($"{lcm}");
        //    }
        //    Console.WriteLine();
        //}

        //Bit12Int majorGSharp = new Bit12Int(0b000100001001);
        //Dictionary<int, List<int>> targets2 = Tet12ChromaMask.GetAllMaskLCMs(majorGSharp);
        //foreach (var target2 in targets2)
        //{
        //    if (target2.Value.Count == 0)
        //        continue;
        //    Console.WriteLine($"--- Key: {target2.Key} ---");
        //    foreach (var lcm in target2.Value)
        //    {
        //        Console.WriteLine($"{lcm}");
        //    }
        //    Console.WriteLine();
        //}


        //// Calculate all unique chord origins - all unique chord patterns at root position (no tritone) and their full match rotations
        //Dictionary<int, HashSet<Bit12Int>> origins = Utils.CalculateUniqueChordOrigins();
        //int maxLcm = 15; // max proper lcm seems to be 15 - no good fraction contains larger a number (16 is identical to 8)
        //List<int> excludedLcms = [18, 20]; // less than 24 but both seem to require numerator 25

        //foreach (var cardinality in origins.Keys)
        //{
        //    // print header
        //    Console.WriteLine($"--- Cardinality: {cardinality} ---");
        //    Console.Write("".PadLeft(3 * cardinality)); // 2 chars for digits, one for space
        //    for (int i = 0; i < 12; i++)
        //        Console.Write($"{i,-2} ");
        //    Console.WriteLine();

        //    foreach (var mask in origins[cardinality])
        //    {
        //        // print mask
        //        Console.Write($"{mask.ToIntervalString()}".PadRight(3 * cardinality));
        //        // print mask rows
        //        Dictionary<int, List<int>> maskLcms = Tet12ChromaMask.GetAllMaskLCMs(mask, maxLcm: maxLcm);
        //        // Exclude bad lcms
        //        foreach (var key in maskLcms.Keys)
        //            foreach (var excludedLcm in excludedLcms)
        //                maskLcms[key].Remove(excludedLcm);

        //        int rows = maskLcms.Values.MaxBy(lcms => lcms.Count)!.Count;
        //        // write lcm at each fundamental
        //        int paddingMultiples = 0;
        //        foreach (var key in maskLcms.Keys)
        //        {
        //            if (maskLcms[key].Count > 0)
        //            {
        //                Console.Write($"{maskLcms[key][0],-2} ".PadLeft((paddingMultiples + 1) * 3));
        //                paddingMultiples = 0;
        //            }
        //            else
        //                paddingMultiples++;
        //        }
        //        if (rows > 1)
        //        {
        //            Console.WriteLine();
        //            Console.Write("".PadRight(3 * cardinality));
        //            foreach (var key in maskLcms.Keys)
        //            {
        //                if (maskLcms[key].Count > 1)
        //                {
        //                    Console.Write($"{maskLcms[key][1],-2} ".PadLeft((paddingMultiples + 1) * 3));
        //                    paddingMultiples = 0;
        //                }
        //                else
        //                    paddingMultiples++;
        //            }
        //        }
        //        Console.WriteLine();
        //    }
        //}

        //    // Calculate tonal cover for melody
        //    Bit12Int chord = 0b000010010001;
        //    Bit12Int melodyBit = 0b000000001000;
        //    Bit12Int totalComb = chord | melodyBit;
        //    var combinations = chord.GetSetBitCombinations();
        //    var melodyCombinations = combinations.Select(comb => comb | melodyBit);

        //    Dictionary<int, List<string>> output = [];
        //    for (int key = 0; key < 12; key++)
        //        output[key] = [];

        //    // Calculate overlapping factors
        //    foreach (var melodyComb in melodyCombinations)
        //    {
        //        foreach (var comb in combinations)
        //        {
        //            // cover all keys?                
        //            if (totalComb != (melodyComb | comb))
        //                continue;

        //            var combLcms = Tet12ChromaMask.GetAllMaskLCMs(comb);
        //            var melodyLcms = Tet12ChromaMask.GetAllMaskLCMs(melodyComb);
        //            for (int key = 0; key < 12; key++)
        //            {
        //                // match at key?
        //                if (combLcms[key].Count > 0 && melodyLcms[key].Count > 0)
        //                {
        //                    // primes 2,3,5 can be upscaled to 8,9,10,12,15
        //                    // - Any match in origin can share a factor with melody set via superset substitution, treating the origin as a subset
        //                    // - Thus any match can work as a tonal cover as long as all keys are covered                        
        //                    string combMatches = $"{comb.ToIntervalString()} ({string.Join(", ", combLcms[key])})";
        //                    string melodyMatches = $"{melodyComb.ToIntervalString()} ({string.Join(", ", melodyLcms[key])})";
        //                    output[key].Add(combMatches);
        //                    output[key].Add(melodyMatches);
        //                    output[key].Add("");
        //                }
        //            }
        //        }
        //    }

        //    for (int key = 0; key < 12; key++)
        //    {
        //        Console.WriteLine($"{key}:");
        //        foreach (var line in output[key])
        //        {
        //            Console.WriteLine(line);
        //        }
        //        Console.WriteLine();
        //    }
        //}

        //// Calculate chord progressions        
        //// 0b000010010001 for major chord
        //// 0b000010001001 for minor chord
        //Bit12Int originChord = 0b000010010001;
        //int maxLcm = 24;
        //var originCombinations = originChord.GetSetBitCombinations();
        //int ogSubsetMin = 3;
        //originCombinations = originCombinations.Where(comb => BitOperations.PopCount((uint)comb) >= ogSubsetMin).ToList();
        //List<int> excludedLcms = [18, 20]; // less than 24 but both seem to require numerator 25

        //int targetChordMaxSize = 4;
        //int targetChordMinSize = 2;
        //Dictionary<int, HashSet<Bit12Int>> uniqueChords = Utils.CalculateUniqueChordOrigins();
        //for (int cardinality = 0; cardinality < 12; cardinality++)
        //    if (cardinality < targetChordMinSize || cardinality > targetChordMaxSize)
        //        uniqueChords.Remove(cardinality);

        //// calculate all chord lcms
        //Dictionary<Bit12Int, Dictionary<int, List<int>>> uniqueChordsToLcms = [];
        //foreach (var keypair in uniqueChords)
        //{
        //    foreach (var chord in keypair.Value)
        //    {
        //        // Exclude bad lcms
        //        Dictionary<int, List<int>> maskLcms = Tet12ChromaMask.GetAllMaskLCMs(chord, maxLcm: maxLcm);
        //        foreach (var key in maskLcms.Keys)
        //            foreach (var excludedLcm in excludedLcms)
        //                maskLcms[key].Remove(excludedLcm);

        //        uniqueChordsToLcms[chord] = maskLcms;
        //    }
        //}

        //Dictionary<int, List<TET12ChordProgression>> output = [];
        //for (int key = 0; key < 12; key++)
        //    output[key] = [];

        //// check all origin chord subsets versus all possible chords
        //foreach (var ogComb in originCombinations)
        //{
        //    Dictionary<int, List<int>> ogCombLcms = Tet12ChromaMask.GetAllMaskLCMs(ogComb, maxLcm: maxLcm);
        //    for (int key = 0; key < 12; key++)
        //    {
        //        for (int cardinality = targetChordMinSize; cardinality < targetChordMaxSize; cardinality++)
        //        {
        //            foreach (var chord in uniqueChords[cardinality])
        //            {
        //                // all rotations of unique chords yields all chords
        //                for (int rotation = 0; rotation < 12; rotation++)
        //                {
        //                    // matches with primes 2,3,5 can be upscaled to 8,9,10,12,15
        //                    // - Any match in origin can share a factor with target via superset substitution, treating the origin as a subset                        
        //                    var chordLcms = uniqueChordsToLcms[chord];
        //                    var rotatedChord = chord << rotation;
        //                    // access rotation of unique chord lcms instead of calculating lcms of all rotated chords
        //                    int rotatedChordRoot = (key - rotation + 12) % 12;
        //                    if (ogCombLcms[key].Count > 0 && chordLcms[rotatedChordRoot].Count > 0)
        //                    {
        //                        TET12ChordProgression progression = new(ogComb, ogCombLcms[key], rotatedChord, chordLcms[rotatedChordRoot], key);
        //                        output[key].Add(progression);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //// Enter a target archetype to check for progression
        //// 0b000010010001 for major chord
        //// 0b000010001001 for minor chord
        //// 0b000001001001 for dim chord
        //Bit12Int targetArchetype = 0b000001001001;
        //HashSet<Bit12Int> uniqueTargets = [];
        //Dictionary<Bit12Int, List<TET12ChordProgression>> uniqueTargetsProgressions = [];
        //for (int key = 0; key < 12; key++)
        //{
        //    Console.WriteLine($"{key}:");
        //    foreach (TET12ChordProgression cp in output[key])
        //    {
        //        if (cp.TargetContains(targetArchetype))
        //        {
        //            if (!uniqueTargetsProgressions.ContainsKey(cp.Target))
        //                uniqueTargetsProgressions[cp.Target] = [];
        //            uniqueTargetsProgressions[cp.Target].Add(cp);
        //            string outputLine = $"{cp.Origin.ToIntervalString()} ({string.Join(", ", cp.OriginLCMs)}) {cp.Target.ToIntervalString()} ({string.Join(", ", cp.TargetLCMs)})";
        //            Console.WriteLine(outputLine);
        //        }
        //    }
        //    Console.WriteLine();
        //}
        //foreach (var uniqueTarget in uniqueTargetsProgressions.Keys)
        //{
        //    Console.Write($"{uniqueTarget.ToIntervalString()}: ");
        //    foreach (var progression in uniqueTargetsProgressions[uniqueTarget])
        //    {
        //        Console.Write($"({progression.Key} - {string.Join(", ", progression.OriginLCMs)} / {string.Join(", ", progression.TargetLCMs)}) ");
        //    }
        //    Console.WriteLine();
        //}
    }
}