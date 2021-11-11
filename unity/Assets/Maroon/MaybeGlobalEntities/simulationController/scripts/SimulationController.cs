//-----------------------------------------------------------------------------
// SimulationController.cs
//
// Class to handle the simulation flow
//
//-----------------------------------------------------------------------------

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maroon.Physics;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Class to handle the simulation flow
/// </summary>
public class SimulationController : MonoBehaviour
{

    [SerializeField] private int updateRate = 1;
    [SerializeField] private int fixedUpdateRate = 1;
    
    /// <summary>
    /// Indicates whether the simulation is running
    /// </summary>
    [SerializeField] private QuantityBool simulationRunning = false;
    [SerializeField] private QuantityBool simulationAllowed = true;

    public UnityEvent onEnteredScene;
    public UnityEvent onStartRunning;
    public UnityEvent onStopRunning;
    
    private bool _inWholeResetMode = false;
    
    /// <summary>
    /// Indicates whether a single step should be simulated
    /// </summary>
    private bool stepSimulation;

    /// <summary>
    /// Indicates whether the simulation should be reset
    /// </summary>
    private bool simulationReset;

    /// <summary>
    /// The objects which must be reset
    /// </summary>
    private IEnumerable<IResetObject> _resetObjects => FindObjectsOfType<MonoBehaviour>().OfType<IResetObject>();

    public UnityEvent OnStart;

    public UnityEvent OnStop;

    public UnityEvent OnReset;

    private static SimulationController _instance;

    public static SimulationController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SimulationController>();

            return _instance;
        }
    }

    public int UpdateRate => updateRate;
    public int FixedUpdateRate => fixedUpdateRate;

    /// <summary>
    /// Initialization
    /// </summary>
    private void Awake()
    {
        stepSimulation = false;
        simulationReset = true;

        SceneManager.sceneUnloaded += (Scene scene) =>
        {
            Time.timeScale = 1;
        };
    }

    private void Start()
    {
        onEnteredScene.Invoke();
    }

    /// <summary>
    /// Simulates one frame
    /// </summary>
    /// <returns>enumerator</returns>
    private IEnumerator SimulateOneFrame()
    {
        yield return 0;
        StopSimulation();
        stepSimulation = false;
    }

    /// <summary>
    /// Property that defines whether the simulation is running
    /// </summary>
    public bool SimulationRunning
    {
        get => simulationRunning.Value;
        set
        {
            if (simulationRunning.Value == value) return;

            if (!simulationAllowed.Value && value) return; //simulation would be set to running although no simulation is allowed
                
            _inWholeResetMode = false;
            
            simulationRunning.Value = value;
            if(simulationRunning.Value) onStartRunning.Invoke();
            else onStopRunning.Invoke();
        }
    }

    public bool SimulationAllowed
    {
        get => simulationAllowed.Value;
        set
        {
            simulationAllowed.Value = value;
            
            if(!simulationAllowed)
                StopSimulation();
        }
    }
    
    /// <summary>
    /// Getter for the stepSimulation field
    /// </summary>
    public bool StepSimulation => stepSimulation;

    /// <summary>
    /// Getter for the simulationReset field
    /// </summary>
    public bool SimulationJustReset => simulationReset;

    /// <summary>
    /// Starts the simulation
    /// </summary>
    public void StartSimulation()
    {
        SimulationRunning = true;
        simulationReset = false;
        _inWholeResetMode = false;

        if(!stepSimulation)
            OnStart?.Invoke();
    }

    /// <summary>
    /// Simulates a single frame
    /// </summary>
    public void SimulateStep()
    {
        stepSimulation = true;
        simulationReset = false;
        _inWholeResetMode = false;
        StartSimulation();
        StartCoroutine(SimulateOneFrame());
    }

    /// <summary>
    /// Stops the simulation
    /// </summary>
    public void StopSimulation()
    {
        SimulationRunning = false;
        OnStop?.Invoke();
    }

    /// <summary>
    /// Resets the simulation
    /// </summary>
    public void ResetSimulation()
    {
        foreach (var resetObject in _resetObjects)
        {
            resetObject.ResetObject();
        }

        StopSimulation();
        simulationReset = true;

        OnReset?.Invoke();
        Debug.Log("onReset: " + OnReset);
        Debug.Log("Simulation reset");

        _inWholeResetMode = true;
    }

    public void ResetWholeSimulation()
    {
        ResetSimulation();
        foreach (var resetObject in _resetObjects.ToList())
        {
            if(resetObject is IResetWholeObject)
                (resetObject as IResetWholeObject).ResetWholeObject();
        }
    }

    public void ResetRespResetWholeSimulation()
    {
        if (!_inWholeResetMode)
            SimulationController.Instance.ResetSimulation();
        else
            SimulationController.Instance.ResetWholeSimulation();
    }

    public void TraceOutput(string output){
        Debug.Log(output);
    }

    public void StartStopSimulation(bool startSimulation)
    {
        if(startSimulation) StartSimulation();
        else StopSimulation();
    }
}
