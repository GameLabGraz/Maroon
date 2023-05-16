using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Maroon.UI;            //Dialogue Manager
using GEAR.Localization;    //MLG
using System.Collections;

public class PlanetaryController : MonoBehaviour, IResetObject
{
    #region HidePlanets
    public GameObject sun;
    public GameObject saturn_ring_1;
    public GameObject saturn_ring_2;
    public ParticleSystem uranusParticleSystem;
    #endregion HidePlanets

    #region StartScreenScenes
    //start Animation                       //want it:
    public GameObject MainCamera;           //off
    public GameObject FormulaUI;            //off

    //start SortingGame                     //want it:
    public GameObject Environment;          //off
    public GameObject SolarSystemCamera;    //on
    public GameObject Planets;              //off//on
    public GameObject SortingMinigame;      //off
    public GameObject Interactibles;        //off
    public GameObject AnimationUI;          //on
    public GameObject PlanetInfoUI;         //off
    private FlyCamera flyCameraScript;      //on
    #endregion StartScreenScenes

    #region SolarSystem
    GameObject[] planets;
    public float G;
    public bool isSunKinematic = true;

    [System.Serializable]
    public class PlanetData : MonoBehaviour
    {
        public float semiMajorAxis;
        public float initialVelocity;
    }
    #endregion SolarSystem

    #region UIToggleButtons
    public Toggle toggleAllTrajectories;
    public Toggle toggleSunKinematic;
    public Toggle toggleSunLight;
    public Toggle toggleSolarFlares;
    public Toggle toggleSGRotation;
    public Toggle toggleSGOrientationGizmo;
    public Toggle toggleARotation;
    public Toggle toggleAOrientationGizmo;
    #endregion UIToggleButtons

    #region SortingGameSpawner
    public GameObject sortingGamePlanetPlaceholderSlots;
    private GameObject[] sortingPlanets;
    private readonly List<int> sortingGameAvailableSlotPositions = new List<int>();

    public int sortedPlanetCount = 0;
    #endregion SortingGameSpawner

    #region ResetAnimation
    private List<Vector3> initialPlanetPositions = new List<Vector3>();
    public float resetG = 100;
    public float resetDelay = 1f;
    public float initialTime = 2f;





    #endregion ResetAnimation

    #region Trajectories
    public List<PlanetTrajectory> planetTrajectories;
    public float lineThickness = 0.4f;
    private List<LineRenderer> lineRenderers;
    private List<Queue<Vector3>> previousPositionsList;

    [System.Serializable]
    public class PlanetTrajectory
    {
        public GameObject planet;
        public Material trajectoryMaterial;
        public int segments = 2000;
    }
    #endregion Trajectories

    #region Helpi
    private DialogueManager _dialogueManager;
    #endregion Helpi

    #region KeyInput
    public Material skyboxStars;
    public Material skyboxDiverseSpace;
    public Material skyboxBlack;
    public Light sunLight;
    public ParticleSystem solarFlares;
    #endregion KeyInput

    //---------------------------------------------------------------------------------------
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
        //Debug.Log("PlanetaryController: Awake(): ");
        DisplayMessageByKey("EnterPlanetarySystem");
        
        SortingMinigame.SetActive(false);
        PlanetInfoUI.SetActive(false);
        sun.SetActive(true);
        //Debug.Log("PlanetaryController: Awake(): sun.SetActive= " + sun.activeSelf);

        InitializePlanets();
        InitializeLineRenderer();
    }


    /*
     *
     */
    void Start()
    {
        //Debug.Log("PlanetaryController: Start(): ");

        SetupToggle();
        GetFlyCameraScript();
        SetupLineRenderer();
        //turn off planets before Animation
        Planets.SetActive(false);
    }


    /*
     * updates he LineRenderer to draw the paths 
     * Dequeue the drawn trajectory paths to be deleted number of segments 
     * toggles the sunlight on key [L]
     */
    void Update()
    {
        //Debug.Log("PlanetaryController: Update(): ");
        HandleKeyInput();
        DrawTrajectory();
    }


    /*
     * Frame-rate independent for physics calculations;
     * applied each fixed frame
     */
    private void FixedUpdate()
    {
        Gravity();
    }
    //---------------------------------------------------------------------------------------


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
     * HandlesKeyInput during Update
     */
    #region KeyInput
    /*
     * HandlesKeyInput during Update
     * switch to StartSortingGameOnInput on key [1]
     * switch to StartAnimationOnInput   on key [2]
     * change skybox                     on key [3],[4],[5]
     * toggle sunlight                   on key [L]
     */
    void HandleKeyInput()
    {
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                switch (keyCode)
                {
                    case KeyCode.Alpha1:
                        StartSortingGameOnInput();
                        break;

                    case KeyCode.Alpha2:
                        StartAnimationOnInput();
                        break;

                    case KeyCode.L:
                        sunLight.gameObject.SetActive(!sunLight.gameObject.activeSelf);
                        // Sync the toggle button state with sunLight's state
                        toggleSunLight.isOn = sunLight.gameObject.activeSelf;
                        break;

                    case KeyCode.Alpha3:
                        RenderSettings.skybox = skyboxBlack;
                        break;

                    case KeyCode.Alpha4:
                        RenderSettings.skybox = skyboxDiverseSpace;
                        break;

                    case KeyCode.Alpha5:
                        RenderSettings.skybox = skyboxStars;
                        break;

                    default:
                        break;
                }
            }
        }
    }
    #endregion KeyInput


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
     * SolarSystems handles Newtons Gravity, Initial Velocity and the planets
    */
    #region SolarSystem
    /*
     * find planets with tag and initializes them
     */
    void InitializePlanets()
    {
        //Debug.Log("PlanetaryController: InitializePlanets():");
        planets = GameObject.FindGameObjectsWithTag("Planet");
        if (planets.Length <= 0)
        {
            //Should not happen
            Debug.Log("No Planets Found:  " + planets.Length);
        }
        else
        {
            foreach (var planet in planets)
            {
                initialPlanetPositions.Add(planet.transform.position);
            }
        }
    }


    void Gravity()
    {
        foreach (GameObject a in planets)
        {
            foreach (GameObject b in planets)
            {
                // Object can't orbit itself
                if (!a.Equals(b))
                {
                    Rigidbody aRigidbody = a.GetComponent<Rigidbody>();
                    Rigidbody bRigidbody = b.GetComponent<Rigidbody>();

                    float m1 = aRigidbody.mass;
                    float m2 = bRigidbody.mass;

                    float r = Vector3.Distance(a.transform.position, b.transform.position);

                    // Newton's law of universal gravitation
                    // F = G * ((m1 * m2) / (r^2))
                    aRigidbody.AddForce((b.transform.position - a.transform.position).normalized *
                        (G * (m1 * m2) / (r * r)));
                }
            }
        }
    }


    /*
     *
     */
    public void InitialVelocity()
    {
        foreach (GameObject a in planets)
        {
            foreach (GameObject b in planets)
            {
                if (!a.Equals(b))
                {
                    // m2 = mass of central object
                    float m2 = b.GetComponent<Rigidbody>().mass;
                    // r = distance between the objects and a is the length of the semi-mayor axis
                    float r = Vector3.Distance(a.transform.position, b.transform.position);

                    a.transform.LookAt(b.transform);

                    // circular orbit instant velocity: v0 = sqrt((G * m2) / r)
                    a.GetComponent<Rigidbody>().velocity += a.transform.right * Mathf.Sqrt((G * m2) / r);
                }
            }
        }
    }


    /*
     * recalculates the InitialVelocity after toggle the sun's isKinematic in the animation
     */
    public void RecalculateInitialVelocity(GameObject a)
    {
        foreach (GameObject b in planets)
        {
            if (!a.Equals(b))
            {
                float m2 = b.GetComponent<Rigidbody>().mass;
                float r = Vector3.Distance(a.transform.position, b.transform.position);

                a.transform.LookAt(b.transform);

                a.GetComponent<Rigidbody>().velocity += a.transform.right * Mathf.Sqrt((G * m2) / r);
            }
        }
    }


    /*
     * unused: semi mayor axis missing
     * 
     */
    void InitialVelocityEliptical()
    {
        foreach (GameObject a in planets)
        {
            foreach (GameObject b in planets)
            {
                if (!a.Equals(b))
                {
                    // m2 = mass of central object
                    float m2 = b.GetComponent<Rigidbody>().mass;
                    // r = distance between the objects
                    float r = Vector3.Distance(a.transform.position, b.transform.position);
                    // a = length of the semi-mayor axis
                    float a_axis = a.GetComponent<PlanetData>().semiMajorAxis;
                    a.transform.LookAt(b.transform);

                    // eliptic orbit instant velocity: v0 = G * m2 * (2 / r - 1 / a)
                    a.GetComponent<Rigidbody>().velocity += a.transform.right * (G * m2 * (2 / r - 1 / a_axis));
                    a.GetComponent<Rigidbody>().velocity += a.transform.right * (G * m2 * (2 / r - 1 / a_axis));
                }
            }
        }
    }
    #endregion SolarSystem
    

    /*
     * Animation
     */
    #region Animation
    /*
     * set skybox
     */
    void SetSkybox()
    {
        RenderSettings.skybox = skyboxStars;
    }

    /*
     * get flycamere script to dis/enable later
     */
    void GetFlyCameraScript()
    {
        flyCameraScript = GetComponent<FlyCamera>();
        if (flyCameraScript == null)
        {
            Debug.Log("PlanetaryController: GetFlyCameraScript(): script not found");
        }
    }
    #endregion Animation


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
            RecalculateInitialVelocity(sun);
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
        //Sun
        //Debug.Log("Mercury checkbox: " + !isOn);
        sun.GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(0, !isOn);
    }

    public void UIToggle1(bool isOn)
    {
        //Mercury
        //Debug.Log("Mercury checkbox: " + !isOn);
        planets[1].GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(1, !isOn);
    }

    public void UIToggle2(bool isOn)
    {
        //Venus
        //Debug.Log("Venus checkbox: " + !isOn);
        planets[2].GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(2, !isOn);
    }

    public void UIToggle3(bool isOn)
    {
        //Earth
        //Debug.Log("Earth checkbox: " + !isOn);
        planets[3].GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(3, !isOn);
    }

    public void UIToggle4(bool isOn)
    {
        //Mars
        //Debug.Log("Mars checkbox: " + !isOn);
        planets[4].GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(4, !isOn);
    }

    public void UIToggle5(bool isOn)
    {
        //Jupiter
        //Debug.Log("Jupiter checkbox: " + !isOn);
        planets[5].GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(5, !isOn);
    }

    public void UIToggle6(bool isOn)
    {
        //Saturn
        //Debug.Log("Saturn checkbox: " + !isOn);
        planets[6].GetComponent<Renderer>().enabled = !planets[6].GetComponent<Renderer>().enabled;
        saturn_ring_1.GetComponent<Renderer>().enabled = !saturn_ring_1.GetComponent<Renderer>().enabled;
        saturn_ring_2.GetComponent<Renderer>().enabled = !saturn_ring_2.GetComponent<Renderer>().enabled;
        ToggleTrajectory(6, !isOn);
    }

    public void UIToggle7(bool isOn)
    {
        //Uranus
        //Debug.Log("Uranus checkbox: " + !isOn);
        planets[7].GetComponent<Renderer>().enabled = !isOn;
        uranusParticleSystem.gameObject.GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(7, !isOn);
    }
    public void UIToggle8(bool isOn)
    {
        //Neptune
        //Debug.Log("Neptune checkbox: " + !isOn);
        planets[8].GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(8, !isOn);
    }

    public void UIToggle9(bool isOn)
    {
        //Moon
        //Debug.Log("Moon checkbox: " + !isOn);
        planets[9].GetComponent<Renderer>().enabled = !isOn;
        ToggleTrajectory(9, isOn);
    }
    #endregion hide planets


    /*
     * StartAnimationOnInput and de/activates gameobjects
     * StartAnimationOnInput and de/activates gameobjects
     */
    #region StartScreenScenes
    /*
     * StartSortingGameOnInput and activates gameobjects
     */
    public void StartSortingGameOnInput()
    {
        //Debug.Log("PlanetaryController: StartAnimationOnInput(): ");
        LeaveAnimation();

        SortingMinigame.SetActive(true);
        PlanetInfoUI.SetActive(true);
        FormulaUI.SetActive(false);

        InitializeAvailableSortingGameSlotPositions();
        DisplayMessageByKey("EnterSortingGame");
    }


    /*
     * LeaveSortingGame and deactivates gameobjects
     */
    public void LeaveSortingGame()
    {
        //Debug.Log("PlanetaryController: LeaveSortingGame(): ");
        SortingMinigame.SetActive(false);
        PlanetInfoUI.SetActive(false); 
    }


    /*
     * StartAnimationOnInput and activates gameobjects
     */
    public void StartAnimationOnInput()
    {
        //Debug.Log("PlanetaryController: StartAnimationOnInput(): ");
        LeaveSortingGame();
        SetSkybox();

        Environment.SetActive(false);
        MainCamera.SetActive(false);
        SolarSystemCamera.SetActive(true);
        Planets.SetActive(true);
        Interactibles.SetActive(false);
        AnimationUI.SetActive(true);
        FormulaUI.SetActive(false);
        flyCameraScript.enabled = true;

        InitialVelocity();
        //InitialVelocityEliptical();

        DisplayMessageByKey("EnterAnimation");
        LeaveSortingGame();
    }


    /*
     * LeaveAnimation and deactivates gameobjects
     */
    public void LeaveAnimation()
    {
        Environment.SetActive(true);
        MainCamera.SetActive(true);
        SolarSystemCamera.SetActive(false);
        Planets.SetActive(false);
        Interactibles.SetActive(true);
        AnimationUI.SetActive(false);
        flyCameraScript.enabled = false;
    }
    #endregion StartScreenScenes


    /*
     * handles reset functionality
     * homeReset / reset
     */
    #region ResetBar
    /*
     * ResetSortingGame planet positions
     */
    public void ResetSortingGame()
    {
        sortingGameAvailableSlotPositions.Clear();
        InitializeAvailableSortingGameSlotPositions();
    }


    /*
     * reset
     */

    public void ResetObject()
    {
        Debug.Log("PlanetaryController: ResetObject(): button pressed");
    
        //reset Animation

        //resetG
        G = resetG;

        //reset time
        Time.timeScale = initialTime; 

        //reset to initialPlanetPosition
        for (int i = 0; i < planets.Length; i++)
        {
            planets[i].transform.position = initialPlanetPositions[i];
            //set is kinematic to stop all physics
            Rigidbody rb = planets[i].GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        //reset trajectory pathy by resetting previousPosition queue and resetting LineRenderer
        for (int i = 0; i < previousPositionsList.Count; i++)
        {
            Queue<Vector3> previousPositions = previousPositionsList[i];
            previousPositions.Clear();

            LineRenderer lr = lineRenderers[i];
            lr.positionCount = 0;
        }

        //delay
        StartCoroutine(RestartAnimationDelay());

    }


    /*
     * 
     */
    IEnumerator RestartAnimationDelay()
    {
        yield return new WaitForSeconds(resetDelay);

        //reapply initial velocities and start planet movement after delay
        //start from index 1; check for sun is kinematic
        for (int i = 1; i < planets.Length; i++) 
        {
            Rigidbody rb = planets[i].GetComponent<Rigidbody>();
            rb.isKinematic = false;  
        }
        UIToggleSunKinematic(true);
        InitialVelocity();

    }



    /*
     * ResetHome button deactivates Animation and SortingGame Gameobjects and cameras
     * activates FormulaUI
     */
    public void ResetHome()
    {
        //Debug.Log("PlanetaryController: ResetHome(): button pressed");
        LeaveSortingGame();
        LeaveAnimation();

        FormulaUI.SetActive(true);
    }
    #endregion ResetBar
}
