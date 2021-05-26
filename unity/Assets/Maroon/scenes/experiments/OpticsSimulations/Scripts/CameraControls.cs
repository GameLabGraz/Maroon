using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 mainCamStartPosition;
    private Quaternion mainCamStartRotation;
    private Camera mainCam;
    [SerializeField]
    public float scrollSpeed = 3.0f;
    private float currentOffset = 0.0f;
    private int mouseButton = 1;

    private float currentCamXpos;
    private float currentCamXoffset;

    private Vector3 camTopPosition = new Vector3(3, 2.5f, 2.5f);
    private Quaternion camTopRotation = Quaternion.Euler(90, 0, 0);
    public bool isTopPosition = false;
    private bool lastFrameTopPos = false;
    private float timeCount = 0.0f;

    [SerializeField]
    private float clampSliderLeft = -2f;
    [SerializeField]
    private float clampSliderRight = 2f;

    void Start()
    {
        mainCamStartPosition = Camera.main.transform.position;
        mainCamStartRotation = Camera.main.transform.rotation;
        currentCamXpos = Camera.main.transform.position.x;
        mainCam = Camera.main;
        currentCamXoffset = Camera.main.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(mouseButton))
        {
            currentOffset = Input.mousePosition.x;
            currentCamXoffset = mainCam.transform.position.x;
        }

        if (Input.GetMouseButton(mouseButton))
        {
            float camxpos = ((Input.mousePosition.x -currentOffset) / Screen.width) * scrollSpeed;
            currentCamXpos = -camxpos + currentCamXoffset;
        }

        //camera smooth transition between 2 points
        currentCamXpos = Mathf.Clamp(currentCamXpos, clampSliderLeft, clampSliderRight);

        Quaternion rotate_to = camTopRotation;
        Vector3 move_to = new Vector3(currentCamXpos, camTopPosition.y, camTopPosition.z);
        Vector3 move_bottom = new Vector3(currentCamXpos, mainCamStartPosition.y, mainCamStartPosition.z);
        if(isTopPosition == lastFrameTopPos)
        {
            timeCount += Time.deltaTime;
        }
        else
        {
            timeCount = 0.0f;
        }
        lastFrameTopPos = isTopPosition;

        if (!isTopPosition)
        {
            rotate_to = mainCamStartRotation;
            move_to = move_bottom;
        }
        timeCount = Mathf.Clamp(timeCount, 0f, 1f);
        //set cam rotation & position
        mainCam.transform.rotation = Quaternion.Slerp(transform.rotation, rotate_to, timeCount);
        mainCam.transform.position = Vector3.Slerp(transform.position, move_to, timeCount);
        

    }

    public void setTopView(bool viewtoset)
    {
        isTopPosition = viewtoset;
    }
}
