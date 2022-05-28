using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonOfPos : MonoBehaviour
{
    public GameObject objectToMove;

    public Transform fatherTransform;

    public float x;
    public float y;
    public float z;

    public float rx;
    public float ry;
    public float rz;
    public float rw;

    public float sx;
    public float sy;
    public float sz;

    private Vector3 originalPosObject;
    private Vector3 originalScaleObject;
    private Quaternion originalRotationObject;

    private bool activeFather;

    // Start is called before the first frame update
    void Start()
    {
        originalPosObject = objectToMove.transform.position;
        originalRotationObject = objectToMove.transform.rotation;
        originalScaleObject = objectToMove.transform.localScale;

        activeFather = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (activeFather)
        {
            if(fatherTransform.hasChanged)
            {
                
                //objectToMove.transform.position = fatherTransform.position;
                //objectToMove.transform.rotation = fatherTransform.rotation;
                objectToMove.transform.localScale = fatherTransform.localScale;

                objectToMove.transform.position = fatherTransform.TransformPoint(x, y, z);
                objectToMove.transform.rotation = fatherTransform.rotation;

                /*
                Vector3 aux = objectToMove.transform.position;
                
                aux.x = aux.x + x;
                aux.y = aux.y + y;
                aux.z = aux.z + z;

                objectToMove.transform.position = aux;
                

                //objectToMove.transform.position = fatherTransform.TransformPoint(x, y, z);

                Quaternion aux2 = new Quaternion();
                aux2.x = rx;
                aux2.y = ry;
                aux2.z = rz;
                aux2.w = rw;

                objectToMove.transform.rotation = aux2;
                */

                //objectToMove.transform.position = fatherTransform.TransformPoint(x, y, z);

                Vector3 aux = objectToMove.transform.lossyScale;
                aux.x = aux.x * sx;
                aux.y = aux.y * sy;
                aux.z = aux.z * sz;

                objectToMove.transform.localScale = aux;
            }
        }
        else
        {
            objectToMove.transform.position = originalPosObject;
            objectToMove.transform.rotation = originalRotationObject;
            objectToMove.transform.localScale = originalScaleObject;
        }
    }

    public void boolButton ()
    {
        activeFather = !activeFather;
    }

    public void sendBack ()
    {
        activeFather = false;
    }
}
