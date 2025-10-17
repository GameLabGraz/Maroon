using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryPlacer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform min = null;
    [SerializeField] private Transform max = null;
    [SerializeField] private float thicknessXY = 0.2f; 
    [SerializeField] private float thicknessZ = 1.0f;

    void Start()
    {
        if (min == null || max == null)
        {
            Debug.LogWarning("Boundary Placer requires min/max to be set to work properly");
            return;
        }

        var colliders = GetComponents<BoxCollider>();
        if (colliders.Length != 4)
        {
            Debug.LogWarning("Boundary Placer expects 4 Box-Colliders on the Object to work, given: " + colliders.Length);
            return;
        }

        // Set collider position to form a outer wall around min/max
        transform.position = min.position;

        var boundarySize = max.position - min.position;
        var localCenter = boundarySize / 2.0f;

        var left = colliders[0];
        var right = colliders[1];
        var top = colliders[2];
        var bottom = colliders[3];

        left.center = localCenter - (boundarySize.x / 2.0f + thicknessXY) * Vector3.right;
        left.size = new Vector3(thicknessXY, boundarySize.y + 4 * thicknessXY, thicknessZ);

        right.center = localCenter + (boundarySize.x / 2.0f + thicknessXY) * Vector3.right;
        right.size = new Vector3(thicknessXY, boundarySize.y + 4 * thicknessXY, thicknessZ);
        
        top.center = localCenter + (boundarySize.y / 2.0f + thicknessXY) * Vector3.up;
        top.size = new Vector3(boundarySize.x + 4 * thicknessXY, thicknessXY, thicknessZ);

        bottom.center = localCenter - (boundarySize.y / 2.0f + thicknessXY) * Vector3.up;
        bottom.size = new Vector3(boundarySize.x + 4 * thicknessXY, thicknessXY, thicknessZ);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
