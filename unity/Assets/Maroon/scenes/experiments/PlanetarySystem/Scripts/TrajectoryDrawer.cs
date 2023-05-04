using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class TrajectoryDrawer : MonoBehaviour
{

    [System.Serializable]
    public class PlanetTrajectory
    {
        public GameObject planet;
        public Material trajectoryMaterial;
    }

    public List<PlanetTrajectory> planetTrajectories;
    public int numSegments = 1000;
    public float lineThickness = 0.5f;

    public Toggle toggleAllTrajectories;


    private List<LineRenderer> lineRenderers;
    private List<Queue<Vector3>> previousPositionsList;

    void Start()
    {
        SetupToggle();

        lineRenderers = new List<LineRenderer>();

            previousPositionsList = new List<Queue<Vector3>>();

        foreach (PlanetTrajectory orbit in planetTrajectories)
        {
    
            GameObject trajectory = new GameObject(orbit.planet.name + "Trajectory");
            trajectory.transform.SetParent(transform);

            LineRenderer lr = trajectory.AddComponent<LineRenderer>();
            lr.positionCount = numSegments;
            lr.startWidth = lineThickness;
            lr.endWidth = lineThickness;

            lr.material = orbit.trajectoryMaterial;

            lineRenderers.Add(lr);

            previousPositionsList.Add(new Queue<Vector3>(numSegments));
        }
    }

    void Update()
    {
        Debug.Log("TrajectoryDrawer(): Update():");

        for (int i = 0; i < planetTrajectories.Count; i++)
        {
            PlanetTrajectory pt = planetTrajectories[i];
            Queue<Vector3> previousPositions = previousPositionsList[i];

            if (previousPositions.Count >= numSegments)
            {
                previousPositions.Dequeue();
            }

            previousPositions.Enqueue(pt.planet.transform.position);
            if(lineRenderers[i].enabled == false)
            {
                Debug.Log("Linerenderer[" + i + "] disabled ");
            }
            lineRenderers[i].positionCount = previousPositions.Count;
            lineRenderers[i].SetPositions(previousPositions.ToArray());
        }
    }

    public void ToggleTrajectory(int index, bool isOn)
    {

        if (index >= 0 && index < lineRenderers.Count)
        {
            lineRenderers[index].enabled = isOn;
        }
    }

    public void UIToggleAllTrajectories(bool isOn)
    {
        Debug.Log("trajectoryDrawer(): ToggleAllTrajectories = " + isOn);
        for (int index = 0; index < planetTrajectories.Count; index++)
        {
            lineRenderers[index].enabled = isOn;
        }
    }

    private void SetupToggle()
    {
        toggleAllTrajectories.onValueChanged.AddListener(UIToggleAllTrajectories);
    }

    private void OnDestroy()
    {
        toggleAllTrajectories.onValueChanged.RemoveListener(UIToggleAllTrajectories);
    }

}
