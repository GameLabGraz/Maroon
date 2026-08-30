using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class SelectionHighlightMarker : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private UnityEngine.Animations.LookAtConstraint _lookAtConstraint;

        [SerializeField] private Color colorInBounds;
        [SerializeField] private Color colorOutOfBounds;

        private void AdjustToCameraMode()
        {
            _lookAtConstraint.enabled = CameraController.Instance.In3DMode;
            if (!CameraController.Instance.In3DMode)
            {
                // TODO: Orient Marker to face orthogonal camera view
                transform.rotation = Quaternion.identity;
            }
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _lookAtConstraint = GetComponent<UnityEngine.Animations.LookAtConstraint>();

            SelectionSystem.Instance.OnSelectionChanged.AddListener((SelectableObject selectable) =>
            {
                if (selectable != null)
                {
                    _spriteRenderer.enabled = selectable.enableSelectionHighlightCircle;
                }
                else
                {
                    _spriteRenderer.enabled = false;
                }
            });

            AdjustToCameraMode();
            CameraController.Instance.OnCameraModeChanged.AddListener(() => { AdjustToCameraMode(); });
        }

        // Note(MartinR): I guess using a PositionConstraint component is better for following other gameobjects,
        //      but for now I'll do the following in LateUpdate
        private void LateUpdate()
        {
            var selected = SelectionSystem.Instance.GetSelectedObject();
            if (selected == null) return;

            transform.position = selected.transform.position;
            float scale = selected.boundingRadius * 1.1f;
            transform.localScale = new Vector3(scale, scale, scale);

            // Set highlight color depending on position inside/outside of bounds
            if (SimulationBox.Instance.Bounds.Contains(transform.position))
            {
                _spriteRenderer.material.color = colorInBounds;
            }
            else
            {
                _spriteRenderer.material.color = colorOutOfBounds;
            }
        }
    }
}
