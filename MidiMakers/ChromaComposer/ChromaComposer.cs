using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;
using Melodroid_2.LCMs;
using Melodroid_2.MidiMakers.TonalCoverComposer;
using Melodroid_2.MusicUtils;
using Serilog;
using System;
namespace Melodroid_2.MidiMakers.ChromaComposer;

public class ChromaComposer
{
    public List<Note> Notes { get; internal set; } = [];
    public int TotalChords = 64;
    public int MidiFundamental = 60; // C4, middle C
    public Random Random = new();

    public void Compose()
    {
        int currentTick = 0;
        int relativeTickLength = 1;
        Log.Information($"--- {nameof(currentTick)}: {currentTick} ---");

        int maxMaskLcm = 12; // maxLcm used when finding chord lcms

        var allLcm8Masks = Tet12ChromaMask.LCM8.GetSetBitCombinations();
        Bit12Int previousChord = allLcm8Masks.RandomElement();
        Notes.AddRange(ChromaToNotes(previousChord, MidiFundamental, currentTick, relativeTickLength));
        Log.Information($"{nameof(previousChord)}: {previousChord.ToIntervalString()}");

        for (int tick = 1; tick < TotalChords; tick++)
        {
            currentTick = tick;
            Log.Information($"--- {nameof(currentTick)}: {currentTick} ---");
            // Select non-empty fundamental offset among possible chord lcms
            Dictionary<int, List<int>> previousChordLcms = Tet12ChromaMask.GetAllMaskLCMs(previousChord, maxMaskLcm);
            int fundamentalOffset = previousChordLcms.Keys.Where(key => previousChordLcms[key].Count > 0).ToList().RandomElement();
            Log.Information($"{nameof(fundamentalOffset)}: {fundamentalOffset}");

            // Select lcm for offset
            int previousChordLcm = previousChordLcms[fundamentalOffset].RandomElement();
            Log.Information($"{nameof(previousChordLcm)}: {previousChordLcm}");

            // Select factor from previous chord lcm
            var lcmPrimes = Utils.Factorise(previousChordLcm);
            int lcmFactor = lcmPrimes
                .OrderBy(x => Random.Next()) // sort randomly
                .Take(Random.Next(1, lcmPrimes.Count + 1)) // take random number of primes - at least one, at most all
                .Aggregate(1, (acc, val) => acc * val); // create lcm factor
            Log.Information($"{nameof(lcmFactor)}: {lcmFactor}");

            // Get all tonal sets with masks sharing lcm factor at fundamental with previous mask
            List<TonalSet> candidateSets = TonalSet.GetTonalSetsWithFactor(lcmFactor, minSubSetSize: 2, maxLCM: maxMaskLcm); // masks in root position            
            TonalSet candidateSet = candidateSets.RandomElement();
            // Shift mask to offset position - e.g. 8@0 0b10010001 << 2 0b1001000100 is 8@2
            Tet12ChromaMask candidateMask = candidateSet.ChromaMask;
            var nextChord = candidateMask.Mask << fundamentalOffset;
            Log.Information($"{nameof(nextChord)}: {Tet12ChromaMask.GetMaskRootLCMs(candidateMask).Order().First()}@{fundamentalOffset} {nextChord.ToIntervalString()}");

            // Use new root position mask to create notes
            Notes.AddRange(ChromaToNotes(nextChord, MidiFundamental, currentTick, relativeTickLength));
            previousChord = nextChord;
        }

        // pad with zero note, wetdry truncates last tick for reasons
        Notes.Add(new Note((SevenBitNumber)0)
        {
            Time = currentTick + 1, // Set time to truncate with zero note
            Length = 1, // for how long does note keep going
            Velocity = (SevenBitNumber)0
        });
    }

    public static List<Note> ChromaToNotes(
        Bit12Int chroma,
        int fundamental,
        int noteTickStart,
        int noteTickLength,
        int velocity = 64)
    {
        List<Note> notes = [];
        List<SevenBitNumber> noteNumbers = [];
        HashSet<int> intervals = chroma.ToIntervals();

        foreach (int interval in intervals)
            noteNumbers.Add((SevenBitNumber)(fundamental + interval));

        foreach (var noteNumber in noteNumbers)
        {
            Note note = new Note(noteNumber)
            {
                Time = noteTickStart, // at what ticks does note start
                Length = noteTickLength, // for how long does note keep going
                Velocity = (SevenBitNumber)velocity
            };
            notes.Add(note);
        }
        return NotesToVoicing(notes);
    }

    /// <summary>
    /// Sorts notes by midi note number, then upshifts any notes closer than threshold by an octave
    /// </summary>
    /// <param name="rawNotes"></param>
    /// <returns></returns>
    static int tick = 0;
    public static List<Note> NotesToVoicing(List<Note> rawNotes, int smallestInterval = 2)
    {
        List<Note> voicedNotes = [];
        var sortedNotes = rawNotes.OrderBy(note => note.NoteNumber).ToList();
        int previousNoteNumber = 0;
        List<int> noteNumbers = [];
        List<int> naiveNoteNumbers = sortedNotes.Select(note => (int)note.NoteNumber).ToList();
        foreach (var note in sortedNotes)
        {
            int noteNumber = note.NoteNumber;
            if (Math.Abs(previousNoteNumber - noteNumber) < smallestInterval)
            {
                // decide up or down
                int noteUp = noteNumber + 12;
                int noteDown = noteNumber - 12;

                List<int> noteNumbersUp = [.. noteNumbers, noteUp];
                List<int> noteNumbersDown = [.. noteNumbers, noteDown];
                if (CountTriads(noteNumbersUp) > CountTriads(noteNumbersDown))
                    noteNumber = noteUp;
                else
                {
                    // tie breaker, create smallest chord spread relative naive numbers
                    // Sometimes one has to look at the upcoming note rather than the proceeding one to create a better voicing
                    // Perhaps it is about creating the smallest note spread without having explicit semitones
                    
                    noteNumbersUp = [.. naiveNoteNumbers, noteUp];
                    noteNumbersDown = [.. naiveNoteNumbers, noteDown];
                    int upDiff = noteNumbersUp.Max() - noteNumbersUp.Min();
                    int downDiff = noteNumbersDown.Max() - noteNumbersDown.Min();                    
                    if (upDiff < downDiff)
                        noteNumber = noteUp;
                    else
                        noteNumber = noteDown;
                }
            }
            else
                previousNoteNumber = note.NoteNumber;
            noteNumbers.Add(noteNumber);
        }

        for (int i = 0; i < sortedNotes.Count; i++)
        {
            Note note = sortedNotes[i];
            Note voicedNote = new((SevenBitNumber)noteNumbers[i])
            {
                Time = note.Time, // at what ticks does note start
                Length = note.Length, // for how long does note keep going
                Velocity = note.Velocity
            };
            voicedNotes.Add(voicedNote);
        }

        tick++;
        return voicedNotes;
    }

    public static int CountTriads(List<int> notes)
    {
        int triads = 0;
        var sortedNotes = notes.Order();
        int previousNoteNumber = sortedNotes.First();
        foreach (var noteNumber in sortedNotes)
        {
            int diff = Math.Abs(previousNoteNumber - noteNumber);
            if (diff == 3 || diff == 4)
                triads++;
            previousNoteNumber = noteNumber;
        }
        return triads;
    }
}