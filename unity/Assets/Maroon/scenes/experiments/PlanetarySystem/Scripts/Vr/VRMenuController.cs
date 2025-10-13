using UnityEngine;


namespace Maroon.Experiments.PlanetarySystem
{
    public class MenuController : MonoBehaviour
    {
        public GameObject uiCanvas;
        public GameObject back;
        public GameObject SliderG;
        public GameObject SliderTimeSpeed;
        public GameObject Timeto1;
        public GameObject ResetButton;
        public GameObject OrientationButton;
        public GameObject RotationButton;
        public GameObject TrajectoriesButton;
        public GameObject ClearTrajectoriesButton;
        public GameObject Language;

        void Start()
        {
            uiCanvas.SetActive(false);
            back.SetActive(false);
            SliderG.SetActive(false);
            SliderTimeSpeed.SetActive(false);
            Timeto1.SetActive(false);
            ResetButton.SetActive(false);
            OrientationButton.SetActive(false);
            RotationButton.SetActive(false);
            TrajectoriesButton.SetActive(false);
            ClearTrajectoriesButton.SetActive(false);
            Language.SetActive(false);
        }
    }
}
