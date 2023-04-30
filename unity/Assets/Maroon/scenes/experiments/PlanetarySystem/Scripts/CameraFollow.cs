//
//Author: Marcel Lohfeyer
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera followCamera;
    public Transform targetObject;
    private Vector3 offset;



    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - targetObject.transform.position;
        
    }


    void LateUpdate()
    {
        //transform.LookAt(targetPlanet);
        transform.position = targetObject.transform.position + offset;
    }
}
