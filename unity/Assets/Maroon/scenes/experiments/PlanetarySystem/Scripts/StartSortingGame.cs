using UnityEngine;

namespace Maroon.Experiments.PlanetarySystem
{
    public class StartSortingGame : MonoBehaviour
    {
        /*
         * starts SortingGame when the screen is clicked
         */
        private void OnMouseDown()
        {
            PlanetaryController.Instance.StartSortingGameOnInput();
        }
    }
}
