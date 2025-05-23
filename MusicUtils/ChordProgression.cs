using Fractions;
using static Melodroid_2.MusicUtils.Utils;
using static Melodroid_2.MusicUtils.MusicTheory;
using System.Text;
namespace Melodroid_2.MusicUtils;

/// <summary>
/// A chord progression shows if two sets of ratios can cluster to sets of fractions which share LCM factors for a given reference point
/// </summary>
public class ChordProgression
{
    public HashSet<double> OriginRatios { get; }
    public HashSet<double> TargetRatios { get; }
    public List<Fraction> ClusterTargets { get; } // The ideal fractions (notes) to which real ratios cluster
    public double ClusterWidth { get; } // Percentage around cluster target where ratio is clustered
    OctaveSweep _originSweep;
    OctaveSweep _targetSweep;
    public ChordProgression(
        HashSet<double> originRatios,
        HashSet<double> targetRatios,
        List<Fraction> clusterTargets,
        double clusterWidth = MaximumBinRadius)
    {
        OriginRatios = originRatios;
        TargetRatios = targetRatios;
        ClusterTargets = clusterTargets;
        ClusterWidth = clusterWidth;
        _originSweep = new(originRatios, ClusterTargets, ClusterWidth);
        _targetSweep = new(targetRatios, ClusterTargets, ClusterWidth);
    }

    public List<string> GetConsoleOutput(int minOriginFractionMatches = 1, int minTargetFractionMatches = 1)
    {
        var consoleRows = new List<string>();
        // Write input
        consoleRows.Add(string.Join(" ", _originSweep.RatiosToSweep));
        consoleRows.Add(string.Join(" ", _targetSweep.RatiosToSweep));

        StringBuilder header = new();
        // Write header        
        header.Append($"root  ");
        header.Append("Common LCM ");
        header.Append("Origin LCM ");
        header.Append("Target LCM ");
        header.Append("Origin/Target matches");
        consoleRows.Add(header.ToString());

        // Write data
        //Dictionary<Fraction, double> previousRow = [];
        for (int sweepIndex = 0; sweepIndex < _originSweep.OctaveSweepData.Count; sweepIndex++)
        {
            StringBuilder consoleRow = new();
            var originData = _originSweep.OctaveSweepData[sweepIndex];
            var targetData = _targetSweep.OctaveSweepData[sweepIndex];

            // Skip if no match on either data
            if (originData.ClusterTargetMatches.Keys.Count == 0 || targetData.ClusterTargetMatches.Keys.Count == 0)
                continue;

            // Skip if no sufficiently large matches
            if (originData.ClusterTargetMatches.Keys.Count < minOriginFractionMatches
                || targetData.ClusterTargetMatches.Keys.Count < minTargetFractionMatches)
                continue;

            int originLCM = (int)LCM(originData.ClusterTargetMatches.Values.Select(fraction => (long)fraction.Denominator).ToArray());
            int targetLCM = (int)LCM(targetData.ClusterTargetMatches.Values.Select(fraction => (long)fraction.Denominator).ToArray());
            var commonFactor = CommonFactors(Factorise(originLCM), Factorise(targetLCM));

            consoleRow.Append($"{originData.Fundamental,-6:F2}");
            consoleRow.Append($"{commonFactor,-11}");
            consoleRow.Append($"{originLCM,-11}");
            consoleRow.Append($"{targetLCM,-11}");
            consoleRow.Append($"({string.Join(" ", originData.ClusterTargetMatches.Values)}) ");
            consoleRow.Append($"({string.Join(" ", targetData.ClusterTargetMatches.Values)})");
            consoleRows.Add(consoleRow.ToString());
        }

        return consoleRows;
    }
}
