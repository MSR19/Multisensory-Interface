using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class PointerGrab : MonoBehaviour
{
    public VRTK_Pointer[] pointers = new VRTK_Pointer[0];
    // Start is called before the first frame update
    void Start()
    {
        foreach (VRTK_Pointer pointer in pointers)
        {
            pointer.enabled = false;
            pointer.pointerRenderer.enabled = false;
            pointer.interactWithObjects = true;
            pointer.pointerRenderer.enabled = true;
            pointer.enabled = true;
        }
    }

    // Update is called once per frame
}
