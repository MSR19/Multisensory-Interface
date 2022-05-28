using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MidiPlayerTK;
public class SoundPosSlider : MonoBehaviour
{
    [SerializeField] private Slider _sliderPos;
    [SerializeField] private TextMeshProUGUI _sliderTextPos;

    [SerializeField] private Slider _sliderMinPos;
    [SerializeField] private TextMeshProUGUI _sliderTextMinPos;

    [SerializeField] private Slider _sliderMaxPos;
    [SerializeField] private TextMeshProUGUI _sliderTextMaxPos;

    public MidiFilePlayer midiFilePlayer;

    private float minPos;
    private float maxPos;
    void Start()
    {
        // de 0 a 1
        minPos = _sliderMinPos.value;
        maxPos = _sliderMaxPos.value;

        _sliderMaxPos.onValueChanged.AddListener((v) =>
        {
            maxPos = v;
            _sliderTextMaxPos.text = v.ToString("0.00");
        });

        _sliderMinPos.onValueChanged.AddListener((v) =>
        {
            minPos = v;
            _sliderTextMinPos.text = v.ToString("0.00");
        });
    }

    private void Update()
    {
        if (midiFilePlayer.MPTK_TickCurrent > 0 && midiFilePlayer.MPTK_TickLast > 0)
        {
            _sliderPos.value = (float)midiFilePlayer.MPTK_TickCurrent / midiFilePlayer.MPTK_TickLast;
            _sliderTextPos.text = ((float)midiFilePlayer.MPTK_TickCurrent / midiFilePlayer.MPTK_TickLast).ToString("0.00");

            if (maxPos > minPos)
            {
                if (_sliderPos.value > maxPos)
                {
                    midiFilePlayer.MPTK_Stop();
                }
            }
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
            midiFilePlayer.MPTK_TickCurrent = (long)((float)midiFilePlayer.MPTK_TickLast * minPos);
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
        midiFilePlayer.MPTK_TickCurrent = (long)((float)midiFilePlayer.MPTK_TickLast * minPos);
    }
}
