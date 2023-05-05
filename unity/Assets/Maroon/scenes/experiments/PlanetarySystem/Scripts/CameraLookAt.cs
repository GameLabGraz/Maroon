using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    public Camera lookAtCamera;
    public Transform targetObject;


    /*
     *
     */
    void Start()
    {
        
    }


    /*
     *
     */
    void Update()
    {
        transform.LookAt(targetObject);
    }
}