using UnityEngine;

public class PlanetTargetLine : MonoBehaviour
{
    public GameObject[] placeholders; // Assign the modified placeholders in the Inspector
    public PlanetCheckOrder planetCheckOrder;

    public void SnapToClosestPlaceholder(GameObject sphere, float snapDistance)
    {
        float minDistance = float.MaxValue;
        GameObject closestPlaceholder = null;

        for (int i = 0; i < placeholders.Length; i++)
        {
            float distance = Vector3.Distance(sphere.transform.position, placeholders[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPlaceholder = placeholders[i];
            }
        }

        if (closestPlaceholder != null && minDistance < snapDistance)
        {
            sphere.transform.position = closestPlaceholder.transform.position;
            planetCheckOrder.CheckCorrectOrder(); // Check order after snapping
        }
    }
}
