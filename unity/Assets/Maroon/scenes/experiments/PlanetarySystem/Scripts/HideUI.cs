using UnityEngine;
using UnityEngine.UI;

public class HideUI : MonoBehaviour
{
    public GameObject AnimationUI;
    public GameObject PlanetInformationUI;
    public Toggle toggleHideUI;


    void Update()
    {
        HideUIOnKeyInput();
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
            if (!AnimationUI.activeSelf)
            {
                AnimationUI.SetActive(true);
                PlanetInformationUI.SetActive(true);
                toggleHideUI.isOn = false;
                //Debug.Log("CameraAndUIController: HideUIOnKeyInput(): [H] UI Active Self: " + AnimationUI.activeSelf);
            }
            else
            {
                AnimationUI.SetActive(false); //turn off UI
                PlanetInformationUI.SetActive(false); //turn off UI
                toggleHideUI.isOn = true;
                //Debug.Log("CameraAndUIController: HideUIOnKeyInput(): [H] UI Active Self: " + AnimationUI.activeSelf);
            }
        }
    }


    /*
     *  toggle HideUI with boolean from ToggleGroup function
     */
    public void ToggleHideUI(bool hide)
    {
        //Debug.Log("CameraAndUIController: ToggleHideUI(): " +  isHidden);
        if (hide)
        {
            AnimationUI.SetActive(false);
            PlanetInformationUI.SetActive(false);
            //Debug.Log("CameraAndUIController: ToggleHideUI(): " + ui.activeSelf);
        }
        else
        {
            AnimationUI.SetActive(true);
            PlanetInformationUI.SetActive(true);
            //Debug.Log("CameraAndUIController: ToggleHideUI(): " + ui.activeSelf);
        }
    }
    #endregion ToggleUI
}