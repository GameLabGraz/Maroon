using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace Maroon.Experiments.PlanetarySystem
{
    public class StartScreenScenes : MonoBehaviour
    {
        public PlanetaryController planetaryController;

        public GameObject SortingMinigame;
        public GameObject SortingGamePlanetInfoUI;
        public GameObject FormulaUI;
        public GameObject Animation;
        public GameObject AnimationUI;
        public GameObject Environment;
        public GameObject Interactibles;
        public GameObject MainCamera;
        public GameObject SortingGameCamera;
        public GameObject TelescopeCamera;
        public GameObject SolarSystemAnimationCamera;
        public Toggle toggleSunLight;
        public Toggle toggleAllTrajectories;
        public GameObject HelpiDialogueUI;

        private Vector3 initialMainCameraPosition;
        private Quaternion initialMainCameraRotation;
        private float initialMainCameraFOV;


        private void Awake()
        {
            SortingMinigame.SetActive(false);
            SortingGamePlanetInfoUI.SetActive(false);
        }


        private void Start()
        {
            StoreInitialMainCamera();
        }


        /*
         * store the StoreInitialCameras's position, rotation, and field of view
         * LERPs the currentCamera(MainCamera) to the targetCameras position
         * targetCameras are just used for theire position not for theire view
         * reverse LERP camera
         */
        #region LerpCamera
        /*
         * store the StoreInitialMainCamera's position, rotation, and field of view
         */
        private void StoreInitialMainCamera()
        {
            if (MainCamera == null)
            {
                Debug.Log("StartScreenScenes: StoreInitialCameras(): MainCamera missing");
            }

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
            LeaveAnimation();
            SortingMinigame.SetActive(true);
            StartCoroutine(LerpCameraStartSortingGame());
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

            planetaryController.UIToggleSGRotation(true);
            planetaryController.DisplayMessageByKey("EnterSortingGame");
        }


        /*
         * LeaveSortingGame and deactivates gameobjects
         */
        public void LeaveSortingGame()
        {
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

            toggleSunLight.isOn = false;
            planetaryController.UIToggleSunLight(false);
            SortingMinigame.SetActive(false);
        }


        /*
         * StartAnimationOnInput and calls LERP camera couroutine
         */
        public void StartAnimationOnInput()
        {
            LeaveSortingGame();
            FormulaUI.SetActive(false);

            toggleAllTrajectories.isOn = true;
            planetaryController.UIToggleAllTrajectories(true);

            StartCoroutine(LerpCameraStartAnimation());
        }


        /*
         * Waits for LERP camera couroutine and de/activates gameobjects
         */
        private IEnumerator LerpCameraStartAnimation()
        {
            yield return StartCoroutine(LerpCameraToPosition(MainCamera, TelescopeCamera, 1f));

            AnimationUI.SetActive(true);
            planetaryController.SetSkybox();

            Environment.SetActive(false);
            Interactibles.SetActive(false);

            Animation.SetActive(true);

            MainCamera.SetActive(false);
            SolarSystemAnimationCamera.SetActive(true);

            planetaryController.ResetAnimation();
            planetaryController.DisplayMessageByKey("EnterAnimation");
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
            planetaryController.ResetCamera();

            SolarSystemAnimationCamera.SetActive(false);
            MainCamera.SetActive(true);
        }
        #endregion StartScreenScenes
    }
}
