using UnityEngine;

namespace Maroon.Experiments.PlanetarySystem
{
    public class FlyCamera : MonoBehaviour
    {
        public Camera flyCamera;
        public GameObject flyCameraFocus;
        [SerializeField] private float mouseSpeed = 50f;
        [SerializeField] private float mouseSensitivity = 100f;

        private float xRotation = 0f;
        private float yRotation = 0f;
        private bool isCameraControlActive = false;
        private bool hasCameraBeenToggled = false;
        //---------------------------------------------------------------------------------------

        /// <summary>
        /// check camera control state
        /// </summary>
        private void Start()
        {
            if (flyCamera == null)
            {
                Debug.Log("FlyCamera: Start(): Camera not assigned.");
                return;
            }

            if (flyCameraFocus == null)
            {
                Debug.Log("FlyCamera: Start(): Camera focus not assigned.");
                return;
            }

            SetCameraControlActive(isCameraControlActive);
        }


        /// <summary>
        /// enables FlyCam after [TAB] key is pressed
        /// Helpi message after first [TAB]
        /// updating Camera
        /// </summary>
        private void Update()
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
                SetCameraControlActive(isCameraControlActive);
                LookAtFocus();
            }

            if (!isCameraControlActive)
                return;

            FlyCameraMovement();

            // UpdateSimulationCameraFov and angle from PlanetaryController
            flyCamera.fieldOfView = PlanetaryController.Instance.SimulationCamera.fieldOfView;
        }


        /// <summary>
        /// movement of the FlyCamera
        /// </summary>
        private void FlyCameraMovement()
        {
            float x = Input.GetAxis("Horizontal") * mouseSpeed * Time.unscaledDeltaTime;
            float y = Input.GetAxis("Vertical") * mouseSpeed * Time.unscaledDeltaTime;
            float z = 0f;

            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Q))
            {
                z += mouseSpeed * Time.unscaledDeltaTime;
            }
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.E))
            {
                z -= mouseSpeed * Time.unscaledDeltaTime;
            }

            flyCamera.transform.Translate(new Vector3(x, z, y));

            // Camera rotation
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.unscaledDeltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.unscaledDeltaTime;

            xRotation += mouseY;
            yRotation += mouseX;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            flyCamera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }


        /// <summary>
        /// locks the cursor when switched between normal mode and FlyCam mode
        /// </summary>
        /// <param name="active"></param>
        private void SetCameraControlActive(bool active)
        {
            Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !active;
        }


        /// <summary>
        /// focus the camera on public object (sun after reset or toggle camera)
        /// </summary>
        private void LookAtFocus()
        {
            if (isCameraControlActive)
            {
                flyCamera.transform.LookAt(flyCameraFocus.transform);
                Vector3 eulerAngles = flyCamera.transform.localEulerAngles;
                eulerAngles.x = 90;
                eulerAngles.y = 0;
                flyCamera.transform.rotation = Quaternion.Euler(eulerAngles);
                xRotation = eulerAngles.x;
                yRotation = eulerAngles.y;
            }
        }
    }
}