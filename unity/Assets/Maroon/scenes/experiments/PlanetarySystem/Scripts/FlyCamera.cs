using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public Camera flyCamera;
    public CameraAndUIController cameraAndUIController;
    public GameObject flyCameraFocus;
    public float speed = 50f;
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool isCameraControlActive = false;
    private bool hasCameraBeenToggled = false;

    /*
     * check camera control state
     */
    void Start()
    {
        if (flyCamera == null)
        {
            Debug.Log("FlyCamera: Start(): Camera not assigned.");
            return;
        }

        if (cameraAndUIController == null)
        {
            Debug.Log("FlyCamera: Start(): cameraAndUIController not assigned.");
            return;
        }

        if (flyCameraFocus == null)
        {
            Debug.Log("FlyCamera: Start(): Sun not assigned.");
            return;
        }

        ToggleCameraControl(isCameraControlActive);
    }


    /*
     * enables FlyCam after [TAB] key is pressed
     * Helpi message after first [TAB]
     * updating Camera
     */
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //send Helpi FlyCameraExplained message after first FlyCamera toggle
            if (!hasCameraBeenToggled)
            {
                //Debug.Log("FlyCamera: Update(): FlyCamera has been toggled for the first time");
                hasCameraBeenToggled = true;
                PlanetaryController.Instance.DisplayMessageByKey("FlyCameraExplained");
            }

            isCameraControlActive = !isCameraControlActive;
            ToggleCameraControl(isCameraControlActive);
            LookAtFokus();
        }

        if (!isCameraControlActive)
            return;

        FlyCamerMovement();

        // Update AnimationCameraFov and angle from PlanetaryController
        flyCamera.fieldOfView = PlanetaryController.Instance.AnimationCamera.fieldOfView;
    }


    /*
     * movement of the FlyCamera
     */
    void FlyCamerMovement()
    {
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
    }



    /*
     * locks the cursor when switched between normal mode and FlyCam mode
     */
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
        if (isCameraControlActive)
        {
            flyCamera.transform.LookAt(flyCameraFocus.transform);
            Vector3 eulerAngles = flyCamera.transform.localEulerAngles;
            xRotation = eulerAngles.x;
            yRotation = eulerAngles.y;
        }
    }
}
