using System.Collections.Generic;
using GameLabGraz.VRInteraction;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.Experiments.PlanetarySystem
{
    public class PlanetarySimulationUIController : MonoBehaviour, IResetObject
    {
        public PlanetaryController planetaryController;
        public PlanetTrajectoryController planetTrajectoryController;
        public PlanetSortingGameController planetSortingGameController;

        [SerializeField] private float gravitationalConstantG = 6.674f;
        [SerializeField] private float timeSpeed = 1f;

        public VRLinearDrive sliderG;
        public VRLinearDrive sliderTimeSpeed;
        public Slider sliderSimulationCameraFov;

        public GameObject Back;
        public GameObject Timeto1Button;
        public GameObject SliderG;
        public GameObject SliderTimeSpeed;
        public Toggle toggleAllTrajectories;
        public Toggle toggleSimRotation;
        public Toggle toggleSunKinematic;
        public Toggle toggleSimOrientationGizmo;
        public Toggle toggleSGOrientationGizmo;
        public Toggle toggleSGRotation;
        public Toggle toggleSunLight;
        public Toggle toggleSolarFlares;
        public Toggle[] planetToggles;
        
        //---------------------------------------------------------------------------------------


        /// <summary>
        /// setup listeners and sliders
        /// </summary>
        private void Start()
        {
            SetupListeners();
            SetupSliders();
        }


        /// <summary>
        /// CheckCollisionsWithSun
        /// </summary>
        private void FixedUpdate()
        {
            CheckCollisionsWithSun();
        }


        /// <summary>
        /// Initialize UI event listeners for toggle buttons
        /// </summary>
        private void SetupListeners()
        {
            sliderG.onValueChanged.AddListener(OnGSliderValueChanged);
            sliderTimeSpeed.onValueChanged.AddListener(OnTimeSliderValueChanged);
            sliderSimulationCameraFov.onValueChanged.AddListener(OnFOVSliderValueChanged);

            toggleAllTrajectories.onValueChanged.AddListener((bool isOn) => planetTrajectoryController.ToggleAllTrajectories(isOn));
            toggleSimRotation.onValueChanged.AddListener((bool isOn) => planetaryController.ToggleSimRotation(isOn));
            toggleSunKinematic.onValueChanged.AddListener((bool isOn) => planetaryController.ToggleSunKinematic(isOn));
            toggleSimOrientationGizmo.onValueChanged.AddListener((bool isOn) => planetaryController.ToggleSimOrientation(isOn));

            toggleSGOrientationGizmo.onValueChanged.AddListener((bool isOn) => planetSortingGameController.ToggleSGOrientation(isOn));
            toggleSGRotation.onValueChanged.AddListener((bool isOn) => planetSortingGameController.ToggleSGRotation(isOn));
            toggleSunLight.onValueChanged.AddListener((bool isOn) => planetSortingGameController.ToggleSunLight(isOn));
            toggleSolarFlares.onValueChanged.AddListener((bool isOn) => planetSortingGameController.ToggleSolarFlares(isOn));
   
            for (int index = 0; index < planetToggles.Length; index++)
            {
                int planet_index = index;
                planetToggles[index].onValueChanged.AddListener((bool isOn) => TogglePlanet(isOn, planet_index));
            }
        }


        /// <summary>
        /// Removes event listeners for toggle buttons
        /// </summary>
        private void OnDestroy()
        {
            sliderG.onValueChanged.RemoveListener(OnGSliderValueChanged);
            sliderTimeSpeed.onValueChanged.RemoveListener(OnTimeSliderValueChanged);
            sliderSimulationCameraFov.onValueChanged.RemoveListener(OnFOVSliderValueChanged);

            toggleAllTrajectories.onValueChanged.RemoveListener((bool isOn) => planetTrajectoryController.ToggleAllTrajectories(isOn));
            toggleSimRotation.onValueChanged.RemoveListener((bool isOn) => planetaryController.ToggleSimRotation(isOn));
            toggleSunKinematic.onValueChanged.RemoveListener((bool isOn) => planetaryController.ToggleSunKinematic(isOn));
            toggleSimOrientationGizmo.onValueChanged.RemoveListener((bool isOn) => planetaryController.ToggleSimOrientation(isOn));

            toggleSGOrientationGizmo.onValueChanged.RemoveListener((bool isOn) => planetSortingGameController.ToggleSGOrientation(isOn));
            toggleSGRotation.onValueChanged.RemoveListener((bool isOn) => planetSortingGameController.ToggleSGRotation(isOn));
            toggleSunLight.onValueChanged.RemoveListener((bool isOn) => planetSortingGameController.ToggleSunLight(isOn));
            toggleSolarFlares.onValueChanged.RemoveListener((bool isOn) => planetSortingGameController.ToggleSolarFlares(isOn));
    
            for (int index = 0; index < planetToggles.Length; index++)
            {
                planetToggles[index].onValueChanged.RemoveAllListeners();
            }
        }
        #region Buttons
        /// <summary>
        /// Sunlight toggle with button
        /// </summary>
        public void ToggleSunLightWithButton()
        {
            bool newValue = !toggleSunLight.isOn;
            toggleSunLight.isOn = newValue;
        }

        /// <summary>
        /// Rotation toggle with button
        /// </summary>
        public void ToggleSGRotationWithButton()
        {
            bool newValue = !toggleSGRotation.isOn;
            toggleSGRotation.isOn = newValue;
        }

        /// <summary>
        /// Orientation toggle with button
        /// </summary>
        public void ToggleSGOrientationWithButton()
        {
            bool newValue = !toggleSGOrientationGizmo.isOn;
            toggleSGOrientationGizmo.isOn = newValue;
        }

        /// <summary>
        /// Solar Flare toggle with button
        /// </summary>
        public void ToggleSolarFlaresWithButton()
        {
            bool newValue = !toggleSolarFlares.isOn;
            toggleSolarFlares.isOn = newValue;
        }

        public void ToggleOptionMenuSimulation()
        {
            bool newValue = !Back.activeSelf;
            Back.SetActive(newValue);
            newValue = !SliderG.activeSelf;
            SliderG.SetActive(newValue);
            newValue = !SliderTimeSpeed.activeSelf;
            SliderTimeSpeed.SetActive(newValue);
            newValue = !Timeto1Button.activeSelf;
            Timeto1Button.SetActive(newValue);
        }

        public void setTimeto1()
        {
            sliderTimeSpeed.ForceToValue(timeSpeed);
        }

        #endregion Buttons
        /// SetupSlider FOV, G, time
        #region sliders
        /// <summary>
        /// SetupSlider FOV, G, time
        /// </summary>
        public void SetupSliders()
        {
            //            if (sliderG != null)
            //            {
            //                sliderG.minimum = 0f;
            //                sliderG.maximum = 25f;
            //                sliderG.ForceToValue(gravitationalConstantG);
            //            }

            //            if (sliderTimeSpeed != null)
            //            {
            //                sliderTimeSpeed.minimum = 0f;
            //                sliderTimeSpeed.maximum = 35f;
            //                sliderTimeSpeed.ForceToValue(timeSpeed);
            //            }

            if (sliderSimulationCameraFov != null)
            {
                sliderSimulationCameraFov.minValue = 10;
                sliderSimulationCameraFov.maxValue = 180;
                sliderSimulationCameraFov.value = planetaryController.SimulationCamera.fieldOfView;
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
            planetaryController.SimulationCamera.fieldOfView = fovValue;
        }


        /// <summary>
        /// hides planets and trajectories after radiobutton is pressed
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="index"></param>
        public void TogglePlanet(bool isOn, int index)
        {
            //Debug.Log(planets[index].name + " checkbox: " + !isOn);
            planetaryController.planets[index].SetActive(!isOn);

            planetTrajectoryController.ToggleTrajectory(index, !isOn);
            planetToggles[index].isOn = isOn;
        }


        /// <summary>
        /// check if planet collides with sun and toggles the planet if there is a collision
        /// </summary>
        private void CheckCollisionsWithSun()
        {
            for (int index = 1; index < planetaryController.planets.Length; index++)
            {
                if (planetaryController.CheckCollisionWithSun(planetaryController.planets[index]))
                {
                    //Debug.Log("PlanetaryController: CheckCollisionsWithSun(): " + planets[index] + " collides with sun");
                    bool hide = true;
                    TogglePlanet(hide, index);
                }
            }
        }


        /// <summary>
        /// reset slider values
        /// </summary>
        public void ResetPlanetarySystemSimulationValues()
        {
            bool isOn = true;
            sliderG.ForceToValue(gravitationalConstantG);
            sliderTimeSpeed.ForceToValue(timeSpeed);

            toggleAllTrajectories.isOn = isOn;
            planetTrajectoryController.ToggleAllTrajectories(isOn);
            toggleSunKinematic.isOn = isOn;
            planetaryController.ToggleSunKinematic(isOn);


        }
        #endregion sliders


        /// <summary>
        /// ResetPlanetarySystemSimulation on reset and on StartPlanetarySystemSimulation
        /// </summary>

        public void ResetPlanetarySystemSimulation()
        {
            bool hide = false;
            for (int index = 0; index < planetToggles.Length; index++)
            {
                TogglePlanet(hide, index);
            }

            planetTrajectoryController.ClearTrajectories();
            ResetCamera();
            ResetPlanetarySystemSimulationValues();
            planetaryController.ResetSimulationPlanets();
        }


        /// <summary>
        /// reset the camera's position and FOV to initial values
        /// </summary>
        public void ResetCamera()
        {
            planetaryController.SimulationCamera.transform.SetPositionAndRotation(planetaryController.initialSimulationCameraPosition, planetaryController.initialSimulationCameraRotation);
            planetaryController.SimulationCamera.fieldOfView = planetaryController.initialSimulationCameraFov;

            sliderSimulationCameraFov.value = planetaryController.initialSimulationCameraFov;
        }


        /// <summary>
        /// reset
        /// </summary>
        public void ResetObject()
        {
            Debug.Log("PlanetaryController: ResetObject(): button pressed");
            ResetPlanetarySystemSimulation();
        }
    }
}