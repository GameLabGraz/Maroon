using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System.Collections.Generic;
using GameLabGraz.VRInteraction;

namespace Maroon.Experiments.PlanetarySystem
{
    public class StartScreenScenes : MonoBehaviour
    {
        public PlanetarySimulationUIController uiController;
        public PlanetaryController planetaryController;
        public PlanetTrajectoryController planetTrajectoryController;
        public PlanetSortingGameController planetSortingGameController;

        public GameObject PlanetarySortingGame;
        public GameObject PlanetarySortingGamePlanetInfoUI;
        public GameObject FormulaUI;
        public GameObject PlanetarySystemSimulation;
        public GameObject PlanetarySystemSimulationUI;
        public GameObject Environment;
        public GameObject InteractablePlanetarySystemScreens;
        public GameObject MainCamera;
        public GameObject InitialMainCamera;
        public GameObject PlanetarySortingGameCamera;
        public GameObject PlanetarySystemSimulationTelescopeCamera;
        public GameObject VRPlayer;
        public Transform planetarySpawnPoint;
        public GameObject PlanetarySystemSimulationCamera;
        public GameObject HelpiDialogueUI;

        public GameObject uiButton;
        public GameObject ExitButton;
        public GameObject SliderG;
        public GameObject SliderTimeSpeed;
        public GameObject TimeTo1Button;

        public VRLinearDrive HandleTimeSpeed;
        public Transform simulationSpawnPoint;
        //---------------------------------------------------------------------------------------


        /// <summary>
        /// deactivate SortingMinigame
        /// </summary>
        private void Awake()
        {
            PlanetarySortingGame.SetActive(true);
            PlanetarySortingGamePlanetInfoUI.SetActive(false);
        }


        // LERPs the currentCamera(MainCamera) to the targetCameras position
        #region LerpCamera
        /// <summary>
        /// LERPs the currentCamere(MainCamera) to the targetCameras position
        /// targetCameras are just used for theire position not for theire view
        /// </summary>
        /// <param name="currentCamera"></param>
        /// <param name="targetCamera"></param>
        /// <param name="lerpDuration"></param>
        /// <returns></returns>
        private IEnumerator LerpCameraToPosition(GameObject currentCamera, GameObject targetCamera, float lerpDuration)
        {
            //Debug.Log("StartScreenScenes(): LerpCameraToPosition(): ");

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


        /// <summary>
        /// reverse LERP camera
        /// </summary>
        /// <param name="currentCamera"></param>
        /// <param name="lerpDuration"></param>
        /// <returns></returns>
        private IEnumerator LerpCameraToInitialPosition(GameObject currentCamera, float lerpDuration)
        {
            yield return StartCoroutine(LerpCameraToPosition(currentCamera, InitialMainCamera, lerpDuration));
        }
        #endregion LerpCamera


        // StartSortingGameOnInput or StartPlanetarySystemSimulationOnInputand de/activates gameobjects after coroutine has finished 
        #region StartScreenScenes
        /// <summary>
        /// StartSortingGameOnInput and calls LERP camera couroutine
        /// </summary>
        public void StartPlanetarySortingGameOnInput()
        {
            //Debug.Log("StartScreenScenes(): StartPlanetarySortingGameOnInput(): ");

            LeavePlanetorySystemSimulation();
            PlanetarySortingGame.SetActive(true);
            ExitButton.SetActive(false);
            SliderG.SetActive(false);
            TimeTo1Button.SetActive(false);
            HandleTimeSpeed.ForceToValue(1f);
            SliderTimeSpeed.SetActive(false);
            uiButton.SetActive(false);
            Player.instance.transform.position = simulationSpawnPoint.position;
            Player.instance.transform.rotation = simulationSpawnPoint.rotation;
            StartCoroutine(LerpCameraStartPlanetarySortingGame());
        }

        /// <summary>
        /// waits for LERP camera couroutine and de/activates gameobjects
        /// </summary>
        /// <returns></returns>
        private IEnumerator LerpCameraStartPlanetarySortingGame()
        {
            //Debug.Log("StartScreenScenes(): LerpCameraStartPlanetarySortingGame(): ");

            yield return StartCoroutine(LerpCameraToPosition(MainCamera, PlanetarySortingGameCamera, 1f));
            PlanetarySortingGame.SetActive(true);
            PlanetarySortingGamePlanetInfoUI.SetActive(true);
            FormulaUI.SetActive(false);

            planetSortingGameController.ToggleSGRotation(true);
            planetaryController.DisplayMessageByKey("EnterPlanetarySortingGame");
        }


        /// <summary>
        /// LeaveSortingGame and deactivates gameobjects
        /// </summary>
        public void LeavePlanetarySortingGame()
        {
            HelpiDialogueUI.SetActive(false);
            PlanetarySortingGamePlanetInfoUI.SetActive(false);
            FormulaUI.SetActive(false);

            StartCoroutine(LerpCameraLeavePlanetarySortingGame());
        }


        /// <summary>
        /// reverse LERP camera when leaving a ScreenScene
        /// </summary>
        /// <returns></returns>
        private IEnumerator LerpCameraLeavePlanetarySortingGame()
        {
            yield return StartCoroutine(LerpCameraToInitialPosition(MainCamera, 0.5f));

            planetSortingGameController.ToggleSunLight(false);
            PlanetarySortingGame.SetActive(false);
        }


        /// <summary>
        /// StartPlanetorySystemSimulationOnInput and calls LERP camera couroutine
        /// </summary>
        public void StartPlanetorySystemSimulationOnInput()
        {
            LeavePlanetarySortingGame();
            FormulaUI.SetActive(false);

            uiButton.SetActive(true);

            planetTrajectoryController.ToggleAllTrajectories(true);
            Player.instance.transform.position = planetarySpawnPoint.position;
            Player.instance.transform.rotation = planetarySpawnPoint.rotation;

            StartCoroutine(LerpCameraStartPlanetarySystemSimulation());
        }


        /// <summary>
        /// waits for LERP camera couroutine and de/activates gameobjects
        /// </summary>
        /// <returns></returns>
        private IEnumerator LerpCameraStartPlanetarySystemSimulation()
        {
            //yield return StartCoroutine(LerpCameraToPosition(MainCamera, PlanetarySystemSimulationTelescopeCamera, 1f));

            planetaryController.SetSkybox();

            Environment.SetActive(false);
            InteractablePlanetarySystemScreens.SetActive(false);

            PlanetarySystemSimulation.SetActive(true);
            uiController.ResetPlanetarySystemSimulation();
            planetaryController.DisplayMessageByKey("EnterPlanetarySystemSimulation");

            yield return null;
        }


        /// <summary>
        /// LeavePlanetorySystemSimulation and deactivates gameobjects
        /// </summary>
        public void LeavePlanetorySystemSimulation()
        {
            HelpiDialogueUI.SetActive(false);
            PlanetarySystemSimulationUI.SetActive(false);

            Environment.SetActive(true);
            InteractablePlanetarySystemScreens.SetActive(true);
            PlanetarySystemSimulation.SetActive(false);
            VRPlayer.SetActive(true);
            VRPlayer.transform.position = Vector3.zero;
            VRPlayer.transform.rotation = Quaternion.identity;
        }
        #endregion StartScreenScenes
    }
}