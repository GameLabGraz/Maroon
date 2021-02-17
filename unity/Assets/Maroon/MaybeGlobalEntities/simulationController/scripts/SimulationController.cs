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
    /// <summary>
    /// Indicates whether the simulation is running
    /// </summary>
    [SerializeField]
    private QuantityBool simulationRunning = false;
    [SerializeField]
    private QuantityBool simulationAllowed = true;

    [SerializeField]
    private float timeScale = 1;

    public UnityEvent onStartRunning;
    public UnityEvent onStopRunning;
    
    private bool _inWholeResetMode = false;

    public float TimeScale
    {
        get => timeScale;
        set
        {
            timeScale = value;
            Time.timeScale = value;
        }
    }

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

    public event EventHandler<EventArgs> OnStart;

    public event EventHandler<EventArgs> OnStop;

    public event EventHandler<EventArgs> OnReset;

    private static SimulationController _instance;

    public static SimulationController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SimulationController>();

            if(_instance == null)
                throw  new NullReferenceException("There is no simulation controller.");

            return _instance;
        }
    }

    /// <summary>
    /// Initialization
    /// </summary>
    private void Awake()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        InitSimController();
    }

    /// <summary>
    /// Initialization function for the Sim Controller
    /// </summary>
    private void InitSimController()
    {
        stepSimulation = false;
        simulationReset = true;
        Time.timeScale = timeScale;
    }

    /// <summary>
    /// Delegate to get notification when the scene has unloaded.
    /// Reset timeScale to 1.0f.
    /// </summary>
    /// <param name="scene">Unloaded scene</param>
    private void OnSceneUnloaded(Scene scene)
    {
        TimeScale = 1.0f;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
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
            OnStart?.Invoke(this, EventArgs.Empty);
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
        OnStop?.Invoke(this, EventArgs.Empty);
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

        OnReset?.Invoke(this, EventArgs.Empty);

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
