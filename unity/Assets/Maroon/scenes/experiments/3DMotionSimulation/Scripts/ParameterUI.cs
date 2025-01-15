using System.Collections.Generic;
using GameLabGraz.UI;
using UnityEngine;
using TMPro;
using Maroon.UI;
using GEAR.Localization;
using Maroon.Physics;
using Maroon.Physics.ThreeDimensionalMotion;
using Maroon.Parameter.ObjectsInUse;
using Maroon.ReusableScripts.ExperimentParameters;

// IMPORTS USED FOR WEBGL
#if UNITY_WEBGL && !UNITY_EDITOR
using Maroon.GlobalEntities;
#endif

namespace Maroon.Parameter
{
    namespace ObjectsInUse
    {
        public enum ParticleObject
        {
            Default,
            Ball,
            Rocket,
            Satellite
        }
    }

    public class ParameterUI : PausableObject
    {
        private ParticleObject _particleInUse = ParticleObject.Default;
        [SerializeField] private GameObject _parameters;
        [SerializeField] private GameObject _initialConditions;
        [SerializeField] private GameObject _dataVisualization;

        private static ParameterUI _instance;
        public static ParameterUI Instance => _instance;
        private DialogueManager _dialogueManager;
    
        public TMP_Dropdown dropdown;

        [SerializeField] private UnityEngine.UI.Button _showInputPanelButton;
        [SerializeField] private TMP_Text _inputPanelButtonText;
        [SerializeField] private UnityEngine.UI.Button _showDataVisualizationButton;
        [SerializeField] private TMP_Text _dataVisualizationButtonText;
        private bool _showInputPanel = true;
        private bool _showDataPanel = true;

        private string _background = "ExperimentRoom";

        [SerializeField] InputField fxIF;
        [SerializeField] InputField fyIF;
        [SerializeField] InputField fzIF;

        private Vector3 _toDeltatSteps = new Vector3(0, 0, 0);
        private Vector3 _xyz = new Vector3(0, 0, 0);
        private Vector3 _vxvyvz = new Vector3(0, 0, 0);

        private float _mass = 1f;
        private float _t0 = 0f;
        private float _deltaT = 0.05f;
        private float _steps = 500f;

        private float _x = 0f;
        private float _y = 0f;
        private float _z = 0f;

        private float _vx = 1f;
        private float _vy = 0f;
        private float _vz = 0f;

        [SerializeField] InputField ifMass;

        [SerializeField] InputField ifT0;
        [SerializeField] InputField ifDeltat;
        [SerializeField] InputField ifSteps;

        [SerializeField] InputField ifX;
        [SerializeField] InputField ifY;
        [SerializeField] InputField ifZ;

        [SerializeField] InputField ifVx;
        [SerializeField] InputField ifVy;
        [SerializeField] InputField ifVz;

        [SerializeField] private UnityEngine.UI.Toggle _showLabel;
        [SerializeField] private UnityEngine.UI.Toggle _showOriginGrid;

        private int _currentConfigIndex = 0;

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
            ParameterLoader.Instance.OnFilesInitialized.AddListener(OnFilesLoadedInital);
        }

        /// <summary>
        /// Inits the dictionary for the formulas and handles the visibility of UI elements
        /// Displays the welcome message
        /// </summary>
        protected override void Start()
        {
            if (_dialogueManager == null)
                _dialogueManager = FindObjectOfType<DialogueManager>();

            string message = LanguageManager.Instance.GetString("Welcome");
            DisplayMessage(message);
        }

        public void OnFilesLoadedInital()
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(ParameterLoader.Instance.GetJsonNames());

#if UNITY_WEBGL && !UNITY_EDITOR
            if (BootstrappingManager.Instance.UrlParameters.TryGetValue(WebGlUrlParameter.Config, out string config))
            {
                if (!ApplyConfig(config)) ApplyConfig("Default");
            }
            else
#endif
            {
                ApplyConfig("Default");
            }
            
            ParameterLoader.Instance.OnFilesInitialized.RemoveListener(OnFilesLoadedInital);
        }

        public bool ApplyConfig(string configName)
        {
            _currentConfigIndex = ParameterLoader.Instance.IndexOfJson(configName);
            dropdown.SetValueWithoutNotify(_currentConfigIndex);
            var parameters = ParameterLoader.Instance.LoadJsonFromFileName(configName);
            
            return parameters != null;
        }

        /// <summary>
        /// Update is called every frame
        /// </summary>
        protected override void HandleUpdate()
        {

        }

        /// <summary>
        /// Dynamic Show/Hide functionality of the UI panels
        /// </summary>
        protected override void HandleFixedUpdate()
        {

        }

        /// <summary>
        /// Show/Hide the Input Panel
        /// </summary>
        public void ToggleInputPanel()
        {
            _showInputPanel = !_showInputPanel;

            _parameters.SetActive(_showInputPanel);
            _initialConditions.SetActive(_showInputPanel);
            _inputPanelButtonText.text = _showInputPanel ? 
                LanguageManager.Instance.GetString("HideInputField") :
                LanguageManager.Instance.GetString("ShowInputField");
        }

        /// <summary>
        /// Show/Hide Data Visualization Panel
        /// </summary>
        public void ToggleDataVisualizationPanel()
        {
            _showDataPanel = !_showDataPanel;

            _dataVisualization.SetActive(_showDataPanel);
            _dataVisualizationButtonText.text = _showDataPanel ?
                LanguageManager.Instance.GetString("HideDataVisualization") :
                LanguageManager.Instance.GetString("ShowDataVisualization");
        }

        /// <summary>
        /// Gets the Function Fx from the UI and brings the formula to the right form
        /// </summary>
        /// <returns>The corrected formula Fx</returns>
        public string GetFunctionFx()
        {
            return fxIF.text;
        }

        /// <summary>
        /// Gets the Function Fy from the UI and brings the formula to the right form
        /// </summary>
        /// <returns>The corrected formula Fy</returns>
        public string GetFunctionFy()
        {
            return fyIF.text;
        }

        /// <summary>
        /// Gets the Function Fz from the UI and brings the formula to the right form
        /// </summary>
        /// <returns>The corrected formula Fz</returns>
        public string GetFunctionFz()
        {
            return fzIF.text;
        }

        /// <summary>
        /// Gets the mass from the UI and checks the value range
        /// </summary>
        /// <returns>The mass</returns>
        public string GetMass()
        {
            return ifMass.text;
        }
        /// <summary>
        /// Sets the value for mass from the UI
        /// </summary>
        /// <param name="value">Mass</param>
        public void SetMass(string value)
        {
            _mass = float.Parse(value); 
        }

        /// <summary>
        /// Sets the value for t0 from the UI
        /// </summary>
        /// <param name="value">T0</param>
        public void SetT0(string value)
        {
            _t0 = System.Convert.ToSingle(value);
        }

        /// <summary>
        /// Sets the value for DeltaT from the UI
        /// </summary>
        /// <param name="value">DeltaT</param>
        public void SetDeltaT(string value)
        {
            _deltaT = float.Parse(value);
        }

        /// <summary>
        /// Sets the value for steps from the UI
        /// </summary>
        /// <param name="value">Steps</param>
        public void SetSteps(string value)
        {
            _steps = System.Convert.ToSingle(value);
        }

        /// <summary>
        /// Getter for t0, deltaT and steps. Adapts values if not in valid range
        /// </summary>
        /// <returns>T0, deltaT and steps</returns>
        public Vector3 GetTimes()
        {
            if (_t0 < 0)
            {
                ShowError(LanguageManager.Instance.GetString("T0Error"));
                _t0 = 0f;
                ifT0.text = "0";
            }
            if (_deltaT <= 0)
            {
                ShowError(LanguageManager.Instance.GetString("DeltaTError"));
                _deltaT = 1f;
                ifDeltat.text = "1";
            }     
            if (_steps <= 0)
            {
                ShowError(LanguageManager.Instance.GetString("StepSizeError"));
                _steps = 1f;
                ifSteps.text = "1";
            }        

            _toDeltatSteps = new Vector3(_t0, _deltaT, _steps);

            return _toDeltatSteps;
        }

        /// <summary>
        /// Sets the value for x from the UI
        /// </summary>
        /// <param name="value">X</param>
        public void SetX(string value)
        {
            _x = System.Convert.ToSingle(value);
        }

        /// <summary>
        /// Sets the value for y from the UI
        /// </summary>
        /// <param name="value">Y</param>
        public void SetY(string value)
        {
            _y = System.Convert.ToSingle(value);
        }

        /// <summary>
        /// Sets the value for z from the UI
        /// </summary>
        /// <param name="value">Z</param>
        public void SetZ(string value)
        {
            _z = System.Convert.ToSingle(value);
        }

        /// <summary>
        /// Getter for x,y and z values
        /// </summary>
        /// <returns>x,y,z</returns>
        public Vector3 GetXYZ()
        {
            _xyz = new Vector3(_x, _y, _z);

            return _xyz;
        }

        /// <summary>
        /// Sets the value for vx from the UI
        /// </summary>
        /// <param name="value">Vx</param>
        public void SetVX(string value)
        {
            _vx = System.Convert.ToSingle(value);
        }

        /// <summary>
        /// Sets the value for vy from the UI
        /// </summary>
        /// <param name="value">Vy</param>
        public void SetVY(string value)
        {
            _vy = System.Convert.ToSingle(value);
        }

        /// <summary>
        /// Sets the value for vz from the UI
        /// </summary>
        /// <param name="value">Vzx</param>
        public void SetVZ(string value)
        {
            _vz = System.Convert.ToSingle(value);
        }

        /// <summary>
        /// Getter for Vx,Vy and Vz values
        /// </summary>
        /// <returns>Vx,Vy,Vz</returns>
        public Vector3 GetVxVyVz()
        {
            _vxvyvz = new Vector3(_vx, _vy, _vz);

            return _vxvyvz;
        }

        /// <summary>
        /// Loads the chosen parameters from the JSON file 
        /// </summary>
        /// <param name="choice">The choice from the UI (Dropdown menu)</param>
        public void DropdownListener(int choice)
        {
            string configName = ParameterLoader.Instance.GetJsonNames()[choice];
            ApplyConfig(configName);
        }

        public void OnLoadedExperimentParameters(ExperimentParameters experimentParameters)
        {
            if (experimentParameters is ThreeDimensionalMotionParameters threeDimensionalMotionParameters)
            {
                LoadExperimentParameters(threeDimensionalMotionParameters);
            }
            else
            {
                Debug.LogError("OnLoadedExperimentParameters requires ThreeDimensionalMotionParameters");
            }
        }


        private void LoadExperimentParameters(ThreeDimensionalMotionParameters parameters)
        {
            _background = parameters.Background;

            _particleInUse = parameters.Particle?.ToLower() switch
            {
                "ball" => ParticleObject.Ball,
                "rocket" => ParticleObject.Rocket,
                "satellite" => ParticleObject.Satellite,
                _ => ParticleObject.Default
            };

            fxIF.text = parameters.fx;
            fyIF.text = parameters.fy;
            fzIF.text = parameters.fz;

            ifMass.text = parameters.m;
            ifT0.text = parameters.T0.ToString();
            ifDeltat.text = parameters.DeltaT.ToString();
            ifSteps.text = parameters.Steps.ToString();

            ifX.text = parameters.X.ToString();
            ifY.text = parameters.Y.ToString();
            ifZ.text = parameters.Z.ToString();

            ifVx.text = parameters.Vx.ToString();
            ifVy.text = parameters.Vy.ToString();
            ifVz.text = parameters.Vz.ToString();

            SkyboxController.Instance.SetBackground(_background);

            MotionCalculation.Instance.ResetObject();
            ValueGraph.Instance.ResetObject();
        }

        public void OnEndEdit(string param)
        {
            MotionCalculation.Instance.ResetObject();
        }

        public Dictionary<string,string> GetExpressions()
        {
            return ((ThreeDimensionalMotionParameters) ParameterLoader.Instance.MostRecentParameters).expressions;
        }

        /// <summary>
        /// Displays a message on the screen. (welcome message, error messages)
        /// </summary>
        /// <param name="message">Message to show</param>
        public void DisplayMessage(string message)
        {
            if (_dialogueManager == null)
                return;

            _dialogueManager.ShowMessage(message);
        }

        /// <summary>
        /// Getter for the object in use (e.g Ball or Satellite)
        /// </summary>
        /// <returns>Object in use</returns>
        public ParticleObject GetObjectInUse()
        {
            return _particleInUse;
        }

        /// <summary>
        /// Getter for the background of the experiment
        /// </summary>
        /// <returns>Background</returns>
        public string GetBackground()
        {
            return _background;
        }

        /// <summary>
        /// Resets the object
        /// </summary>
        public void ResetObject()
        {
        }

        /// <summary>
        /// Function to display error messages
        /// </summary>
        /// <param name="message">Message to show</param>
        private void ShowError(string message)
        {
            DisplayMessage(message);
        }
    }
}
