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

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Class to handle the simulation flow
/// </summary>
public class SimCntrl : MonoBehaviour
{
    /// <summary>
    /// Indicates wheter the simulation is running
    /// </summary>
    private static bool simulationRunning;

    /// <summary>
    /// Indicates wheter a single step should be simulated
    /// </summary>
    private static bool stepSimulation;

    /// <summary>
    /// Indicates wheter the simulation should be reset
    /// </summary>
    private static bool simulationReset;

    /// <summary>
    /// The objects which must be reset
    /// </summary>
    private List<IResetObject> resetObjects;

    /// <summary>
    /// Initialization
    /// </summary>
    public void Awake()
    {
        resetObjects = new List<IResetObject>();
        InitSimController();

        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    /// <summary>
    /// A delegate to get notifications when a scene has loaded
    /// </summary>
    /// <param name="scene">the scene</param>
    /// <param name="mode">the load scene mode</param>
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
      InitSimController();
      if (scene.name != "FallingCoil" && scene.name != "FallingMagnet" && scene.name != "FaradaysLaw")
        StartSimulation();
     
    }

    /// <summary>
    /// Initialization function for the Sim Controller
    /// </summary>
    public void InitSimController()
    {
        simulationRunning = false;
        stepSimulation = false;
        simulationReset = true;
        Time.timeScale = 0;

        resetObjects.Clear();
        GameObject[] objects = GameObject.FindGameObjectsWithTag("ResetObject");
        foreach (GameObject obj in objects)
        {
            IResetObject reset_obj = obj.GetComponentInParent<IResetObject>();
            if (reset_obj != null)
                resetObjects.Add(reset_obj);

        }
    }

    /// <summary>
    /// Adds a reset object
    /// </summary>
    /// <param name="reset_obj">the object to be reset</param>
    public void AddNewResetObject(IResetObject reset_obj)
    {
        if (reset_obj != null)
            resetObjects.Add(reset_obj);
    }

    /// <summary>
    /// Removes a reset object
    /// </summary>
    /// <param name="reset_obj">the reset object to remove</param>
    public void RemoveResetObject(IResetObject reset_obj)
    {
        resetObjects.Remove(reset_obj);
    }

    /// <summary>
    /// Simulates one frame
    /// </summary>
    /// <returns>enumerator</returns>
    IEnumerator SimulateOneFrame()
    {
        yield return 0;
        StopSimulation();
        stepSimulation = false;
    }

    /// <summary>
    /// Property that defines wheter the simulation is running
    /// </summary>
    public bool SimulationRunning
    {
        get
        {
            return simulationRunning;
        }
        set
        {
          Scene scene = SceneManager.GetActiveScene();
          if (scene.name == "FallingCoil" || scene.name == "FallingMagnet" || scene.name == "FaradaysLaw")
            Time.timeScale = value == true ? Teal.TimeScaleSimulationRunning : 0f;
          else
            Time.timeScale = 1f;
          simulationRunning = value;
        }
    }

    /// <summary>
    /// Getter for the stepSimulation field
    /// </summary>
    public bool StepSimulation
    {
        get
        {
            return stepSimulation;
        }
    }

    /// <summary>
    /// Getter for the simulationReset field
    /// </summary>
    public bool SimulationJustReset
    {
        get
        {
            return simulationReset;
        }
    }

    /// <summary>
    /// Starts the simulation
    /// </summary>
    public void StartSimulation()
    {
        SimulationRunning = true;
        simulationReset = false;
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
        SimulationRunning = false;
    }

    /// <summary>
    /// Resets the simulation
    /// </summary>
    public void ResetSimulation()
    {
        foreach (IResetObject resetObject in resetObjects)
        {
            resetObject.resetObject();
        }

        StopSimulation();
        simulationReset = true;
        Debug.Log("Reset");
    }
}

/// <summary>
/// Definition to make the Simulation Controller a singleton
/// </summary>
public sealed class SimController : Singleton<SimCntrl>
{
}
