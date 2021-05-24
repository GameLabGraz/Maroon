using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 mainCamStartPosition;
    private Camera mainCam;
    private Camera blurCam;
    public float movementRange;
    private float currentOffset = 0.0f;
    private float movementSpeed = 1.5f;
    private int mouseButton = 1;

    void Start()
    {
        mainCamStartPosition = Camera.main.transform.position;
        movementRange = 1.0f;
        mainCam = Camera.main;
        blurCam = Camera.main.transform.Find("BlurCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(mouseButton))
        {
            currentOffset = Input.mousePosition.x;
        }

        if (Input.GetMouseButton(mouseButton))
        {
            float camxpos = ((Input.mousePosition.x -currentOffset) / Screen.width) * movementRange*3.0f;
            
            //mainCam.transform.position = new Vector3(-camxpos + mainCam.transform.position.x, mainCam.transform.position.y, mainCam.transform.position.z);
            mainCam.transform.position = new Vector3(-camxpos + mainCamStartPosition.x,  mainCamStartPosition.y, mainCamStartPosition.z);
        }
        if (Input.GetMouseButtonUp(mouseButton))
        {
            mainCamStartPosition = mainCam.transform.position;
        }
    }




}
