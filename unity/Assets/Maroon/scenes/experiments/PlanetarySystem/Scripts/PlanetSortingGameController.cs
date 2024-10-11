using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.PlanetarySystem
{
    public class PlanetSortingGameController : MonoBehaviour
    {
        public int sortedPlanetCount = 0;
        [SerializeField] private GameObject sortingGamePlanetPlaceholderSlots;
        GameObject[] sortingPlanets;
        private readonly List<int> sortingGameAvailableSlotPositions = new List<int>();

        [SerializeField] private Light sunLight;
        [SerializeField] private ParticleSystem solarFlares;
        //---------------------------------------------------------------------------------------

        /// <summary>
        /// HandleKeyInput sunlight 
        /// </summary>
        private void Update()
        {
            HandleKeyInput();
        }


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
         * find planets with tag SortingPlanet and initializes them
         */
        private void InitializeSortingPlanets()
        {
            //Debug.Log("PlanetaryController: InitializePlanets():");
            sortingPlanets = GameObject.FindGameObjectsWithTag("SortingGamePlanet");
            if (sortingPlanets.Length <= 0)
            {
                //Should not happen
                Debug.Log("PlanetSortingGameController: InitializeSortingPlanets(): No sortingPlanets found:  " + sortingPlanets.Length);
            }
        }


        /*
         * Initialize the availablePositions list with all possible sorting positions
         * called from StartSortingGame
         */
        public void InitializeAvailableSortingGameSlotPositions()
        {
            //Debug.Log("PlanetSortingGameController: InitializeAvailableSortingGameSlotPositions():");
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


        // Sorting game toggle functions
        #region SGToggleFunctions
        /// <summary>
        /// toggles the rotation of the minigame sortable planets after button press
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleSGRotation(bool isOn)
        {
            //Debug.Log("PlanetSortingGameController(): UIToggleSGRotation = " + isOn);
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
        /// toggles the rotation of the minigame sortable planets button press
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleSGOrientation(bool isOn)
        {
            //Debug.Log("PlanetSortingGameController(): UIToggleSGOrientation = " + isOn);
            InitializeSortingPlanets();

            foreach (GameObject sortingPlanet in sortingPlanets)
            {
                //Debug.Log("PlanetSortingGameController(): UIToggleSGOrientation(): planet = " + planet);
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
        /// toggles the sun's solarflares in the sorting game after button press
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleSolarFlares(bool isOn)
        {
            //Debug.Log("PlanetSortingGameController(): UIToggleSolarFlares = " + isOn);
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
            //Debug.Log("PlanetSortingGameController(): UIToggleSunLight = " + !isOn);
            sunLight.gameObject.SetActive(isOn);
        }
        #endregion SGToggleFunctions


        /// <summary>
        /// ResetSortingGame planet positions
        /// </summary>
        public void ResetSortingGame()
        {
            sortingGameAvailableSlotPositions.Clear();
            InitializeAvailableSortingGameSlotPositions();
        }
    }
}