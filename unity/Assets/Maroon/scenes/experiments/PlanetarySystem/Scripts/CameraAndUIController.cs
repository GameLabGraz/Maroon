using UnityEngine;
using UnityEngine.UI;

public class CameraAndUIController : MonoBehaviour
{
    public GameObject AnimationUI;
    public GameObject SortingGamePlanetInfoUI;
    public Toggle toggleHideUI;


    //cameraLookAt
    //public Camera lookAtCamera;
    //public Transform lookAtTargetObject;
    //public Dropdown cameraLookAtDropdown;

    //cameraFollow
    //public Camera followCamera;
    //public Transform cameraFollowTargetObject;
    //private Vector3 cameraFollowOffset;
    //public Dropdown cameraFollowDropdown;

    //public List<GameObject> targetPlanet; // Create a list of GameObjects that will be the targets


    void Start()
    {
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
}