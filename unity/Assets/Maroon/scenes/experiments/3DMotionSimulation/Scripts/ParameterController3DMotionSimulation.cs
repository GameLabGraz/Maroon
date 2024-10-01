using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Maroon.GlobalEntities;

namespace Maroon
{
    public enum Parameter3DMotionSimulation 
    {
        Default,
        Satellite,
        BallInTheWind,
        ChargedParticle,
        Resonance,
        ParametricOscillator,
        RelaxationOscillator,
        RocketLaunch,
        CoupledOscillators,
    }

    public class ParameterController3DMotionSimulation : MonoBehaviour
    {
        public UnityEvent<Parameter3DMotionSimulation, int> OnParameterChanged = new UnityEvent<Parameter3DMotionSimulation, int>();
        public Parameter3DMotionSimulation CurrentParameter { get; private set; }

        private static ParameterController3DMotionSimulation _instance = null;
        public static ParameterController3DMotionSimulation Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (BootstrappingManager.Instance.UrlParameters.TryGetValue(WebGlUrlParameter.Config, out string config))
            {
                SetParameter(config);
            }
            else
#endif
            {
                SetParameter(Parameter3DMotionSimulation.Default);
            }
        }

        public void SetParameter(Parameter3DMotionSimulation parameter)
        {
            CurrentParameter = parameter;
            int parameterIndex = GetParameterIndex(CurrentParameter);
            OnParameterChanged.Invoke(CurrentParameter, parameterIndex);
        }

        public void SetParameter(string parameter)
        {
            try {
                Parameter3DMotionSimulation convertedParameter = (Parameter3DMotionSimulation)System.Enum.Parse(typeof(Parameter3DMotionSimulation), parameter);
                SetParameter(convertedParameter);
            } catch (System.ArgumentException) {
                Debug.LogError($"Parameter {parameter} is not a valid parameter for 3D Motion Simulation");
            }
        }

        public Parameter3DMotionSimulation GetParameterByIndex(int index)
        {
            Parameter3DMotionSimulation[] values = (Parameter3DMotionSimulation[])System.Enum.GetValues(typeof(Parameter3DMotionSimulation));

            if (index >= 0 && index < values.Length)
            {
                return values[index];
            }
            else
            {
                Debug.LogError($"Index {index} is out of range for Parameter3DMotionSimulation.");
                return Parameter3DMotionSimulation.Default;
            }
        }

        public int GetParameterIndex(Parameter3DMotionSimulation parameter)
        {
            Parameter3DMotionSimulation[] values = (Parameter3DMotionSimulation[])System.Enum.GetValues(typeof(Parameter3DMotionSimulation));

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == parameter)
                {
                    return i;
                }
            }

            Debug.LogError($"Parameter {parameter} is not a valid parameter for 3D Motion Simulation");
            return -1;
        }

        public List<string> GetParameterNames()
        {
            Parameter3DMotionSimulation[] values = (Parameter3DMotionSimulation[])System.Enum.GetValues(typeof(Parameter3DMotionSimulation));
            List<string> names = new List<string>();
            
            foreach (Parameter3DMotionSimulation value in values)
            {
                names.Add(value.ToString());
            }

            return names;
        }
    }
}