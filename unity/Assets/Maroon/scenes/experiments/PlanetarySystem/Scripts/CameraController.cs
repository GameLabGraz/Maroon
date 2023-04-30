using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Camera controlledCamera;
    public Slider cameraFovSlider;
    public Slider camereAngleSlider;

    private float initialFieldOfView;

    public Dropdown cameraLookAtDropdown;
    public Dropdown cameraFollowDropdown;

    public float distance = 10f;
    public float height = 10f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    //public List<GameObject> targetPlanet; // Create a list of GameObjects that will be the targets

    void Start()
    {
        if (controlledCamera == null)
        {
            Debug.Log("CameraController(): controlledCamera missing");
            controlledCamera = Camera.main;
        }

        // Store the camera's initial position, rotation, and field of view
        initialPosition     = controlledCamera.transform.position;
        initialRotation     = controlledCamera.transform.rotation;
        initialFieldOfView  = controlledCamera.fieldOfView;


        if (cameraFovSlider != null)
        {
            cameraFovSlider.minValue = 10;
            cameraFovSlider.maxValue = 200;
            cameraFovSlider.value    = controlledCamera.fieldOfView;
            cameraFovSlider.onValueChanged.AddListener(OnFOVSliderValueChanged);
        }

        if (camereAngleSlider != null)
        {
            camereAngleSlider.onValueChanged.AddListener(OnCameraAngleSliderValueChanged);
        }

        if (cameraLookAtDropdown != null)
        {
            /* cameraLookAtDropdown.ClearOptions();
             List<string> targetNames = new List<string>();
             foreach (GameObject target in targetPlanet)
             {
                 targetNames.Add(target.name);
             }
             cameraLookAtDropdown.AddOptions(targetNames);
             cameraLookAtDropdown.onValueChanged.AddListener(OnTargetDropdownValueChanged);
            */
        }
    }

    private void OnTargetDropdownValueChanged(int index)
    {
        //GameObject target = targetPlanet[index];
        //controlledCamera.transform.LookAt(targetPlanet.transform);
    }

    private void OnCameraAngleSliderValueChanged(float angleValue)
    {
        /* float angle = angleValue * 90f;

         Vector3 position = new Vector3(
             Mathf.Cos(angle * Mathf.Deg2Rad) * distance,
             height,
             Mathf.Sin(angle * Mathf.Deg2Rad) * distance
         );

         Quaternion rotation = Quaternion.LookRotation(
             Vector3.zero - position,
             Vector3.up
         );

         transform.position = position;
         transform.rotation = rotation;

         */
    }

    private void OnFOVSliderValueChanged(float fovValue)
    {
        controlledCamera.fieldOfView = fovValue;
    }

    public void ResetCamera()
    {
        // reset the camera's position, rotation, and field of view to their initial values
        controlledCamera.transform.position = initialPosition;
        controlledCamera.transform.rotation = initialRotation;
        controlledCamera.fieldOfView        = initialFieldOfView;

        // reset slider
        cameraFovSlider.value               = initialFieldOfView;
        //camereAngleSlider.value             = 9;
    }
}