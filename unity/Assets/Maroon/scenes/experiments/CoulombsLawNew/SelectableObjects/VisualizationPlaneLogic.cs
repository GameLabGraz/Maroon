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
        public SelectableObject selectable; // public because the UI-logic also needs to access this

        [SerializeField] private GroundPinLogic groundPin;

        // Note(MartinR): Instead of having setters for all parameters, other objects can 
        //  just update the public members and call SetPlaneParametersAndUpdateMesh
        public int heatmapMode; // 0 = disabled, 1 = Potential, 2 = Magnitude
        public bool equipotentialLinesEnabled;
        public float equipotentialLinesSpacing = 30000.0f;
        public float transparency = 0.5f;

        public Vector3 position = Vector3.zero; //Note: In 2D camera mode this may not be the same as transform.position
        public Vector3 planeNormal = Vector3.back;

        private Vector3 positionOffset = Vector3.zero;

        [SerializeField] private GUIFloatInputLogic uiPotentialRange;
        [SerializeField] private GUIFloatInputLogic uiPotentialInterpolationExponent;
        [SerializeField] private GUIFloatInputLogic uiMaxMagnitude;
        [SerializeField] private GUIFloatInputLogic uiMagnitudeInterpolationExponent;

        public void UpdateMeshAndDraggable()
        {
            positionOffset = Vector3.zero;
            if (!CameraController.Instance.In3DMode)
            {
                // In 2D-Mode we move the plane further back so it doesn't occlude other objects
                Bounds box = SimulationBox.Instance.Bounds;
                positionOffset = new Vector3(0, 0, (box.max.z - box.min.z) + 1.0f);
            }

            transform.position = position + positionOffset;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;

            // Update mesh
            Mesh newMesh = ChargedPlane.CalculateClippedPlaneMeshWithVolume(position, planeNormal, THICKNESS);
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
            selectable.OnDraggedOutOfBounds.AddListener((SelectableObject _unused) => 
            {
                Vector3 pos = transform.position - positionOffset;
                // This extra check is needed because of the positionOffset
                if (SimulationBox.Instance.Bounds.Contains(pos)) return;

                gameObject.SetActive(false); 
                SelectionSystem.SetSelectedObject(null);
            });
            selectable.OnMoved.AddListener((SelectableObject _unused) => 
            { 
                position = transform.position - positionOffset; 
                UpdateMeshAndDraggable(); 
            });
        }

        // Updating shader data happens in LateUpdate
        void LateUpdate()
        {
            float groundPotential = 0.0f;
            if (groundPin != null && groundPin.isActiveAndEnabled && groundPin.applyToVisualization)
            {
                groundPotential = ElectricField.Instance.GetPotential(groundPin.transform.position, true);
            }

            // Get plane equation (See comment in shader about layout)
            Vector4 planeEquation = new Vector4(planeNormal.x, planeNormal.y, planeNormal.z, -Vector3.Dot(planeNormal, position));

            // Update shader properties
            var material = meshRenderer.material;
            ElectricField.Instance.electricFieldPackedGPUData.SetUniformsForMaterial(material);

            material.SetFloat(Shader.PropertyToID("_Transparency"), transparency);
            material.SetVector(Shader.PropertyToID("_PlaneEquation"), planeEquation);
            material.SetVector(Shader.PropertyToID("_PositionOffset"), positionOffset);

            material.SetInteger(Shader.PropertyToID("_HeatmapMode"), heatmapMode);
            material.SetFloat(Shader.PropertyToID("_VoltageRange"), uiPotentialRange.GetValue());
            material.SetFloat(Shader.PropertyToID("_VoltageOffset"), groundPotential);
            material.SetFloat(Shader.PropertyToID("_VoltageInterpolationExponent"), uiPotentialInterpolationExponent.GetValue());
            material.SetFloat(Shader.PropertyToID("_MaxMagnitude"), uiMaxMagnitude.GetValue());
            material.SetFloat(Shader.PropertyToID("_MagnitudeInterpolationExponent"), uiMagnitudeInterpolationExponent.GetValue());

            material.SetInteger(Shader.PropertyToID("_DrawEquipotentialLines"), equipotentialLinesEnabled ? 1 : 0);
            material.SetFloat(Shader.PropertyToID("_LineSpacingVoltage"), equipotentialLinesSpacing);
        }
    }
}
