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
            draggable.OnDraggedOutOfBounds.AddListener((DraggableObject _unused) => { Destroy(gameObject); });

            selectable.OnMovedWithGizmo.AddListener((SelectableObject _unused) => { position = transform.position; UpdateMeshAndDraggable(); });
            draggable.OnMoved.AddListener((DraggableObject _unused) => { position = transform.position; UpdateMeshAndDraggable(); });
        }

        // Updating shader data happens in LateUpdate
        void LateUpdate()
        {
            // If the scene has no charged objects, we notify the shader, as otherwise
            // the plane would be completely grey as every point lies on the 0-equipotential-line
            bool sceneHasObjectWithCharge = false;

            // Get packed particle data
            List<Vector4> pointChargeData = new List<Vector4>();
            foreach (var pointCharge in ElectricField.Instance.chargedPoints)
            {
                var pos = pointCharge.transform.position;
                Vector4 packedInfo = new Vector4(pos.x, pos.y, pos.z, pointCharge.GetCharge());
                pointChargeData.Add(packedInfo);

                sceneHasObjectWithCharge = sceneHasObjectWithCharge || Mathf.Abs(pointCharge.GetCharge()) != 0;
            }

            // Trim/Resize particle count since we cannot resize the vector array
            //    --> https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.SetVectorArray.html
            int activePointChargeCount = pointChargeData.Count;
            const int MAX_POINT_CHARGES = 100; // Currently hardcoded in shader
            for (var i = pointChargeData.Count; i < MAX_POINT_CHARGES; i++)
            {
                pointChargeData.Add(new Vector4(0f, 0f, 0f, 0f));
            }
            if (pointChargeData.Count > MAX_POINT_CHARGES)
            {
                pointChargeData.RemoveRange(MAX_POINT_CHARGES - 1, pointChargeData.Count - MAX_POINT_CHARGES);
            }

            // Get packed charged rod data
            List<Vector4> chargedRodPositions = new List<Vector4>();
            List<Vector4> chargedRodDirections = new List<Vector4>();
            foreach (var chargedRod in ElectricField.Instance.chargedRods)
            {
                var pos = chargedRod.transform.position;
                var dir = chargedRod.GetDirection();
                Vector4 packedPos = new Vector4(pos.x, pos.y, pos.z, chargedRod.GetChargeDensity());
                Vector4 packedDir = new Vector4(dir.x, dir.y, dir.z, 0);
                chargedRodPositions.Add(packedPos);
                chargedRodDirections.Add(packedDir);

                sceneHasObjectWithCharge = sceneHasObjectWithCharge || Mathf.Abs(chargedRod.GetChargeDensity()) != 0;
            }

            int activeChargedRodCount = chargedRodPositions.Count;
            const int MAX_CHARGED_RODS = 30; // Currently hardcoded in shader
            for (var i = chargedRodPositions.Count; i < MAX_CHARGED_RODS; i++)
            {
                chargedRodPositions.Add(Vector4.zero);
                chargedRodDirections.Add(Vector4.zero);
            }
            if (chargedRodPositions.Count > MAX_CHARGED_RODS)
            {
                chargedRodPositions.RemoveRange(MAX_CHARGED_RODS - 1, chargedRodPositions.Count - MAX_CHARGED_RODS);
                chargedRodDirections.RemoveRange(MAX_CHARGED_RODS - 1, chargedRodDirections.Count - MAX_CHARGED_RODS);
            }

            // Get charged plane packed data
            List<Vector4> chargedPlaneEquations = new List<Vector4>();
            List<float> chargedPlaneChargeDensities = new List<float>();
            foreach (var chargedPlane in ElectricField.Instance.chargedPlanes)
            {
                var normal = chargedPlane.GetNormal();
                Vector4 equation = new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(normal, chargedPlane.transform.position));
                chargedPlaneEquations.Add(equation);
                chargedPlaneChargeDensities.Add(chargedPlane.GetChargeDensity());

                sceneHasObjectWithCharge = sceneHasObjectWithCharge || Mathf.Abs(chargedPlane.GetChargeDensity()) != 0;
            }

            int activeChargedPlaneCount = chargedPlaneEquations.Count;
            const int MAX_CHARGED_PLANES = 30; // Currently hardcoded in shader
            for (var i = chargedPlaneEquations.Count; i < MAX_CHARGED_PLANES; i++)
            {
                chargedPlaneEquations.Add(Vector4.zero);
                chargedPlaneChargeDensities.Add(0.0f);
            }
            if (chargedPlaneEquations.Count > MAX_CHARGED_PLANES)
            {
                chargedPlaneEquations.RemoveRange(MAX_CHARGED_PLANES - 1, chargedPlaneEquations.Count - MAX_CHARGED_PLANES);
                chargedPlaneChargeDensities.RemoveRange(MAX_CHARGED_PLANES - 1, chargedPlaneChargeDensities.Count - MAX_CHARGED_PLANES);
            }

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

            if (!sceneHasObjectWithCharge)
            {
                activePointChargeCount = 0;
                activeChargedRodCount = 0;
                activeChargedPlaneCount = 0;
            }

            // Update shader properties
            var material = meshRenderer.material;
            material.SetInt(Shader.PropertyToID("_PointChargeCount"), activePointChargeCount);
            material.SetVectorArray(Shader.PropertyToID("_PointChargeData"), pointChargeData);
            material.SetInt(Shader.PropertyToID("_ChargedRodCount"), activeChargedRodCount);
            material.SetVectorArray(Shader.PropertyToID("_ChargedRodPositions"), chargedRodPositions);
            material.SetVectorArray(Shader.PropertyToID("_ChargedRodDirections"), chargedRodDirections);
            material.SetInt(Shader.PropertyToID("_ChargedPlaneCount"), activeChargedPlaneCount);
            material.SetVectorArray(Shader.PropertyToID("_ChargedPlaneEquations"), chargedPlaneEquations);
            material.SetFloatArray(Shader.PropertyToID("_ChargedPlaneChargeDensities"), chargedPlaneChargeDensities);

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
