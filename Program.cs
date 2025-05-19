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
    public static readonly List<Fraction> MinorChord_Tritone = [
        Unison,
        MinorThird,
        Tritone,
        PerfectFifth,
    ];


    static void Main(string[] args)
    {
        // Tonal Coverage Test
        var tonalCoverageCalculator = new TonalCoverageCalculator(MinorChord_Tritone.Select(fraction => (double)fraction).ToList());
        foreach (var setPair in tonalCoverageCalculator.TonalCoverages.Keys)
        {
            foreach (var set in setPair)
            {
                Console.Write($"({string.Join(" ", set.Select(n => n.ToString("F1")))})");
            }
            Console.WriteLine();
            foreach (var tonalCoverage in tonalCoverageCalculator.TonalCoverages[setPair])
            {
                Console.WriteLine($"{tonalCoverage}");
            }
        }

        // Octave Sweep Test -- move data printing to octave sweep class?
        //List<Fraction> FractionsToSweep = MajorChordFractions;
        //List<Fraction> ClusterTargets = GoodFractions;
        //double clusterWidth = MaximumBinRadius;
        //double sweepStep = 0.001;
        //var ratiosToSweep = FractionsToSweep.Select(fraction => fraction.ToDouble()).ToList();
        //OctaveSweep sweep = new(ratiosToSweep, ClusterTargets, clusterWidth, sweepStep);

        //// Write header
        //Console.Write($"      ");
        //foreach (Fraction target in ClusterTargets)
        //    Console.Write($"{target,-5} ");
        //Console.Write($"{"LCM",-5}");
        //Console.Write($"{"Hits",-5}");
        //Console.WriteLine();

        //// Write data
        //Dictionary<Fraction, double> previousRow = [];
        //foreach (var sweepData in sweep.OctaveSweepData)
        //{
        //    // Skip line if empty
        //    if (sweepData.ClusterTargetMatches.Count == 0)
        //        continue;

        //    // Create row to print
        //    Dictionary<Fraction, double> currentRow = []; // Target keys, ratio sweep value
        //    foreach (double targetRatio in sweepData.ClusterTargetMatches.Keys)
        //        currentRow[sweepData.ClusterTargetMatches[targetRatio]] = targetRatio;

        //    // Skip row if identical to last
        //    if (currentRow.Keys.SequenceEqual(previousRow.Keys))
        //        continue;

        //    // Print row
        //    double fundamental = sweepData.Fundamental;
        //    Console.Write($"{fundamental,-4:F2}: ");
        //    foreach (Fraction target in ClusterTargets)
        //    {
        //        if (currentRow.ContainsKey(target))
        //            Console.Write($"{target.ToString(),-5:F1} ");
        //        else
        //            Console.Write("      ");
        //    }
        //    var lcm = LCM(sweepData.ClusterTargetMatches.Values.Select(fraction => (long)fraction.Denominator).ToArray());
        //    Console.Write($"{lcm,-5}");
        //    Console.Write($"{sweepData.ClusterTargetMatches.Count(),-5}");
        //    Console.WriteLine();
        //    previousRow = currentRow;
        //}
    }
}
