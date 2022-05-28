using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class animationController : MonoBehaviour
{

    public TextMeshPro animationText;
    public bool active;
    // Start is called before the first frame update
    void Start()
    {
        if (active)
            animationText.text = "Animation On";
        else
            animationText.text = "Animation Off";
    }

    // Update is called once per frame
    public void updateAnimationText()
    {
        if (active)
        {
            active = false;
            animationText.text = "Animation Off";
        }
        else
        {
            active = true;
            animationText.text = "Animation On";
        }
    }
}
