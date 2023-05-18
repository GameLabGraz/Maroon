using UnityEngine;
using UnityEngine.UI;

public class HideUI : MonoBehaviour
{
    public GameObject AnimationUI;
    public GameObject SortingGamePlanetInfoUI;
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