using Fractions;
using Melodroid_2.LCMs;
using Melodroid_2.MidiMakers;
using Melodroid_2.MidiMakers.ChromaComposer;
using Melodroid_2.MidiMakers.RandomComposer;
using Melodroid_2.MidiMakers.TonalCoverComposer;
using Melodroid_2.MusicUtils;
using Serilog;
using System.Collections.Immutable;
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

        // Tonal Coverage Test                
        //var FractionsToSweep = new List<double>()
        //{
        //    (double) MinorSeventh,
        //    (double) MinorSecond,
        //    (double) PerfectFourth,
        //    (double) MajorSixth,
        //    (double) Unison,
        //    (double) MinorThird,
        //    (double) Tritone,
        //};
        //var ratiosToSweep = FractionsToSweep.Select(fraction => (double)fraction).ToList();
        //var tonalCoverageCalculator = new TonalCoverageCalculator(
        //    ratiosToSweep,
        //    clusterWidth: 0.012);
        //foreach (var consoleRow in tonalCoverageCalculator.GetConsoleOutput(3, 2, maxSubsetLcm: 24))
        //    Console.WriteLine(consoleRow);


        //// Octave Sweep Test                
        //var FractionsToSweep = new List<double>()
        //{
        ////(double)Unison,
        ////(double)MinorSecond,
        ////(double)MajorSecond,
        ////(double)MinorThird,
        //(double)MajorThird,
        ////(double)PerfectFourth,
        ////Tritone,
        ////(double)PerfectFifth,
        ////(double)MinorSixth,
        ////(double)MajorSixth,
        ////(double)MinorSeventh,
        //(double)MajorSeventh
        //};
        //List<Fraction> ClusterTargets = GoodFractions;
        //double clusterWidth = 0.013;
        //double sweepStep = 0.001;
        //var ratiosToSweep = FractionsToSweep.Select(fraction => (double)fraction).ToHashSet();
        //OctaveSweep sweep = new(ratiosToSweep, ClusterTargets, clusterWidth, sweepStep);
        //foreach (var consoleRow in sweep.GetConsoleOutput(fullMatchOnly: true, skipDuplicateResults: true))
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

        Dictionary<int, HashSet<Bit12Int>> origins = Utils.CalculateAllChordOrigins();

        foreach (var cardinality in origins.Keys)
        {
            Console.WriteLine($"--- Cardinality: {cardinality} ---");
            foreach (var mask in origins[cardinality])
            {
                Console.WriteLine($"{mask.ToIntervalString()}");
            }
        }
    }
}
