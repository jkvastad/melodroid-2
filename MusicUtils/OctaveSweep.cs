using Fractions;
using System.Text;
using static Melodroid_2.MusicUtils.Utils;

namespace Melodroid_2.MusicUtils;
/// <summary>
/// An octave sweep takes real ratios and clusters them to cluster targets (fractions) for some fundamentals on the octave (ideally all fundamentals but that is computationally impossible and unnecessary).
/// An octave sweep consists of 
/// * A set of ratios (real numbers on [1,2))
/// * A set of cluster targets (fractions on [1,2))
/// * A cluster width (relative distance around cluster targets in which to bin ratios)
/// * A sweep step size (increments of the fundamental per sweep step)
/// The sweep returns all clustering results per sweep step
/// </summary>
public class OctaveSweep
{
    public List<double> RatiosToSweep { get; set; } = [];
    public List<Fraction> ClusterTargets { get; set; } // The ideal fractions (notes) to which real ratios cluster
    public double ClusterWidth { get; } // Percentage around cluster target where ratio is clustered
    public double SweepStepSize { get; } // The ratio increment size for the fundamental when sweeping
    public List<SweepData> OctaveSweepData { get; } = [];
    public OctaveSweep(HashSet<double> ratiosToSweep, List<Fraction> clusterTargets, double clusterWidth, double sweepStep = 0.01, bool sanityCheck = false)
    {
        RatiosToSweep = [.. ratiosToSweep.Order()];
        ClusterTargets = [.. clusterTargets.OrderBy(fraction => fraction.ToDouble())];
        ClusterWidth = clusterWidth;
        SweepStepSize = sweepStep;

        List<(double lower, double upper)> clusterRanges = CalculateClusterRanges(ClusterTargets, ClusterWidth);
        if (sanityCheck)
            SanityCheckClusterWidthToClusterTargets(clusterRanges);

        double fundamental = 1.0;
        while (fundamental < 2)
        {
            OctaveSweepData.Add(new SweepData(fundamental, clusterRanges, ClusterTargets, RatiosToSweep));
            fundamental += SweepStepSize;
        }
    }

    /// <summary>
    /// Throws an argument exception if cluster ranges overlap
    /// </summary>
    private void SanityCheckClusterWidthToClusterTargets(List<(double lower, double upper)> clusterRanges)
    {
        for (int i = 0; i < clusterRanges.Count - 1; i++)
            if (clusterRanges[i].upper >= clusterRanges[i + 1].lower)
                Console.WriteLine($"WARNING: Cluster Width causes Cluster Target cluster ranges to overlap at cluster target number {i}");
    }

    public static List<(double lower, double upper)> CalculateClusterRanges(List<Fraction> ClusterTargets, double ClusterWidth)
    {
        List<(double lower, double upper)> clusterRanges = [];
        foreach (var clusterTarget in ClusterTargets)
        {
            double clusterTargetAsDouble = clusterTarget.ToDouble();
            double upperClusterRange = clusterTargetAsDouble + clusterTargetAsDouble * ClusterWidth;
            double lowerClusterRange = clusterTargetAsDouble - clusterTargetAsDouble * ClusterWidth;
            clusterRanges.Add((lowerClusterRange, upperClusterRange));
        }

        return clusterRanges;
    }

    /// <summary>
    /// SweepData represents the result of an octave sweep step.
    /// </summary>
    public class SweepData
    {
        public double Fundamental { get; } // The fundamental used for the clustering
        // The ratios being swept octave renormalised to the current sweep fundamental.
        // Octave renormalisation refers to renormalising a fraction according to a new fundamental, and then octave transposing the result to the interval [1,2)       
        public List<(double lower, double upper)> ClusterRanges { get; }
        public List<double> OctaveRenormalisedRatios { get; } = [];
        public Dictionary<double, Fraction> ClusterTargetMatches { get; } = [];
        public List<Fraction> ClusterTargets { get; } = [];
        public List<double> RatiosToSweep { get; }
        public SweepData(double fundamental, List<(double lower, double upper)> clusterRanges, List<Fraction> clusterTargets, List<double> ratiosToSweep)
        {
            Fundamental = fundamental;
            ClusterRanges = clusterRanges;
            ClusterTargets = clusterTargets;
            RatiosToSweep = ratiosToSweep;

            OctaveRenormaliseRatios();
            ClusterOctaveRenormalisedRatios();
        }
        public void OctaveRenormaliseRatios()
        {
            foreach (var ratio in RatiosToSweep)
            {
                //Fundamental is > 1 so only need to normalise new ratio upwards
                double newRatio = ratio / Fundamental;
                if (newRatio < 1)
                    newRatio *= 2;
                OctaveRenormalisedRatios.Add(newRatio);
            }
        }
        private void ClusterOctaveRenormalisedRatios()
        {
            for (int i = 0; i < ClusterRanges.Count; i++)
            {
                (double lower, double upper) = ClusterRanges[i];
                foreach (var ratio in OctaveRenormalisedRatios)
                {
                    if (ratio > lower && ratio < upper)
                    {
                        ClusterTargetMatches[ratio] = ClusterTargets[i];
                        break; // input should be sanitized - no overlapping cluster ranges -> one match is all that is possible
                    }
                }
            }
        }
    }

    public List<string> GetConsoleOutput()
    {
        var consoleRows = new List<string>();
        StringBuilder header = new();
        // Write header        
        header.Append($"      ");
        foreach (Fraction target in ClusterTargets)
            header.Append($"{target,-5} ");
        header.Append($"{"LCM",-5}");
        header.Append($"{"Hits",-5}");
        header.AppendLine();
        consoleRows.Add(header.ToString());

        // Write data
        Dictionary<Fraction, double> previousRow = [];
        foreach (var sweepData in OctaveSweepData)
        {
            StringBuilder consoleRow = new();
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
            consoleRow.Append($"{fundamental,-4:F2}: ");
            foreach (Fraction target in ClusterTargets)
            {
                if (currentRow.ContainsKey(target))
                    consoleRow.Append($"{target.ToString(),-5:F1} ");
                else
                    consoleRow.Append("      ");
            }
            var lcm = LCM(sweepData.ClusterTargetMatches.Values.Select(fraction => (long)fraction.Denominator).ToArray());            
            consoleRow.Append($"{lcm,-5}");
            consoleRow.Append($"{sweepData.ClusterTargetMatches.Count(),-5}");            
            consoleRows.Add(consoleRow.ToString());

            previousRow = currentRow;
        }

        return consoleRows;
    }
}