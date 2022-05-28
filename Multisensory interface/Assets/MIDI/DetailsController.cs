using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DetailsController : MonoBehaviour
{

    public TextMeshPro detailsText;
    public bool active;

    // Start is called before the first frame update
    void Start()
    {
        if (active)
            detailsText.text = "Haptic Details On";
        else
            detailsText.text = "Haptic Details Off";
    }

    // Update is called once per frame
    public void updateDeatilsText()
    {
        if (active)
        {
            active = false;
            detailsText.text = "Haptic Details Off";
        }
        else
        {
            active = true;
            detailsText.text = "Haptic Details On";
        }
    }
}