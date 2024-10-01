using System;
using System.Collections.Generic;
using GameLabGraz.UI;
using Maroon.scenes.experiments.FallingBallViscosimeter.Scripts;
using Maroon.UI;
using UnityEngine;
using TMPro;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Maroon.Physics.Viscosimeter
{
    public class ViscosimeterManager : MonoBehaviour, IResetObject
    {
        public static ViscosimeterManager Instance;
        private static float inaccuracyRange = 0.1f;
    
        public TMP_Text debug_text;

        public Ball ball;
        public Pycnometer pycnometer;

        public decimal fluid_density_ = 0.0m;
        public QuantityFloat fluid_temperature_ = 25.0f;
        public float FluidTemperature
        {
            get => fluid_temperature_.Value;
            set
            {
                fluid_temperature_.Value = value;
            }
        }

        public Slider temperatureSlider;

        public QuantityBool measurement_mode_ = false;
        public bool MeasurementMode
        {
            get => measurement_mode_;
            set
            {
                measurement_mode_.Value = value;
                ToggleMeasurementBoxes(value);
            }
        }
    
        public List<MeasurableObject> measurableObjects;

        private decimal ballMaxSpeed = -1.0m;

        [SerializeField] private TMP_Dropdown _fluid_dropdown;
        private FluidViscosityData[] _fluids;
        private FluidViscosityData _current_fluid;
    
        private void Awake()
        { 
#if UNITY_EDITOR
            QualitySettings.vSyncCount = 0;  // VSync must be disabled
            Application.targetFrameRate = 60;
#endif
            temperatureSlider.AllowReset(false);
            if(Instance == null)
            {
                Instance = this;
            }
            PrepareFluids();
            CalculateFluidDensity();
            GetAllMeasurableObjects();
        }

        private void PrepareFluids()
        {
            _fluids = Resources.LoadAll<FluidViscosityData>(
                "FluidData");
            _current_fluid = _fluids[0];
            foreach (var fluid in _fluids)
            {
                _fluid_dropdown.options.Add(new TMP_Dropdown.OptionData() {text=fluid.fluidName});
            }
        }

        public void OnFluidChange()
        {
            _current_fluid = _fluids[_fluid_dropdown.value];
        }
    
    
        public decimal CalculateDynamicViscosity(decimal temperature)
        {
            decimal kelvin = CelsiusToKelvin(temperature);
            return (decimal)(_current_fluid.viscosity_a * Math.Pow(_current_fluid.viscosity_b, (double)kelvin) + _current_fluid.viscosity_c);
        }
    
        public decimal CelsiusToKelvin(decimal celsius)
        {
            return celsius + 273.15m;
        }
        private void CalculateFluidDensity()
        {
            fluid_density_ = (decimal)CelsiusToKelvin((decimal)FluidTemperature) * (decimal)_current_fluid.density_a + (decimal)_current_fluid.density_b; //kg/m^3
        }

        public void TogglePycnometerFill(bool fill)
        {
            if(fill)
            {
                pycnometer.FillPycnometer();
            }
            else
            {
                pycnometer.EmptyPycnometer();
            }
        }


        private void ToggleMeasurementBoxes(bool mode)
        {
            BroadcastMessage("toggleMeasurement", mode);
        }

        public void ToggleMeasurementMode()
        {
            MeasurementMode = !MeasurementMode;
        }

        private void GetAllMeasurableObjects()
        {
            Object[] foundObjects = FindObjectsOfType<MeasurableObject>();

            measurableObjects = new List<MeasurableObject>();

            foreach (Object obj in foundObjects)
            {
                measurableObjects.Add(obj as MeasurableObject);
            }
        }

        void FixedUpdate()
        {
            CalculateFluidDensity();
            UpdateDebugText();
        }

        void UpdateDebugText()
        {
            decimal ball_velocity = ball.Velocity;
            if (ball_velocity > ballMaxSpeed)
            {
                ballMaxSpeed = ball_velocity;
            }
            debug_text.text = "Ball Velocity:\n" + ball_velocity.ToString("0.00000000") + "\nMax Vel:\n" + ballMaxSpeed.ToString("0.00000000") + "\nFluid density\n" + fluid_density_;
      
        }

        public void ResetObject()
        {
            ballMaxSpeed = -1;
        }

        public static decimal AddInaccuracy(decimal input)
        {
            decimal value = input * (decimal)inaccuracyRange;
            return input + (decimal)Random.Range(-(float)value, (float)value);
        }
    }
}