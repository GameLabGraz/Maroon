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

        void Start()
        {
            uiCanvas.SetActive(false);
            back.SetActive(false);
            SliderG.SetActive(false);
            SliderTimeSpeed.SetActive(false);
            Timeto1.SetActive(false);
        }
    }
}
