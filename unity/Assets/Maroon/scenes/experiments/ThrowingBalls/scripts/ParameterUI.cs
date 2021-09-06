using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Maroon.UI;
using ObjectsInUse;

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
   
    public Dropdown dropdown;
    List<Dropdown.OptionData> menuOptions;

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
    /// Initialization
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

        DisplayMessage("Welcome to the Throwing Balls Experiment. You can use the control buttons at the bottom to Start, Reset or Step through the " +
            "calculation. Please press the reset button if you change any parameters beforehand.");

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
    /// Update is called every frame
    /// </summary>
    protected override void HandleFixedUpdate()
    {
        if (_showInputPanel)
        {
            _parameters.SetActive(true);
            _initialConditions.SetActive(true);
            _showInputPanelButton.interactable = true;
            _inputPanelButtonText.text = "Hide Input Field";
        }
        else
        {
            _parameters.SetActive(false);
            _initialConditions.SetActive(false);
            _showInputPanelButton.interactable = true;
            _inputPanelButtonText.text = "Show Input Field";
        }

        if (_showDataPanel)
        {
            _dataVisualization.SetActive(true);
            _showDataVisualizationButton.interactable = true;
            _dataVisualizationButtonText.text = "Hide Data Visualization";
        }
        else
        {
            _dataVisualization.SetActive(false);
            _showDataVisualizationButton.interactable = true;
            _dataVisualizationButtonText.text = "Show Data Visualization";
        }
    }

    public void ShowInputPanel()
    {
        if (_showInputPanel)
            _showInputPanel = false;
        else
        {
            _showInputPanel = true;
        }
    }

    public void ShowDataVisualizationPanel()
    {
        if (_showDataPanel)
            _showDataPanel = false;
        else
        {
            _showDataPanel = true;
        }
    }

    public string GetFunctionFx()
    {
        return GetCorrectedFormula(fxIF.text);
    }

    public string GetFunctionFy()
    {
        return GetCorrectedFormula(fyIF.text);
    }

    public string GetFunctionFz()
    {
        return GetCorrectedFormula(fzIF.text);
    }

    public float GetMass()
    {
        if (_mass <= 0)
        {
            ShowError("Mass can not be negativ or equal to 0. Mass is set to 1");
            _mass = 1f;
            ifMass.text = "1";
        }
        
        return _mass;
    }

    public void SetMass(string value)
    {
        _mass = float.Parse(value); 
    }

    public void SetT0(string value)
    {
        _t0 = System.Convert.ToSingle(value);
    }

    public void SetDeltaT(string value)
    {
        _deltaT = float.Parse(value);
    }

    public void SetSteps(string value)
    {
        _steps = System.Convert.ToSingle(value);
    }

    public Vector3 GetTimes()
    {
        if (_t0 < 0)
        {
            ShowError("t0 can not be a negativ value. t0 is set to 0");
            _t0 = 0f;
            ifT0.text = "0";
        }
        if (_deltaT <= 0)
        {
            ShowError("Delta t can not be negativ or equal to 0. Delta t is set to 1");
            _deltaT = 1f;
            ifDeltat.text = "1";
        }     
        if (_steps <= 0)
        {
            ShowError("Step size can not be negativ or equal to 0. Step size is set to 1");
            _steps = 1f;
            ifSteps.text = "1";
        }        

        _toDeltatSteps = new Vector3(_t0, _deltaT, _steps);

        return _toDeltatSteps;
    }

    public void SetX(string value)
    {
        _x = System.Convert.ToSingle(value);
    }

    public void SetY(string value)
    {
        _y = System.Convert.ToSingle(value);
    }

    public void SetZ(string value)
    {
        _z = System.Convert.ToSingle(value);
    }

    public Vector3 GetXYZ()
    {
        _xyz = new Vector3(_x, _y, _z);

        return _xyz;
    }

    public void SetVX(string value)
    {
        _vx = System.Convert.ToSingle(value);
    }

    public void SetVY(string value)
    {
        _vy = System.Convert.ToSingle(value);
    }

    public void SetVZ(string value)
    {
        _vz = System.Convert.ToSingle(value);
    }

    public Vector3 GetVxVyVz()
    {
        _vxvyvz = new Vector3(_vx, _vy, _vz);

        return _vxvyvz;
    }

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
                dropdown.SetValueWithoutNotify(5);
                break;
            case 5:
                LoadParametersFromFile(choice);
                dropdown.SetValueWithoutNotify(6);
                break;
            case 6:
                LoadParametersFromFile(choice);
                dropdown.SetValueWithoutNotify(7);
                break;
            default:
                LoadDefault();
                dropdown.SetValueWithoutNotify(0);
                break;
        }
        SkyboxController.Instance.SetBackground(_background);
    }

    private void LoadParametersFromFile(int file)
    {
        List<FileController.Parameters> parameters = FileController.Instance.LoadJsonFile(file);

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

  
    public void LoadExternParametersFromFile(List<FileController.Parameters> parameters)
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
   
    private string GetCorrectedFormula(string formula)
    {
        string tmp = formula.ToLower();
        
        foreach (KeyValuePair<string, string> entry in _functions)
        {
            tmp = tmp.Replace(entry.Key, entry.Value);
        }
       
        return tmp;
    }

    public void DisplayMessage(string message)
    {
        if (_dialogueManager == null)
            return;

        _dialogueManager.ShowMessage(message);
    }

    public ParticleObject GetObjectInUse()
    {
        return _particleInUse;
    }

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
        _inputPanelButtonText.text = "Show Input Field";
        _dataVisualizationButtonText.text = "Show Data Visualization";
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

    private void ShowError(string message)
    {
        DisplayMessage(message);
    }

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
