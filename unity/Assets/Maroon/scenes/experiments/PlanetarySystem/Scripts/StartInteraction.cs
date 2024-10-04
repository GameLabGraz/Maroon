using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Experiments.PlanetarySystem
{
    public class StartInteraction : MonoBehaviour
    {
        /*
         * UnityEvent to starts SortingGame or Animation when the screen is clicked
         */
        public UnityEvent onMouseDownEvent;

        private void OnMouseDown()
        {
            onMouseDownEvent.Invoke();
        }
    }
}