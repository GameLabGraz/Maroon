using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class VisualizationPlaneLogic : MonoBehaviour
    {
        public const float THICKNESS = 0.01f; // In unity units

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshCollider meshCollider;
        [SerializeField] private GameObject selectionHighlightObject;
        [SerializeField] private VectorFieldFullscreenLogic vectorFieldLogic; // For access to electric-field buffers
        public DraggableObject draggable;
        public SelectableObject selectable;

        // Note(MartinR): Instead of having setters for all parameters, other objects can 
        //  just update the public members and call SetPlaneParametersAndUpdateMesh
        public bool heatmapEnabled;
        public bool equipotentialLinesEnabled;

        public static float heatmapCutoff = 200000.0f;
        public static float heatmapFalloff = 5.0f;
        public static float equipotentialLinesMaximum = 200000.0f;
        public static float equipotentialLinesSpacing = 30000.0f;

        public float transparency;
        public bool fixedPosition;
        public float fixedPositionVirtualZ;

        public Vector3 position = Vector3.zero; //Note: In 2D camera mode this may not be the same as transform.position
        public Vector3 planeNormal = Vector3.back;

        public void UpdateMeshAndDraggable()
        {
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;

            // Update draggable and movement gizmo
            draggable.draggableEnabled = !fixedPosition;
            selectable.enableMovementGizmo = !fixedPosition;
            SelectionSystem.Instance.movementGizmo.UpdateArrowsDependingOnSelection();

            // Update mesh
            Mesh newMesh = null;
            if (fixedPosition)
            {
                Vector3 normal = Vector3.back;
                var bounds = SimulationBox.Instance.Bounds;
                var pos = bounds.center;
                pos.z = bounds.max.z; // Place at end of box in fixed position mode
                if (!CameraController.Instance.In3DMode)
                {
                    pos.z += 3.0f; // Moves it further back in orthogonal mode to make room for vector-field
                }
                transform.position = pos;
                newMesh = ChargedPlane.CalculateClippedPlaneMeshWithVolume(bounds.center, planeNormal, THICKNESS);
            }
            else
            {
                transform.position = position;
                newMesh = ChargedPlane.CalculateClippedPlaneMeshWithVolume(position, planeNormal, THICKNESS);
            }

            meshFilter.mesh = newMesh;
            meshCollider.sharedMesh = newMesh;
        }

        private void Awake()
        {
            UpdateMeshAndDraggable();
            CameraController.Instance.OnCameraModeChanged.AddListener(() => { UpdateMeshAndDraggable(); });
            SimulationBox.Instance.OnBoundsChanged.AddListener((Bounds _unused) => { UpdateMeshAndDraggable(); });

            selectable.OnObjectSelectedOrDeselected.AddListener((bool selected) =>
            {
                selectionHighlightObject.SetActive(selected);
            });
            draggable.OnDraggedOutOfBounds.AddListener((DraggableObject _unused) => 
            { 
                gameObject.SetActive(false); 
                SelectionSystem.SetSelectedObject(null);
            });

            selectable.OnMovedWithGizmo.AddListener((SelectableObject _unused) => { position = transform.position; UpdateMeshAndDraggable(); });
            draggable.OnMoved.AddListener((DraggableObject _unused) => { position = transform.position; UpdateMeshAndDraggable(); });
        }

        // Updating shader data happens in LateUpdate
        void LateUpdate()
        {
            // Get plane equation (See comment in shader about layout)
            Vector4 planeEquation = Vector4.zero;
            if (fixedPosition)
            {
                Vector3 normal = planeNormal;
                normal = Vector3.back;
                var bounds = SimulationBox.Instance.Bounds;
                Vector3 pointOnPlane = SimulationBox.Instance.Bounds.center;
                pointOnPlane.z = Mathf.Lerp(bounds.min.z, bounds.max.z, fixedPositionVirtualZ / 2 + 0.5f);
                planeEquation = new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(normal, pointOnPlane));
            }
            else
            {
                planeEquation = new Vector4(planeNormal.x, planeNormal.y, planeNormal.z, -Vector3.Dot(planeNormal, position));
            }

            // Update shader properties
            var material = meshRenderer.material;
            vectorFieldLogic.chargedObjectComputeBuffers.SetUniformsForMaterial(material);

            material.SetFloat(Shader.PropertyToID("_Transparency"), transparency);
            material.SetVector(Shader.PropertyToID("_PlaneEquation"), planeEquation);

            material.SetInteger(Shader.PropertyToID("_DrawHeatmap"), heatmapEnabled ? 1 : 0);
            material.SetFloat(Shader.PropertyToID("_HeatmapMaxVoltage"), heatmapCutoff);
            material.SetFloat(Shader.PropertyToID("_HeatmapFalloff"), heatmapFalloff);

            material.SetInteger(Shader.PropertyToID("_DrawEquipotentialLines"), equipotentialLinesEnabled ? 1 : 0);
            material.SetFloat(Shader.PropertyToID("_LineMaxVoltage"), equipotentialLinesMaximum);
            material.SetFloat(Shader.PropertyToID("_LineSpacingVoltage"), equipotentialLinesSpacing);
        }
    }
}
