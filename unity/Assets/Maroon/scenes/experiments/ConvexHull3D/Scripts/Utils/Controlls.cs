using UnityEngine;
using UnityEngine.EventSystems;

namespace Maroon.ComputerScience.ConvexHull3D
{
    public class SimpleCameraController : MonoBehaviour
    {
        [Header("Rotation Settings")]
        public float rotationSpeed = 3f;

        [Header("Target")]
        public Transform target;
        public Transform secondaryTarget;

        private Vector3 lastMousePosition;
        private bool isRotating = false;


        void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                if(!EventSystem.current.IsPointerOverGameObject())
                {
                    lastMousePosition = Input.mousePosition;
                    isRotating = true;
                }
            }

            if(Input.GetMouseButtonUp(0))
            {
                isRotating = false;
            }

            if(isRotating)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                lastMousePosition = Input.mousePosition;
                
                target.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
                target.Rotate(transform.right, delta.y * rotationSpeed, Space.World);

                if(secondaryTarget != null)
                {
                    secondaryTarget.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
                    secondaryTarget.Rotate(transform.right, delta.y * rotationSpeed, Space.World);
                }
            }
        }
    }
}