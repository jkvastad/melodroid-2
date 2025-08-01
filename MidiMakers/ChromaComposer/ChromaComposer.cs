﻿using Melanchall.DryWetMidi.Common;
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
    public int TotalChords = 256;
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
    /// Voices notes to avoid explicit semitones
    /// </summary>
    /// <param name="rawNotes"></param>
    /// <returns></returns>
    static int tick = 0;
    public static List<Note> NotesToVoicing(List<Note> rawNotes)
    {
        List<Note> voicedNotes = [];
        var sortedNotes = rawNotes.OrderBy(note => note.NoteNumber).ToList();
        int previousNoteNumber = 0;
        List<int> noteNumbers = [];
        int meanNoteNumber = sortedNotes.Sum(note => note.NoteNumber) / sortedNotes.Count;
        foreach (var note in sortedNotes)
        {
            int noteNumber = note.NoteNumber;
            if (Math.Abs(previousNoteNumber - noteNumber) < 2)
            {
                if (noteNumber < meanNoteNumber)
                    noteNumbers[^1] += 12; // increase lower part of semitone
                else
                    noteNumber -= 12; // decrease higher part of semitone
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
}