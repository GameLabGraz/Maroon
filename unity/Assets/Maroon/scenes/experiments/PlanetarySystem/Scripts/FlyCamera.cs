using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public Camera flyCamera;
    public CameraController cameraController;
    public GameObject flyCameraFocus;
    public float speed = 50f;
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool cameraControlActive = false;

    void Start()
    {
        if (flyCamera == null)
        {
            Debug.LogError("FlyCamera: Camera not assigned.");
            return;
        }

        if (cameraController == null)
        {
            Debug.LogError("FlyCamera: CameraController not assigned.");
            return;
        }

        if (flyCameraFocus == null)
        {
            Debug.LogError("FlyCamera: Sun GameObject not assigned.");
            return;
        }

        ToggleCameraControl(cameraControlActive);
    }

    void Update()
    {
        if (flyCamera == null || cameraController == null || flyCameraFocus == null) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            cameraControlActive = !cameraControlActive;
            ToggleCameraControl(cameraControlActive);
            LookAtFokus();
        }

        if (!cameraControlActive) return;

        // Camera movement
        float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float y = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float z = 0f;

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Q))
        {
            z += speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.E))
        {
            z -= speed * Time.deltaTime;
        }

        flyCamera.transform.Translate(new Vector3(x, z, y));

        // Camera rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation += mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        flyCamera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        // Update camera FOV and angle from CameraController
        flyCamera.fieldOfView = cameraController.controlledCamera.fieldOfView;
    }

    private void ToggleCameraControl(bool active)
    {
        if (active)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /*
     * focus the camera on public object (sun after reset or toggle camera)
     */
    private void LookAtFokus()
    {
        if (cameraControlActive)
        {
            flyCamera.transform.LookAt(flyCameraFocus.transform);
            Vector3 eulerAngles = flyCamera.transform.localEulerAngles;
            xRotation = eulerAngles.x;
            yRotation = eulerAngles.y;
        }
    }
}
