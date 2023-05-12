using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Camera controlledCamera;
    [SerializeField] public Slider cameraFovSlider;
    [SerializeField] private Slider timeSpeedSlider;

    private float initialFieldOfView;

    //cameraLookAt
    //public Camera lookAtCamera;
    //public Transform lookAtTargetObject;
    //public Dropdown cameraLookAtDropdown;

    //cameraFollow
    //public Camera followCamera;
    //public Transform cameraFollowTargetObject;
    //private Vector3 cameraFollowOffset;
    //public Dropdown cameraFollowDropdown;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    //public List<GameObject> targetPlanet; // Create a list of GameObjects that will be the targets


    /*
     * stores initial camera 
     * setup slider
     */
    void Start()
    {
        StoreInitialCamera();

        SetupTimeSpeedSlider();
        SetupFOVSlider();

        /*
              if (cameraLookAtDropdown != null)
              {
                   cameraLookAtDropdown.ClearOptions();
                   List<string> targetNames = new List<string>();
                   foreach (GameObject target in targetPlanet)
                   {
                       targetNames.Add(target.name);
                   }
                   cameraLookAtDropdown.AddOptions(targetNames);
                   cameraLookAtDropdown.onValueChanged.AddListener(OnTargetDropdownValueChanged);
              }
          */
        //cameraFollow
        //cameraFollowOffset = transform.position - cameraFollowTargetObject.transform.position;
    }


    void Update()
    {
        //transform.LookAt(lookAtTargetObject);
    }


    void LateUpdate()
    {
        //transform.LookAt(cameraFollowTargetObject);
        //transform.position = cameraFollowTargetObject.transform.position + cameraFollowOffset;
    }


    /*
     *
     */
    private void OnTargetDropdownValueChanged(int index)
    {
        //GameObject target = targetPlanet[index];
        //controlledCamera.transform.LookAt(targetPlanet.transform);
    }

    /*
     * ControlledCamera store/reset
     */
    #region ControlledCamera
    /*
     * Store the camera's initial position, rotation, and field of view
     */
    void StoreInitialCamera()
    {
        if (controlledCamera == null)
        {
            Debug.Log("CameraController: StoreInitialCamera(): controlledCamera missing");
            controlledCamera = Camera.main;
        }

        initialPosition = controlledCamera.transform.position;
        initialRotation = controlledCamera.transform.rotation;
        initialFieldOfView = controlledCamera.fieldOfView;
    }

    /*
     *  reset the camera's position and field of view to their initial values
     */
    public void ResetCamera()
    {
        controlledCamera.transform.position = initialPosition;
        controlledCamera.transform.rotation = initialRotation;
        controlledCamera.fieldOfView        = initialFieldOfView;

        cameraFovSlider.value               = initialFieldOfView;
    }
    #endregion ControlledCamera


    /*
     * handles slider
     */
    #region slider
    /*
     * setup time/speed slider
     */
    void SetupTimeSpeedSlider()
    {
        if (timeSpeedSlider != null)
        {
            timeSpeedSlider.minValue = 0f;
            timeSpeedSlider.maxValue = 25f;
            Time.timeScale = timeSpeedSlider.value;

            timeSpeedSlider.onValueChanged.AddListener(OnTimeSliderValueChanged);
        }
    }


    /*
     * setup FOV slider
     */
    void SetupFOVSlider()
    {
        if (cameraFovSlider != null)
        {
            cameraFovSlider.minValue = 10;
            cameraFovSlider.maxValue = 200;
            cameraFovSlider.value = controlledCamera.fieldOfView;
            cameraFovSlider.onValueChanged.AddListener(OnFOVSliderValueChanged);
        }
    }


    /*
     * changes the time/speed value after slider input
     */
    void OnTimeSliderValueChanged(float value)
    {
        Time.timeScale = value;
    }


    /*
     * changes the FOV value after slider input
     */
    private void OnFOVSliderValueChanged(float fovValue)
    {
        controlledCamera.fieldOfView = fovValue;
    }

    /*
     * removes listeners OnDestroy
     */
    private void OnDestroy()
    {
        if (timeSpeedSlider != null)
            timeSpeedSlider.onValueChanged.RemoveListener(OnTimeSliderValueChanged);

        if (cameraFovSlider != null)
            cameraFovSlider.onValueChanged.RemoveListener(OnFOVSliderValueChanged);
    }
    #endregion slider
}