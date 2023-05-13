using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Maroon.UI;            //Dialogue Manager
using GEAR.Localization;    //MLG

public class PlanetaryController : MonoBehaviour
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

    public Material skyboxStars;
    public Material skyboxDiverseSpace;
    public Material skyboxBlack;

    public Light sunLight;
    public ParticleSystem solarFlares;

    public List<PlanetTrajectory> planetTrajectories;
    public float lineThickness = 0.5f;

    public Toggle toggleAllTrajectories;
    public Toggle toggleSunKinematic;
    public Toggle toggleSunLight;
    public Toggle toggleSolarFlares;
    public Toggle toggleSGRotation;
    public Toggle toggleSGOrientationGizmo;
    public Toggle toggleARotation;
    public Toggle toggleAOrientationGizmo;

    public GameObject sortingGamePlanetPlaceholderSlots;
    private GameObject[] sortingPlanets;
    private readonly List<int> sortingGameAvailableSlotPositions = new List<int>();

    public int sortedPlanetCount = 0;

    private List<LineRenderer> lineRenderers;
    private List<Queue<Vector3>> previousPositionsList;

    private DialogueManager _dialogueManager;


    [System.Serializable]
    public class PlanetTrajectory
    {
        public GameObject planet;
        public Material trajectoryMaterial;
        public int segments = 2000;
    }

    /*
     * Instance of PlanetaryController
     */
    #region PlanetaryControllerInstance
    private static PlanetaryController _instance;
    public static PlanetaryController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PlanetaryController>();
            return _instance;
        }
    }
    #endregion PlanetaryControllerInstance


    /*
     * initialize LineRenderer before toggle
     */
    private void Awake()
    {
        DisplayMessageByKey("EnterPlanetarySystem");

        //set skybox
        RenderSettings.skybox = skyboxBlack;

        InitializeLineRenderer();
    }


    /*
     *
     */
    void Start()
    {
        //Debug.Log("PlanetaryController: Start(): ");
        SetupToggle();
        SetupLineRenderer();
    }




    /*
     * updates he LineRenderer to draw the paths 
     * Dequeue the drawn trajectory paths to be deleted number of segments 
     * toggles the sunlight on key [L]
     */
    void Update()
    {
        //Debug.Log("PlanetaryController: Update(): ");
        ChangeSkybox();
        TurnOnSunlightOnInput();

        DrawTrajectory();
    }

    /*
     * create LineRender and Trajectories
     */
    #region Trajectories
    /*
     * initialize LineRenderer
     */
    void InitializeLineRenderer()
    {
        lineRenderers = new List<LineRenderer>();
        previousPositionsList = new List<Queue<Vector3>>();
    }


    /*
     * create a child Trajectory  GameObject for each planet
     * adds and sets up LineRenderer
     * add LineRenderer to LineRenderers list
     * creates a position queue sized number of segments and adds it to  previousPositionsList
     * initially hide the Trajecories
     * update Toggle
     */
    void SetupLineRenderer()
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
        toggleAllTrajectories.isOn = false;
    }


    /*
     * loops through planetTrajectories and accesses the queue od previous positions
     * creating/drawing a trail trajectorie
     * dequeues oldest position when segments reched
     */
    void DrawTrajectory()
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
            //if (lineRenderers[i].enabled == false)
            //    Debug.Log("Linerenderer[" + i + "] disabled ");

            lineRenderers[i].positionCount = previousPositions.Count;
            lineRenderers[i].SetPositions(previousPositions.ToArray());
        }
    }
    #endregion Trajectories


    /*
     * toggle sunlight after key [L] is pressed
     */
    #region Sunlight
    void TurnOnSunlightOnInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            //Debug.Log("L key pressed.");
            sunLight.gameObject.SetActive(!sunLight.gameObject.activeSelf);
            // Sync the toggle button state with sunLight's state
            toggleSunLight.isOn = sunLight.gameObject.activeSelf;
        }
    }
    #endregion Sunlight



    /*
     * changes the skybox on key 3,4,5
     */
    #region Skybox
    void ChangeSkybox()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RenderSettings.skybox = skyboxBlack;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            RenderSettings.skybox = skyboxDiverseSpace;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            RenderSettings.skybox = skyboxStars;
        }
    }
    #endregion Skybox


    /*
     * handles the SortingGame
     */
    #region SortingGameSpawner
    /*
     * Initialize the availablePositions list with all possible sorting positions
     * called from StartSortingGame
     */
    public void InitializeAvailableSortingGameSlotPositions()
    {
        //Debug.Log("PlanetaryController: InitializeAvailableSortingGameSlotPositions():");
        int boxSize = 24;
        for (int i = 0; i < boxSize; i++)
        {
            sortingGameAvailableSlotPositions.Add(i);
        }

        sortingPlanets = GameObject.FindGameObjectsWithTag("SortingGamePlanet");
        if (sortingPlanets.Length < 1)
            Debug.Log("PlanetaryController: InitializeAvailableSortingGameSlotPositions(): no sortingPlanets found");

        SpawnSortingPlanets();
    }


    /*
     * Spawn the SortingPlanets on a random sortingGameAvailableSlotPositions
     */
    void SpawnSortingPlanets()
    {
        //Debug.Log("PlanetaryController: SpawnSortingPlanets():");
        for (int i = 0; i < sortingPlanets.Length; i++)
        {
            // Randomly select a position index from the sortingGameAvailableSlotPositions list
            int randomIndex = Random.Range(0, sortingGameAvailableSlotPositions.Count);

            // Get the selected position index and remove it from the list to avoid duplicate positions
            int positionIndex = sortingGameAvailableSlotPositions[randomIndex];
            sortingGameAvailableSlotPositions.RemoveAt(randomIndex);

            // Get the child object of the placeholder GameObject at the selected position index
            Transform spawnPosition = sortingGamePlanetPlaceholderSlots.transform.GetChild(positionIndex);
            sortingPlanets[i].transform.SetParent(spawnPosition);
            sortingPlanets[i].transform.position = spawnPosition.position;
        }
    }
    #endregion SortingGameSpawner


    /*
     * displays Helpi masseges by key
     */
    #region Helpi
    /*
     * displays Halpi masseges by key
     */
    public void DisplayMessageByKey(string key)
    {
        if (_dialogueManager == null)
            _dialogueManager = FindObjectOfType<DialogueManager>();

        if (_dialogueManager == null)
            return;

        var message = LanguageManager.Instance.GetString(key);

        _dialogueManager.ShowMessage(message);
    }
    #endregion Helpi

    /*
     * sets up and handles UI toggle buttons
     */
    #region UIToggleButtons
    /*
     * toggles the rotation of the minigame sortable planets after button press
     */
    public void UIToggleSGRotation(bool isOn)
    {
        //Debug.Log("PlanetController(): UIToggleSGRotation = " + isOn);

        GameObject[] sortablePlanets = GameObject.FindGameObjectsWithTag("SortingGamePlanet");
        if (sortablePlanets.Length <= 0)
        {
            Debug.Log("No sortablePlanet found:  " + sortablePlanets.Length);
            return;
        }

        //Debug.Log("PlanetController(): UIToggleSGRotation(): sortablePlanets.Length = " + sortablePlanets.Length);
        foreach (GameObject planet in sortablePlanets)
        {
            //Debug.Log("PlanetController(): UIToggleSGRotation(): planet = " + planet);
            PlanetRotation rotationScript = planet.GetComponent<PlanetRotation>();
            if (rotationScript != null)
            {
                rotationScript.enabled = isOn;
            }
        }
    }


    /*
     * toggles the rotation of the animation planets button press
     */
    public void UIToggleARotation(bool isOn)
    {
        //Debug.Log("PlanetController(): UIToggleARotation = " + isOn);

        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        if (planets.Length <= 0)
        {
            Debug.Log("No sortablePlanet found:  " + planets.Length);
            return;
        }

        //Debug.Log("PlanetController(): UIToggleARotation(): planets.Length = " + sortablePlanets.Length);
        foreach (GameObject planet in planets)
        {
            //Debug.Log("PlanetController(): UIToggleARotation(): planet = " + planet);
            PlanetRotation rotationScript = planet.GetComponent<PlanetRotation>();
            if (rotationScript != null)
            {
                rotationScript.enabled = isOn;
            }
        }
    }


    /*
     * toggles the rotation of the minigame sortable planets button press
     */
    public void UIToggleSGOrientation(bool isOn)
    {
        //Debug.Log("PlanetController(): UIToggleSGOrientation = " + isOn);
        GameObject[] sortablePlanets = GameObject.FindGameObjectsWithTag("SortingGamePlanet");
        if (sortablePlanets.Length <= 0)
        {
            Debug.Log("No sortablePlanet found:  " + sortablePlanets.Length);
            return;
        }

        //Debug.Log("PlanetController(): UIToggleSGOrientation(): sortablePlanets.Length = " + sortablePlanets.Length);
        foreach (GameObject planet in sortablePlanets)
        {
            //Debug.Log("PlanetController(): UIToggleSGOrientation(): planet = " + planet);
            GameObject orientationGizmo = planet.transform.Find("orientation gizmo").gameObject;
            if (orientationGizmo != null)
            {
                orientationGizmo.SetActive(isOn);
            }
            else
            {
                Debug.Log("PlanetController(): UIToggleSGOrientation(): No orientation gizmo found");
            }
        }
    }


   /*
    * toggles the rotation of the animation planets button press
    */
    public void UIToggleAOrientation(bool isOn)
    {
        //Debug.Log("PlanetController(): UIToggleAOrientation = " + isOn);
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        if (planets.Length <= 0)
        {
            Debug.Log("No sortablePlanet found:  " + planets.Length);
            return;
        }

        //Debug.Log("PlanetController(): UIToggleAOrientation(): sortablePlanets.Length = " + sortablePlanets.Length);
        foreach (GameObject planet in planets)
        {
            //Debug.Log("PlanetController(): UIToggleAOrientation(): planet = " + planet);
            GameObject orientationGizmo = planet.transform.Find("orientation gizmo").gameObject;
            if (orientationGizmo != null)
            {
                orientationGizmo.SetActive(isOn);
            }
            else
            {
                Debug.Log("PlanetController(): UIToggleAOrientation(): No orientation gizmo found");
            }
        }
    }


    /*
     * toggles specific planets trajectory after button press
     */
    public void ToggleTrajectory(int index, bool isOn)
    {
        //Debug.Log("PlanetController(): ToggleTrajectory [" + index + "] = " + isOn);
        if (lineRenderers == null)
        {
            //Debug.Log("PlanetController: ToggleTrajectory(): lineRenderers is null");
            return;
        }
        if (index >= lineRenderers.Count || index < 0)
        {
            Debug.Log("PlanetController: ToggleTrajectory(): Invalid index: " + index);
            return;
        }
        LineRenderer lr = lineRenderers[index];
        if (lr == null)
        {
            Debug.Log("PlanetController: ToggleTrajectory(): LineRenderer at index " + index + " is null");
            return;
        }
        lr.enabled = isOn;
    }


    /*
     * toggles all trajectories after button press
     */
    public void UIToggleAllTrajectories(bool isOn)
    {
        //Debug.Log("PlanetController(): ToggleAllTrajectories = " + isOn);
        for (int index = 0; index < planetTrajectories.Count; index++)
        {
            lineRenderers[index].enabled = isOn;
        }
    }


    /*
     * toggles the sun's kinematic and recalcuates its initial velocity after button press
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
    * toggles the sun's solarflares in the sorting game after button press
    */
    public void UIToggleSolarFlares(bool isOn)
    {
        //Debug.Log("PlanetController(): UIToggleSolarFlares = " + isOn);
        solarFlares.gameObject.SetActive(isOn);
        if (isOn)
        {
            solarFlares.Play();
        }
        else
        {
            solarFlares.Stop();
        }
    }


    /*
     * toggles the sun's pointlight in the sorting game after button press
     */
    public void UIToggleSunLight(bool isOn)
    {
        //Debug.Log("PlanetController(): UIToggleSunLight = " + !isOn);
        sunLight.gameObject.SetActive(isOn);
    }


    /*
     * sets up listeners for toggle buttons
     */
    private void SetupToggle()
    {
        toggleAllTrajectories.onValueChanged.AddListener(UIToggleAllTrajectories);
        toggleSunKinematic.onValueChanged.AddListener(UIToggleSunKinematic);
        toggleSunLight.onValueChanged.AddListener(UIToggleSunLight);
        toggleSolarFlares.onValueChanged.AddListener(UIToggleSolarFlares);
        toggleSGRotation.onValueChanged.AddListener(UIToggleSGRotation);
        toggleARotation.onValueChanged.AddListener(UIToggleARotation);
        toggleSGOrientationGizmo.onValueChanged.AddListener(UIToggleSGOrientation);
        toggleAOrientationGizmo.onValueChanged.AddListener(UIToggleAOrientation);
    }


    /*
     * removes listeners for toggle buttons
     */
    private void OnDestroy()
    {
        toggleAllTrajectories.onValueChanged.RemoveListener(UIToggleAllTrajectories);
        toggleSunKinematic.onValueChanged.RemoveListener(UIToggleSunKinematic);
        toggleSunLight.onValueChanged.RemoveListener(UIToggleSunLight);
        toggleSolarFlares.onValueChanged.RemoveListener(UIToggleSolarFlares);
        toggleSGRotation.onValueChanged.RemoveListener(UIToggleSGRotation);
        toggleARotation.onValueChanged.RemoveListener(UIToggleARotation);
        toggleSGOrientationGizmo.onValueChanged.RemoveListener(UIToggleSGOrientation);
        toggleAOrientationGizmo.onValueChanged.RemoveListener(UIToggleAOrientation);
    }
    #endregion UIToggleButtons

    /*
     * hidesplanets and trajectories after radiobutton is pressed
     */
    #region hide planets
    public void UIToggle0(bool isOn)
    {
        //Debug.Log("current checkbox state: mercury: " + !isOn);
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
