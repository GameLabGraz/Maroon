using System.Collections.Generic;
using UnityEngine;
using Maroon.UI;            //Dialogue Manager
using GEAR.Localization;    //MLG
using System.Collections;


namespace Maroon.Experiments.PlanetarySystem
{
    public class PlanetaryController : MonoBehaviour
    {
        public StartScreenScenes startScreenScenes;

        #region Cameras
        public Camera SimulationCamera;
        [HideInInspector] public float initialSimulationCameraFov;
        [HideInInspector] public Vector3 initialSimulationCameraPosition;
        [HideInInspector] public Quaternion initialSimulationCameraRotation;
        #endregion Cameras

        [HideInInspector] public float G; // gravitational constant 6.674
        private GameObject sun;
        public GameObject[] planets;
        public int sortedPlanetCount = 0;

        private readonly List<Vector3> initialPlanetPositions = new List<Vector3>();
        private readonly List<Vector3> initialPlanetRotations = new List<Vector3>();
        [SerializeField] private float resetSimulationDelay = 0.3f;

        private DialogueManager _dialogueManager;
        [SerializeField] private Material skyboxMaterial;
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

        }


        /// <summary>
        /// general start setup
        /// </summary>
        private void Start()
        {
            InitializeAndScalePlanets();
            StoreInitialCamera();
            startScreenScenes.PlanetarySystemSimulation.SetActive(false);
        }


        /// <summary>
        /// updates he LineRenderer to draw the paths 
        /// dequeue the drawn trajectory paths to be deleted number of segments 
        /// </summary>
        private void Update()
        {
            SimulationCameraMouseWheelFOV();
        }


        /// <summary>
        /// Gravity
        /// frame-rate independent for physics calculations
        /// applied each fixed frame
        /// </summary>
        private void FixedUpdate()
        {
            Gravity();
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


        // Simulation
        #region Simulation
        /// <summary>
        /// set skybox
        /// </summary>
        public void SetSkybox()
        {
            RenderSettings.skybox = skyboxMaterial;
        }


        /// <summary>
        /// StoreInitialCameras
        /// </summary>
        private void StoreInitialCamera()
        {
            if (SimulationCamera == null)
            {
                Debug.Log("PlanetaryController: StoreInitialAnimationCamera(): AnimationCamera missing");
            }

            initialSimulationCameraPosition = SimulationCamera.transform.position;
            initialSimulationCameraRotation = SimulationCamera.transform.rotation;
            initialSimulationCameraFov = SimulationCamera.fieldOfView;
        }


        /// <summary>
        /// change SimulationCamera FOV with mouse scroll wheel
        /// </summary>
        private void SimulationCameraMouseWheelFOV()
        {
            float scrollData = Input.GetAxis("Mouse ScrollWheel");
            float mouseScrollSensitivity = 40;

            if (scrollData != 0)
            {
                SimulationCamera.fieldOfView -= scrollData * mouseScrollSensitivity;
                SimulationCamera.fieldOfView = Mathf.Clamp(SimulationCamera.fieldOfView, 10, 180);
            }
        }


        /// <summary>
        /// check if planets and suns collider intersect
        /// </summary>
        /// <param name="planet"></param>
        /// <returns></returns>
        public bool CheckCollisionWithSun(GameObject planet)
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
        #endregion Simulation


        // toggle functions
        #region ToggleFunctions
        /// <summary>
        /// toggles the rotation of the animation planets button press
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleSimRotation(bool isOn)
        {
            //Debug.Log("PlanetController(): ToggleSimRotation = " + isOn);
            foreach (GameObject planet in planets)
            {
                //Debug.Log("PlanetController(): ToggleSimRotation(): planet = " + planet);
                PlanetRotation rotationScript = planet.GetComponent<PlanetRotation>();
                if (rotationScript != null)
                {
                    rotationScript.enabled = isOn;
                }
            }
        }


        /// <summary>
        /// toggles the rotation of the simulation planets button press
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleSimOrientation(bool isOn)
        {
            //Debug.Log("PlanetController(): ToggleSimOrientation(): planet.Length = " + planet.Length);
            foreach (GameObject planet in planets)
            {
                //Debug.Log("PlanetController(): ToggleSimOrientation(): planet = " + planet);
                GameObject orientationGizmo = planet.transform.Find("orientation_gizmo").gameObject;
                if (orientationGizmo != null)
                {
                    orientationGizmo.SetActive(isOn);
                }
                else
                {
                    Debug.Log("PlanetController(): ToggleSimOrientation(): No orientation gizmo found");
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
        #endregion ToggleFunctions


        /// <summary>
        /// increments the snapped planets and displays a Helpi message when all planets except pluto are in its place 
        /// </summary>
        public void IncrementSnappedPlanetCount()
        {
            int solarSystemPlanet = 10;
            sortedPlanetCount++;

            // check if we have snapped all 10 out of 11 planets, excluding pluto, including moon ad sun
            if (sortedPlanetCount >= solarSystemPlanet)
            {
                DisplayMessageByKey("OrderedSortingGame");
            }
        }


        // handles homeReset / reset functinality
        #region ResetBar
        /// <summary>
        /// Reset planets during ResetPlanetarySystemSimulation on reset 
        /// </summary>
        public void ResetSimulationPlanets()
            {
                for (int planet = 0; planet < planets.Length; planet++)
            {
                planets[planet].transform.position = initialPlanetPositions[planet];
                planets[planet].transform.localEulerAngles = initialPlanetRotations[planet];

                //set is kinematic to stop all physics
                Rigidbody rb = planets[planet].GetComponent<Rigidbody>();
                rb.isKinematic = true;
            }

            StartCoroutine(RestartSimulationDelay());
        }


        /// <summary>
        /// ResetPlanetarySystemSimulation after delay sets all planets kinematic to stop physics
        /// reaplies InitialVelocity
        /// </summary>
        /// <returns></returns>
        private IEnumerator RestartSimulationDelay()
        {
            yield return new WaitForSeconds(resetSimulationDelay);

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
        /// ResetHome button deactivates Simulation and SortingGame Gameobjects
        /// activates FormulaUI
        /// stopps all LERP camera coroutines
        /// </summary>
        public void ResetHome()
        {
            //Debug.Log("PlanetaryController: ResetHome(): button pressed");
            StopAllCoroutines();
            startScreenScenes.LeavePlanetarySortingGame();
            startScreenScenes.LeavePlanetorySystemSimulation();

            DisplayMessageByKey("EnterPlanetarySystem");
            startScreenScenes.FormulaUI.SetActive(true);
        }
        #endregion ResetBar
    }
}