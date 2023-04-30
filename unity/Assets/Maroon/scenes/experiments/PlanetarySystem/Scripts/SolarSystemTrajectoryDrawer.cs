using UnityEngine;
using System.Collections.Generic;

public class SolarSystemTrajectoryDrawer : MonoBehaviour
{
    [System.Serializable]
    public class PlanetTrajectory
    {
        public GameObject planet;
        public Color color;
    }

    public List<PlanetTrajectory> planetTrajectories;
    public int numSegments = 1000;
    public float lineThickness = 0.5f;

    private List<LineRenderer> lineRenderers;
    private List<Queue<Vector3>> previousPositionsList;

    void Start()
    {
        lineRenderers = new List<LineRenderer>();
        previousPositionsList = new List<Queue<Vector3>>();

        foreach (PlanetTrajectory pt in planetTrajectories)
        {
            GameObject trajectory = new GameObject(pt.planet.name + "Trajectory");
            trajectory.transform.SetParent(transform);

            LineRenderer lr = trajectory.AddComponent<LineRenderer>();
            lr.positionCount = numSegments;
            lr.startWidth = lineThickness;
            lr.endWidth = lineThickness;

            Material trajectoryMaterial = new Material(Shader.Find("Unlit/Color"));
            trajectoryMaterial.color = pt.color;
            lr.material = trajectoryMaterial;

            lineRenderers.Add(lr);

            previousPositionsList.Add(new Queue<Vector3>(numSegments));
        }
    }

    void Update()
    {
        for (int i = 0; i < planetTrajectories.Count; i++)
        {
            PlanetTrajectory pt = planetTrajectories[i];
            Queue<Vector3> previousPositions = previousPositionsList[i];

            if (previousPositions.Count >= numSegments)
            {
                previousPositions.Dequeue();
            }

            previousPositions.Enqueue(pt.planet.transform.position);

            lineRenderers[i].positionCount = previousPositions.Count;
            lineRenderers[i].SetPositions(previousPositions.ToArray());
        }
    }
}
