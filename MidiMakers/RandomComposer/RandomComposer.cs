using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;
using Melodroid_2.LCMs;
using Serilog;

namespace Melodroid_2.MidiMakers.RandomComposer;

/// <summary>
/// Creates equally distributed note clusters
/// </summary>
public class RandomComposer
{
    public List<Note> Notes { get; internal set; } = [];
    public int TotalChords = 256;
    public int MidiFundamental = 60; // C4, middle C
    public int MaxNotesPerChord = 5; // Cannot exceed 12
    public int MinNotesPerChord = 2;
    public Random Random = new();

    public void Compose()
    {
        if (MaxNotesPerChord > 12)
            throw new ArgumentException($"{nameof(MaxNotesPerChord)} cannot exceed 12");
        int relativeTickLength = 1;

        for (int tick = 0; tick < TotalChords; tick++)
        {
            // get random chroma
            int notesInChord = Random.Next(MinNotesPerChord, MaxNotesPerChord + 1);
            HashSet<int> chromaNotes = [];
            while (chromaNotes.Count < notesInChord)
            {
                int chromaNote = Random.Next(0, 12);
                chromaNotes.Add(chromaNote);
            }
            int chordChroma = 0;
            foreach (int chromaNote in chromaNotes)
                chordChroma += 1 << chromaNote;

            Bit12Int chord = new(chordChroma);
            Notes.AddRange(ChromaToNotes(chord, MidiFundamental, tick, relativeTickLength));
        }

        // pad with zero note, wetdry truncates last tick for reasons
        Notes.Add(new Note((SevenBitNumber)0)
        {
            Time = TotalChords, // Set time to truncate with zero note
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
