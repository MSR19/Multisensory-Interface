using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using TMPro;

public class Bottuons3D : MonoBehaviour
{
    public GameObject maxSlider;
    public GameObject minSlider;
    public GameObject speedSlider;
    
    public TextMeshPro textSpeedSlider;
    public TextMeshPro textMinSlider;
    public TextMeshPro textMaxSlider;

    public TextMeshPro textMusicName;

    public int minSongI;
    public int maxSongI;

    private float maxMusicPos;
    private float minMusicPos;

    private float deltaSlider;

    public MidiFilePlayer midiFilePlayer;

    // Start is called before the first frame update
    void Start()
    {
        maxMusicPos = 1;
        minMusicPos = 0;
        deltaSlider = (float)0.3;

        textMusicName.text = midiFilePlayer.MPTK_MidiName;
    }

    // Update is called once per frame
    void Update()
    {
        if(maxSlider.transform.hasChanged)
        {
            maxMusicPos = maxSlider.transform.localPosition.x + (float)0.5;
            textMaxSlider.text = string.Format("{0:N2}", maxMusicPos);
        }
        if(minSlider.transform.hasChanged)
        {
            minMusicPos = minSlider.transform.localPosition.x + (float)0.5;
            textMinSlider.text = string.Format("{0:N2}", minMusicPos);
        }
        if(speedSlider.transform.hasChanged)
        {
            if (speedSlider.transform.localPosition.x < (float)-3 + deltaSlider)
            {
                midiFilePlayer.MPTK_Speed = (float)0.25;
                textSpeedSlider.text = "0.25";
            }
            else if ((float)-1.5 - deltaSlider < speedSlider.transform.localPosition.x && speedSlider.transform.localPosition.x < (float)-1.5 + deltaSlider)
            {
                midiFilePlayer.MPTK_Speed = (float)0.50;
                textSpeedSlider.text = "0.5";
            }
                
            else if ((float)0 - deltaSlider < speedSlider.transform.localPosition.x && speedSlider.transform.localPosition.x < (float)0 + deltaSlider)
            {
                midiFilePlayer.MPTK_Speed = 1;
                textSpeedSlider.text = "1";
            }
                
            else if ((float)1.5 - deltaSlider < speedSlider.transform.localPosition.x && speedSlider.transform.localPosition.x < (float)1.5 + deltaSlider)
            {
                midiFilePlayer.MPTK_Speed = (float)1.5;
                textSpeedSlider.text = "1.5";
            }
                
            else if (speedSlider.transform.localPosition.x > (float)3-deltaSlider)
            {
                midiFilePlayer.MPTK_Speed = 2;
                textSpeedSlider.text = "2";
            }
                
        }

        if (midiFilePlayer.MPTK_TickCurrent > 0 && midiFilePlayer.MPTK_TickLast > 0)
        {
            if ((float)midiFilePlayer.MPTK_TickCurrent / midiFilePlayer.MPTK_TickLast > maxMusicPos)
                midiFilePlayer.MPTK_Stop();
        }
    }
    public void Play()
    {
        if (midiFilePlayer.MPTK_IsPaused)
        {
            midiFilePlayer.MPTK_UnPause();
        }
        else
        {
            midiFilePlayer.MPTK_Play();
            midiFilePlayer.MPTK_TickCurrent = (long)((float)midiFilePlayer.MPTK_TickLast * minMusicPos);
        }
    }
    public void Pause()
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
    public void Reset_Sound()
    {
        midiFilePlayer.MPTK_Stop();
        midiFilePlayer.MPTK_Play();
        midiFilePlayer.MPTK_TickCurrent = (long)((float)midiFilePlayer.MPTK_TickLast * minMusicPos);
    }

    public void Next_Song()
    {
        midiFilePlayer.MPTK_Stop();
        if (midiFilePlayer.MPTK_MidiIndex < maxSongI)
            midiFilePlayer.MPTK_MidiIndex++;
        else
            midiFilePlayer.MPTK_MidiIndex = minSongI;
        //midiFilePlayer.MPTK_Play();
        //midiFilePlayer.MPTK_TickCurrent = (long)((float)midiFilePlayer.MPTK_TickLast * minMusicPos);
        textMusicName.text = midiFilePlayer.MPTK_MidiName;
    }

    public void Last_Song()
    {
        midiFilePlayer.MPTK_Stop();
        if (midiFilePlayer.MPTK_MidiIndex != 0 && midiFilePlayer.MPTK_MidiIndex  > minSongI)
            midiFilePlayer.MPTK_MidiIndex--;
        else
            midiFilePlayer.MPTK_MidiIndex = maxSongI;
        //midiFilePlayer.MPTK_Play();
        //midiFilePlayer.MPTK_TickCurrent = (long)((float)midiFilePlayer.MPTK_TickLast * minMusicPos);
        textMusicName.text = midiFilePlayer.MPTK_MidiName;
    }
}
