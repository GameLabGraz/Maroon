using UnityEngine;

namespace Maroon.Physics.Viscosimeter
{
    [RequireComponent(typeof(MeasurableObject))]
    public class MeasurementBox : MonoBehaviour
    {

        private MeshRenderer renderer_;

        private void OnMouseEnter()
        {
            renderer_.material.color = Color.red;
        }

        private void OnMouseExit()
        {
            renderer_.material.color = Color.white;
        }

        private void Awake()
        {
            renderer_ = GetComponent<MeshRenderer>();
            renderer_.enabled = false;
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

    }
}