using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics.Viscosimeter
{
    public class SnapController : MonoBehaviour
    {
        public List<SnapPoint> snapPoints;
        public List<DragDrop> draggableObjects;
        public float snapRange = 0.5f;

        void Start()
        {
            foreach (DragDrop draggable in draggableObjects)
            {
                draggable.dragEndedSnapCallback = OnDragEnded;
            }
        }

        private void OnDragEnded(DragDrop draggable)
        {
            float closestDistance = -1;
            SnapPoint closestSnapPoint = null;

            foreach (SnapPoint snapPoint in snapPoints)
            {
                float currentDistance = Vector3.Distance(draggable.transform.position, snapPoint.transform.position);
                if (closestSnapPoint == null || currentDistance < closestDistance)
                {
                    closestSnapPoint = snapPoint;
                    closestDistance = currentDistance;
                }


            }

            if (closestSnapPoint != null && closestSnapPoint.currentObject == null && closestDistance <= snapRange && SnapIsAllowed(draggable, closestSnapPoint))
            {
                draggable.transform.position = closestSnapPoint.transform.position;
                draggable.snapPoint = closestSnapPoint;
                closestSnapPoint.currentObject = draggable;
            }
        }

        private bool SnapIsAllowed(DragDrop draggable, SnapPoint snapPoint)
        {
            if ((snapPoint.onlyPycnometerSnap && draggable.gameObject.GetComponent<Pycnometer>()) | !snapPoint.onlyPycnometerSnap)
            {
                return true;
            }

            return false;
        }

    }
}