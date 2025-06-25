using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class DraggableObject : MonoBehaviour
    {
        public UnityEngine.Events.UnityEvent<DraggableObject> OnDraggedOutOfBounds;
        public UnityEngine.Events.UnityEvent<DraggableObject> OnMoved;

        [Tooltip("If not set, the script searches for an attached rigidbody on Awake")]
        [SerializeField] private Rigidbody _rigidbody = null;

        private Vector3 _objectPositionAtDragStart;
        private Vector3 _objectToMousePosOffsetAtDragStart;
        private bool _rigidbodyWasKinematicAtDragStart;

        public bool draggableEnabled = true;

        private void Awake()
        {
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
        }

        private bool dragActive = false;
        private void OnMouseDown()
        {
            if (!draggableEnabled) return;
            if (SelectionSystem.IsMouseOverVisibleUIElement()) return;
            dragActive = true;

            if (_rigidbody != null)
            {
                _rigidbodyWasKinematicAtDragStart = _rigidbody.isKinematic;
                _rigidbody.isKinematic = true;
            }

            _objectPositionAtDragStart = transform.position;
            _objectToMousePosOffsetAtDragStart = 
                _objectPositionAtDragStart - 
                CameraController.GetMousePointOnPlaneParallelToCamera(_objectPositionAtDragStart);
        }

        private void OnMouseDrag()
        {
            if (!draggableEnabled || !dragActive) return;

            // Calculate and set new position based on Mouse-Pos
            transform.position = 
                CameraController.GetMousePointOnPlaneParallelToCamera(_objectPositionAtDragStart) + 
                _objectToMousePosOffsetAtDragStart;

            // Note(MartinR): Just setting transform.position causes problems when Physics interpolation is enabled.
            //      _rigidBody.MovePosition also does not seem to do the trick, I guess because it is expected to be called during FixedUpdate?
            //      Setting rigidBody.position seems to work in all cases
            if (_rigidbody != null)
            {
                _rigidbody.position = transform.position;
            }

            OnMoved.Invoke(this);
        }

        private void OnMouseUp()
        {
            if (!draggableEnabled || !dragActive) return;
            dragActive = false;

            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = _rigidbodyWasKinematicAtDragStart;
            }

            if (!SimulationBox.Instance.Bounds.Contains(transform.position))
            {
                OnDraggedOutOfBounds.Invoke(this);
            }
        }
    }
}
