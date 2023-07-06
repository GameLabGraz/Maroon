using UnityEngine;


namespace Maroon.Experiments.PlanetarySystem
{
    public class StartAnimation : MonoBehaviour
    {
        /*
         * starts Animation when the screens are clicked
         */
        private void OnMouseDown()
        {
            PlanetaryController.Instance.StartAnimationOnInput();
        }
    }
}