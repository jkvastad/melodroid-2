using Fractions;
using Melodroid_2.MusicUtils;
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
        //var keyOffset = (double)MinorSecond;
        ////var targetRatios = new HashSet<double>() { 1.0666666666666667, 1.25, 1.50 };
        //var targetRatios = DimChord.Select(fraction => (double)(fraction * keyOffset)).ToHashSet();
        ////var keyOffset = PerfectFourth;
        ////var targetRatios = MajorChordFractions.Select(fraction => (double)(fraction * keyOffset)).ToHashSet();        
        //ChordProgression chordProgression = new(originRatios, targetRatios, GoodFractions, clusterWidth: 0.02);
        //foreach (var consoleRow in chordProgression.GetConsoleOutput(3, 3))
        //    Console.WriteLine(consoleRow);

        // Tonal Coverage Test                
        //var FractionsToSweep = MinorChord_Tritone;
        //var ratiosToSweep = FractionsToSweep.Select(fraction => (double)fraction).ToList();
        //var tonalCoverageCalculator = new TonalCoverageCalculator(
        //    ratiosToSweep,
        //    clusterWidth: 0.01);
        //foreach (var consoleRow in tonalCoverageCalculator.GetConsoleOutput(3, 2))
        //    Console.WriteLine(consoleRow);


        // Octave Sweep Test                
        var FractionsToSweep = DimChord;
        List<Fraction> ClusterTargets = GoodFractions;
        double clusterWidth = 0.01;
        double sweepStep = 0.001;
        var ratiosToSweep = FractionsToSweep.Select(fraction => (double)fraction).ToHashSet();
        OctaveSweep sweep = new(ratiosToSweep, ClusterTargets, clusterWidth, sweepStep);
        foreach (var consoleRow in sweep.GetConsoleOutput())
            Console.WriteLine(consoleRow);
        Console.WriteLine("---");
        var ratiosToSweep2 = new HashSet<double>() { 1.0666666666666667, 1.25, 1.50 };
        //var ratiosToSweep2 = DimChord.ToHashSet();        
        OctaveSweep sweep2 = new(ratiosToSweep2, ClusterTargets, clusterWidth, sweepStep);
        foreach (var consoleRow2 in sweep2.GetConsoleOutput())
            Console.WriteLine(consoleRow2);

    }
}
