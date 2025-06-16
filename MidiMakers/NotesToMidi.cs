using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
//using Melanchall.DryWetMidi.MusicTheory;
using Melodroid_2.MidiMakers.TonalCoverComposer;

namespace Melodroid_2.MidiMakers;

/// <summary>
/// Class for turning DryWetMidi.Notes to a Midi File
/// A midifile usually defines time by multiples of a "tick".
///     A ticks length in time is set by two factors:
///         - ticks per quarter note (a.k.a. TimeDivision in MidiFile, a.k.a. Pulses Per Quarter note or PPQ)
///         - - ticks mainly affect how notes are created, as their lengths and starting points are defined in ticks
///         - Microseconds Per Quarter Note (MPQN set by tempoevent when creating a TrackChunk which is added to the MidiFile)
///         The tick length in seconds is then MPQN/(PPQ*1 000 000)
///         BPM 120 with a quarter note resolution of 4 in 4/4 time gives tick length (500 000/4*1 000 000) = 0.125s
/// <param name="ticksPerQuarterNote">The smallest logical time division of the music, defined per quarter note by MIDI convention</param>
/// </summary>
public class NotesToMidi
{
    public static Note SilentNote { get; private set; } = new Note((SevenBitNumber)0)
    {
        Time = 0, // Set time to truncate with zero note
        Length = 1, // for how long does note keep going
        Velocity = (SevenBitNumber)0
    };

    // Note example, notes are defined in ticks start, duration and velocity
    //  new (NoteName.A, 4)
    //        {
    //            Time = totalMidiTicks, // at what ticks does note start
    //            Length = songTimeDivision, // for how long does note keep going
    //            Velocity = (SevenBitNumber)0}
    // based on https://melanchall.github.io/drywetmidi/index.html#getting-started
    public static void WriteNotesToMidi(
        List<Note> notes,
        string folderPath,
        string fileName,
        short ticksPerQuarterNote = 1,
        double bpm = 60,
        bool overWrite = false)
    {
        //TODO tempo map does not affect writing midi, seems like it must be handled when creating the notes.
        MidiFile midiFile = new MidiFile();
        midiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(ticksPerQuarterNote);

        TrackChunk trackChunk = new TrackChunk();
        using (var notesManager = trackChunk.ManageNotes())
        {
            var managerNotes = notesManager.Objects;
            //add notes                       
            foreach (var note in notes)
            {
                managerNotes.Add(note);
            }            
        }

        midiFile.Chunks.Add(trackChunk);
        midiFile.Write(Path.Combine(folderPath, fileName + ".mid"), overWrite);
    }

    public static List<Note> TimeEventsToNotes(List<TimeEvent> timeEvents)
    {
        int timeScaling = 1;
        // TODO: handle bpm by adjusting note length 
        List<Note> notes = [];
        Dictionary<int, int> liveNotes = []; //note, tick start
        // track when midi values turn on and off, save h
        int currentTick = 0;
        foreach (var timeEvent in timeEvents)
        {
            foreach (var keyPair in timeEvent.MidiOnOff.KeyOnOff)
            {
                // note on, start recording note.
                if (keyPair.Value == true)
                {
                    // If already active, add note and start new
                    if (liveNotes.ContainsKey(keyPair.Key))
                        AddNote(keyPair.Key);
                    liveNotes[keyPair.Key] = currentTick;
                }

                // note off, create note
                if (keyPair.Value == false)
                    AddNote(keyPair.Key); // consumes note from liveNotes list

            }
            currentTick++;
        }
        List<int> liveNoteNumbers = liveNotes.Keys.ToList();
        foreach (var noteNumber in liveNoteNumbers)
            AddNote(noteNumber);               

        return notes;

        void AddNote(int noteNumber)
        {
            Note note = new Note((SevenBitNumber)noteNumber)
            {
                Time = timeScaling * liveNotes[noteNumber], // at what ticks does note start
                Length = timeScaling * (currentTick - liveNotes[noteNumber]), // for how long does note keep going
                Velocity = (SevenBitNumber)64
            };
            notes.Add(note);
            liveNotes.Remove(noteNumber);
        }
    }
}
