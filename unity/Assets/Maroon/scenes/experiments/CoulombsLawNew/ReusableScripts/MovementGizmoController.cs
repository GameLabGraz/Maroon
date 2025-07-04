using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class MovementGizmoController : MonoBehaviour
    {
        const float MIN_ANGLE = 20.0f;
        const float FADE_ANGLE_DISTANCE = 10.0f;

        [SerializeField] [Range(0, 1)] private float offsetFromObject = 0.1f;
        [SerializeField] [Range(0, 1)] private float arrowSize = 0.1f;

        private MovementGizmoArrow[] arrows = new MovementGizmoArrow[6]; // See Awake to know which arrows are which

        private void InitializeArrows()
        {
            // Find references to arrows
            arrows[0] = transform.Find("PositiveX").GetComponent<MovementGizmoArrow>();
            arrows[1] = transform.Find("PositiveY").GetComponent<MovementGizmoArrow>();
            arrows[2] = transform.Find("PositiveZ").GetComponent<MovementGizmoArrow>();
            arrows[3] = transform.Find("NegativeX").GetComponent<MovementGizmoArrow>();
            arrows[4] = transform.Find("NegativeY").GetComponent<MovementGizmoArrow>();
            arrows[5] = transform.Find("NegativeZ").GetComponent<MovementGizmoArrow>();

            // Set arrow meshes and color
            Mesh arrowMesh = ArrowMeshCreator.CreateArrowMesh(.2f, 1.2f, .4f, 16, 4);
            foreach (var arrow in arrows)
            {
                var meshFilter = arrow.GetComponent<MeshFilter>();
                meshFilter.mesh = arrowMesh;
                Color color = Color.black;
                color[arrow.dimension] = 1.0f;
                arrow.GetComponent<MeshRenderer>().material.color = color;
            }

            // Note: ArrowMesh points towards positive z (forward) by default
            arrows[0].transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.right);
            arrows[1].transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
            arrows[2].transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.forward);
            arrows[3].transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.left);
            arrows[4].transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.down);
            arrows[5].transform.rotation = Quaternion.FromToRotation(Vector3.forward, Vector3.back);

            // Initial Positioning (So that it looks nice in edit-mode)
            for (int i = 0; i < arrows.Length; i++)
            {
                var arrow = arrows[i];
                Vector3 offsetDir = Vector3.zero;
                offsetDir[i % 3] = i >= 3 ? -1 : 1; // See Awake for how arrow-indices relate to dimensions

                arrow.transform.localScale = new Vector3(arrowSize, arrowSize, arrowSize);
                arrow.transform.localPosition = offsetDir * (arrowSize + offsetFromObject);
            }
        }

        // Note(MartinR): Uncomment if you want to play around/see gizmo in editor, but this causes some warnings to appear
        // private void OnValidate() { InitializeArrows(); }

        private void Awake()
        {
            InitializeArrows();
            UpdateArrowsDependingOnSelection();
            SelectionSystem.Instance.OnSelectionChanged.AddListener((SelectableObject selected) => 
            {
                UpdateArrowsDependingOnSelection();
            });
            CameraController.Instance.OnCameraModeChanged.AddListener(() => UpdateArrowsDependingOnSelection());
        }

        public void UpdateArrowsDependingOnSelection()
        {
            // Hide/Show arrows depending on selection
            var selected = SelectionSystem.Instance.GetSelectedObject();
            bool arrowsActive = selected != null && selected.enableMovementGizmo;
            bool in3D = CameraController.Instance.In3DMode;
            foreach (var arrow in arrows) {
                arrow.gameObject.SetActive(arrowsActive); 
            }
            if (!arrowsActive) return;

            // Position arrows around selected object
            Vector3 camDir = Camera.main.transform.TransformDirection(Vector3.forward);
            for (int i = 0; i < arrows.Length; i++)
            {
                var arrow = arrows[i];

                // Set position
                Vector3 arrowDir = Vector3.zero;
                arrowDir[i % 3] = i >= 3 ? -1 : 1; // See Awake for how arrow-indices relate to dimensions
                arrow.transform.position = 
                    selected.transform.position + 
                    arrowDir * (selected.boundingRadius + offsetFromObject + arrowSize);
                arrow.dimension = i % 3;

                // Set arrow scale (Arrows are disabled if camDir and arrowDir are too close together
                float angle = Mathf.Acos(Mathf.Abs(Vector3.Dot(camDir, arrowDir))) / (2.0f * Mathf.PI) * 360.0f; // Angle in Degree
                arrow.validDragTarget = angle >= MIN_ANGLE;
                float scale = arrowSize;
                if (!arrow.validDragTarget)
                {
                    // Fade arrow out if angle is too small
                    float t = (angle - (MIN_ANGLE - FADE_ANGLE_DISTANCE)) / (FADE_ANGLE_DISTANCE);
                    t = Mathf.Clamp(t, 0.0f, 1.0f);
                    scale = scale * t;
                }
                arrow.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        private void LateUpdate() { UpdateArrowsDependingOnSelection(); }
    }
}
