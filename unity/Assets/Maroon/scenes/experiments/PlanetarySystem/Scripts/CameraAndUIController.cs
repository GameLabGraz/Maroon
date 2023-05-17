using UnityEngine;
using UnityEngine.UI;

public class CameraAndUIController : MonoBehaviour
{
    public Camera controlledCamera;
    [SerializeField] public Slider cameraFovSlider;
    [SerializeField] private Slider timeSpeedSlider;

    [SerializeField] private Slider gSlider;
    [SerializeField] private float G;

    public GameObject AnimationUI;
    public GameObject SortingGamePlanetInfoUI;
    public Toggle toggleHideUI;

    [SerializeField] private float initialFieldOfView;

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

    //---------------------------------------------------------------------------------------
    /*
     * Instance of CameraAndUIController
     */
    #region CameraAndUIControllerIntance
    private static CameraAndUIController _instance;
    public static CameraAndUIController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<CameraAndUIController>();
            return _instance;
        }
    }
    #endregion CameraAndUIControllerInstance



    /*
     * stores initial camera 
     * setup slider
     */
    void Start()
    {
        StoreInitialCamera();

        SetupGSlider();
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
        HideUIOnKeyInput();

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
     * hide UI on key or toggle input 
     */
    #region ToggleUI
    /*
     *  hide UI on Key input and update toggle
     */
    void HideUIOnKeyInput()
    {
        if (Input.GetKeyUp(KeyCode.H))
        {
            if (!AnimationUI.activeSelf)// && (!SortingGamePlanetInformationUI.activeSelf))
            {
                AnimationUI.SetActive(true); //turn on UI
                SortingGamePlanetInfoUI.SetActive(true); //turn on UI
                toggleHideUI.isOn = false;
                //Debug.Log("CameraAndUIController: HideUIOnKeyInput(): [H] UI Active Self: " + AnimationUI.activeSelf);
            }
            else
            {
                AnimationUI.SetActive(false); //turn off UI
                SortingGamePlanetInfoUI.SetActive(false); //turn off UI
                toggleHideUI.isOn = true;
                //Debug.Log("CameraAndUIController: HideUIOnKeyInput(): [H] UI Active Self: " + AnimationUI.activeSelf);
            }
        }
    }


    /*
     *  change with boolean from ToggleGroup function
     */
    public void ToggleHideUI(bool hide)
    {
        //Debug.Log("CameraAndUIController: ToggleHideUI(): " +  isHidden);
        if (hide)
        {
            AnimationUI.SetActive(false); //turn on UI
            SortingGamePlanetInfoUI.SetActive(false); //turn on UI
            //Debug.Log("CameraAndUIController: ToggleHideUI(): " + ui.activeSelf);
        }
        else
        {
            AnimationUI.SetActive(true); //turn off UI
            SortingGamePlanetInfoUI.SetActive(true); //turn off UI
            //Debug.Log("CameraAndUIController: ToggleHideUI(): " + ui.activeSelf);
        }
    }
    #endregion ToggleUI


    /*
     * ControlledCamera store/reset
     */
    #region ControlledCamera
    /*
     * store the camera's initial position, rotation, and field of view
     */
    void StoreInitialCamera()
    {
        if (controlledCamera == null)
        {
            Debug.Log("CameraAndUIController: StoreInitialCamera(): controlledCamera missing");
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
     * setup gravitational constant G slider
     */
    void SetupGSlider()
    {
        if (gSlider != null)
        {
            gSlider.value = G;
            PlanetaryController.Instance.G = G;
            gSlider.onValueChanged.AddListener(OnGValueChanged);
        }
    }


    /*
     * setup time/speed slider
     */
    void SetupTimeSpeedSlider()
    {
        if (timeSpeedSlider != null)
        {
            //timeSpeedSlider.minValue = 0f;
            //timeSpeedSlider.maxValue = 50f;
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
     * changes the G value after slider input
     */
    private void OnGValueChanged(float gValue)
    {
        PlanetaryController.Instance.G = gValue;
    }


    /*
     * changes the time/speed value after slider input
     */
    void OnTimeSliderValueChanged(float timeSpeedValue)
    {
        Time.timeScale = timeSpeedValue;
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
        if (gSlider != null)
            gSlider.onValueChanged.RemoveListener(OnGValueChanged);

        if (timeSpeedSlider != null)
            timeSpeedSlider.onValueChanged.RemoveListener(OnTimeSliderValueChanged);

        if (cameraFovSlider != null)
            cameraFovSlider.onValueChanged.RemoveListener(OnFOVSliderValueChanged);
    }
    #endregion slider
}