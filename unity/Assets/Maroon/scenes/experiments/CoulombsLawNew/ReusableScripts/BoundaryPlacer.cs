using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class BoundaryPlacer : MonoBehaviour
    {
        [SerializeField] private float thickness = 0.2f;
        [SerializeField] private PhysicMaterial physicMaterial;

        private void Start()
        {
            UpdateBoundaries();
            SimulationBox.Instance.OnBoundsChanged.AddListener((Bounds newBounds) => { UpdateBoundaries(); });
        }

        private void UpdateBoundaries()
        {
            var box = SimulationBox.Instance.Bounds;
    
            // Get all colliders
            var colliders = GetComponents<BoxCollider>();
            if (colliders.Length != 6)
            {
                for (int i = 0; i < 6 - colliders.Length; i++)
                {
                    var collider = gameObject.AddComponent<BoxCollider>();
                    collider.sharedMaterial = physicMaterial;
                }
            }
            colliders = GetComponents<BoxCollider>();

            // Set collider position to form a outer wall around box
            transform.position = box.center;
            for (int i = 0; i < 6; i++)
            {
                var collider = colliders[i];
                collider.sharedMaterial = physicMaterial;

                int dimension = i % 3;
                float sign = i < 3 ? 1.0f : -1.0f;

                Vector3 pos = Vector3.zero;
                pos[dimension] = sign * (box.extents[dimension] + thickness/2.0f);
                Vector3 size = box.size;
                size[dimension] = thickness;

                collider.center = pos;
                collider.size = size;
            }
        }
    }
}
