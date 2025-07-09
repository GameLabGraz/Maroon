using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Experiments.CoulombsLawNew
{
    public class ChargedRod : MonoBehaviour
    {
        public const float RADIUS = 0.04f; // In unity units // NOTE: This may actually be the diameter!
        public const float MAX_CHARGE_DENSITY = 5e-6f; // In Coulomb/meter

        private float chargeDensity;

        // Line is defined by point (transform.position and direction)
        [SerializeField] private Vector3 direction;

        [Header("References to components")]
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private SelectableObject selectableComponent;

        [Header("References to Child-objects")]
        [SerializeField] private GameObject childStartSphere;
        [SerializeField] private GameObject childEndSphere;
        [SerializeField] private GameObject childCylinder;
        [SerializeField] private GameObject childSelectionSphere;

        private Vector3 lastUpdatePos;

        void Awake()
        {
            SetRodParameters(transform.position, direction);
            SetChargeDensity(chargeDensity);
            childSelectionSphere.SetActive(false);
            lastUpdatePos = transform.position;

            // Register Callbacks
            selectableComponent.OnObjectSelectedOrDeselected.AddListener((bool isSelected) => { childSelectionSphere.SetActive(isSelected); });
            selectableComponent.OnMoved.AddListener((SelectableObject _unused) => { Update3DRepresentation(); });
            selectableComponent.OnDraggedOutOfBounds.AddListener((SelectableObject _unused) =>
            {
                GameObject.Destroy(this.gameObject);
            });

            // Register in Efield
            ElectricField.Instance.chargedRods.Add(this);
        }

        private void OnDestroy()
        {
            ElectricField.Instance?.chargedRods.Remove(this);
        }

        public void SetRodParameters(Vector3 position, Vector3 direction)
        {
            if (direction.magnitude < 0.01f)
            {
                direction = Vector3.up;
            }
            direction = direction.normalized;
            this.direction = direction;
            transform.position = position;
            Update3DRepresentation();
        }

        public Vector3 GetDirection() { return direction; }

        public float GetChargeDensity() { return chargeDensity; }
        public void SetChargeDensity(float newChargeDensity) 
        {
            // newChargeDensity = Mathf.Clamp(newChargeDensity, -MAX_CHARGE_DENSITY, MAX_CHARGE_DENSITY);
            chargeDensity = newChargeDensity;

            // Set color of all child objects
            Color color = ChargedPoint.ChargeValueToColor(chargeDensity, MAX_CHARGE_DENSITY);
            childStartSphere.GetComponent<MeshRenderer>().material.color = color;
            childEndSphere.GetComponent<MeshRenderer>().material.color = color;
            childCylinder.GetComponent<MeshRenderer>().material.color = color;
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

        public void Update3DRepresentation()
        {
            var pos = transform.position;

            // Find intersection of rod and SimulationBox (For visual display)
            Ray ray = new Ray(pos, direction);
            float t0, t1;
            bool rayIntersectsBox = RayBoxIntersection(ray, SimulationBox.Instance.Bounds, out t0, out t1);

            childStartSphere.SetActive(rayIntersectsBox);
            childEndSphere.SetActive(rayIntersectsBox);
            childCylinder.SetActive(rayIntersectsBox);
            if (!rayIntersectsBox) return;

            var posA = ray.GetPoint(t0);
            var posB = ray.GetPoint(t1);

            // Update this transform (parent) first, as child objects are otherwise influenced by parent transform changing
            var capsuleYOffset = Vector3.Dot((posA + posB) / 2 - pos, direction);
            transform.rotation = Quaternion.FromToRotation(Vector3.up, posA - posB);
            transform.localScale = Vector3.one;
            capsuleCollider.height = (posA - posB).magnitude + 2.0f * RADIUS;
            capsuleCollider.radius = RADIUS;
            capsuleCollider.center = new Vector3(0, -capsuleYOffset, 0);

            // Update child transforms
            childStartSphere.transform.position = posA;
            childStartSphere.transform.localScale = 2.0f * RADIUS * Vector3.one;

            childEndSphere.transform.position = posB;
            childEndSphere.transform.localScale = 2.0f * RADIUS * Vector3.one;

            childCylinder.transform.position = (posA + posB) / 2;
            // Cylinder mesh points upwards by default
            childCylinder.transform.rotation = transform.rotation;
            childCylinder.transform.localScale = new Vector3(2.0f * RADIUS, (posA - posB).magnitude / 2, 2.0f * RADIUS);
        }

    }
}
