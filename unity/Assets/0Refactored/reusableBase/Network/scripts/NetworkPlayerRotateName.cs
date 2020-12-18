using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerRotateName : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Camera activeCamera = Camera.main;
        if (activeCamera)
            transform.LookAt(transform.position + activeCamera.transform.rotation * Vector3.forward, activeCamera.transform.rotation * Vector3.up);
    }
}
