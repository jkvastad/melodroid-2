namespace Melodroid_2.MusicUtils;

/// <summary>
/// Performs a key sweep, where all rotations of a given set of keys is matched against sets of lcms to check for inclusion.
/// </summary>
public class TET12KeySweeper
{
    public static readonly Bit12Int LCM2 = new(0b10000001);
    public static readonly Bit12Int LCM3 = new(0b1000100001);
    public static readonly Bit12Int LCM4 = new(0b10010001);

    public static Dictionary<int, Bit12Int> LcmsOfInterest = new()
        {
            {2,LCM2},
            {3,LCM3},
            {4,LCM4},
        };

    /// <summary>
    /// Performs a key sweep, mapping full matches of the provided keys to their respective rotations
    /// </summary>
    /// <param name="keys"></param>
    /// <returns>Dictionary where key is right rotations of the key set (increasing fundamental), value is a list of ints representing the lcm values fully matched </returns>
    public static Dictionary<int, List<int>> KeySweep(Bit12Int keys, Dictionary<int, Bit12Int>? lcmsToMatch = null)
    {
        lcmsToMatch ??= LcmsOfInterest;
        Dictionary<int, List<int>> fullMatchesPerRotation = [];
        for (int rotation = 0; rotation < 12; rotation++)
        {
            Bit12Int rotatedKeys = keys >> rotation;
            // Check if rotated keys are a subset of any lcm, if so note it as full match
            foreach (int lcm in lcmsToMatch.Keys)
            {
                if ((rotatedKeys & lcmsToMatch[lcm]) == rotatedKeys)
                {
                    if (!fullMatchesPerRotation.ContainsKey(rotation))
                        fullMatchesPerRotation[rotation] = [];
                    fullMatchesPerRotation[rotation].Add(lcm);
                }
            }
        }

        return fullMatchesPerRotation;
    }
}
