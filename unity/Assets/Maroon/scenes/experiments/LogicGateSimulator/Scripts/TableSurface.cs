using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableSurface : MonoBehaviour
{
    public BoxCollider surfaceCollider;
    public float edgePadding = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        if (surfaceCollider == null)
        {
            surfaceCollider = GetComponent<BoxCollider>();
        }

        if (edgePadding < 0f)
        {
            edgePadding = 0f;
        }
    }

    public Vector3 GetSurfaceNormal()
    {
        Vector3 normal = surfaceCollider.transform.TransformDirection(Vector3.up);

        return normal.normalized;
    }


    public Vector3 ClampMovement(BoxCollider[] movingColliders, Vector3 wantedWorldMovement)
    {
        if (surfaceCollider == null)
        {
            return Vector3.zero;
        }

        if (movingColliders == null)
        {
            return Vector3.zero;
        }

        Transform tableTransform = surfaceCollider.transform;

        Vector3 wantedLocalMovement = tableTransform.InverseTransformVector(wantedWorldMovement);

        wantedLocalMovement.y = 0f;

        float blockMinimumX = float.PositiveInfinity;
        float blockMaximumX = float.NegativeInfinity;
        float blockMinimumZ = float.PositiveInfinity;
        float blockMaximumZ = float.NegativeInfinity;

        bool foundCollider = false;

        foreach (BoxCollider boxCollider in movingColliders)
        {
            if (boxCollider == null)
            {
                continue;
            }

            if (!boxCollider.enabled)
            {
                continue;
            }

            if (!boxCollider.gameObject.activeInHierarchy)
            {
                continue;
            }

            Vector3 colliderCenter = boxCollider.center;

            Vector3 colliderHalfSize = boxCollider.size * 0.5f;

            //check collider corners
            for (int x = -1; x <= 1; x = x + 2)
            {
                for (int y = -1; y <= 1; y = y + 2)
                {
                    for (int z = -1; z <= 1; z = z + 2)
                    {
                        Vector3 cornerDirection = new Vector3(x, y, z);

                        Vector3 localCorner = colliderCenter + Vector3.Scale(
                                colliderHalfSize, cornerDirection);

                        Vector3 worldCorner = boxCollider.transform.TransformPoint(
                                localCorner);

                        Vector3 tableCorner = tableTransform.InverseTransformPoint(
                                worldCorner);

                        if (tableCorner.x < blockMinimumX)
                        {
                            blockMinimumX = tableCorner.x;
                        }

                        if (tableCorner.x > blockMaximumX)
                        {
                            blockMaximumX = tableCorner.x;
                        }

                        if (tableCorner.z < blockMinimumZ)
                        {
                            blockMinimumZ = tableCorner.z;
                        }

                        if (tableCorner.z > blockMaximumZ)
                        {
                            blockMaximumZ = tableCorner.z;
                        }
                    }
                }
            }

            foundCollider = true;
        }

        if (!foundCollider)
        {
            return Vector3.zero;
        }

        float worldUnitsPerLocalX = tableTransform.TransformVector(
                Vector3.right).magnitude;

        float worldUnitsPerLocalZ = tableTransform.TransformVector(
                Vector3.forward).magnitude;

        if (worldUnitsPerLocalX <= Mathf.Epsilon)
        {
            return Vector3.zero;
        }

        if (worldUnitsPerLocalZ <= Mathf.Epsilon)
        {
            return Vector3.zero;
        }

        float paddingX = edgePadding / worldUnitsPerLocalX;

        float paddingZ = edgePadding / worldUnitsPerLocalZ;

        float tableMinimumX = surfaceCollider.center.x;

        tableMinimumX = tableMinimumX - surfaceCollider.size.x * 0.5f;

        tableMinimumX = tableMinimumX + paddingX;

        float tableMaximumX = surfaceCollider.center.x;

        tableMaximumX = tableMaximumX + surfaceCollider.size.x * 0.5f;

        tableMaximumX = tableMaximumX - paddingX;

        float tableMinimumZ = surfaceCollider.center.z;

        tableMinimumZ = tableMinimumZ - surfaceCollider.size.z * 0.5f;

        tableMinimumZ = tableMinimumZ + paddingZ;

        float tableMaximumZ = surfaceCollider.center.z;

        tableMaximumZ = tableMaximumZ + surfaceCollider.size.z * 0.5f;

        tableMaximumZ = tableMaximumZ - paddingZ;

        float minimumXMovement = tableMinimumX - blockMinimumX;

        float maximumXMovement = tableMaximumX - blockMaximumX;

        float minimumZMovement = tableMinimumZ - blockMinimumZ;

        float maximumZMovement = tableMaximumZ - blockMaximumZ;

        wantedLocalMovement.x = ClampAxisMovement(wantedLocalMovement.x,
            minimumXMovement, maximumXMovement);

        wantedLocalMovement.z = ClampAxisMovement(wantedLocalMovement.z,
            minimumZMovement,maximumZMovement);

        return tableTransform.TransformVector(wantedLocalMovement);
    }

    float ClampAxisMovement(float wantedMovement, float minimumMovement,
        float maximumMovement)
    {
        if (minimumMovement > maximumMovement)
        {
            return 0f;
        }

        float limitedMovement = Mathf.Clamp(wantedMovement,
            minimumMovement, maximumMovement);

        if (wantedMovement > 0f)
        {
            return Mathf.Clamp(limitedMovement, 0f, wantedMovement);
        }
        else
        {
            return Mathf.Clamp(limitedMovement, wantedMovement, 0f);
        }
    }
}