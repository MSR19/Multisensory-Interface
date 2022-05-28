using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MidiPlayerTK;
public class SoundSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _sliderText;

    public MidiFilePlayer midiFilePlayer;
    void Start()
    {
        _slider.onValueChanged.AddListener((v) =>
        {
            
            if (v == 0) {
                _sliderText.text = "1/4";
                midiFilePlayer.MPTK_Speed = 0.25F;
            }
            else if (v == 1)
            {
                _sliderText.text = "1/2";
                midiFilePlayer.MPTK_Speed = 0.5F;
            }
            else if (v == 2)
            {
                _sliderText.text = "1";
                midiFilePlayer.MPTK_Speed = 1;
            }
            else if (v == 3)
            {
                _sliderText.text = "3/2";
                midiFilePlayer.MPTK_Speed = 1.5F;
            }
            else if (v == 4)
            {
                _sliderText.text = "2";
                midiFilePlayer.MPTK_Speed = 2;
            }
            
        });
    }

}
