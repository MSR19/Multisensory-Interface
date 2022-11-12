using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MidiPlayerTK;                                 // uses MIDI Pro Toolkit

namespace DemoMVP
{
    /// <summary>
    /// Computer Keyboard Button (CKB) Controller
    /// 
    /// This is intended to be the "Hello, World!" equivalent of the MIDI Pro Toolkit
    /// (MPTK).
    /// 
    /// This code assumes that the Scene contains:
    ///  * A MidiStreamPlayer added via the Inspector.
    /// 
    /// This code initializes MPTK engine and creates the objects necessary to play a note when the spacebar is
    /// pressed, stopping the note when the spacebar is released.
    /// </summary>
    public class TheSimplestMidiGenerator : MonoBehaviour
    {
        // This class is able to play MIDI event: play note, play chord, patch change, apply effect, ... see doc!
        // https://mptkapi.paxstellar.com/d9/d1e/class_midi_player_t_k_1_1_midi_stream_player.html
        public MidiStreamPlayer midiStreamPlayer;   // Initialized at Start() or could be set in the Inspector

        // description of MIDI event
        // https://mptkapi.paxstellar.com/d9/d50/class_midi_player_t_k_1_1_m_p_t_k_event.html
        private MPTKEvent mptkEvent;

        // Start is called before the first frame update
        void Start()
        {
            midiStreamPlayer = FindObjectOfType<MidiStreamPlayer>();
            if (midiStreamPlayer == null)
                Debug.LogWarning("Can't find a MidiStreamPlayer Prefab in the current scene hierarchy. You can add it with the MPTK menu in Unity editor.");
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Assign our "Hello, World!"-equivalent note (using MPTKEvent's defaults plus Value = 60 for C5.
                // HelperNoteLabel class could be your friend)
                mptkEvent = new MPTKEvent() { Value = 60 };

                // Start playing our "Hello, World!"-equivalent note
                midiStreamPlayer.MPTK_PlayEvent(mptkEvent);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                // Stop playing our "Hello, World!"-equivalent note
                midiStreamPlayer.MPTK_StopEvent(mptkEvent);
            }
        }
    }
}