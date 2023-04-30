using UnityEngine;
using System.Collections.Generic;

public class PlanetTrajectoryDrawer : MonoBehaviour
{
    public GameObject planet;
    public int numSegments = 250;
    public float lineThickness = 0.5f;

    private LineRenderer lineRenderer;
    private Queue<Vector3> previousPositions;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numSegments;
        lineRenderer.startWidth = lineThickness;
        lineRenderer.endWidth = lineThickness;

        previousPositions = new Queue<Vector3>(numSegments);
    }

    void Update()
    {
        if (previousPositions.Count >= numSegments)
        {
            previousPositions.Dequeue();
        }

        previousPositions.Enqueue(planet.transform.position);

        lineRenderer.positionCount = previousPositions.Count;
        lineRenderer.SetPositions(previousPositions.ToArray());
    }
}
