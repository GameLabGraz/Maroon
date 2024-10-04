using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Experiments.PlanetarySystem
{
    public class StartInteraction : MonoBehaviour
    {
        public UnityEvent onMouseDownEvent;
        public Color hoverColor;

        // screen png
        private Material material;
        private Color originalColor;

        /*
         * Store the screen png
         */
        private void Start()
        {
            material = GetComponent<Renderer>().material;
            originalColor = material.color;
        }

        /*
         * UnityEvent to starts SortingGame or Animation when the screen is clicked
         */
        private void OnMouseDown()
        {
            onMouseDownEvent.Invoke();
        }

        /*
         * Update the material's color on hover
         */
        private void OnMouseEnter()
        {
            material.color = hoverColor;
        }

        /*
         * Restore the original color on mouse exit
         */
        private void OnMouseExit()
        {
            material.color = originalColor;
        }
    }
}