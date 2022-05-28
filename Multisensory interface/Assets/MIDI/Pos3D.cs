using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pos3D : MonoBehaviour

{

    private Vector3 mOffset;
    private Vector3 positionMin;
    private Vector3 positionMax;
    public float maxX;
    public float minX;

    private float value;

    private float mZCoord;

    private void Start()
    {
        Vector3 position = gameObject.transform.localPosition;
        position.x = maxX;
        positionMax = gameObject.transform.parent.TransformPoint(position);
        position.x = minX;
        positionMin = gameObject.transform.parent.TransformPoint(position);
    }
    

    void OnMouseDown()

    {

        mZCoord = Camera.main.WorldToScreenPoint(

            gameObject.transform.position).z;



        // Store offset = gameobject world pos - mouse world pos

        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();

    }



    private Vector3 GetMouseAsWorldPoint()

    {

        // Pixel coordinates of mouse (x,y)

        Vector3 mousePoint = Input.mousePosition;



        // z coordinate of game object on screen

        mousePoint.z = mZCoord;



        // Convert it to world points

        return Camera.main.ScreenToWorldPoint(mousePoint);

    }



    public void OnMouseDrag()

    {
        Vector3 aux;
        aux = transform.position;
        aux.x = Mathf.Clamp(GetMouseAsWorldPoint().x, positionMin.x, positionMax.x);      
        transform.position = aux;

        Vector3 aux2 = gameObject.transform.parent.InverseTransformPoint(aux);

        value = aux2.x + (float)0.5;

        print(value); 
    }

}


