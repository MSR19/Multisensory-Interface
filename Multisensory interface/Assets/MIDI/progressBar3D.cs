using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class progressBar3D : MonoBehaviour
{
    public float minX;
    public float maxX;
    private float deltapercentageX;
    public float minXsize;
    public float maxXsize;
    private float deltapercentageXsize;

    private float value;
    public bool Positive;

    public MidiFilePlayer midiFilePlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (!(minXsize > 0 && maxXsize > 0))
        {
            deltapercentageX = 1;
            deltapercentageXsize = 1;
            return;
        }

        deltapercentageX = (maxX - minX);
        deltapercentageXsize = (maxXsize - minXsize);
        value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (midiFilePlayer.MPTK_TickCurrent > 0 && midiFilePlayer.MPTK_TickLast > 0)
        {
            if (Positive)
                value = (float)midiFilePlayer.MPTK_TickCurrent / midiFilePlayer.MPTK_TickLast;
            else 
                value = 1 - (float)midiFilePlayer.MPTK_TickCurrent / midiFilePlayer.MPTK_TickLast; 
        }

        Vector3 aux;
        
        aux = transform.localPosition;
        aux.x = minX + deltapercentageX * value;
        transform.localPosition = aux;

        aux = transform.localScale;
        aux.x = minXsize + deltapercentageXsize * value;
        transform.localScale = aux;
    }
}
