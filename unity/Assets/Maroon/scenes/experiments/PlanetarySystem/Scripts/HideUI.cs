using UnityEngine;
using UnityEngine.UI;


namespace Maroon.Experiments.PlanetarySystem
{
    public class HideUI : MonoBehaviour
    {
        public GameObject AnimationUI;
        public GameObject PlanetInformationUI;
        public Toggle toggleHideUI;


        private void Update()
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
        private void HideUIOnKeyInput()
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
            AnimationUI.SetActive(!hide);
            PlanetInformationUI.SetActive(!hide);
            //Debug.Log("CameraAndUIController: ToggleHideUI(): " + ui.activeSelf);
        }
        #endregion ToggleUI
    }
}