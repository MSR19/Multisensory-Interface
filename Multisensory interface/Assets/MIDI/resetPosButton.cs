using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetPosButton : MonoBehaviour
{

    [SerializeField]
    public List<GameObject> objectsToBeReset;

    private List<Vector3> objectsInitialTransformPostion;
    private List<Quaternion> objectsInitialTransformRotation;

    // Start is called before the first frame update
    void Start()
    {

        objectsInitialTransformPostion = new List<Vector3>();
        objectsInitialTransformRotation = new List<Quaternion>();

        for (int i = 0; i != objectsToBeReset.Count; i++)
        {
            objectsInitialTransformPostion.Add(objectsToBeReset[i].transform.position);
            objectsInitialTransformRotation.Add(objectsToBeReset[i].transform.rotation);
            print("AGORA: " + objectsToBeReset[i].transform.position);
        }
    }
    public void ResetButton()
    {
        //print("OLA");
        print(objectsInitialTransformPostion.Count);
        for(int i = 0; i != objectsInitialTransformPostion.Count; i++)
        {
            //print("ANTES: " + objectsToBeReset[i].transform.position);
            objectsToBeReset[i].transform.position = objectsInitialTransformPostion[i];
            objectsToBeReset[i].transform.rotation = objectsInitialTransformRotation[i];
            //print("DEPOIS: " + objectsToBeReset[i].transform.position);
        }
    }
}
