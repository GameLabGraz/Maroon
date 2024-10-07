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
                bool visible = !AnimationUI.activeSelf;
                SetUIActive(visible);
                toggleHideUI.isOn = !visible;
            }
        }


        /*
         *  toggle HideUI and SetUIActive with boolean from ToggleGroup function
         */
        public void SetUIActive(bool active)
        {
            AnimationUI.SetActive(!active);
            PlanetInformationUI.SetActive(!active);
            //Debug.Log("CameraAndUIController: SetUIActive(active): " + active);
        }
        #endregion ToggleUI
    }
}