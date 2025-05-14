using Fractions;
using Melodroid_2.MusicUtils;
using static Melodroid_2.MusicUtils.MusicTheory;

namespace Melodroid_2;

internal class Program
{
    static void Main(string[] args)
    {
        List<Fraction> MajorChord = [Unison, PerfectFifth, MajorThird];
        List<Fraction> ClusterTargets = GoodFractions;
        double clusterWidth = MaximumBinRadius;
        double sweepStep = 0.001;
        var ratiosToSweep = MajorChord.Select(fraction => fraction.ToDouble()).ToList();
        OctaveSweep sweep = new(ratiosToSweep, ClusterTargets, clusterWidth, sweepStep);

        // Write header
        Console.Write($"      ");
        foreach (Fraction target in ClusterTargets)
            Console.Write($"{target,-5} ");
        Console.WriteLine();

        // Write data
        foreach (var sweepData in sweep.OctaveSweepData)
        {
            // Skip line if empty
            if (sweepData.ClusterTargetMatches.Count == 0)
                continue;

            double fundamental = sweepData.Fundamental;
            Console.Write($"{fundamental,-4:F2}: ");
            foreach (Fraction target in ClusterTargets)
            {
                if (sweepData.ClusterTargetMatches.ContainsValue(target))
                {
                    double ratioMatchingTarget = 0;
                    foreach (var keyValue in sweepData.ClusterTargetMatches)
                    {
                        if (keyValue.Value == target)
                        {
                            ratioMatchingTarget = keyValue.Key;
                        }
                    }
                    if (ratioMatchingTarget > 0)
                        //Console.Write($"{ratioMatchingTarget,-5:F1} ");
                        Console.Write($"{target.ToString(),-5:F1} ");
                    else
                        Console.Write("      ");
                }
                else
                    Console.Write("      ");
            }
            Console.WriteLine();
        }
    }
}
