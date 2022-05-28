using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosLimiter3D : MonoBehaviour
{
    public float minX;
    public float maxX;
    
    public float minY;
    public float maxY;

    public float minZ;
    public float maxZ;

    private Quaternion rotate;

    // Start is called before the first frame update
    private void Start()
    {
        rotate = transform.localRotation;
    }
    // Update is called once per frame
    public void correctTragectory()
    {
            Vector3 aux = transform.localPosition;
            if (aux.x > maxX)
                aux.x = maxX;
            else if (aux.x < minX)
                aux.x = minX;
            if (aux.y > maxY)
                aux.y = maxY;
            else if (aux.y < minY)
                aux.y = minY;
            if (aux.z > maxZ)
                aux.z = maxZ;
            else if (aux.z < minZ)
                aux.z = minZ;

            transform.localPosition = aux;
            transform.localRotation = rotate;
    }
}
