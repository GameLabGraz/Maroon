using System.Collections.Generic;
using GameLabGraz.UI;
using UnityEngine;
using TMPro;
using Maroon.UI;
using ObjectsInUse;
using GEAR.Localization;
using Maroon.Physics;

namespace ObjectsInUse
{
    public enum ParticleObject
    {
        Default,
        Ball,
        Satellite
    }
}

public class ParameterUI : PausableObject, IResetObject
{
    private ParticleObject _particleInUse = ParticleObject.Default;
    [SerializeField] private GameObject _parameters;
    [SerializeField] private GameObject _initialConditions;
    [SerializeField] private GameObject _dataVisualization;

    private static ParameterUI _instance;
    private DialogueManager _dialogueManager;

    private Dictionary<string, string> _functions = new Dictionary<string, string>();
   
    public TMP_Dropdown dropdown;
    List<TMP_Dropdown.OptionData> menuOptions;

    [SerializeField] private UnityEngine.UI.Button _showInputPanelButton;
    [SerializeField] private TMP_Text _inputPanelButtonText;
    [SerializeField] private UnityEngine.UI.Button _showDataVisualizationButton;
    [SerializeField] private TMP_Text _dataVisualizationButtonText;
    private bool _showInputPanel = false;
    private bool _showDataPanel = false;

    private string _background = "ExperimentRoom";
    private bool _resetBackground = true;
    private bool _dropdownReset = false;

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

    /// <summary>
    /// Inits the dictionary for the formulas and handles the visibility of UI elements
    /// Displays the welcome message
    /// </summary>
    protected override void Start()
    {
        if (_dialogueManager == null)
            _dialogueManager = FindObjectOfType<DialogueManager>();

        _functions.Add("abs", "Abs");
        _functions.Add("acos", "Acos");
        _functions.Add("asin", "Asin");
        _functions.Add("atan", "Atan");
        _functions.Add("ceiling", "Ceiling");
        _functions.Add("cos", "Cos");
        _functions.Add("exp", "Exp");
        _functions.Add("floor", "Floor");
        _functions.Add("ieeeremainder", "IEEERemainder");
        _functions.Add("log", "Log");
        _functions.Add("log10", "Log10");
        _functions.Add("max", "Max");
        _functions.Add("min", "Min");
        _functions.Add("pow", "Pow");
        _functions.Add("round", "Round");
        _functions.Add("sign", "Sign");
        _functions.Add("sin", "Sin");
        _functions.Add("sqrt", "Sqrt");
        _functions.Add("tan", "Tan");
        _functions.Add("truncate", "Truncate");

        string message = LanguageManager.Instance.GetString("Welcome");
        DisplayMessage(message);

        _showInputPanelButton.interactable = false;
        _showDataVisualizationButton.interactable = false;
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
        if (_showInputPanel)
        {
            _parameters.SetActive(true);
            _initialConditions.SetActive(true);
            _showInputPanelButton.interactable = true;
            _inputPanelButtonText.text = LanguageManager.Instance.GetString("HideInputField");
        }
        else
        {
            _parameters.SetActive(false);
            _initialConditions.SetActive(false);
            _showInputPanelButton.interactable = true;
            _inputPanelButtonText.text = LanguageManager.Instance.GetString("ShowInputField");
        }

        if (_showDataPanel)
        {
            _dataVisualization.SetActive(true);
            _showDataVisualizationButton.interactable = true;
            _dataVisualizationButtonText.text = LanguageManager.Instance.GetString("HideDataVisualization");
        }
        else
        {
            _dataVisualization.SetActive(false);
            _showDataVisualizationButton.interactable = true;
            _dataVisualizationButtonText.text = LanguageManager.Instance.GetString("ShowDataVisualization");
        }
    }

    /// <summary>
    /// Show/Hide the Input Panel
    /// </summary>
    public void ShowInputPanel()
    {
        if (_showInputPanel)
            _showInputPanel = false;
        else
            _showInputPanel = true;
    }

    /// <summary>
    /// Show/Hide Data Visualization Panel
    /// </summary>
    public void ShowDataVisualizationPanel()
    {
        if (_showDataPanel)
            _showDataPanel = false;
        else
            _showDataPanel = true;
    }

    /// <summary>
    /// Gets the Function Fx from the UI and brings the formula to the right form
    /// </summary>
    /// <returns>The corrected formula Fx</returns>
    public string GetFunctionFx()
    {
        return GetCorrectedFormula(fxIF.text);
    }

    /// <summary>
    /// Gets the Function Fy from the UI and brings the formula to the right form
    /// </summary>
    /// <returns>The corrected formula Fy</returns>
    public string GetFunctionFy()
    {
        return GetCorrectedFormula(fyIF.text);
    }

    /// <summary>
    /// Gets the Function Fz from the UI and brings the formula to the right form
    /// </summary>
    /// <returns>The corrected formula Fz</returns>
    public string GetFunctionFz()
    {
        return GetCorrectedFormula(fzIF.text);
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
    public void LoadParameters(int choice)
    {
        _resetBackground = false;
        SimulationController.Instance.ResetSimulation();

        switch (choice)
        {
            case 0:
                LoadDefault();
                dropdown.SetValueWithoutNotify(0);
                break;
            case 1:
                LoadParametersFromFile(choice);
                dropdown.SetValueWithoutNotify(1);
                break;
            case 2:
                LoadParametersFromFile(choice);
                dropdown.SetValueWithoutNotify(2);
                break;
            case 3:
                LoadParametersFromFile(choice);
                dropdown.SetValueWithoutNotify(3);
                break;
            case 4:
                LoadParametersFromFile(choice);
                dropdown.SetValueWithoutNotify(4);
                break;
            case 5:
                LoadParametersFromFile(choice);
                dropdown.SetValueWithoutNotify(5);
                break;
            case 6:
                LoadParametersFromFile(choice);
                dropdown.SetValueWithoutNotify(6);
                break;
            default:
                LoadDefault();
                dropdown.SetValueWithoutNotify(0);
                break;
        }
        SkyboxController.Instance.SetBackground(_background);
    }

    /// <summary>
    /// Handles loading the parameters from the (intern) JSON file and sets the member variables
    /// </summary>
    /// <param name="file">File to load</param>
    private void LoadParametersFromFile(int file)
    {
        List<ParameterLoader.Parameters> parameters = ParameterLoader.Instance.LoadJsonFile(file);

        foreach (var par in parameters)     
        {
            _background = par.Background;

            if (par.Particle == "Satellite")
                _particleInUse = ParticleObject.Satellite;
            else if (par.Particle == "Ball")
                _particleInUse = ParticleObject.Ball;
            else
                _particleInUse = ParticleObject.Default;

            fxIF.text = GetCorrectedFormula(par.FunctionX);
            fyIF.text = GetCorrectedFormula(par.FunctionY);
            fzIF.text = GetCorrectedFormula(par.FunctionZ);

            ifMass.text = par.Mass.ToString();
            
            ifT0.text = par.T0.ToString();
            ifDeltat.text = par.DeltaT.ToString();
            ifSteps.text = par.Steps.ToString();

            ifX.text = par.X.ToString();
            ifY.text = par.Y.ToString();
            ifZ.text = par.Z.ToString();

            ifVx.text = par.Vx.ToString();
            ifVy.text = par.Vy.ToString();
            ifVz.text = par.Vz.ToString();
        }
    }

    /// <summary>
    /// Handles loading the parameters from the EXTERN JSON file and sets the member variables
    /// </summary>
    /// <param name="parameters">Parameter list of the extern JSON file</param>
    public void LoadExternParametersFromFile(List<ParameterLoader.Parameters> parameters)
    {
        foreach (var par in parameters) 
        {
            _background = par.Background;
 
            if (par.Particle == "Satellite")
                _particleInUse = ParticleObject.Satellite;
            else if (par.Particle == "Ball")
                _particleInUse = ParticleObject.Ball;
            else
                _particleInUse = ParticleObject.Default;

            fxIF.text = GetCorrectedFormula(par.FunctionX);
            fyIF.text = GetCorrectedFormula(par.FunctionY);
            fzIF.text = GetCorrectedFormula(par.FunctionZ);
            
            ifMass.text = par.Mass.ToString();
            ifT0.text = par.T0.ToString();
            ifDeltat.text = par.DeltaT.ToString();
            ifSteps.text = par.Steps.ToString();

            ifX.text = par.X.ToString();
            ifY.text = par.Y.ToString();
            ifZ.text = par.Z.ToString();

            ifVx.text = par.Vx.ToString();
            ifVy.text = par.Vy.ToString();
            ifVz.text = par.Vz.ToString();
        }
        SkyboxController.Instance.SetBackground(_background);
    }
   
    /// <summary>
    /// Brings the given formula into the correct form for evaluating
    /// </summary>
    /// <param name="formula">Formula to check</param>
    /// <returns>The corrected formula</returns>
    private string GetCorrectedFormula(string formula)
    {
        string tmp = formula.ToLower();
        
        foreach (KeyValuePair<string, string> entry in _functions)
        {
            tmp = tmp.Replace(entry.Key, entry.Value);
        }
       
        return tmp;
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

    public static ParameterUI Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ParameterUI>();
            return _instance;
        }
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public void ResetObject()
    {
        _showLabel.isOn = false;
        _showOriginGrid.isOn = true;
        _showInputPanel = false;
        _showDataPanel = false;
        _showInputPanelButton.interactable = false;
        _showDataVisualizationButton.interactable = false;
        _inputPanelButtonText.text = LanguageManager.Instance.GetString("ShowInputField");
        _dataVisualizationButtonText.text = LanguageManager.Instance.GetString("ShowDataVisualization");
        _parameters.SetActive(true);
        _initialConditions.SetActive(true);
        _dataVisualization.SetActive(true);

        _particleInUse = ParticleObject.Default;

        if (_resetBackground)
            _background = "ExperimentRoom";
        
        _resetBackground = true;
        dropdown.SetValueWithoutNotify(0);
        LoadDefault();
    }

    /// <summary>
    /// Function to display error messages
    /// </summary>
    /// <param name="message">Message to show</param>
    private void ShowError(string message)
    {
        DisplayMessage(message);
    }

    /// <summary>
    /// Loads the default parameters for resetting the experiment.
    /// Hard coded because of WebGL version
    /// </summary>
    private void LoadDefault()
    {
        _background = "ExperimentRoom";

        fxIF.text = "-x";
        fyIF.text = "0";
        fzIF.text = "0";

        ifMass.text = "1";

        ifT0.text = "0";
        ifDeltat.text = "0,05";
        ifSteps.text = "500";

        ifX.text = "0";
        ifY.text = "0";
        ifZ.text = "0";

        ifVx.text = "1";
        ifVy.text = "0";
        ifVz.text = "0";

        _mass = 1;
        _t0 = 0;
        _deltaT = 0.05f;
        _steps = 500;
        _x = 0;
        _y = 0;
        _z = 0;
        _vx = 1;
        _vy = 0;
        _vz = 0;
    }
}
