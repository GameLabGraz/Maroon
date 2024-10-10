using System.Collections;
using UnityEngine;


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
        public GameObject HelpiDialogueUI;

        private Vector3 initialMainCameraPosition;
        private Quaternion initialMainCameraRotation;
        private float initialMainCameraFOV;

        /// <summary>
        /// deactivate SortingMinigame
        /// </summary>
        private void Awake()
        {
            SortingMinigame.SetActive(false);
            SortingGamePlanetInfoUI.SetActive(false);
        }


        private void Start()
        {
            StoreInitialMainCamera();
        }


         // LERPs the currentCamera(MainCamera) to the targetCameras position
        #region LerpCamera
        /// <summary>
        /// store the StoreInitialMainCamera's position, rotation, and field of view
        /// </summary>
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


        // StartSortingGameOnInput or StartAnimationOnInputand de/activates gameobjects after coroutine has finished 
        #region StartScreenScenes
        /// <summary>
        /// StartSortingGameOnInput and calls LERP camera couroutine
        /// </summary>
        public void StartSortingGameOnInput()
        {
            LeaveAnimation();
            SortingMinigame.SetActive(true);
            StartCoroutine(LerpCameraStartSortingGame());
        }

        /// <summary>
        /// waits for LERP camera couroutine and de/activates gameobjects
        /// </summary>
        /// <returns></returns>
        private IEnumerator LerpCameraStartSortingGame()
        {
            yield return StartCoroutine(LerpCameraToPosition(MainCamera, SortingGameCamera, 1f));
            SortingMinigame.SetActive(true);
            SortingGamePlanetInfoUI.SetActive(true);
            FormulaUI.SetActive(false);

            planetaryController.ToggleSGRotation(true);
            planetaryController.DisplayMessageByKey("EnterSortingGame");
        }


        /// <summary>
        /// LeaveSortingGame and deactivates gameobjects
        /// </summary>
        public void LeaveSortingGame()
        {
            HelpiDialogueUI.SetActive(false);
            SortingGamePlanetInfoUI.SetActive(false);
            FormulaUI.SetActive(false);

            StartCoroutine(LerpCameraLeave());
        }


        /// <summary>
        /// reverse LERP camera when leaving a ScreenScene
        /// </summary>
        /// <returns></returns>
        private IEnumerator LerpCameraLeave()
        {
            yield return StartCoroutine(LerpCameraToInitialPosition(MainCamera, 0.5f));

            planetaryController.ToggleSunLight(false);
            SortingMinigame.SetActive(false);
        }


        /// <summary>
        /// StartAnimationOnInput and calls LERP camera couroutine
        /// </summary>
        public void StartAnimationOnInput()
        {
            LeaveSortingGame();
            FormulaUI.SetActive(false);

            planetaryController.ToggleAllTrajectories(true);

            StartCoroutine(LerpCameraStartAnimation());
        }


        /// <summary>
        /// waits for LERP camera couroutine and de/activates gameobjects
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// LeaveAnimation and deactivates gameobjects
        /// </summary>
        public void LeaveAnimation()
        {
            HelpiDialogueUI.SetActive(false);
            AnimationUI.SetActive(false);

            Environment.SetActive(true);
            Interactibles.SetActive(true);

            Animation.SetActive(false);
            SolarSystemAnimationCamera.SetActive(false);
            MainCamera.SetActive(true);
        }
        #endregion StartScreenScenes
    }
}
