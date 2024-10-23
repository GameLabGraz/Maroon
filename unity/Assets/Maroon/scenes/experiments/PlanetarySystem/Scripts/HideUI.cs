using UnityEngine;
using UnityEngine.UI;


namespace Maroon.Experiments.PlanetarySystem
{
    public class HideUI : MonoBehaviour
    {
        public GameObject UI;
        public GameObject PlanetInformationUI;
        public Toggle toggleHideUI;


        private void Update()
        {
            HideUIOnKeyInput();
        }


        /// <summary>
        /// hide UI on Key input and update toggle
        /// </summary>
        private void HideUIOnKeyInput()
        {
            if (Input.GetKeyUp(KeyCode.H))
            {
                bool visible = !UI.activeSelf;
                SetUIActive(visible);
                toggleHideUI.isOn = !visible;
            }
        }


        /// <summary>
        /// toggle HideUI and SetUIActive with boolean from ToggleGroup function
        /// </summary>
        /// <param name="active"></param>
        public void SetUIActive(bool hide)
        {
            UI.SetActive(!hide);
            PlanetInformationUI.SetActive(!hide);
            //Debug.Log("HideUI: SetUIActive(active): " + hide);
        }
    }
}