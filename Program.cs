using Fractions;
using Melodroid_2.MusicUtils;
using static Melodroid_2.MusicUtils.MusicTheory;
using static Melodroid_2.MusicUtils.Utils;

namespace Melodroid_2;

internal class Program
{
    static void Main(string[] args)
    {
        List<Fraction> FractionsToSweep = LCM15.Except([MajorSixth]).ToList();
        List<Fraction> ClusterTargets = GoodFractions;
        double clusterWidth = MaximumBinRadius;
        double sweepStep = 0.001;
        var ratiosToSweep = FractionsToSweep.Select(fraction => fraction.ToDouble()).ToList();
        OctaveSweep sweep = new(ratiosToSweep, ClusterTargets, clusterWidth, sweepStep);

        // Write header
        Console.Write($"      ");
        foreach (Fraction target in ClusterTargets)
            Console.Write($"{target,-5} ");
        Console.Write($"LCM", -5);
        Console.WriteLine();

        // Write data
        Dictionary<Fraction, double> previousRow = [];
        foreach (var sweepData in sweep.OctaveSweepData)
        {
            // Skip line if empty
            if (sweepData.ClusterTargetMatches.Count == 0)
                continue;
           
            // Create row to print
            Dictionary<Fraction, double> currentRow = []; // Target keys, ratio sweep value
            foreach (double targetRatio in sweepData.ClusterTargetMatches.Keys)
                currentRow[sweepData.ClusterTargetMatches[targetRatio]] = targetRatio;

            // Skip row if identical to last
            if (currentRow.Keys.SequenceEqual(previousRow.Keys))
                continue;

            // Print row
            double fundamental = sweepData.Fundamental;
            Console.Write($"{fundamental,-4:F2}: ");
            foreach (Fraction target in ClusterTargets)
            {
                if (currentRow.ContainsKey(target))
                    Console.Write($"{target.ToString(),-5:F1} ");
                else
                    Console.Write("      ");
            }
            var lcm = LCM(sweepData.ClusterTargetMatches.Values.Select(fraction => (long)fraction.Denominator).ToArray());
            Console.Write(lcm);
            Console.WriteLine();
            previousRow = currentRow;
        }
    }
}
