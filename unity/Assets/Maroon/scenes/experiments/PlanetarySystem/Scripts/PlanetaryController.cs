using System.Collections.Generic;
using UnityEngine;
using Maroon.UI;            //Dialogue Manager
using GEAR.Localization;    //MLG
using System.Collections;



namespace Maroon.Experiments.PlanetarySystem
{
    public class PlanetaryController : MonoBehaviour, IResetObject
    {
        public PlanetarySimulationUIController uiController;
        public PlanetTrajectoryController planetTrajectoryController;
        public StartScreenScenes startScreenScenes;


        #region Cameras
        [SerializeField] private GameObject MainCamera;                 //off
        [SerializeField] private GameObject SolarSystemAnimationCamera; //on
        [SerializeField] private GameObject SortingGameCamera;          //off
        [SerializeField] private GameObject TelescopeCamera;            //off
        public Camera AnimationCamera;

        public float initialAnimationCameraFov;
        public Vector3 initialAnimationCameraPosition;
        public Quaternion initialAnimationCameraRotation;
        #endregion Cameras

        #region SolarSystem
        public float G; // gravitational constant 6.674
        private GameObject sun;

        public GameObject[] planets;
        #endregion SolarSystem

        #region SortingGameSpawner
        public int sortedPlanetCount = 0;
        [SerializeField] private GameObject sortingGamePlanetPlaceholderSlots;
        GameObject[] sortingPlanets;
        private readonly List<int> sortingGameAvailableSlotPositions = new List<int>();
        #endregion SortingGameSpawner

        #region ResetAnimation
        private readonly List<Vector3> initialPlanetPositions = new List<Vector3>();
        private readonly List<Vector3> initialPlanetRotations = new List<Vector3>();
        [SerializeField] private float resetAnimationDelay = 0.3f;
        #endregion ResetAnimation

        #region Helpi
        private DialogueManager _dialogueManager;
        #endregion Helpi

        #region KeyInput
        [SerializeField] private Material skyboxMaterial;
        [SerializeField] private Light sunLight;
        [SerializeField] private ParticleSystem solarFlares;
        #endregion KeyInput

        //---------------------------------------------------------------------------------------
       
        // Instance of PlanetaryController
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


        /// <summary>
        /// initialize LineRenderer before toggle
        /// </summary>
        private void Awake()
        {
            DisplayMessageByKey("EnterPlanetarySystem");

            sun = planets[0];
            sun.SetActive(true);

            InitializeAndScalePlanets();
            planetTrajectoryController.InitializeLineRenderer();
        }


        /// <summary>
        /// general start setup
        /// </summary>
        private void Start()
        {
            StoreInitialCamera();
            planetTrajectoryController.SetupLineRenderer();
            startScreenScenes.Animation.SetActive(false);
        }


        /// <summary>
        /// updates he LineRenderer to draw the paths 
        /// dequeue the drawn trajectory paths to be deleted number of segments 
        /// </summary>
        private void Update()
        {
            HandleKeyInput();
            AnimationCameraMouseWheelFOV();
            planetTrajectoryController.DrawTrajectory(); // switch off and activate Particle System in planet prefab for optional trajectory approach
        }


        /// <summary>
        /// Gravity
        /// frame-rate independent for physics calculations
        /// applied each fixed frame
        /// </summary>
        private void FixedUpdate()
        {
            Gravity();
            CheckCollisionsWithSun();
        }


        // displays Helpi masseges by key
        #region Helpi
        /// <summary>
        /// displays Halpi masseges by key
        /// </summary>
        /// <param name="key"></param>
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


        // HandlesKeyInput during Update
        #region KeyInput
        /// <summary>
        /// HandlesKeyInput during Update
        /// toggle sunlight                   on key [L]
        /// </summary>
        private void HandleKeyInput()
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    switch (keyCode)
                    {
                        case KeyCode.L:
                            sunLight.gameObject.SetActive(!sunLight.gameObject.activeSelf);
                            // Sync the toggle button state with sunLight's state
                            //ToggleSunLight.isOn = sunLight.gameObject.activeSelf;
                            ToggleSunLight(sunLight.gameObject.activeSelf);
                            break;

                        default:
                            break;
                    }
                }
            }
        }
        #endregion KeyInput


        // handles the SortingGame
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


        //SolarSystems handles Newtons Gravity, Initial Velocity and the planets
        #region SolarSystem
        private const float MASS_SCALE_FACTOR = 1000;                   /// 1000///
        private const float DIAMETER_SCALE_FACTOR = 10000;              ///10000///
        private const float DIAMETER_ADDITIONAL_SUN_SCALE_FACTOR = 5;   ///   10/// additional scale factor ti shrink the sun
        private const float DISTANCE_SCALE_FACTOR = 4f;                 ///    4///
        private const float GAS_GIANTS_SCALE_FACTOR = 2.5f;             ///  2.5/// additional scale factor for the Gas Giants 5-8 to bring them closer for vizualisation
        private const float SUN_RADIUS_ADDITIONAL_OFFSET = 3f;          ///    3/// additional offset multiplying the suns radius to the distances to get more visual destinction

        /// <summary>
        /// assign the original NASA Planetdata
        /// scales down PlanetInfo with various ScaleFactors to fit the visulization
        /// G is multiplied by 10 for scaling
        /// </summary>
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


        /// <summary>
        /// SetupSetupMass by assigning the mass from PlanetInfo to Rigidbody, scaled down by 1000.
        /// </summary>
        /// <param name="rb"></param>
        /// <param name="info"></param>
        private void SetupMass(Rigidbody rb, PlanetInfo info)
        {
            rb.mass = info.mass / MASS_SCALE_FACTOR;
            //Debug.Log("PlanetaryController: SetupMass(): scaled rb.mass for " + info + "is " + rb.mass);
        }


        /// <summary>
        /// SetupRotation and store planets obliquityToOrbit at z axis
        /// </summary>
        /// <param name="planet"></param>
        /// <param name="info"></param>
        private void SetupRotation(GameObject planet, PlanetInfo info)
        {
            Vector3 initialRotationAngle = new Vector3(0, 0, info.obliquityToOrbit);
            planet.transform.localEulerAngles = initialRotationAngle;
            initialPlanetRotations.Add(planet.transform.localEulerAngles);
            //Debug.Log("PlanetaryController: SetupRotation(): Setting initial rotation for " + planet.name + " to " + initialRotationAngle);
        }


        /// <summary>
        /// SetupSize calculated and scaled from the PlanetInfo diameter
        /// </summary>
        /// <param name="planet"></param>
        /// <param name="info"></param>
        /// <param name="sun"></param>
        /// <param name="sunRadius"></param>
        /// <returns></returns>
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


        /// <summary>
        /// SetupDistanceFromSun
        /// * scaled distances from the PlanetInfo  + sunRadius applied after scaling the planets
        /// * distanceFromSun = semiMajorAxis
        /// * perihelion = distanceFromSunAtPerihelion where the velocity is maximum
        /// </summary>
        /// <param name="planet"></param>
        /// <param name="info"></param>
        /// <param name="sun"></param>
        /// <param name="sunRadius"></param>
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


        /// <summary>
        /// Newton's law of universal gravitation
        /// </summary>
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


        /// <summary>
        /// planets InitialVelocity for circular orbits
        /// </summary>
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


        /// <summary>
        /// recalculates the InitialVelocity after toggle the sun's isKinematic in the animation
        /// </summary>
        /// <param name="a"></param>
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


        // Animation game
        #region Animation
        /// <summary>
        /// set skybox
        /// </summary>
        public void SetSkybox()
        {
            RenderSettings.skybox = skyboxMaterial;
        }


        /// <summary>
        /// change AnimationCamera FOV with mouse scroll wheel
        /// </summary>
        private void AnimationCameraMouseWheelFOV()
        {
            float scrollData = Input.GetAxis("Mouse ScrollWheel");
            float mouseScrollSensitivity = 40;

            if (scrollData != 0)
            {
                AnimationCamera.fieldOfView -= scrollData * mouseScrollSensitivity;
                AnimationCamera.fieldOfView = Mathf.Clamp(AnimationCamera.fieldOfView, 10, 180);
            }
        }



        /// <summary>
        /// check if planets and suns collider intersect
        /// </summary>
        /// <param name="planet"></param>
        /// <returns></returns>
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


        /// <summary>
        /// check if planet collides with sun and toggles the planet if there is a collision
        /// </summary>
        private void CheckCollisionsWithSun()
        {
            for (int index = 1; index < planets.Length; index++)
            {
                if (CheckCollisionWithSun(planets[index]))
                {
                    //Debug.Log("PlanetaryController: CheckCollisionsWithSun(): " + planets[index] + " collides with sun");
                    bool hide = true;
                    TogglePlanet(hide, index);
                }
            }
        }
        #endregion Animation


        


        // toggle functions
        #region ToggleFunctions
        /// <summary>
        /// toggles the rotation of the minigame sortable planets after button press
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleSGRotation(bool isOn)
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


        /// <summary>
        /// toggles the rotation of the animation planets button press
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleARotation(bool isOn)
        {
            //Debug.Log("PlanetController(): ToggleARotation = " + isOn);
            foreach (GameObject planet in planets)
            {
                //Debug.Log("PlanetController(): ToggleARotation(): planet = " + planet);
                PlanetRotation rotationScript = planet.GetComponent<PlanetRotation>();
                if (rotationScript != null)
                {
                    rotationScript.enabled = isOn;
                }
            }
        }


        /// <summary>
        /// toggles the rotation of the minigame sortable planets button press
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleSGOrientation(bool isOn)
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


        /// <summary>
        /// toggles the rotation of the animation planets button press
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleAOrientation(bool isOn)
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


        /// <summary>
        /// toggles the sun's kinematic and recalcuates its initial velocity after button press
        /// </summary>
        /// <param name="isKinematic"></param>
        public void ToggleSunKinematic(bool isKinematic)
        {
            Rigidbody sunRb = planets[0].GetComponent<Rigidbody>();
            sunRb.isKinematic = isKinematic;
            if (!isKinematic)
            {
                RecalculateInitialVelocity(planets[0]);
            }
        }


        /// <summary>
        /// toggles the sun's solarflares in the sorting game after button press
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleSolarFlares(bool isOn)
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


        /// <summary>
        /// toggles the sun's pointlight in the sorting game after button press
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleSunLight(bool isOn)
        {
            //Debug.Log("PlanetController(): UIToggleSunLight = " + !isOn);
            sunLight.gameObject.SetActive(isOn);
        }
        #endregion ToggleFunctions


        /// <summary>
        /// hides planets and trajectories after radiobutton is pressed
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="index"></param>
        public void TogglePlanet(bool isOn, int index)
        {
            //Debug.Log(planets[index].name + " checkbox: " + !isOn);
            planets[index].SetActive(!isOn);

            planetTrajectoryController.ToggleTrajectory(index, !isOn);
            uiController.planetToggles[index].isOn = isOn;
        }


        /// <summary>
        /// StoreInitialCameras
        /// </summary>
        private void StoreInitialCamera()
        {
            if (AnimationCamera == null)
            {
                Debug.Log("PlanetaryController: StoreInitialAnimationCamera(): AnimationCamera missing");
            }

            initialAnimationCameraPosition = AnimationCamera.transform.position;
            initialAnimationCameraRotation = AnimationCamera.transform.rotation;
            initialAnimationCameraFov = AnimationCamera.fieldOfView;
        }


        // handles homeReset / reset vunctinality
        #region ResetBar
        /// <summary>
        /// ResetSortingGame planet positions
        /// </summary>
        public void ResetSortingGame()
        {
            sortingGameAvailableSlotPositions.Clear();
            InitializeAvailableSortingGameSlotPositions();
        }


        /// <summary>
        /// Reset planets during ResetAnimation on reset 
        /// </summary>
        public void ResetAnimationPlanets()
            {
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


        /// <summary>
        /// ResetAnimation after delay sets all planets kinematic to stop physics
        /// reaplies InitialVelocity
        /// </summary>
        /// <returns></returns>
        private IEnumerator RestartAnimationDelay()
        {
            yield return new WaitForSeconds(resetAnimationDelay);

            //reapply initial velocities and start planet movement after delay
            for (int planet = 0; planet < planets.Length; planet++)
            {
                Rigidbody rb = planets[planet].GetComponent<Rigidbody>();
                rb.isKinematic = false;
            }

            ToggleSunKinematic(true);
            InitialVelocity();
        }


        /// <summary>
        /// ResetAnimation on reset and on StartAnimation
        /// </summary>
        public void ResetAnimation()
        {
            bool hide = false;
            for (int index = 0; index < uiController.planetToggles.Length; index++)
            {
                TogglePlanet(hide, index);
            }

            uiController.ResetCamera();
            uiController.ResetAnimationValues();
            planetTrajectoryController.ClearTrajectories();
            ResetAnimationPlanets();
        }


        /// <summary>
        /// ResetHome button deactivates Animation and SortingGame Gameobjects
        /// activates FormulaUI
        /// stopps all LERP camera coroutines
        /// </summary>
        public void ResetHome()
        {
            //Debug.Log("PlanetaryController: ResetHome(): button pressed");
            StopAllCoroutines();
            startScreenScenes.LeaveSortingGame();
            startScreenScenes.LeaveAnimation();

            DisplayMessageByKey("EnterPlanetarySystem");
            startScreenScenes.FormulaUI.SetActive(true);
        }


        /// <summary>
        /// reset
        /// </summary>
        public void ResetObject()
        {
            //Debug.Log("PlanetaryController: ResetObject(): button pressed");
            ResetAnimation();
        }
        #endregion ResetBar
    }
}