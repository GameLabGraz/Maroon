using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Experiments.PlanetarySystem
{
    public class StartInteraction : MonoBehaviour
    {
        public UnityEvent onMouseDownEvent;
        private Material material;
        public Color hoverColor;
        private Color originalColor;


        /// <summary>
        /// UnityEvent to starts SortingGame or Simulation when the screen is clicked
        /// </summary>
        private void OnMouseDown()
        {
            onMouseDownEvent.Invoke();
        }


        /// <summary>
        /// update the material's color on hover
        /// </summary>
        private void OnMouseEnter()
        {
            material = GetComponent<Renderer>().material;
            originalColor = material.color;
            material.color = hoverColor;
        }


        /// <summary>
        /// restore the original color on mouse exit
        /// </summary>
        private void OnMouseExit()
        {
            material.color = originalColor;
        }
    }
}