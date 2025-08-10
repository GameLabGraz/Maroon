using UnityEngine;


namespace Maroon.Experiments.PlanetarySystem
{
    public class MenuController : MonoBehaviour
    {
        public GameObject uiCanvas;
        public GameObject back;

        void Start()
        {
            uiCanvas.SetActive(false);
            back.SetActive(false);
        }
    }
}
