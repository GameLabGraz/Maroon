using UnityEngine;
using UnityEngine.UI;

namespace Maroon.Experiments.PlanetarySystem
{
    public class PlanetarySimulationUIController : MonoBehaviour
    {
        public PlanetaryController planetaryController;
        public PlanetTrajectoryController planetTrajectoryController;
        public PlanetSortingGameController planetSortingGameController;

        [SerializeField] private float gravitationalConstantG = 6.674f;
        [SerializeField] private float timeSpeed = 1f;

        public Slider sliderG;
        public Slider sliderTimeSpeed;
        public Slider sliderAnimationCameraFov;

        public Toggle toggleAllTrajectories;
        public Toggle toggleARotation;
        public Toggle toggleSunKinematic;
        public Toggle toggleAOrientationGizmo;
        public Toggle toggleSGOrientationGizmo;
        public Toggle toggleSGRotation;
        public Toggle toggleSunLight;
        public Toggle toggleSolarFlares;
        public Toggle[] planetToggles;


        /// <summary>
        /// setup listeners and sliders
        /// </summary>
        private void Start()
        {
            SetupListeners();
            SetupSliders();
        }


        /// <summary>
        /// Initialize UI event listeners for toggle buttons
        /// </summary>
        private void SetupListeners()
        {
            sliderG.onValueChanged.AddListener(OnGSliderValueChanged);
            sliderTimeSpeed.onValueChanged.AddListener(OnTimeSliderValueChanged);
            sliderAnimationCameraFov.onValueChanged.AddListener(OnFOVSliderValueChanged);

            toggleAllTrajectories.onValueChanged.AddListener((bool isOn) => planetTrajectoryController.ToggleAllTrajectories(isOn));
            toggleARotation.onValueChanged.AddListener((bool isOn) => planetaryController.ToggleARotation(!isOn));
            toggleSunKinematic.onValueChanged.AddListener((bool isOn) => planetaryController.ToggleSunKinematic(isOn));
            toggleAOrientationGizmo.onValueChanged.AddListener((bool isOn) => planetaryController.ToggleAOrientation(isOn));

            toggleSGOrientationGizmo.onValueChanged.AddListener((bool isOn) => planetSortingGameController.ToggleSGOrientation(isOn));
            toggleSGRotation.onValueChanged.AddListener((bool isOn) => planetSortingGameController.ToggleSGRotation(isOn));
            toggleSunLight.onValueChanged.AddListener((bool isOn) => planetSortingGameController.ToggleSunLight(isOn));
            toggleSolarFlares.onValueChanged.AddListener((bool isOn) => planetSortingGameController.ToggleSolarFlares(isOn));
   
            for (int index = 0; index < planetToggles.Length; index++)
            {
                int planet_index = index;
                planetToggles[index].onValueChanged.AddListener((bool isOn) => planetaryController.TogglePlanet(isOn, planet_index));
            }
        }


        /// <summary>
        /// Removes event listeners for toggle buttons
        /// </summary>
        private void OnDestroy()
        {
            sliderG.onValueChanged.RemoveListener(OnGSliderValueChanged);
            sliderTimeSpeed.onValueChanged.RemoveListener(OnTimeSliderValueChanged);
            sliderAnimationCameraFov.onValueChanged.RemoveListener(OnFOVSliderValueChanged);

            toggleAllTrajectories.onValueChanged.RemoveListener((bool isOn) => planetTrajectoryController.ToggleAllTrajectories(isOn));
            toggleARotation.onValueChanged.RemoveListener((bool isOn) => planetaryController.ToggleARotation(isOn));
            toggleSunKinematic.onValueChanged.RemoveListener((bool isOn) => planetaryController.ToggleSunKinematic(isOn));
            toggleAOrientationGizmo.onValueChanged.RemoveListener((bool isOn) => planetaryController.ToggleAOrientation(isOn));

            toggleSGOrientationGizmo.onValueChanged.RemoveListener((bool isOn) => planetSortingGameController.ToggleSGOrientation(isOn));
            toggleSGRotation.onValueChanged.RemoveListener((bool isOn) => planetSortingGameController.ToggleSGRotation(isOn));
            toggleSunLight.onValueChanged.RemoveListener((bool isOn) => planetSortingGameController.ToggleSunLight(isOn));
            toggleSolarFlares.onValueChanged.RemoveListener((bool isOn) => planetSortingGameController.ToggleSolarFlares(isOn));
    
            for (int index = 0; index < planetToggles.Length; index++)
            {
                planetToggles[index].onValueChanged.RemoveAllListeners();
            }
        }


        /// SetupSlider FOV, G, time
        #region sliders
        /// <summary>
        /// SetupSlider FOV, G, time
        /// </summary>
        public void SetupSliders()
        {
            if (sliderG != null)
            {
                sliderG.minValue = 0f;
                sliderG.maxValue = 25f;
                sliderG.value = gravitationalConstantG;
            }

            if (sliderTimeSpeed != null)
            {
                sliderTimeSpeed.minValue = 0f;
                sliderTimeSpeed.maxValue = 35f;
                sliderG.value = timeSpeed;
            }

            if (sliderAnimationCameraFov != null)
            {
                sliderAnimationCameraFov.minValue = 10;
                sliderAnimationCameraFov.maxValue = 180;
                sliderAnimationCameraFov.value = planetaryController.AnimationCamera.fieldOfView;
            }
        }


        /// <summary>
        /// changes the G value after slider input
        /// </summary>
        /// <param name="gValue"></param>
        public void OnGSliderValueChanged(float gValue)
        {
            planetaryController.G = gValue;
        }


        /// <summary>
        /// changes the time/speed value after slider input
        /// </summary>
        /// <param name="timeSpeedValue"></param>
        public void OnTimeSliderValueChanged(float timeSpeedValue)
        {
            Time.timeScale = timeSpeedValue;
        }


        /// <summary>
        /// changes the FOV value after slider input
        /// </summary>
        /// <param name="fovValue"></param>
        public void OnFOVSliderValueChanged(float fovValue)
        {
            planetaryController.AnimationCamera.fieldOfView = fovValue;
        }


        /// <summary>
        /// reset slider values
        /// </summary>
        public void ResetAnimationValues()
        {
            sliderG.value = gravitationalConstantG;
            sliderTimeSpeed.value = timeSpeed;
        }
        #endregion sliders


        /// <summary>
        /// ResetAnimation on reset and on StartAnimation
        /// </summary>
        
        public void ResetAnimation()
        {
            bool hide = false;
            for (int index = 0; index < planetToggles.Length; index++)
            {
                planetaryController.TogglePlanet(hide, index);
            }

            planetTrajectoryController.ClearTrajectories();
            ResetCamera();
            ResetAnimationValues();
            planetaryController.ResetAnimationPlanets();
        }


        /// <summary>
        /// reset the camera's position and FOV to initial values
        /// </summary>
        public void ResetCamera()
        {
            planetaryController.AnimationCamera.transform.SetPositionAndRotation(planetaryController.initialAnimationCameraPosition, planetaryController.initialAnimationCameraRotation);
            planetaryController.AnimationCamera.fieldOfView = planetaryController.initialAnimationCameraFov;

            sliderAnimationCameraFov.value = planetaryController.initialAnimationCameraFov;
        }
    }
}