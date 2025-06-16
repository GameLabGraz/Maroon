using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    public class ChargedRod : MonoBehaviour
    {
        public const float RADIUS = 0.05f; // In unity units

        // Line is defined by start and end point
        [SerializeField] private Vector3 startPos;
        [SerializeField] private Vector3 endPos;

        [SerializeField] private GameObject childStartSphere;
        [SerializeField] private GameObject childEndSphere;
        [SerializeField] private GameObject childCylinder;
        [SerializeField] private CapsuleCollider capsuleCollider;

        void Awake()
        {
            SetRodPosition(startPos, endPos);
        }

        private void OnValidate()
        {
            SetRodPosition(startPos, endPos);
        }

        // Note(MartinR): Unity Bounds has a bounds.intersectsRay, but that doesn't return both intersection points
        //      Test inspired by "https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-box-intersection.html"
        public static bool RayBoxIntersection(Ray ray, Bounds box, out float t0, out float t1)
        {
            // Move box to coord system center
            ray.origin = ray.origin - box.center;
            // Flip coordinate system so ray direction coordinates are all positive
            Vector3 directionSign = new Vector3(Mathf.Sign(ray.direction.x), Mathf.Sign(ray.direction.y), Mathf.Sign(ray.direction.z));
            ray.direction = Vector3.Scale(ray.direction, directionSign); // Vector3.Scale is just elementwise multiplication
            ray.origin = Vector3.Scale(ray.origin, directionSign);

            // First figure out in which t intervals the ray crosses the min/max coordinates for X/Y/Z
            Vector3 extends = box.extents;
            float tXMax = (extends.x - ray.origin.x) / ray.direction.x;
            float tXMin = (-extends.x - ray.origin.x) / ray.direction.x;

            float tYMax = (extends.y - ray.origin.y) / ray.direction.y;
            float tYMin = (-extends.y - ray.origin.y) / ray.direction.y;

            float tZMax = (extends.z - ray.origin.z) / ray.direction.z;
            float tZMin = (-extends.z - ray.origin.z) / ray.direction.z;

            // Find intersection of all 3 intervals
            t0 = Mathf.Max(Mathf.Max(tXMin, tYMin), tZMin);
            t1 = Mathf.Min(Mathf.Min(tXMax, tYMax), tZMax);

            // If intersection is empty, no collision
            return t0 <= t1;
        }

        public void SetRodPosition(Vector3 startPos, Vector3 endPos)
        {
            this.startPos = startPos;
            this.endPos = endPos;

            if ((endPos - startPos).magnitude < 0.01f)
            {
                endPos = startPos + Vector3.up;
            }

            // Find intersection of rod and SimulationBox (For visual display)
            Ray ray = new Ray(startPos, endPos - startPos);
            float t0, t1;
            bool rayIntersectsBox = RayBoxIntersection(ray, SimulationBox.Instance.Bounds, out t0, out t1);

            childStartSphere.SetActive(rayIntersectsBox);
            childEndSphere.SetActive(rayIntersectsBox);
            childCylinder.SetActive(rayIntersectsBox);
            if (!rayIntersectsBox) return;

            var posA = ray.GetPoint(t0);
            var posB = ray.GetPoint(t1);

            // Update collider first, as child objects are otherwise influenced by parent transform changing
            transform.position = (posA + posB) / 2;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, posA - posB);
            transform.localScale = Vector3.one;
            capsuleCollider.height = (posA - posB).magnitude + RADIUS;
            capsuleCollider.radius = RADIUS/2;

            // Update child transforms
            childStartSphere.transform.position = posA;
            childStartSphere.transform.localScale = new Vector3(RADIUS, RADIUS, RADIUS);

            childEndSphere.transform.position = posB;
            childEndSphere.transform.localScale = new Vector3(RADIUS, RADIUS, RADIUS);

            childCylinder.transform.position = (posA + posB) / 2;
            // Cylinder mesh points upwards by default
            childCylinder.transform.rotation = transform.rotation;
            childCylinder.transform.localScale = new Vector3(RADIUS, (posA - posB).magnitude / 2, RADIUS);
        }

        public Vector3 GetStartPos() { return startPos; }
        public Vector3 GetEndPos() { return endPos; }
    }
}
