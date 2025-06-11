using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class DraggableObject : MonoBehaviour
    {
        public UnityEngine.Events.UnityEvent<DraggableObject> OnDraggedOutOfBounds;

        [Tooltip("If not set, the script searches for an attached rigidbody on Awake")]
        [SerializeField] private Rigidbody _rigidbody = null;

        private bool _dragIsActive = false;
        private Vector3 _objectPositionAtDragStart;
        private Vector3 _objectToMousePosOffsetAtDragStart;
        private bool _rigidbodyWasKinematicAtDragStart;

        private void Awake()
        {
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnMouseDown()
        {
            if (_rigidbody != null)
            {
                _rigidbodyWasKinematicAtDragStart = _rigidbody.isKinematic;
                _rigidbody.isKinematic = true;
            }

            _dragIsActive = true;
            _objectPositionAtDragStart = transform.position;
            _objectToMousePosOffsetAtDragStart = 
                _objectPositionAtDragStart - 
                CameraController.GetMousePointOnPlaneParallelToCamera(_objectPositionAtDragStart);
        }

        private void OnMouseDrag()
        {
            if (!_dragIsActive) return;

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
        }

        private void OnMouseUp()
        {
            if (_rigidbody != null)
            {
                _rigidbody.isKinematic = _rigidbodyWasKinematicAtDragStart;
            }

            _dragIsActive = false;
            if (!SimulationBox.Instance.Bounds.Contains(transform.position))
            {
                OnDraggedOutOfBounds.Invoke(this);
            }
        }
    }
}
