using Fractions;
using static Melodroid_2.MusicUtils.Utils;
using static Melodroid_2.MusicUtils.MusicTheory;
using System.Collections.Immutable;
using System.Collections.Generic;
using static Melodroid_2.MusicUtils.OctaveSweep;
using System.Text;
namespace Melodroid_2.MusicUtils;

/// <summary>
/// A tonal coverage splits a set of ratios into (possibly redundantly overlapping) sets and compares their (post fraction-binned) LCMs for matching primes.
/// Example: 
/// Ratios {1, 6/5, sqrt(2), 3/2} can be split into {1, 6/5, 3/2} with LCM 12 at (original) 6/5, and {5/3, 1, 6/5} with LCM 15 at (original) 6/5.
/// The resulting wave packets appear to stretch/contract while keeping most of the original frequencies
/// A tonal coverage is thus defined by:
///     * An original set of ratios
///     * Two sets of fractions (binned from subsets of the ratios whose union is the original ratio set)
///     * A fundamental (use to octave renormalise the original ratios and produce the binned fractions)
///     * A pair of LCMs, one for each set of fractions
///     * The shared factors (e.g. 3 if the LCMs are 12 and 15)
/// </summary>
public class TonalCoverageCalculator
{
    public HashSet<double> OriginalRatios { get; set; }
    public List<HashSet<double>> RatioPowerSet { get; set; }
    public List<Fraction> ClusterTargets { get; set; } = GoodFractions;
    public double ClusterWidth;
    public double SweepStep = 0.001;
    public Dictionary<HashSet<HashSet<double>>, List<TonalCoverage>> TonalCoverages = new(new SetOfSetComparer<double>());
    public TonalCoverageCalculator(List<double> originalRatios, double clusterWidth = MaximumBinRadius)
    {
        ClusterWidth = clusterWidth;
        OriginalRatios = new(originalRatios);
        RatioPowerSet =
            GetPowerSet(originalRatios.ToArray()).Select(subset => new HashSet<double>(subset))
            .Where(set => set.Count > 0)
            .ToList();

        // 1. Check all pairs of power set subsets whose union is the original ratio set, without repetition
        // 2. For each such pair, compare their partial tonal coverages
        // 3. Per fundamental, create the pairs (ratioSet1, fractionSet1), (ratioSet2, fractionSet2), LCM, common factor (the tonal coverage)
        // 4. filter tonal coverages for interesting results

        // Map pairs of power set subsets to their tonal coverages        
        foreach (var subset1 in RatioPowerSet)
        {
            foreach (var subset2 in RatioPowerSet)
            {
                var union = subset1.Union(subset2);
                HashSet<HashSet<double>> subsets = [subset1, subset2];
                if (OriginalRatios.SetEquals(union) && !TonalCoverages.ContainsKey(subsets))
                {
                    var sweep1 = new OctaveSweep(subset1, ClusterTargets, ClusterWidth, SweepStep);
                    var sweep2 = new OctaveSweep(subset2, ClusterTargets, ClusterWidth, SweepStep);
                    List<TonalCoverage> tonalCoverages = [];
                    for (int sweepIndex = 0; sweepIndex < sweep1.OctaveSweepData.Count; sweepIndex++)
                    {
                        SweepData sweepData1 = sweep1.OctaveSweepData[sweepIndex];
                        SweepData sweepData2 = sweep2.OctaveSweepData[sweepIndex];
                        if (sweepData1.ClusterTargetMatches.Count > 0 && sweepData2.ClusterTargetMatches.Count > 0)
                        {
                            var partialCoverage1 = new PartialTonalCoverage(sweepData1);
                            var partialCoverage2 = new PartialTonalCoverage(sweepData2);
                            var tonalCoverage = new TonalCoverage(partialCoverage1, partialCoverage2);
                            tonalCoverages.Add(tonalCoverage);
                        }
                    }
                    TonalCoverages[subsets] = tonalCoverages;
                }
            }
        }
    }

    /// A tonal coverage consists of:
    ///     * An original set of ratios
    ///     * Two sets of fractions (binned from subsets of the ratios whose union is the original ratio set)
    ///     * A fundamental (use to octave renormalise the original ratios and produce the binned fractions)
    ///     * A pair of LCMs, one for each set of fractions
    ///     * The shared factors (e.g. 3 if the LCMs are 12 and 15)
    public class TonalCoverage
    {
        public List<double> OriginalRatios { get; }
        public double Fundamental { get; }
        public List<List<double>> RatioSubsets { get; }
        public List<List<Fraction>> FractionSubsets { get; }
        public List<int> FractionLCMs { get; }
        public int CommonLCMFactor { get; }
        public int StringDecimalPlaces { get; set; } = 2;

        public TonalCoverage(PartialTonalCoverage partial1, PartialTonalCoverage partial2)
        {
            OriginalRatios = [.. partial1.OriginalRatios.Union(partial2.OriginalRatios)];
            Fundamental = partial1.Fundamental;
            RatioSubsets = [partial1.OriginalRatios, partial2.OriginalRatios]; ;
            FractionSubsets = [[.. partial1.ClusterTargetMatches.Values], [.. partial2.ClusterTargetMatches.Values]];
            FractionLCMs = [partial1.Lcm, partial2.Lcm];

            List<int> primes = [];
            foreach (var prime1 in partial1.LcmPrimes)
                if (partial2.LcmPrimes.Contains(prime1))
                    primes.Add(prime1);
            if (primes.Count > 0)
                CommonLCMFactor = primes.Aggregate((a, b) => a * b);
            else
                CommonLCMFactor = 0;
        }

        public override string ToString()
        {

            var sb = new StringBuilder();
            string format = $"{{0,-3:F{StringDecimalPlaces}}}: ";
            sb.Append(string.Format(format, Fundamental));
            sb.Append($"{CommonLCMFactor} ");
            for (int i = 0; i < RatioSubsets.Count; i++)
            {
                sb.Append($"({string.Join(" ", FractionSubsets[i])}) ");
                sb.Append($"{string.Join(" ", FractionLCMs[i])} ");
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// One part of a tonal coverage
    /// </summary>
    public class PartialTonalCoverage
    {
        public List<double> OriginalRatios { get; }
        public double Fundamental { get; }
        public Dictionary<double, Fraction> ClusterTargetMatches { get; }
        public int Lcm { get; }
        public List<int> LcmPrimes { get; }

        public PartialTonalCoverage(OctaveSweep.SweepData sweepData)
        {
            OriginalRatios = sweepData.RatiosToSweep;
            Fundamental = sweepData.Fundamental;
            ClusterTargetMatches = sweepData.ClusterTargetMatches;
            Lcm = (int)LCM(ClusterTargetMatches.Values.Select(fraction => (long)fraction.Denominator).ToArray());
            LcmPrimes = new List<int>(Factorise(Lcm));
        }
    }
    /// <summary>
    /// Gets printable version of tonal coverages
    /// </summary>
    /// <param name="fractionMatchMinSize"> The minimum size for a fraction match, e.g. set it to 3 for only comparing triad or larger matches</param>
    /// <returns></returns>
    public List<string> GetConsoleOutput(
        int fractionMatchMinSize = 1,
        int fundamentalDecimalsDisplayed = 2,
        int ratioSetDecimalsDisplayed = 2,
        double printThreshold = 0.11)
    {
        List<string> consoleRows = [];
        string format = "F" + ratioSetDecimalsDisplayed;

        foreach (var setPair in TonalCoverages.Keys)
        {
            List<string> rowBatch = [];
            var powerSetSubsets = new StringBuilder();
            foreach (var set in setPair)
            {
                powerSetSubsets.Append($"({string.Join(" ", set.Select(n => n.ToString(format)))})");
            }
            rowBatch.Add(powerSetSubsets.ToString());

            string previousLine = "";
            double previousFundamental = 0;
            foreach (var tonalCoverage in TonalCoverages[setPair])
            {
                // Only print matches of sufficient size
                if (tonalCoverage.FractionSubsets.Any(subset => subset.Count < fractionMatchMinSize))
                    continue;

                // Only print distinct subsets
                HashSet<HashSet<Fraction>> uniqueFractionSets = new(new SetComparer<Fraction>());
                foreach (var fractionSet in tonalCoverage.FractionSubsets)
                    uniqueFractionSets.Add(new(fractionSet));
                if (uniqueFractionSets.Count != tonalCoverage.FractionSubsets.Count)
                    continue;

                // Only print unique lines
                tonalCoverage.StringDecimalPlaces = fundamentalDecimalsDisplayed;
                if (tonalCoverage.ToString() != previousLine)
                {
                    //Only print if fundamental diff is above threshold from last printed line
                    if (Math.Abs(tonalCoverage.Fundamental - previousFundamental) < printThreshold)
                    {
                        continue;
                    }

                    // Only print lcm of reasonable size
                    if (tonalCoverage.FractionLCMs.Any(lcm => lcm > 15))
                        continue;

                    rowBatch.Add($"{tonalCoverage}");
                    previousFundamental = tonalCoverage.Fundamental;
                    previousLine = tonalCoverage.ToString();
                }
            }
            // Only print sets with interesting matches
            if (previousLine != "")
            {
                rowBatch.Add("");
                consoleRows.AddRange(rowBatch);
            }
        }
        return consoleRows;
    }
}