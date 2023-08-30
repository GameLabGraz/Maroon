using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Maroon.UI;            //Dialogue Manager
using GEAR.Localization;    //MLG
using System.Collections;



namespace Maroon.Experiments.PlanetarySystem
{
    public class PlanetaryController : MonoBehaviour, IResetObject
    {
        #region Cameras
        [SerializeField] private GameObject MainCamera;                 //off
        [SerializeField] private GameObject SolarSystemAnimationCamera; //on
        [SerializeField] private GameObject SortingGameCamera;          //off
        [SerializeField] private GameObject TelescopeCamera;            //off
        public Camera AnimationCamera;

        private float initialAnimationCameraFov;
        private Vector3 initialAnimationCameraPosition;
        private Quaternion initialAnimationCameraRotation;

        private Vector3 initialMainCameraPosition;
        private Quaternion initialMainCameraRotation;
        private float initialMainCameraFOV;
        #endregion Cameras

        #region SolarSystem
        private float G;
        private GameObject sun;
        //[SerializeField] private bool isSunKinematic = true;

        [System.Serializable]
        public class PlanetData : MonoBehaviour
        {
            public float semiMajorAxis;
            [SerializeField] private float initialVelocity;
        }
        public GameObject[] planets;
        #endregion SolarSystem

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

        #region StartScreenScenes
        //want it:
        [SerializeField] private GameObject FormulaUI;              //off
        [SerializeField] private GameObject Environment;            //off
        [SerializeField] private GameObject Animation;                //off//on
        [SerializeField] private GameObject SortingMinigame;        //off
        [SerializeField] private GameObject Interactibles;          //off
        [SerializeField] private GameObject AnimationUI;            //on
        [SerializeField] private GameObject SortingGamePlanetInfoUI;//off
        [SerializeField] private FlyCamera flyCameraScript;         //on
        #endregion StartScreenScenes

        #region HidePlanets
        public GameObject saturn_ring_1;
        public GameObject saturn_ring_2;
        public Light sunHalo;
        #endregion HidePlanets

        #region SortingGameSpawner
        public int sortedPlanetCount = 0;
        [SerializeField] private GameObject sortingGamePlanetPlaceholderSlots;
        GameObject[] sortingPlanets;
        private readonly List<int> sortingGameAvailableSlotPositions = new List<int>();
        #endregion SortingGameSpawner

        #region ResetAnimation
        private readonly List<Vector3> initialPlanetPositions = new List<Vector3>();
        private List<Vector3> initialPlanetRotations = new List<Vector3>();

        public float resetAnimationDelay = 0.3f;
        public float gravitationalConstantG = 6.674f;
        public float timeSpeed = 1f;
        #endregion ResetAnimation

        #region Helpi
        private DialogueManager _dialogueManager;
        public GameObject HelpiDialogueUI;
        #endregion Helpi

        #region KeyInput
        [SerializeField] private Material skyboxStars;
        [SerializeField] private Material skyboxDiverseSpace;
        [SerializeField] private Material skyboxBlack;
        [SerializeField] private Light sunLight;
        [SerializeField] private ParticleSystem solarFlares;
        #endregion KeyInput

        #region Slider
        [SerializeField] private Slider sliderG;
        [SerializeField] private Slider sliderTimeSpeed;
        [SerializeField] private Slider sliderAnimationCameraFov;
        #endregion Slider

        #region UIToggleButtons
        [SerializeField] private Toggle toggleAllTrajectories;
        [SerializeField] private Toggle toggleARotation;
        [SerializeField] private Toggle toggleSunKinematic;
        [SerializeField] private Toggle toggleAOrientationGizmo;

        [SerializeField] private Toggle toggleSGRotation;
        [SerializeField] private Toggle toggleSGOrientationGizmo;
        [SerializeField] private Toggle toggleSunLight;
        [SerializeField] private Toggle toggleSolarFlares;

        [SerializeField] private Toggle[] planetToggles;
        #endregion UIToggleButtons


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
            DisplayMessageByKey("EnterPlanetarySystem");

            SortingMinigame.SetActive(false);
            SortingGamePlanetInfoUI.SetActive(false);
            sun = planets[0];
            sun.SetActive(true);

            InitializeAndScalePlanets();
            InitializeLineRenderer();
        }


        /*
         * general start setup
         */
        private void Start()
        {
            SetupToggle();
            SetupSliders();
            StoreInitialCameras();
            GetFlyCameraScript();
            SetupLineRenderer();
            Animation.SetActive(false);
        }


        /*
         * updates he LineRenderer to draw the paths 
         * dequeue the drawn trajectory paths to be deleted number of segments 
         */
        private void Update()
        {
            HandleKeyInput();
            AnimationCameraMouseWheelFOV();
            DrawTrajectory(); // switch off and activate Particle System in planet prefab for optional trajectory approach
        }


        /*
         * frame-rate independent for physics calculations
         * applied each fixed frame
         */
        private void FixedUpdate()
        {
            Gravity();
            CheckCollisionsWithSun();
        }

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
        private void HandleKeyInput()
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    switch (keyCode)
                    {
                        case KeyCode.F1:
                            StartSortingGameOnInput();
                            ClearTrajectories();
                            break;

                        case KeyCode.F2:
                            StartAnimationOnInput();
                            break;

                        case KeyCode.L:
                            sunLight.gameObject.SetActive(!sunLight.gameObject.activeSelf);
                            // Sync the toggle button state with sunLight's state
                            toggleSunLight.isOn = sunLight.gameObject.activeSelf;
                            break;

                        case KeyCode.F3:
                            RenderSettings.skybox = skyboxBlack;
                            break;

                        case KeyCode.F4:
                            RenderSettings.skybox = skyboxDiverseSpace;
                            break;

                        case KeyCode.F5:
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
         * find planets with tag Planet and initializes them
         */
        private void InitializeSortingPlanets()
        {
            //Debug.Log("PlanetaryController: InitializePlanets():");
            sortingPlanets = GameObject.FindGameObjectsWithTag("SortingGamePlanet");
            if (planets.Length <= 0)
            {
                //Should not happen
                Debug.Log("PlanetaryController: InitializeSortingPlanets(): No sortingPlanets found:  " + sortingPlanets.Length);
            }
        }


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

            InitializeSortingPlanets();
            SpawnSortingPlanets();
        }


        /*
         * Spawn the SortingPlanets on a random sortingGameAvailableSlotPositions
         */
        private void SpawnSortingPlanets()
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
         * G is multiplied by 10 for scaling
         * assign the original NASA Planetdata
         * scales down PlanetInfo with various ScaleFactors to fit the visulization
        */
        #region SolarSystem
        /*
         * assign the original NASA Planetdata
         * scales down PlanetInfo with various ScaleFactors to fit the visulization
         */
        private const float MASS_SCALE_FACTOR = 1000;                   /// 1000///
        private const float DIAMETER_SCALE_FACTOR = 10000;              ///10000///
        private const float DIAMETER_ADDITIONAL_SUN_SCALE_FACTOR = 5;   ///   10/// additional scale factor ti shrink the sun
        private const float DISTANCE_SCALE_FACTOR = 4f;                 ///    4///
        private const float GAS_GIANTS_SCALE_FACTOR = 2.5f;             ///  2.5/// additional scale factor for the Gas Giants 5-8 to bring them closer for vizualisation
        private const float SUN_RADIUS_ADDITIONAL_OFFSET = 3f;          ///    3/// additional offset multiplying the suns radius to the distances to get more visual destinction
        private void InitializeAndScalePlanets()
        {
            //Debug.Log("PlanetaryController: InitializeAndScalePlanets():");
            initialPlanetPositions.Clear();
            initialPlanetRotations.Clear();

            if (planets.Length <= 0)
            {
                Debug.LogError("PlanetaryController: InitializeAndScalePlanets(): No planets found: " + planets.Length);
                return;
            }

            GameObject sun = null;
            float sunRadius = 0;

            //apply data from PlanetInfo
            foreach (var planet in planets)
            {
                PlanetInfo planetInfo = planet.GetComponent<PlanetInfo>();
                Rigidbody planetRigidbody = planet.GetComponent<Rigidbody>();

                if (planetInfo == null)
                {
                    Debug.LogError("PlanetaryController: InitializeAndScalePlanets(): Missing PlanetInfo for: " + planet.name);
                    continue;
                }

                if (planetRigidbody == null)
                {
                    Debug.LogError("PlanetaryController: InitializeAndScalePlanets()");
                    continue;
                }

                SetupMass(planetRigidbody, planetInfo);
                SetupRotation(planet, planetInfo);

                float scaleSize = SetupSize(planet, planetInfo, ref sun, ref sunRadius);
                planet.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
            }

            foreach (var planet in planets)
            {
                PlanetInfo planetInfo = planet.GetComponent<PlanetInfo>();
                if (planetInfo != null)
                {
                    SetupDistanceFromSun(planet, planetInfo, sun, sunRadius);
                }
            }
        }


        /*
         * SetupSetupMass by assigning the mass from PlanetInfo to Rigidbody, scaled down by 1000.
         */
        private void SetupMass(Rigidbody rb, PlanetInfo info)
        {
            rb.mass = info.mass / MASS_SCALE_FACTOR;
            //Debug.Log("PlanetaryController: SetupMass(): scaled rb.mass for " + info + "is " + rb.mass);
        }


        /*
         * SetupRotation and store planets obliquityToOrbit at z axis
         */
        private void SetupRotation(GameObject planet, PlanetInfo info)
        {
            Vector3 initialRotationAngle = new Vector3(0, 0, info.obliquityToOrbit);
            planet.transform.localEulerAngles = initialRotationAngle;
            initialPlanetRotations.Add(planet.transform.localEulerAngles);
            //Debug.Log("PlanetaryController: SetupRotation(): Setting initial rotation for " + planet.name + " to " + initialRotationAngle);
        }



        /*
         * SetupSize calculated and scaled from the PlanetInfo diameter
         */
        private float SetupSize(GameObject planet, PlanetInfo info, ref GameObject sun, ref float sunRadius)
        {
            float scaleSize = info.diameter / DIAMETER_SCALE_FACTOR;

            if (info.PlanetInformationOf == PlanetInformation.Sun)
            {
                sun = planet;
                sun.transform.position = Vector3.zero;
                scaleSize /= DIAMETER_ADDITIONAL_SUN_SCALE_FACTOR;
                sunRadius = (scaleSize / 2) * SUN_RADIUS_ADDITIONAL_OFFSET;
                //Debug.Log("PlanetaryController: SetupSize(): sun radius: " + sunRadius);
            }
            // additional scaling for Gas Giants 5-8
            else if (info.PlanetInformationOf >= PlanetInformation.Jupiter && info.PlanetInformationOf <= PlanetInformation.Neptune)
            {
                scaleSize /= GAS_GIANTS_SCALE_FACTOR;
            }

            return scaleSize;
        }


        /*
         * SetupDistanceFromSun
         * scaled distances from the PlanetInfo  + sunRadius applied after scaling the planets
         * distanceFromSun = semiMajorAxis
         * perihelion = distanceFromSunAtPerihelion where the velocity is maximum
         */
        private void SetupDistanceFromSun(GameObject planet, PlanetInfo info, GameObject sun, float sunRadius)
        {
            float semiMajorAxis = sunRadius + (info.distanceFromSun / DISTANCE_SCALE_FACTOR);

            if (info.PlanetInformationOf >= PlanetInformation.Jupiter && info.PlanetInformationOf <= PlanetInformation.Neptune)
            {
                semiMajorAxis = sunRadius + (info.distanceFromSun / (DISTANCE_SCALE_FACTOR * GAS_GIANTS_SCALE_FACTOR));
            }

            Vector3 directionFromSun = Vector3.right;
            planet.transform.position = sun.transform.position + directionFromSun * semiMajorAxis;
            //Debug.Log("PlanetaryController: SetupDistanceFromSun(): planet.transform.localScale " + planet.name + " is " + planet.transform.position);
            initialPlanetPositions.Add(planet.transform.position);
        }


        /*
         * Newton's law of universal gravitation
         */
        private void Gravity()
        {
            foreach (GameObject a in planets)
            {
                foreach (GameObject b in planets)
                {
                    // Object can't orbit itself
                    if (!a.Equals(b))
                    {
                        float m1 = a.GetComponent<Rigidbody>().mass;
                        float m2 = b.GetComponent<Rigidbody>().mass;

                        float r = Vector3.Distance(a.transform.position, b.transform.position);

                        // Newton's law of universal gravitation
                        // F = G * ((m1 * m2) / (r^2))
                        a.GetComponent<Rigidbody>().AddForce((b.transform.position - a.transform.position).normalized *
                            (G * 10 * (m1 * m2) / (r * r)));
                    }
                }
            }
        }


        /*
         * planets InitialVelocity for circular orbits
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
                        a.GetComponent<Rigidbody>().velocity += a.transform.right * Mathf.Sqrt((G * 10 * m2) / r);
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

                    a.GetComponent<Rigidbody>().velocity += a.transform.right * Mathf.Sqrt((G * 10 * m2) / r);
                }
            }
        }
        #endregion SolarSystem


        /*
         * AnimationCamera reset
         * Skybox
         */
        #region Animation
        /*
         * set skybox
         */
        private void SetSkybox()
        {
            RenderSettings.skybox = skyboxStars;
        }


        /*
         * change AnimationCamera FOV with mouse scroll wheel
         */
        private void AnimationCameraMouseWheelFOV()
        {
            float scrollData = Input.GetAxis("Mouse ScrollWheel");
            float mouseScrollSensitivity = 40;

            if (scrollData != 0)
            {
                AnimationCamera.fieldOfView -= scrollData * mouseScrollSensitivity;
                AnimationCamera.fieldOfView = Mathf.Clamp(AnimationCamera.fieldOfView, 10, 180);

                // update sliderAnimationCameraFov
                sliderAnimationCameraFov.value = AnimationCamera.fieldOfView;
            }
        }


        /*
         *  reset the camera's position and field of view to their initial values
         */
        public void ResetCamera()
        {
            AnimationCamera.transform.SetPositionAndRotation(initialAnimationCameraPosition, initialAnimationCameraRotation);
            AnimationCamera.fieldOfView = initialAnimationCameraFov;

            sliderAnimationCameraFov.value = initialAnimationCameraFov;
        }


        /*
         * get flycamere script to dis/enable later
         */
        private void GetFlyCameraScript()
        {
            flyCameraScript = GetComponent<FlyCamera>();
            if (flyCameraScript == null)
            {
                Debug.Log("PlanetaryController: GetFlyCameraScript(): script not found");
            }
        }


        /*
         * check if planets and suns collider intersect
         */
        private bool CheckCollisionWithSun(GameObject planet)
        {
            Collider planetCollider = planet.GetComponent<Collider>();
            Collider sunCollider = sun.GetComponent<Collider>();

            // check if planet and sun colliders intersect
            if (planetCollider != null && sunCollider != null &&
                planetCollider.bounds.Intersects(sunCollider.bounds))
            {
                return true;
            }

            return false;
        }


        /*
         * check if planet collides with sun and toggles the planet if there is a collision
         */
        private void CheckCollisionsWithSun()
        {
            for (int index = 1; index < planets.Length; index++)
            {
                if (CheckCollisionWithSun(planets[index]))
                {
                    //Debug.Log("PlanetaryController: CheckCollisionsWithSun(): " + planets[index] + " collides with sun");
                    UIToggle(true, index);
                }
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
        private void InitializeLineRenderer()
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
        private void SetupLineRenderer()
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
         * loops through planetTrajectories and accesses the queue of previous positions
         * creating/drawing a trail trajectory
         * dequeues oldest position when segments reached
         */
        private void DrawTrajectory()
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


        /*
         * clear trajectory path by resetting previousPosition queue and resetting LineRenderer
         */
        public void ClearTrajectories()
        {
            for (int i = 0; i < previousPositionsList.Count; i++)
            {
                Queue<Vector3> previousPositions = previousPositionsList[i];
                previousPositions.Clear();

                LineRenderer lr = lineRenderers[i];
                lr.positionCount = 0;
            }

            //clear particle sytsem
            foreach (GameObject planet in planets)
            {
                ParticleSystem ps = planet.GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    ps.Clear();
                }
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
            InitializeSortingPlanets();

            foreach (GameObject planet in sortingPlanets)
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
            InitializeSortingPlanets();

            foreach (GameObject sortingPlanet in sortingPlanets)
            {
                //Debug.Log("PlanetController(): UIToggleSGOrientation(): planet = " + planet);
                GameObject orientationGizmo = sortingPlanet.transform.Find("orientation gizmo").gameObject;
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
            //Debug.Log("PlanetController(): UIToggleAOrientation(): planet.Length = " + planet.Length);
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
            if (lineRenderers == null || planets == null)
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
            if (lr == null || planets[index] == null)
            {
                Debug.Log("PlanetController: ToggleTrajectory(): LineRenderer or planets[index] at index " + index + " is null");
                return;
            }
            lr.enabled = isOn;

            //toggle particle system
            ParticleSystem ps = planets[index].GetComponentInChildren<ParticleSystem>();
            if (ps == null)
            {
                //Debug.Log("PlanetController: ToggleTrajectory(): ParticleSystem in Planet at index " + index + " is null or deactivated off in planet prefab");
                return;
            }

            if (isOn)
            {
                ps.Play();
            }
            else
            {
                ps.Clear();
                ps.Pause();
            }
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

            //toggle particle system
            // If there's a planet at this index, toggle its ParticleSystem.
            foreach (GameObject planet in planets)
            {
                ParticleSystem ps = planet.GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    if (isOn)
                    {
                        ps.Play();
                    }
                    else
                    {
                        ps.Clear();
                        ps.Pause();
                    }
                }
            }
        }


        /*
         * toggles the sun's kinematic and recalcuates its initial velocity after button press
         */
        public void UIToggleSunKinematic(bool isKinematic)
        {
            Rigidbody sunRb = planets[0].GetComponent<Rigidbody>();
            sunRb.isKinematic = isKinematic;
            if (!isKinematic)
            {
                RecalculateInitialVelocity(planets[0]);
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

            for (int index = 0; index < planetToggles.Length; index++)
            {
                int planet_index = index;
                planetToggles[index].onValueChanged.AddListener((bool isOn) => UIToggle(isOn, planet_index));
            }
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

            sliderG.onValueChanged.RemoveListener(OnGValueChanged);
            sliderTimeSpeed.onValueChanged.RemoveListener(OnTimeSliderValueChanged);
            sliderAnimationCameraFov.onValueChanged.RemoveListener(OnFOVSliderValueChanged);

            for (int index = 0; index < planetToggles.Length; index++)
            {
                planetToggles[index].onValueChanged.RemoveAllListeners();
            }
        }
        #endregion UIToggleButtons


        /*
         * hides planets and trajectories after radiobutton is pressed
         */
        #region HidePlanets

        public void UIToggle(bool isOn, int index)
        {
            //Debug.Log(planets[index].name + " checkbox: " + !isOn);

            planets[index].SetActive(!isOn);

            if (index == 0)
            {
                sunHalo.enabled = !isOn;
            }

            if (index == 6)
            {
                saturn_ring_1.SetActive(!isOn);
                saturn_ring_2.SetActive(!isOn);
            }

            ToggleTrajectory(index, !isOn);
            planetToggles[index].isOn = isOn;
        }
        #endregion HidePlanets


        /*
         * store the StoreInitialCameras's position, rotation, and field of view
         * LERPs the currentCamera(MainCamera) to the targetCameras position
         * targetCameras are just used for theire position not for theire view
         * reverse LERP camera
         */
        #region LerpCamera
        /*
         * store the StoreInitialCameras's position, rotation, and field of view
         */
        private void StoreInitialCameras()
        {
            if (AnimationCamera == null)
            {
                Debug.Log("CameraAndUIController: StoreInitialAnimationCamera(): AnimationCamera missing");
            }

            initialAnimationCameraPosition = AnimationCamera.transform.position;
            initialAnimationCameraRotation = AnimationCamera.transform.rotation;
            initialAnimationCameraFov = AnimationCamera.fieldOfView;

            initialMainCameraPosition = MainCamera.transform.position;
            initialMainCameraRotation = MainCamera.transform.rotation;
            initialMainCameraFOV = MainCamera.GetComponent<Camera>().fieldOfView;
        }


        /*
         * LERPs the currentCamere(MainCamera) to the targetCameras position
         * targetCameras are just used for theire position not for theire view
         */
        private IEnumerator LerpCameraToPosition(GameObject currentCamera, GameObject targetCamera, float lerpDuration)
        {
            float time = 0f;
            Vector3 initialPosition = currentCamera.transform.position;
            Quaternion initialRotation = currentCamera.transform.rotation;
            float initialFOV = currentCamera.GetComponent<Camera>().fieldOfView;
            float targetFOV = targetCamera.GetComponent<Camera>().fieldOfView;

            while (time < lerpDuration)
            {
                time += Time.deltaTime;
                float t = time / lerpDuration;

                currentCamera.transform.position = Vector3.Lerp(initialPosition, targetCamera.transform.position, t);
                currentCamera.transform.rotation = Quaternion.Lerp(initialRotation, targetCamera.transform.rotation, t);
                currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(initialFOV, targetFOV, t);

                yield return null;
            }
        }


        /*
         * reverse LERP camera
         */
        private IEnumerator LerpCameraToInitialPosition(GameObject currentCamera, float lerpDuration)
        {
            float time = 0f;
            Vector3 initialPosition = currentCamera.transform.position;
            Quaternion initialRotation = currentCamera.transform.rotation;
            float initialFOV = currentCamera.GetComponent<Camera>().fieldOfView;

            while (time < lerpDuration)
            {
                time += Time.deltaTime;
                float t = time / lerpDuration;

                currentCamera.transform.position = Vector3.Lerp(initialPosition, initialMainCameraPosition, t);
                currentCamera.transform.rotation = Quaternion.Lerp(initialRotation, initialMainCameraRotation, t);
                currentCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(initialFOV, initialMainCameraFOV, t);

                yield return null;
            }
        }
        #endregion LerpCamera


        /*
         * StartSortingGameOnInput and de/activates gameobjects after coroutine has finished 
         * StartAnimationOnInput and de/activates gameobjects after coroutine has finished 
         * 
         */
        #region StartScreenScenes
        /*
         * StartSortingGameOnInput and calls LERP camera couroutine
         */
        public void StartSortingGameOnInput()
        {
            //Debug.Log("PlanetaryController: StartAnimationOnInput(): ");
            LeaveAnimation();

            SortingMinigame.SetActive(true);
            UIToggleSGRotation(false);
            StartCoroutine(LerpCameraStartSortingGame());

            //ResetSortingGame();
        }


        /*
         * Waits for LERP camera couroutine and de/activates gameobjects
         */
        private IEnumerator LerpCameraStartSortingGame()
        {
            yield return StartCoroutine(LerpCameraToPosition(MainCamera, SortingGameCamera, 1f));
            SortingMinigame.SetActive(true);
            SortingGamePlanetInfoUI.SetActive(true);
            FormulaUI.SetActive(false);

            DisplayMessageByKey("EnterSortingGame");
        }


        /*
         * LeaveSortingGame and deactivates gameobjects
         */
        public void LeaveSortingGame()
        {
            //Debug.Log("PlanetaryController: LeaveSortingGame(): ");
            HelpiDialogueUI.SetActive(false);
            SortingGamePlanetInfoUI.SetActive(false);
            FormulaUI.SetActive(false);

            StartCoroutine(LerpCameraLeave());
        }


        /*
         * Reverse LERP camera when leaving a ScreenScene
         */
        private IEnumerator LerpCameraLeave()
        {
            yield return StartCoroutine(LerpCameraToInitialPosition(MainCamera, 0.5f));

            UIToggleSunLight(false);
            toggleSunLight.isOn = false;
            SortingMinigame.SetActive(false);
        }


        /*
         * StartAnimationOnInput and calls LERP camera couroutine
         */
        public void StartAnimationOnInput()
        {
            //Debug.Log("PlanetaryController: StartAnimationOnInput(): ");
            LeaveSortingGame();
            FormulaUI.SetActive(false);

            UIToggleAllTrajectories(true);
            toggleAllTrajectories.isOn = true;

            StartCoroutine(LerpCameraStartAnimation());
        }


        /*
         * Waits for LERP camera couroutine and de/activates gameobjects
         */
        private IEnumerator LerpCameraStartAnimation()
        {
            yield return StartCoroutine(LerpCameraToPosition(MainCamera, TelescopeCamera, 1f));

            AnimationUI.SetActive(true);
            SetSkybox();

            Environment.SetActive(false);
            Interactibles.SetActive(false);

            Animation.SetActive(true);
            flyCameraScript.enabled = true;

            MainCamera.SetActive(false);
            SolarSystemAnimationCamera.SetActive(true);

            ResetAnimation();
            DisplayMessageByKey("EnterAnimation");
        }


        /*
         * LeaveAnimation and deactivates gameobjects
         */
        public void LeaveAnimation()
        {
            HelpiDialogueUI.SetActive(false);
            AnimationUI.SetActive(false);

            Environment.SetActive(true);
            Interactibles.SetActive(true);

            Animation.SetActive(false);
            flyCameraScript.enabled = false;
            ResetCamera();

            SolarSystemAnimationCamera.SetActive(false);
            MainCamera.SetActive(true);
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
         * ResetAnimation on reset and on StartAnimation
         */
        private void ResetAnimation()
        {
            bool planetIsHidden = false;
            for (int index = 0; index < planetToggles.Length; index++)
            {
                UIToggle(planetIsHidden, index);
            }

            ClearTrajectories();
            sliderG.value = gravitationalConstantG;
            sliderTimeSpeed.value = timeSpeed;

            //reset planets to initialPlanetPosition
            for (int planet = 0; planet < planets.Length; planet++)
            {
                planets[planet].transform.position = initialPlanetPositions[planet];
                planets[planet].transform.localEulerAngles = initialPlanetRotations[planet];

                //set is kinematic to stop all physics
                Rigidbody rb = planets[planet].GetComponent<Rigidbody>();
                rb.isKinematic = true;
            }

            StartCoroutine(RestartAnimationDelay());
        }


        /*
         * after delay sets all planets kinematic to stop physics
         * reaplies InitialVelocity
         */
        private IEnumerator RestartAnimationDelay()
        {
            yield return new WaitForSeconds(resetAnimationDelay);

            //reapply initial velocities and start planet movement after delay
            for (int planet = 0; planet < planets.Length; planet++)
            {
                Rigidbody rb = planets[planet].GetComponent<Rigidbody>();
                rb.isKinematic = false;
            }

            UIToggleSunKinematic(true);

            InitialVelocity();
            //InitialVelocityEliptical();
        }


        /*
         * reset
         */
        public void ResetObject()
        {
            //Debug.Log("PlanetaryController: ResetObject(): button pressed");
            ResetAnimation();
        }


        /*
         * ResetHome button deactivates Animation and SortingGame Gameobjects
         * activates FormulaUI
         * stopps all LERP camera coroutines
         */
        public void ResetHome()
        {
            //Debug.Log("PlanetaryController: ResetHome(): button pressed");
            StopAllCoroutines();
            LeaveSortingGame();
            LeaveAnimation();

            DisplayMessageByKey("EnterPlanetarySystem");
            FormulaUI.SetActive(true);
        }
        #endregion ResetBar


        /*
         * SetupSlider FOV, G, time
         */
        #region slider
        /*
         * SetupSlider FOV, G, time
         */
        private void SetupSliders()
        {
            if (sliderG != null)
            {
                sliderG.minValue = 0f;
                sliderG.maxValue = 25f;
                sliderG.onValueChanged.AddListener(OnGValueChanged);
            }

            if (sliderTimeSpeed != null)
            {
                sliderTimeSpeed.minValue = 0f;
                sliderTimeSpeed.maxValue = 35f;
                sliderTimeSpeed.onValueChanged.AddListener(OnTimeSliderValueChanged);
            }

            if (sliderAnimationCameraFov != null)
            {
                sliderAnimationCameraFov.minValue = 10;
                sliderAnimationCameraFov.maxValue = 180;
                sliderAnimationCameraFov.value = AnimationCamera.fieldOfView;
                sliderAnimationCameraFov.onValueChanged.AddListener(OnFOVSliderValueChanged);
            }
        }


        /*
         * changes the G value after slider input
         */
        private void OnGValueChanged(float gValue)
        {
            G = gValue;
        }


        /*
         * changes the time/speed value after slider input
         */
        void OnTimeSliderValueChanged(float timeSpeedValue)
        {
            Time.timeScale = timeSpeedValue;
        }


        /*
         * changes the FOV value after slider input
         */
        private void OnFOVSliderValueChanged(float fovValue)
        {
            AnimationCamera.fieldOfView = fovValue;
        }

        #endregion slider
    }
}