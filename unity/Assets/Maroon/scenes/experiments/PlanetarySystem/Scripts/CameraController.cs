using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Camera controlledCamera;
    public Slider cameraFovSlider;

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
     * stores initial camera position
     * setup FOV slider
     */
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
            cameraFovSlider.value = controlledCamera.fieldOfView;
            cameraFovSlider.onValueChanged.AddListener(OnFOVSliderValueChanged);
        }

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

   /*
    *
    */
    void Update()
    {
        //transform.LookAt(lookAtTargetObject);
    }


   /*
    *
    */
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
     * changes the FOV value
     */
    private void OnFOVSliderValueChanged(float fovValue)
    {
        controlledCamera.fieldOfView = fovValue;
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


    /*
     * removes listeners OnDestroy
     */
    private void OnDestroy()
    {
        if (cameraFovSlider != null)
        {
            cameraFovSlider.onValueChanged.RemoveListener(OnFOVSliderValueChanged);
        }
    }
}