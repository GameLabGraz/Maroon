using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetController : MonoBehaviour
{
    public GameObject sun;
    public GameObject mercury;
    public GameObject venus;
    public GameObject earth;
    public GameObject mars;
    public GameObject jupiter;
    public GameObject saturn;
    public GameObject saturn_ring_1;
    public GameObject saturn_ring_2;
    public GameObject uranus;
    public ParticleSystem uranusParticleSystem;
    public GameObject neptune;
    public GameObject moon;

    public List<PlanetTrajectory> planetTrajectories;
    public float lineThickness = 0.5f;

    public Toggle toggleAllTrajectories;
    public Toggle toggleSunKinematic;


    private List<LineRenderer> lineRenderers;
    private List<Queue<Vector3>> previousPositionsList;

    [System.Serializable]
    public class PlanetTrajectory
    {
        public GameObject planet;
        public Material trajectoryMaterial;
        public int segments = 2000;
    }


    private static PlanetController _instance;
    public static PlanetController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PlanetController>();
            return _instance;
        }
    }


     /*
      *
      */
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
            lr.positionCount = orbit.segments;
            lr.startWidth = lineThickness;
            lr.endWidth = lineThickness;

            lr.material = orbit.trajectoryMaterial;

            lineRenderers.Add(lr);
            previousPositionsList.Add(new Queue<Vector3>(orbit.segments));

            lr.enabled = false;
        }
        toggleAllTrajectories.isOn = false;
    }


    /*
     *
     */
    void Update()
    {
        //Debug.Log("PlanetController(): Update():");
        for (int i = 0; i < planetTrajectories.Count; i++)
        {
            PlanetTrajectory pt = planetTrajectories[i];
            Queue<Vector3> previousPositions = previousPositionsList[i];

            if (previousPositions.Count >= pt.segments)
            {
                previousPositions.Dequeue();
            }

            previousPositions.Enqueue(pt.planet.transform.position);
            //if (lineRenderers[i].enabled == false)
            //    Debug.Log("Linerenderer[" + i + "] disabled ");

            lineRenderers[i].positionCount = previousPositions.Count;
            lineRenderers[i].SetPositions(previousPositions.ToArray());
        }
    }


    /*
     * toggles specific planets trajectory after button press
     */
    public void ToggleTrajectory(int index, bool isOn)
    {
        Debug.Log("PlanetController(): ToggleTrajectory [" + index + "] = " + isOn);
        if (index >= 0 && index < lineRenderers.Count)
        {
            lineRenderers[index].enabled = isOn;
        }
    }


    /*
     * toggles all trajectories after button press
     */
    public void UIToggleAllTrajectories(bool isOn)
    {
        Debug.Log("PlanetController(): ToggleAllTrajectories = " + isOn);
        for (int index = 0; index < planetTrajectories.Count; index++)
        {
            lineRenderers[index].enabled = isOn;
        }
    }


    /*
     * toggles the sun#s kinematic and recalcuates its initial velocity after button press
     */
    public void UIToggleSunKinematic(bool isKinematic)
    {
        Rigidbody sunRb = sun.GetComponent<Rigidbody>();
        sunRb.isKinematic = isKinematic;
        if (!isKinematic)
        {
            SolarSystem solarSystem = FindObjectOfType<SolarSystem>();
            solarSystem.RecalculateInitialVelocity(sun);
        }

    }


    /*
     * sets up listeners for toggle buttons
     */
    private void SetupToggle()
    {
        toggleAllTrajectories.onValueChanged.AddListener(UIToggleAllTrajectories);
        toggleSunKinematic.onValueChanged.AddListener(UIToggleSunKinematic);
    }


    /*
     * removes listeners for toggle buttons
     */
    private void OnDestroy()
    {
        toggleAllTrajectories.onValueChanged.RemoveListener(UIToggleAllTrajectories);
        toggleSunKinematic.onValueChanged.RemoveListener(UIToggleSunKinematic);
    }


   /*
    * hidesplanets and trajectories after radiobutton is pressed
    */
    #region hide planets
    public void UIToggle0(bool isOn)
    {
        //Debug.Log("current checkbox state: sun: " + !isOn);
        sun.GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(0, !isOn);
    }

    public void UIToggle1(bool isOn)
    {
        //Debug.Log("current checkbox state: mercury: " + !isOn);
        mercury.GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(1, !isOn);
    }

    public void UIToggle2(bool isOn)
    {
        //Debug.Log("current checkbox state: venus: " + !isOn);
        venus.GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(2, !isOn);
    }

    public void UIToggle3(bool isOn)
    {
        //Debug.Log("current checkbox state: earth: " + !isOn);
        earth.GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(3, !isOn);
    }

    public void UIToggle4(bool isOn)
    {
        //Debug.Log("current checkbox state: mars: " + !isOn);
        mars.GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(4, !isOn);
    }

    public void UIToggle5(bool isOn)
    {
        //Debug.Log("current checkbox state: jupiter: " + !isOn);
        jupiter.GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(5, !isOn);
    }

    public void UIToggle6(bool isOn)
    {
        //Debug.Log("current checkbox state: saturn: " + !isOn);
        saturn.GetComponent<Renderer>().enabled = !saturn.GetComponent<Renderer>().enabled;
        saturn_ring_1.GetComponent<Renderer>().enabled = !saturn_ring_1.GetComponent<Renderer>().enabled;
        saturn_ring_2.GetComponent<Renderer>().enabled = !saturn_ring_2.GetComponent<Renderer>().enabled;
        ToggleTrajectory(6, !isOn);
    }

    public void UIToggle7(bool isOn)
    {
        //Debug.Log("current checkbox state: uranus: " + !isOn);
        uranus.GetComponent<Renderer>().enabled = !isOn;
        uranusParticleSystem.gameObject.GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(7, !isOn);
    }
    public void UIToggle8(bool isOn)
    {
        //Debug.Log("current checkbox state: neptune: " + !isOn);
        neptune.GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(8, !isOn);
    }

    public void UIToggle9(bool isOn)
    {
        //Debug.Log("current checkbox state: moon: " + !isOn);
        moon.GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(9, isOn);
    }
    #endregion hide planets
}
