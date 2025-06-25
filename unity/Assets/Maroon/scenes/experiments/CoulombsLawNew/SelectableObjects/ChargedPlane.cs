using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class ChargedPlane : MonoBehaviour
    {
        public const float THICKNESS = 0.02f; // In unity units
        public const float MAX_CHARGE_DENSITY = 1e-5f; // In Coulomb/meter^2

        [SerializeField] private Vector3 planeNormal = Vector3.up;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshCollider meshCollider;
        [SerializeField] private MeshRenderer meshRenderer;

        [SerializeField] private GameObject selectionHighlightSphere;
        [SerializeField] private DraggableObject draggableComponent;
        [SerializeField] private SelectableObject selectableComponent;

        private float chargeDensity = 0.0f;

        private void Awake()
        {
            UpdateChargedPlaneMesh();
            SetChargeDensity(chargeDensity); // Initializes color

            selectionHighlightSphere.SetActive(false);
            selectableComponent.OnObjectSelectedOrDeselected.AddListener((bool isSelected) => selectionHighlightSphere.SetActive(isSelected));
            draggableComponent.OnDraggedOutOfBounds.AddListener((DraggableObject _unused) => { GameObject.Destroy(this.gameObject); });
            draggableComponent.OnMoved.AddListener((DraggableObject _unused) => { UpdateChargedPlaneMesh(); });
            selectableComponent.OnMovedWithGizmo.AddListener((SelectableObject _unused) => { UpdateChargedPlaneMesh(); });

            ElectricField.Instance.chargedPlanes.Add(this);
        }

        private void OnDestroy()
        {
            ElectricField.Instance?.chargedPlanes.Remove(this);
        }
        public void SetPlaneParameters(Vector3 position, Vector3 normal)
        {
            if (normal.magnitude < 0.001f)
            {
                normal = Vector3.right;
            }
            this.planeNormal = normal.normalized;
            this.transform.position = position;
            this.transform.rotation = Quaternion.identity;

            UpdateChargedPlaneMesh();
        }
        public Vector3 GetNormal() { return planeNormal; }

        public float GetChargeDensity() { return chargeDensity; }

        public void SetChargeDensity(float newChargeDensity) 
        {
            newChargeDensity = Mathf.Clamp(newChargeDensity, -MAX_CHARGE_DENSITY, MAX_CHARGE_DENSITY);
            chargeDensity = newChargeDensity;
            meshRenderer.material.color = ChargedPoint.ChargeValueToColor(chargeDensity, MAX_CHARGE_DENSITY);
        }

        private void UpdateChargedPlaneMesh()
        {
            var newMesh = CalculateClippedPlaneMeshWithVolume(transform.position, planeNormal, THICKNESS);
            meshFilter.mesh = newMesh;
            meshCollider.sharedMesh = newMesh;
        }


        // Mesh-Clipping starts here

        // Note(MartinR):
        //      What I wanted to do here is the following: 
        //      Since planes are infinite, I wanted to only render it inside the SimulationBox so that it doesn't completely 
        //      clutter the view. I also wanted to give the plane some thickness (Render it as a box), so it is still clickable/selectable
        //      even when looking at it from an orthographic view parallel to the plane.
        //      So I would need the intersection between the SimulationBox and the plane-box as a mesh, which turns out to be a somewhat
        //      complicated problem (intersection of 2 convex polygons, then extracting vertices and faces from that).
        //      Instead of doing that I decided to use polygon-clipping to clip the 2D-plane inside the 3D-box with
        //      the Sutherland-Hodgeman polygon clipping algorithm, and then extrude the resulting 2D-polygon into a 3D mesh
        private static float SignedDistanceToPlane(Vector3 pos, Vector4 plane)
        {
            return Vector4.Dot(new Vector4(pos.x, pos.y, pos.z, 1.0f), plane);
        }

        private static Vector3 GetLinePlaneIntersectionPoint(Vector3 a, Vector3 b, Vector4 plane)
        {
            float ta = SignedDistanceToPlane(a, plane);
            float tb = SignedDistanceToPlane(b, plane);
            float t = -tb / (ta - tb);
            return t * a + (1 - t) * b;
        }

        private static void ClipVerticesOnPlane(List<Vector3> vertices, List<Vector3> clippedVertices, Vector4 plane)
        {
            clippedVertices.Clear();
            if (vertices.Count == 0) return;

            bool currInside = SignedDistanceToPlane(vertices[0], plane) >= 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 curr = vertices[i];
                Vector3 next = vertices[(i + 1) % vertices.Count];

                // Check if we are going over the plane
                bool nextInside = SignedDistanceToPlane(next, plane) >= 0;

                // Add intersection if we are crossing
                if (nextInside != currInside)
                {
                    clippedVertices.Add(GetLinePlaneIntersectionPoint(curr, next, plane));
                }
                // Add next if inside
                if (nextInside)
                {
                    clippedVertices.Add(next);
                }
                currInside = nextInside;
            }
        }

        private static List<Vector3> CalculateClippedPlaneVertices(Vector3 position, Vector3 normal)
        {
            // Get clipping planes (Note: in object local space)
            Bounds bounds = SimulationBox.Instance.Bounds;
            Vector3 min = bounds.min - position;
            Vector3 max = bounds.max - position;
            Vector4[] clippingPlanes = new Vector4[]
            {
                new Vector4( 1,  0,  0, -min.x),
                new Vector4( 0,  1,  0, -min.y),
                new Vector4( 0,  0,  1, -min.z),
                new Vector4(-1,  0,  0, max.x),
                new Vector4( 0, -1,  0, max.y),
                new Vector4( 0,  0, -1, max.z),
            };

            // Define Vertices
            Quaternion rotation = Quaternion.FromToRotation(Vector3.back, normal);
            List<Vector3> vertices = new List<Vector3>
            {
                rotation * (100 * new Vector3(-1, -1, 0)),
                rotation * (100 * new Vector3(-1,  1, 0)),
                rotation * (100 * new Vector3( 1,  1, 0)),
                rotation * (100 * new Vector3( 1, -1, 0)),
            };

            // Clip vertices against all planes (May generate more vertices)
            List<Vector3> clippedVertices = new List<Vector3>();
            for (int i = 0; i < clippingPlanes.Length; i++)
            {
                ClipVerticesOnPlane(vertices, clippedVertices, clippingPlanes[i]);
                vertices.Clear();
                var swap = vertices;
                vertices = clippedVertices;
                clippedVertices = swap;
            }

            return vertices;
        }

        public static Mesh CalculateClippedPlaneMeshWithVolume(Vector3 position, Vector3 planeNormal, float thickness)
        {
            var vertices = CalculateClippedPlaneVertices(position, planeNormal);

            var meshVertices = new List<Vector3>();
            foreach (var vertex in vertices)
            {
                meshVertices.Add(vertex + planeNormal * thickness / 2); // Upper face vertex
                meshVertices.Add(vertex + planeNormal * thickness / 2); // Side face upper vertex
                meshVertices.Add(vertex - planeNormal * thickness / 2); // Side face lower vertex
                meshVertices.Add(vertex - planeNormal * thickness / 2); // Lower face vertex
            }

            // Generate triangle indices (Top + bottom face and border faces)
            List<int> indices = new List<int>();
            for (int i = 1; i + 1 < vertices.Count; i++)
            {
                // Upper face
                indices.Add((0) * 4);
                indices.Add((i) * 4);
                indices.Add((i + 1) * 4);

                // Lower face
                indices.Add((0) * 4 + 3);
                indices.Add((i + 1) * 4 + 3);
                indices.Add((i) * 4 + 3);
            }

            // Side-Faces
            for (int i = 0; i < vertices.Count; i++)
            {
                int i0 = i * 4 + 1; 
                int i1 = i * 4 + 2; 
                int i2 = (i + 1) % vertices.Count * 4 + 1; 
                int i3 = (i + 1) % vertices.Count * 4 + 2; 

                indices.Add(i0);
                indices.Add(i3);
                indices.Add(i2);
                indices.Add(i0);
                indices.Add(i1);
                indices.Add(i3);
            }

            // Generate normals from triangles
            List<Vector3> normals = new List<Vector3>();
            foreach (var vertex in meshVertices)
            {
                normals.Add(Vector3.one);
            }
            for (int i = 0; i < indices.Count; i += 3)
            {
                Vector3 a = meshVertices[indices[i + 0]];
                Vector3 b = meshVertices[indices[i + 1]];
                Vector3 c = meshVertices[indices[i + 2]];

                Vector3 normal = Vector3.Cross(b - a, c - b).normalized;
                normals[indices[i + 0]] = normal;
                normals[indices[i + 1]] = normal;
                normals[indices[i + 2]] = normal;
            }

            Mesh mesh = new Mesh();
            mesh.SetVertices(meshVertices);
            mesh.SetNormals(normals);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            return mesh;
        }
    }
}
