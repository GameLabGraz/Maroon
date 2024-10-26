using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Experiments.PlanetarySystem
{
    public class StartInteraction : MonoBehaviour
    {
        public UnityEvent onMouseDownEvent;
        private Material currentMaterial;
        public Color hoverColor;
        private Color originalColor;


        /// <summary>
        /// store current material
        /// </summary>
        private void Start()
        {
            currentMaterial = GetComponent<Renderer>().sharedMaterial;
            originalColor = currentMaterial.color;
        }


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
            currentMaterial.color = hoverColor;
        }


        /// <summary>
        /// restore the original color on mouse exit
        /// </summary>
        private void OnMouseExit()
        {
            currentMaterial.color = originalColor;
        }
    }
}