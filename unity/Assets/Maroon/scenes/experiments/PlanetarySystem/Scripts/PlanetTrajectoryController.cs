using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.PlanetarySystem
{
    public class PlanetTrajectoryController : MonoBehaviour
    {
        public PlanetaryController planetaryController;

        public List<PlanetTrajectory> planetTrajectories;
        [SerializeField] private float lineThickness = 0.4f;
        private List<LineRenderer> lineRenderers;
        private List<Queue<Vector3>> previousPositionsList;

        [System.Serializable]
        public class PlanetTrajectory
        {
            public GameObject planet;
            public Material trajectoryMaterial;
            public int segments = 2000;
        }
        //---------------------------------------------------------------------------------------


        /// <summary>
        /// general start setup
        /// </summary>
        private void Start()
        {
            InitializeLineRenderer();
            SetupLineRenderer();
        }


        /// <summary>
        /// updates he LineRenderer to draw the paths 
        /// dequeue the drawn trajectory paths to be deleted number of segments 
        /// </summary>
        private void Update()
        {
            DrawTrajectory(); // switch off and activate Particle System in planet prefab for optional trajectory approach
        }


        //create LineRender and Trajectories
        #region Trajectories
        /// <summary>
        /// initialize LineRenderer
        /// </summary>
        public void InitializeLineRenderer()
        {
            lineRenderers = new List<LineRenderer>();
            previousPositionsList = new List<Queue<Vector3>>();
        }


        /// <summary>
        /// create a child Trajectory  GameObject for each planet
        /// adds and sets up LineRenderer
        /// add LineRenderer to LineRenderers list
        /// creates a position queue sized number of segments and adds it to  previousPositionsList
        /// initially hide the Trajecories
        /// update Toggle
        /// </summary>
        public void SetupLineRenderer()
        {
            foreach (PlanetTrajectory orbit in planetTrajectories)
            {
                GameObject trajectory = new GameObject(orbit.planet.name + "Trajectory");
                trajectory.transform.SetParent(transform);

                LineRenderer lr = trajectory.AddComponent<LineRenderer>();
                lr.positionCount = orbit.segments;
                lr.startWidth = lineThickness;
                lr.endWidth = lineThickness;

                lr.material = orbit.trajectoryMaterial;

                lineRenderers.Add(lr);
                previousPositionsList.Add(new Queue<Vector3>(orbit.segments));

                lr.enabled = false;
            }
        }


        /// <summary>
        /// loops through planetTrajectories and accesses the queue of previous positions
        /// creating/drawing a trail trajectory
        /// dequeues oldest position when segments reached
        /// </summary>
        public void DrawTrajectory()
        {
            for (int i = 0; i < planetTrajectories.Count; i++)
            {
                PlanetTrajectory pt = planetTrajectories[i];
                Queue<Vector3> previousPositions = previousPositionsList[i];

                if (previousPositions.Count >= pt.segments)
                {
                    previousPositions.Dequeue();
                }

                previousPositions.Enqueue(pt.planet.transform.position);
                lineRenderers[i].positionCount = previousPositions.Count;
                lineRenderers[i].SetPositions(previousPositions.ToArray());
            }
        }


        /// <summary>
        /// clear trajectory path by resetting previousPosition queue and resetting LineRenderer
        /// </summary>
        public void ClearTrajectories()
        {
            for (int i = 0; i < previousPositionsList.Count; i++)
            {
                Queue<Vector3> previousPositions = previousPositionsList[i];
                previousPositions.Clear();

                LineRenderer lr = lineRenderers[i];
                lr.positionCount = 0;
            }

            // clear particle sytsem
            foreach (GameObject planet in planetaryController.planets)
            {
                ParticleSystem ps = planet.GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    ps.Clear();
                }
            }
        }
        #endregion Trajectories

        //create ToggleTrajectories
        #region ToggleTrajectories
        /// <summary>
        /// toggles specific planets trajectory after button press
        /// </summary>
        /// <param name="index"></param>
        /// <param name="isOn"></param>
        public void ToggleTrajectory(int index, bool isOn)
        {
            //Debug.Log("PlanetController(): ToggleTrajectory [" + index + "] = " + isOn);
            if (lineRenderers == null || planetaryController.planets == null)
            {
                //Debug.Log("PlanetController: ToggleTrajectory(): lineRenderers or planets is null");
                return;
            }
            if (index >= lineRenderers.Count || index < 0)
            {
                //Debug.Log("PlanetController: ToggleTrajectory(): Invalid index: " + index);
                return;
            }
            LineRenderer lr = lineRenderers[index];
            if (lr == null || planetaryController.planets[index] == null)
            {
                Debug.Log("PlanetController: ToggleTrajectory(): LineRenderer or planets[index] at index " + index + " is null");
                return;
            }
            lr.enabled = isOn;
        }


        /// <summary>
        /// toggles all trajectories after button press
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleAllTrajectories(bool isOn)
        {
            //Debug.Log("PlanetController(): ToggleAllTrajectories = " + isOn);
            for (int index = 0; index < planetTrajectories.Count; index++)
            {
                lineRenderers[index].enabled = isOn;
            }
        }
        #endregion ToggleTrajectories
    }
}