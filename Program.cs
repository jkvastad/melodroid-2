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


    static void Main(string[] args)
    {
        // Chord Crogression Test
        //var originRatios = MajorChordFractions.Select(fraction => (double)(fraction)).ToHashSet();
        //var keyOffset = PerfectFourth;
        //var targetRatios = MajorChordFractions.Select(fraction => (double)(fraction * keyOffset)).ToHashSet();
        //ChordProgression chordProgression = new(originRatios, targetRatios, GoodFractions, clusterWidth: 0.01);
        //foreach (var consoleRow in chordProgression.GetConsoleOutput())
        //{
        //    Console.WriteLine(consoleRow);
        //}

        // Tonal Coverage Test        
        //List<Fraction> FractionsToSweep = LCM15.Except([MajorSixth]).ToList();
        //List<Fraction> FractionsToSweep = AugmentedChord;
        //List<double> FractionsToSweep = AugmentedChord;
        //var ratiosToSweep = FractionsToSweep.Select(fraction => (double)fraction).ToList();
        //var tonalCoverageCalculator = new TonalCoverageCalculator(
        //    ratiosToSweep,
        //    clusterWidth: 0.01);
        //foreach (var consoleRow in tonalCoverageCalculator.GetConsoleOutput(5, 2))
        //    Console.WriteLine(consoleRow);


        // Octave Sweep Test
        //List<Fraction> FractionsToSweep = LCM15.Except([MajorSixth]).ToList();
        List<Fraction> FractionsToSweep = MajorScale;
        //List<double> FractionsToSweep = DimChord;
        List<Fraction> ClusterTargets = GoodFractions;
        double clusterWidth = 0.01;
        double sweepStep = 0.001;
        var ratiosToSweep = FractionsToSweep.Select(fraction => (double)fraction).ToHashSet();
        OctaveSweep sweep = new(ratiosToSweep, ClusterTargets, clusterWidth, sweepStep);
        foreach (var consoleRow in sweep.GetConsoleOutput())
            Console.WriteLine(consoleRow);

    }
}
