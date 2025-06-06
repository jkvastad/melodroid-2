using Melodroid_2.LCMs;
using System;

namespace Melodroid_2.MidiMakers;

public class TonalCoverComposer
{
    public List<TimeEvent> TimeEvents { get; set; }
    public int TotalTimeEvents { get; set; } = 4;

    public void Compose()
    {
        HashSet<int> SoundingNotes = new(); //Midi notes which are currently sounding
        TimeEvent firstTimeEvent = new();
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

    public static Bit12Int MidiToChroma(int midiKey)
    {
        int normalisedMidi = midiKey % 12;
        Bit12Int chroma = new Bit12Int(1) << normalisedMidi;
        return chroma; //midi 0 is C-1, midi 60 is middle C
    }
}

public class TimeEvent
{
    Dictionary<int, bool> KeyOnOff { get; set; }
    public TonalCover TonalCover { get; set; }
}
public class TonalCover
{
    List<TonalSet> TonalSets { get; set; }
    public TonalCover(List<TonalSet> tonalSets)
    {
        TonalSets = tonalSets;
    }
}

public class TonalSet
{
    public virtual Bit12Int ChromaMask { get; set; } // e.g. major triad is new(0b10010001)    
    public virtual Dictionary<int, List<int>> FullMatchLcmFactors { get; set; } = new();
    public TonalSet(Bit12Int chromaMask)
    {
        //TODO: map chroma mask to full match LCM factors
        // perhaps a dictionary taking chroma masks and outputing full match lcm factors?

    }

    public static Dictionary<int, List<int>> FullMatchFactorsLcm8 = new()
    {
        [0] = { 2, 2, 2 },
        [2] = { 3, 3 },
        [4] = { 2, 5 },
        [7] = { 2, 2, 3 },
        [11] = { 3, 5 }
    };
    public static Dictionary<int, List<int>> FullMatchFactorsLcm15 = new()
    {
        [0] = { 2, 2, 2 },
        [2] = { 3, 3 },
        [4] = { 2, 5 },
        [7] = { 2, 2, 3 },
        [11] = { 3, 5 }
    };
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