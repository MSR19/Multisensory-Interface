using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class SoundButtons : MonoBehaviour
{
    public MidiFilePlayer midiFilePlayer;

   
   public void Play ()
    {
        midiFilePlayer.MPTK_Play();
    }
    public void Pause ()
    {
        if (midiFilePlayer.MPTK_IsPaused)
            midiFilePlayer.MPTK_UnPause();
        else
            midiFilePlayer.MPTK_Pause();
    }
    public void Stop_Sound()
    {
        midiFilePlayer.MPTK_Stop();
    }
}
