using Melodroid_2.LCMs;
using Melodroid_2.MusicUtils;

namespace Melodroid_2.MidiMakers.TonalCoverComposer;

//TODO: Investigate composing using arbitrary tonal set factors, or only factors from good lcms
//  (e.g. lcm 8 is also 72@0 in 12 tet, but also a subset of 24@0 - itself a tonal cover of 8@0, 12@0)
/// <summary>
/// Composes based on tonal covers:
/// On-note events must belong to a Tonal Set (well sounding set of notes)
/// All sounding keys must belong to some tonal set
/// All tonal sets must share a common fundamental with a common lcm factor (not necessarily a completely shared lcm)
/// </summary>
public class TonalCoverComposer
{
    public List<TimeEvent> TimeEvents { get; set; }
    public int TotalTimeEvents { get; set; } = 4;

    public void Compose()
    {
        HashSet<int> SoundingNotes = new(); //Midi notes which are currently sounding

        List<int> allLcm8Chromas = Tet12ChromaMask.LCM8.GetSetBitCombinations();
        Tet12ChromaMask tet12ChromaMask = new(allLcm8Chromas.RandomElement());

        Dictionary<int, bool> keyOnOff = [];
        MidiOnOff midiOnOff = new(keyOnOff);
        TimeEvent firstTimeEvent = new(midiOnOff);

        // randomly choose a well sounding set - define space of all well sounding sets. lcm 8 subset?

        //TonalSet firstTonalSet = new TonalSet(); //select one of all possible tonal sets
        //firstTimeEvent.TonalCover = new([firstTonalSet]);
        for (int i = 1; i < TotalTimeEvents; i++)
        {

        }

        // 1st time event:
        // randomly select lcm
        // randomly select fundamental from e.g. C3 range
        // select voicing preferring lcm
        // create time event with absolute voiced keys

        // Subsequent time events:
        // Map sounding keys from previous time event to tonal cover
        // Randomly select lcm factor and fundamental from random subset of previous tonal cover 
        // Select new lcm containing lcm factor at fundamental
        // Select new voicing, possibly keeping old notes.

        // ideas: define tonal cover as tonal sets, link tonal sets to lcm factors
    }

    /// <summary>
    /// Tries to voice chroma mask as triads with fundamental as root
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="maskFundamental"></param>
    /// <returns></returns>
    public static Dictionary<int, bool> ChromaToTriadMidi(Tet12ChromaMask mask, int maskFundamental)
    {
        throw new NotImplementedException();
    }

    public static Bit12Int MidiToChroma(int midiKey)
    {
        int normalisedMidi = midiKey % 12;
        Bit12Int chroma = new Bit12Int(1) << normalisedMidi;
        return chroma; //midi 0 is C-1, midi 60 is middle C
    }
}

/// <summary>
/// TimeEvent to be converted to MIDI instructions
/// Contains information regarding midi key on/off
/// TODO: Perhaps meta data regarding harmonic structure?
/// </summary>
public class TimeEvent
{
    public MidiOnOff MidiOnOff { get; set; }
    public TimeEvent(MidiOnOff midiOnOff)
    {
        MidiOnOff = midiOnOff;
    }
    //public TonalCover TonalCover { get; set; }
}

public class MidiOnOff
{
    public Dictionary<int, bool> KeyOnOff { get; set; }
    public MidiOnOff(Dictionary<int, bool> keyOnOff)
    {
        KeyOnOff = keyOnOff;
    }

}



//public class TonalSet8 : TonalSet
//{
//    public override Bit12Int ChromaMask { get => new(0b100010010101); set => base.ChromaMask = value; }
//    public override Dictionary<int, List<int>> FullMatchLcmFactors
//    {
//        get => new()
//        {
//            [0] = { 2, 2, 2 },
//            [2] = { 3, 3 },
//            [4] = { 2, 5 },
//            [7] = { 2, 2, 3 },
//            [11] = { 3, 5 }
//        };
//        set => base.FullMatchLcmFactors = value;
//    }
//}

public class Voicing
{
    public int MidiFundamental { get; set; }
    public List<int> RelativeKeys { get; set; } // midi keys for voicing relative to MidiFundamental
    public int Lcm { get; } // Lcm for the voicing for fast lookup
    public Voicing(int midiFundamental, List<int> relativeKeys, int lcm)
    {
        MidiFundamental = midiFundamental;
        RelativeKeys = relativeKeys;
    }
}