using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
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
    // Note example, notes are defined in ticks start, duration and velocity
    //  new (NoteName.A, 4)
    //        {
    //            Time = totalMidiTicks, // at what ticks does note start
    //            Length = songTimeDivision, // for how long does note keep going
    //            Velocity = (SevenBitNumber)0}
    // based on https://melanchall.github.io/drywetmidi/index.html#getting-started
    public static void WriteNotesToMidi(
        List<Note> midiNotes,
        string folderPath,
        string fileName,
        short ticksPerQuarterNote = 1,
        double bpm = 60,
        bool overWrite = false)
    {
        MidiFile midiFile = new MidiFile();
        midiFile.TimeDivision = new TicksPerQuarterNoteTimeDivision(ticksPerQuarterNote);
        midiFile.ReplaceTempoMap(TempoMap.Create(Tempo.FromBeatsPerMinute(bpm)));

        TrackChunk trackChunk = new TrackChunk();
        using (var notesManager = trackChunk.ManageNotes())
        {
            var notes = notesManager.Objects;            
            //add notes                       
            foreach (var note in midiNotes)
            {
                notes.Add(note);
            }
        }

        midiFile.Chunks.Add(trackChunk);
        midiFile.Write(Path.Combine(folderPath, fileName + ".mid"), overWrite);
    }

    public static List<Note> TimeEventsToNotes(List<TimeEvent> timeEvents)
    {
        List<Note> notes = [];
        Dictionary<int, int> liveNotes = []; //note, tick start
        // track when midi values turn on and off, save h
        int currentTick = 0;
        foreach (var timeEvent in timeEvents)
        {
            foreach (var keyPair in timeEvent.MidiOnOff.KeyOnOff)
            {
                // note on, start recording note
                if (keyPair.Value == true)
                    liveNotes[keyPair.Key] = currentTick;

                // note off, create note
                if (keyPair.Value == false)
                    AddNote(keyPair.Key); // consumes note from liveNotes list

            }
            currentTick++;
        }

        return notes;

        void AddNote(int noteNumber)
        {
            Note note = new Note((SevenBitNumber)noteNumber)
            {
                Time = liveNotes[noteNumber], // at what ticks does note start
                Length = currentTick - liveNotes[noteNumber], // for how long does note keep going
                Velocity = (SevenBitNumber)64
            };
            notes.Add(note);
            liveNotes.Remove(noteNumber);
        }
    }
}
