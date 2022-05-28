using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class SoundAnimation : MonoBehaviour
{
    [SerializeField] public GameObject cube;

    private float initialX;
    public float deltaX;
    
    public MidiFilePlayer midiFilePlayer;

    void Start()
    {
        Vector3 position;
        position = cube.transform.position;
        initialX = position.x;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = cube.transform.position;
        if (midiFilePlayer.MPTK_TickCurrent > 0 && midiFilePlayer.MPTK_TickLast > 0)
        {
            position.x = initialX + ((float)midiFilePlayer.MPTK_TickCurrent / midiFilePlayer.MPTK_TickLast) * deltaX;
            cube.transform.position = position;
        }
    }
}
