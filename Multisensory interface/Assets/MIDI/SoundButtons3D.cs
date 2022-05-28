using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))] //A collider is needed to receive clicks
public class SoundButtons3D : MonoBehaviour
{
    public Material materialAnimation;
    private Material materialOriginal;

    public float distanceTravel;
    public float deltaAnimation;
    private bool animationDown;
    private bool animationUp;

    private Vector3 origianlTranforamtion;
    private float minZ;
    private float maxZ;

    public UnityEvent interactEvent;
    public void OnMouseDown()
    {
        interactEvent.Invoke();
        if(animationDown == false && animationUp == false)
        {
            animationDown = true;
            GetComponent<MeshRenderer>().material = materialAnimation;
            return;
        }   
    }

    private void Start()
    {
        origianlTranforamtion = transform.localPosition;
        animationDown = false;
        animationUp = false;

        materialOriginal = GetComponent<MeshRenderer>().material;

        minZ = origianlTranforamtion.z - distanceTravel;
        maxZ = origianlTranforamtion.z;

    }

    private void Update()
    {
        if(animationUp)
        {
            if (transform.localPosition.z == maxZ)
            {
                GetComponent<MeshRenderer>().material = materialOriginal;
                animationUp = false;
                return;
            }
            Vector3 aux = transform.localPosition;
            if (minZ > maxZ)
                aux.z = transform.localPosition.z - deltaAnimation;
            if (minZ < maxZ)
                aux.z = transform.localPosition.z + deltaAnimation;
            transform.localPosition = aux;
        }
        if(animationDown)
        {
            if(minZ > maxZ)
                if (transform.localPosition.z >= minZ)
                {
                    animationDown = false;
                    animationUp = true;
                    return;
                }
                    
            if(minZ < maxZ)
                if (transform.localPosition.z <= minZ)
                {
                    animationDown = false;
                    animationUp = true;
                    return;
                }

            Vector3 aux = transform.localPosition;
            if (minZ > maxZ)
                aux.z = transform.localPosition.z + deltaAnimation;
            if(minZ < maxZ)
                aux.z = transform.localPosition.z - deltaAnimation;
            transform.localPosition = aux;
        }
    }

}
