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
    private ParticleObject particle_in_use_ = ParticleObject.Default;
    [SerializeField] private GameObject parameters_;
    [SerializeField] private GameObject initial_conditions_;

    private static ParameterUI _instance;
    private DialogueManager _dialogueManager;

    Dictionary<string, string> functions_ = new Dictionary<string, string>();
   
    public Dropdown dropdown;
    List<Dropdown.OptionData> menuOptions;

    [SerializeField] private UnityEngine.UI.Button showInputPanelButton;
    [SerializeField] private TMP_Text InputPanelButtonText;
    private bool show_panels_ = false;

    private string background_ = "ExperimentRoom";
    private bool reset_background_ = true;
    private bool dropdown_reset_ = false;

    [SerializeField] InputField fxIF;
    [SerializeField] InputField fyIF;
    [SerializeField] InputField fzIF;

    private Vector3 to_deltat_steps = new Vector3(0, 0, 0);
    private Vector3 xyz = new Vector3(0, 0, 0);
    private Vector3 vxvyvz = new Vector3(0, 0, 0);

    private float mass_ = 1f;
    private float t0_ = 0f;
    private float deltat_ = 0.05f;
    private float steps_ = 500f;

    private float x_ = 0f;
    private float y_ = 0f;
    private float z_ = 0f;

    private float vx_ = 1f;
    private float vy_ = 0f;
    private float vz_ = 0f;

    [SerializeField] InputField if_mass;

    [SerializeField] InputField if_t0;
    [SerializeField] InputField if_deltat;
    [SerializeField] InputField if_steps;

    [SerializeField] InputField if_x;
    [SerializeField] InputField if_y;
    [SerializeField] InputField if_z;

    [SerializeField] InputField if_vx;
    [SerializeField] InputField if_vy;
    [SerializeField] InputField if_vz;

    [SerializeField] private UnityEngine.UI.Toggle showLabelToggle_;
    [SerializeField] private UnityEngine.UI.Toggle showOriginGrid_;

    /// <summary>
    /// Initialization
    /// </summary>
    protected override void Start()
    {
        if (_dialogueManager == null)
            _dialogueManager = FindObjectOfType<DialogueManager>();

        functions_.Add("abs", "Abs");
        functions_.Add("acos", "Acos");
        functions_.Add("asin", "Asin");
        functions_.Add("atan", "Atan");
        functions_.Add("ceiling", "Ceiling");
        functions_.Add("cos", "Cos");
        functions_.Add("exp", "Exp");
        functions_.Add("floor", "Floor");
        functions_.Add("ieeeremainder", "IEEERemainder");
        functions_.Add("log", "Log");
        functions_.Add("log10", "Log10");
        functions_.Add("max", "Max");
        functions_.Add("min", "Min");
        functions_.Add("pow", "Pow");
        functions_.Add("round", "Round");
        functions_.Add("sign", "Sign");
        functions_.Add("sin", "Sin");
        functions_.Add("sqrt", "Sqrt");
        functions_.Add("tan", "Tan");
        functions_.Add("truncate", "Truncate");

        displayMessage("Welcome to the Throwing Balls Experiment. You can use the control buttons at the bottom to Start, Reset or Step through the " +
            "calculation. Please press the reset button if you change any parameters beforehand.");

        showInputPanelButton.interactable = false;
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
        if (show_panels_)
        {
            parameters_.SetActive(true);
            initial_conditions_.SetActive(true);
            showInputPanelButton.interactable = true;
            InputPanelButtonText.text = "Hide Input Field";
        }
        else
        {
            parameters_.SetActive(false);
            initial_conditions_.SetActive(false);
            showInputPanelButton.interactable = true;
            InputPanelButtonText.text = "Show Input Field";
        }
    }

    public void showInputPanel()
    {
        if (show_panels_)
            show_panels_ = false;
        else
        {
            show_panels_ = true;
        }
    }

    public string getFunctionFx()
    {
        return getCorrectedFormula(fxIF.text);
    }

    public string getFunctionFy()
    {
        return getCorrectedFormula(fyIF.text);
    }

    public string getFunctionFz()
    {
        return getCorrectedFormula(fzIF.text);
    }

    public float getMass()
    {
        if (mass_ <= 0)
        {
            showError("Mass can not be negativ or equal to 0. Mass is set to 1");
            mass_ = 1f;
            if_mass.text = "1";
        }
        
        return mass_;
    }

    public void setMass(string value)
    {
        mass_ = float.Parse(value); 
    }

    public void setT0(string value)
    {
        t0_ = System.Convert.ToSingle(value);
    }

    public void setDeltaT(string value)
    {
        deltat_ = float.Parse(value);
    }

    public void setSteps(string value)
    {
        steps_ = System.Convert.ToSingle(value);
    }

    public Vector3 getTimes()
    {
        if (t0_ < 0)
        {
            showError("t0 can not be a negativ value. t0 is set to 0");
            t0_ = 0f;
            if_t0.text = "0";
        }
        if (deltat_ <= 0)
        {
            showError("Delta t can not be negativ or equal to 0. Delta t is set to 1");
            deltat_ = 1f;
            if_deltat.text = "1";
        }     
        if (steps_ <= 0)
        {
            showError("Step size can not be negativ or equal to 0. Step size is set to 1");
            steps_ = 1f;
            if_steps.text = "1";
        }        

        to_deltat_steps = new Vector3(t0_, deltat_, steps_);

        return to_deltat_steps;
    }

    public void setX(string value)
    {
        x_ = System.Convert.ToSingle(value);
    }

    public void setY(string value)
    {
        y_ = System.Convert.ToSingle(value);
    }

    public void setZ(string value)
    {
        z_ = System.Convert.ToSingle(value);
    }

    public Vector3 getXYZ()
    {
        xyz = new Vector3(x_, y_, z_);

        return xyz;
    }

    public void setVX(string value)
    {
        vx_ = System.Convert.ToSingle(value);
    }

    public void setVY(string value)
    {
        vy_ = System.Convert.ToSingle(value);
    }

    public void setVZ(string value)
    {
        vz_ = System.Convert.ToSingle(value);
    }

    public Vector3 getVxVyVz()
    {
        vxvyvz = new Vector3(vx_, vy_, vz_);

        return vxvyvz;
    }

    public void loadParameters(int choice)
    {
        reset_background_ = false;
  
        //if (dropdown_reset_)
            SimulationController.Instance.ResetSimulation();

        switch (choice)
        {
            case 0:
                //loadParametersFromFile(choice);
                loadDefault();
                dropdown.SetValueWithoutNotify(0);
                //particle_in_use_ = ParticleObject.Default;
                break;
            case 1:
                //loadSatellite();
                loadParametersFromFile(choice);
                dropdown.SetValueWithoutNotify(1);
                //particle_in_use_ = ParticleObject.Satellite;
                break;
            case 2:
                //loadBallInTheWind();
                loadParametersFromFile(choice);
                dropdown.SetValueWithoutNotify(2);
                //particle_in_use_ = ParticleObject.Ball;
                break;
            case 3:
                //loadBallInTheWind();
                loadParametersFromFile(choice);
                dropdown.SetValueWithoutNotify(3);
                //particle_in_use_ = ParticleObject.Default;
                break;
            case 4:
                //loadBallInTheWind();
                loadParametersFromFile(choice);
                dropdown.SetValueWithoutNotify(5);
                //particle_in_use_ = ParticleObject.Default;
                break;
            case 5:
                //loadBallInTheWind();
                loadParametersFromFile(choice);
                dropdown.SetValueWithoutNotify(6);
                //particle_in_use_ = ParticleObject.Default;
                break;
            case 6:
                //loadBallInTheWind();
                loadParametersFromFile(choice);
                dropdown.SetValueWithoutNotify(7);
                //particle_in_use_ = ParticleObject.Default;
                break;
            default:
                //loadParametersFromFile(0);
                loadDefault();
                dropdown.SetValueWithoutNotify(0);
                //particle_in_use_ = ParticleObject.Default;
                break;
        }
        SkyboxController.Instance.setBackground(background_);
    }

    private void loadParametersFromFile(int file)
    {
        Debug.Log("loadParametersFromFile(" + file + ")");
        List<FileController.Parameters> parameters = FileController.Instance.loadJsonFile(file);

        foreach (var par in parameters)     
        {
            background_ = par.Background;

            if (par.Particle == "Satellite")
                particle_in_use_ = ParticleObject.Satellite;
            else if (par.Particle == "Ball")
                particle_in_use_ = ParticleObject.Ball;
            else
                particle_in_use_ = ParticleObject.Default;

            fxIF.text = getCorrectedFormula(par.FunctionX);
            fyIF.text = getCorrectedFormula(par.FunctionY);
            fzIF.text = getCorrectedFormula(par.FunctionZ);

            if_mass.text = par.Mass.ToString();
            
            if_t0.text = par.T0.ToString();
            if_deltat.text = par.DeltaT.ToString();
            if_steps.text = par.Steps.ToString();

            if_x.text = par.X.ToString();
            if_y.text = par.Y.ToString();
            if_z.text = par.Z.ToString();

            if_vx.text = par.Vx.ToString();
            if_vy.text = par.Vy.ToString();
            if_vz.text = par.Vz.ToString();
        }
    }

  
    public void loadExternParametersFromFile(List<FileController.Parameters> parameters)
    {
        Debug.Log("loadExternParametersFromFile");

        foreach (var par in parameters) 
        {
            background_ = par.Background;
            Debug.Log("background: " + background_);

            Debug.Log("Object = " + par.Particle);

            if (par.Particle == "Satellite")
            {
                Debug.Log("loadExternParametersFromFile Object = " + par.Particle);
                particle_in_use_ = ParticleObject.Satellite;
            }
            else if (par.Particle == "Ball")
                particle_in_use_ = ParticleObject.Ball;
            else
                particle_in_use_ = ParticleObject.Default;

            fxIF.text = getCorrectedFormula(par.FunctionX);
            Debug.Log("fxIF.text: " + fxIF.text);
            fyIF.text = getCorrectedFormula(par.FunctionY);
            fzIF.text = getCorrectedFormula(par.FunctionZ);
            
            if_mass.text = par.Mass.ToString();
            Debug.Log("if_mass.text: " + if_mass.text);

            if_t0.text = par.T0.ToString();
            if_deltat.text = par.DeltaT.ToString();
            if_steps.text = par.Steps.ToString();

            if_x.text = par.X.ToString();
            if_y.text = par.Y.ToString();
            if_z.text = par.Z.ToString();

            if_vx.text = par.Vx.ToString();
            if_vy.text = par.Vy.ToString();
            if_vz.text = par.Vz.ToString();
        }
        Debug.Log("loadExternParametersFromFile done");
        SkyboxController.Instance.setBackground(background_);
        //particle_in_use_ = ParticleObject.Default;
    }
   
    private string getCorrectedFormula(string formula)
    {
        string tmp = formula.ToLower();
        
        foreach (KeyValuePair<string, string> entry in functions_)
        {
            tmp = tmp.Replace(entry.Key, entry.Value);
        }
       
        return tmp;
    }

    public void displayMessage(string message)
    {
        if (_dialogueManager == null)
            return;

        _dialogueManager.ShowMessage(message);
    }

    public ParticleObject getObjectInUse()
    {
        return particle_in_use_;
    }

    public string getBackground()
    {
        return background_;
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
        //Debug.Log("Reset Parameter UI\n");
        //Debug.Log("Reset Parameter UI - load default\n");
        showLabelToggle_.isOn = false;
        showOriginGrid_.isOn = true;
        show_panels_ = false;
        showInputPanelButton.interactable = false;
        InputPanelButtonText.text = "Show Input Field";
        parameters_.SetActive(true);
        initial_conditions_.SetActive(true);

        particle_in_use_ = ParticleObject.Default;
        if (reset_background_)
            background_ = "ExperimentRoom";
        
        reset_background_ = true;
        dropdown.SetValueWithoutNotify(0);
        loadDefault();
    }

    private void showError(string message)
    {
        displayMessage(message);
    }

    private void loadDefault()
    {
        background_ = "ExperimentRoom";

        fxIF.text = "-x";
        fyIF.text = "0";
        fzIF.text = "0";

        if_mass.text = "1";

        if_t0.text = "0";
        if_deltat.text = "0,05";
        if_steps.text = "500";

        if_x.text = "0";
        if_y.text = "0";
        if_z.text = "0";

        if_vx.text = "1";
        if_vy.text = "0";
        if_vz.text = "0";

        mass_ = 1;
        t0_ = 0;
        deltat_ = 0.05f;
        steps_ = 500;
        x_ = 0;
        y_ = 0;
        z_ = 0;
        vx_ = 1;
        vy_ = 0;
        vz_ = 0;
    }
}
