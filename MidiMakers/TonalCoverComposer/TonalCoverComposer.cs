using Melodroid_2.LCMs;
using Melodroid_2.MusicUtils;
using Serilog;

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
    public List<TimeEvent> TimeEvents { get; set; } = [];
    public int TotalTimeEvents { get; set; } = 16;
    public static int InitialFundamental = 60; // C4, middle C

    public void Compose()
    {
        var allLcm8Chromas = Tet12ChromaMask.LCM8.GetSetBitCombinations(); // Get all tonal set subsets
        Tet12ChromaMask firstChromaMask = new(allLcm8Chromas.RandomElement()); // Take random subset        
        Dictionary<int, bool> keyOnOff = Tet12ChromaMask.ChromaToTriadMidi(firstChromaMask, InitialFundamental); // create voicing

        Log.Information($"--- TimeEvent {0} ---");
        Log.Information($"{nameof(firstChromaMask)}: {firstChromaMask.Mask.ToIntervalString()}");
        Log.Information($"Voicing: {string.Join(" ", keyOnOff.Keys)}");

        MidiOnOff firstMidiOnOff = new(keyOnOff); // select on keys for first event
        TonalSet firstTonalSet = new(firstChromaMask);
        TonalCover firstTonalCover = new(new([firstTonalSet]));
        TimeEvent firstTimeEvent = new(firstTonalCover, firstMidiOnOff); // create first time event
        TimeEvents.Add(firstTimeEvent);

        for (int i = 1; i < TotalTimeEvents; i++)
        {
            //TODO: fix multiple errors:
            //  - New tonal sets do not share factor at correct position
            //  - ? When selecting previous tonal set, same set may be chosen repeatedly
            //  - - perhaps it is important that a tonal set has on keys recently?

            Log.Information($"--- TimeEvent {i} ---");
            TimeEvent previousTimeEvent = TimeEvents.Last();
            // take random tonal set from previous time event
            TonalSet previousTonalSet = previousTimeEvent.TonalCover.TonalSets.RandomElement();
            Log.Information($"{nameof(previousTonalSet)}: {previousTonalSet.ChromaMask.Mask.ToIntervalString()}");

            // select random lcm factor from random fundamental from selected tonal set
            Dictionary<int, List<int>> maskLCMs = previousTonalSet.ChromaMask.GetAllMaskLCMs(); // lcms implicitly capped
            // only keep legal (non zero) lcms
            var keys = maskLCMs.Keys;
            foreach (var key in keys)
            {
                if (maskLCMs[key].All(lcm => lcm == 0))
                    maskLCMs.Remove(key);
            }
            int fundamentalShift = maskLCMs.Keys.ToList().RandomElement();
            int maskLCM = maskLCMs[fundamentalShift].RandomElement();
            int lcmFactor = Utils.Factorise(maskLCM).RandomElement();
            Log.Information($"{maskLCM}@{fundamentalShift}:{lcmFactor}");

            // select random new tonal set sharing factor at fundamental
            TonalSet tonalSetWithFactor = // gets sets with factor at root position
                TonalSet.GetTonalSetsWithFactor(lcmFactor).RandomElement();
            // shift mask to place root at fundamental shift - e.g. 4@7: 0b000010010001 (C Major 4@0) << 7 == 0b100010000100 (Gmajor 4@7)            
            TonalSet newTonalSet = new(tonalSetWithFactor.ChromaMask.Mask << fundamentalShift);
            Log.Information($"{nameof(newTonalSet)}: {newTonalSet.ChromaMask.Mask.ToIntervalString()}");

            // create new tonal cover from these two sets
            TonalCover newTonalCover = new([previousTonalSet, newTonalSet]);

            // adjust midi keys based on tonal cover difference - turn on new unique keys, turn of obsoleted keys, keep unvarying keys 

            // get chroma for keys to sound from new tonal cover
            Tet12ChromaMask newCoverMask = newTonalCover.GetChromaMask();
            // get chroma for old tonal cover
            Tet12ChromaMask oldCoverMask = previousTimeEvent.TonalCover.GetChromaMask();

            // calculate keys to keep, turn on, turn off
            Tet12ChromaMask keysInAnyMask = new(oldCoverMask.Mask | newCoverMask.Mask);
            var previousMidi = previousTimeEvent.MidiOnOff;
            MidiOnOff newMidiOnOff = new(previousMidi); // deep copy old keys, to turn off old on keys            

            // turn off keys only present in old mask
            Tet12ChromaMask keysOnlyInOldMask = new(keysInAnyMask.Mask ^ newCoverMask.Mask); // if keys in new cover mask, xor to false
            newMidiOnOff.TurnAllChromaKeysOff(keysOnlyInOldMask);

            //Remove duplicate off keys, makes midi creation easier
            foreach (var oldKeyPair in previousMidi.KeyOnOff)
                if (newMidiOnOff.KeyOnOff.ContainsKey(oldKeyPair.Key))
                    if (oldKeyPair.Value == false && newMidiOnOff.KeyOnOff[oldKeyPair.Key] == false)
                        newMidiOnOff.KeyOnOff.Remove(oldKeyPair.Key);

            // turn on new keys not present in old mask
            Tet12ChromaMask keysOnlyInNewMask = new(keysInAnyMask.Mask ^ oldCoverMask.Mask); // if keys in old cover mask, xor to false
            // all masks use initial fundamental as root key
            var newKeys = Tet12ChromaMask.ChromaToTriadMidi(keysOnlyInNewMask, InitialFundamental);
            foreach (var keyPair in newKeys)
                newMidiOnOff.KeyOnOff[keyPair.Key] = keyPair.Value;

            // Remove duplicate on keys
            foreach (var oldKeyPair in previousMidi.KeyOnOff)
                if (newMidiOnOff.KeyOnOff.ContainsKey(oldKeyPair.Key))
                    if (oldKeyPair.Value == true && newMidiOnOff.KeyOnOff[oldKeyPair.Key] == true)
                        newMidiOnOff.KeyOnOff.Remove(oldKeyPair.Key);

            Log.Information($"Voicing: {string.Join(" ", newMidiOnOff.KeyOnOff.Keys.Where(key => newMidiOnOff.KeyOnOff[key]))}");
            // create new time event
            TimeEvent timeEvent = new(newTonalCover, newMidiOnOff);
            TimeEvents.Add(timeEvent);
        }

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
    public TonalCover TonalCover { get; set; }

    // constructor for new series of time events
    public TimeEvent(TonalCover tonalCover, MidiOnOff midiOnOff)
    {
        // create midi voicing from tonal set
        MidiOnOff = midiOnOff;
        TonalCover = tonalCover;
    }
}

public class MidiOnOff
{
    public Dictionary<int, bool> KeyOnOff { get; set; }
    public MidiOnOff(Dictionary<int, bool> keyOnOff)
    {
        KeyOnOff = keyOnOff;
    }
    // Deep copy constructor
    public MidiOnOff(MidiOnOff midiOnOff)
    {
        Dictionary<int, bool> keyOnOff = [];
        foreach (var keyPair in midiOnOff.KeyOnOff)
        {
            keyOnOff[keyPair.Key] = keyPair.Value;
        }
        KeyOnOff = keyOnOff;
    }

    // Chroma mask must be in root position
    public void TurnAllChromaKeysOff(Tet12ChromaMask chromaMask)
    {
        SetAllChromaKeys(chromaMask, false);
    }

    private void SetAllChromaKeys(Tet12ChromaMask chromaMask, bool keyValue)
    {
        List<int> intervals = chromaMask.ChromaToIntervals();
        foreach (var key in KeyOnOff.Keys)
        {
            if (intervals.Contains(key % 12))
                KeyOnOff[key] = keyValue;
        }
    }
}