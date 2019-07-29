//-----------------------------------------------------------------------------
// SimulationController.cs
//
// Class to handle the simulation flow
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Class to handle the simulation flow
/// </summary>
public class SimulationController : MonoBehaviour
{
    /// <summary>
    /// Indicates wheter the simulation is running
    /// </summary>
    [SerializeField]
    private bool simulationRunning = false;

    [SerializeField]
    private float timeScale = 1;

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
    /// Indicates wheter a single step should be simulated
    /// </summary>
    private bool stepSimulation;

    /// <summary>
    /// Indicates wheter the simulation should be reset
    /// </summary>
    private bool simulationReset;

    /// <summary>
    /// The objects which must be reset
    /// </summary>
    private List<IResetObject> resetObjects;

    public event EventHandler<EventArgs> OnStart;

    public event EventHandler<EventArgs> OnStop;

    public event EventHandler<EventArgs> OnReset;

    /// <summary>
    /// Initialization
    /// </summary>
    private void Awake()
    {
        resetObjects = new List<IResetObject>();
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

        resetObjects.Clear();
        foreach (var obj in GameObject.FindGameObjectsWithTag("ResetObject"))
        {
            var resetObj = obj.GetComponentInParent<IResetObject>();
            if (resetObj != null)
                resetObjects.Add(resetObj);

        }
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
    /// Adds a reset object
    /// </summary>
    /// <param name="resetObj">the object to be reset</param>
    public void AddNewResetObject(IResetObject resetObj)
    {
        if (resetObj != null)
            resetObjects.Add(resetObj);
    }

    /// <summary>
    /// Removes a reset object
    /// </summary>
    /// <param name="resetObj">the reset object to remove</param>
    public void RemoveResetObject(IResetObject resetObj)
    {
        resetObjects.Remove(resetObj);
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
    public bool SimulationRunning => simulationRunning;

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
        simulationRunning = true;
        simulationReset = false;

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
        StartSimulation();
        StartCoroutine(SimulateOneFrame());
    }

    /// <summary>
    /// Stops the simulation
    /// </summary>
    public void StopSimulation()
    {
        simulationRunning = false;
        OnStop?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Resets the simulation
    /// </summary>
    public void ResetSimulation()
    {
        foreach (var resetObject in resetObjects)
        {
            resetObject.ResetObject();
        }

        StopSimulation();
        simulationReset = true;

        OnReset?.Invoke(this, EventArgs.Empty);

        Debug.Log("Reset");
    }
}
