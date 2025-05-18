using static Melodroid_2.MusicUtils.Utils;
namespace Melodroid_2.MusicUtils;

/// <summary>
/// A tonal coverage splits a set of ratios into (possibly redundantly overlapping) sets and compares their LCMs for matching primes.
/// Example: 
/// Ratios {1, 6/5, sqrt(2), 3/2} can be split into {1, 6/5, 3/2} with LCM 12 at (original) 6/5, and {5/3, 1, 6/5} with LCM 15 at (original) 6/5.
/// The resulting wave packets appear to stretch/contract while keeping most of the original frequencies
/// </summary>
public class TonalCoverage
{
    public List<double> OriginalRatios { get; set; }
    public List<List<double>> RatioPowerSet { get; set; }
    public TonalCoverage(List<double> originalRatios)
    {
        OriginalRatios = originalRatios;
        RatioPowerSet = GetPowerSet(originalRatios.ToArray());
    }
}
