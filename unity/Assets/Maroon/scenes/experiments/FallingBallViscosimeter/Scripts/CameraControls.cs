using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made using Code from https://www.youtube.com/watch?v=sD0vNXQYY_U

public class CameraControls : MonoBehaviour
{
    private float rotationSpeed = 500f;
    private Vector3 mouseWorldPosStart;
    [SerializeField]private float zoomMin = 0.1f;
    private float zoomMax = 1f;
    public Camera zoomCamera;
    
    public float zoomSpeed = 1.1f;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            mouseWorldPosStart = zoomCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1))
        {
            Pan();
        }
        float mouseWheelDirection = Input.GetAxis("Mouse ScrollWheel");
        if (mouseWheelDirection > 0)
        {
            //zoom in
            Zoom(true);
        }

        if (mouseWheelDirection < 0)
        {
            //zoom out
            Zoom(false);
        }
    }


    private void Pan()
    {
        if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
        {
            Vector3 mouseWorldPosDiff = mouseWorldPosStart - zoomCamera.ScreenToWorldPoint(Input.mousePosition);
            transform.position += mouseWorldPosDiff;
        }
    }

    void Zoom(bool zoomIn)
    {
        mouseWorldPosStart = zoomCamera.ScreenToWorldPoint(Input.mousePosition);
        if (zoomIn)
        { 
            zoomCamera.orthographicSize = Mathf.Clamp(zoomCamera.orthographicSize / zoomSpeed, zoomMin, zoomMax);
        }
        else
        {
            zoomCamera.orthographicSize = Mathf.Clamp(zoomCamera.orthographicSize * zoomSpeed, zoomMin, zoomMax);
        }
          
        Vector3 mouseWorldPosDiff = mouseWorldPosStart - zoomCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position += mouseWorldPosDiff;
    }
    
}
