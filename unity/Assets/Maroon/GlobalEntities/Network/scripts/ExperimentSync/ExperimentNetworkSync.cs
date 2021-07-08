using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ExperimentNetworkSync : NetworkBehaviour
{
    /// <summary>
    /// Called on the server with the connection id of the client that has finished loading the scene.
    /// </summary>
    [Server]
    public virtual void ClientLoadedScene(NetworkConnection conn) {}
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (Maroon.NetworkManager.Instance == null)
            return;
        if (Maroon.NetworkManager.Instance.IsInControl)
        {
            OnGetControl();
        }
        else
        {
            OnLoseControl();
        }
    }

    protected virtual void OnEnable()
    {
        if (Maroon.NetworkManager.Instance == null)
            return;
        AddListeners();
    }

    protected virtual void OnDisable()
    {
        if (Maroon.NetworkManager.Instance == null)
            return;
        RemoveListeners();
    }

    protected virtual void OnGetControl() { }
    
    protected virtual void OnLoseControl() { }

    protected virtual void AddListeners()
    {
        Maroon.NetworkManager.Instance.onGetControl.AddListener(OnGetControl);
        Maroon.NetworkManager.Instance.onLoseControl.AddListener(OnLoseControl);

        try
        {
            SimulationController.Instance.onStartRunning.AddListener(OnSimulationStart);
            SimulationController.Instance.onStopRunning.AddListener(OnSimulationStop);

            //Needed so Buttons are not randomly reactivated
            SimulationController.Instance.OnStart += DeactivateInteractionIfNotInControl;
            SimulationController.Instance.OnStop += DeactivateInteractionIfNotInControl;
        }
        catch (NullReferenceException e)
        {
            //there is no Simulation Controller in this experiment
        }
    }
    
    protected virtual void RemoveListeners()
    {
        Maroon.NetworkManager.Instance.onGetControl.RemoveListener(OnGetControl);
        Maroon.NetworkManager.Instance.onLoseControl.RemoveListener(OnLoseControl);

        try
        {
            SimulationController.Instance.onStartRunning.RemoveListener(OnSimulationStart);
            SimulationController.Instance.onStopRunning.RemoveListener(OnSimulationStop);

            //Needed so Buttons are not randomly reactivated
            SimulationController.Instance.OnStart -= DeactivateInteractionIfNotInControl;
            SimulationController.Instance.OnStop -= DeactivateInteractionIfNotInControl;
        }
        catch (NullReferenceException e)
        {
            //there is no Simulation Controller in this experiment
        }
    }
    
    public void DeactivateInteractionIfNotInControl(object sender = null, EventArgs e = null)
    {
        if (!Maroon.NetworkManager.Instance.IsInControl)
        {
            OnLoseControl();
        }
    }

    #region SyncronizeEvents

    //No parameters
    protected void SyncEvent(string eventActivatedCoroutine)
    {
        if(Maroon.NetworkManager.Instance.IsInControl)
            CmdSyncEvent(eventActivatedCoroutine);
    }
    
    [Command(requiresAuthority = false)]
    private void CmdSyncEvent(string eventActivatedCoroutine)
    {
        RpcSyncEvent(eventActivatedCoroutine);
    }
    
    [ClientRpc]
    private void RpcSyncEvent(string eventActivatedCoroutine)
    {
        //If in Control: Already executed it!
        if (!Maroon.NetworkManager.Instance.IsInControl)
        {
            StartCoroutine(eventActivatedCoroutine);
        }
    }
    
    //float
    protected void SyncEvent(string eventActivatedCoroutine, float value)
    {
        if(Maroon.NetworkManager.Instance.IsInControl)
            CmdSyncFloatEvent(eventActivatedCoroutine, value);
    }
    
    [Command(requiresAuthority = false)]
    private void CmdSyncFloatEvent(string eventActivatedCoroutine, float value)
    {
        RpcSyncFloatEvent(eventActivatedCoroutine, value);
    }
    
    [ClientRpc]
    private void RpcSyncFloatEvent(string eventActivatedCoroutine, float value)
    {
        //If in Control: Already executed it!
        if (!Maroon.NetworkManager.Instance.IsInControl)
        {
            StartCoroutine(eventActivatedCoroutine, value);
        }
    }
    
    //int
    protected void SyncEvent(string eventActivatedCoroutine, int value)
    {
        if(Maroon.NetworkManager.Instance.IsInControl)
            CmdSyncIntEvent(eventActivatedCoroutine, value);
    }
    
    [Command(requiresAuthority = false)]
    private void CmdSyncIntEvent(string eventActivatedCoroutine, int value)
    {
        RpcSyncIntEvent(eventActivatedCoroutine, value);
    }
    
    [ClientRpc]
    private void RpcSyncIntEvent(string eventActivatedCoroutine, int value)
    {
        //If in Control: Already executed it!
        if (!Maroon.NetworkManager.Instance.IsInControl)
        {
            StartCoroutine(eventActivatedCoroutine, value);
        }
    }
    
    //string
    protected void SyncEvent(string eventActivatedCoroutine, string value)
    {
        if(Maroon.NetworkManager.Instance.IsInControl)
            CmdSyncStringEvent(eventActivatedCoroutine, value);
    }
    
    [Command(requiresAuthority = false)]
    private void CmdSyncStringEvent(string eventActivatedCoroutine, string value)
    {
        RpcSyncStringEvent(eventActivatedCoroutine, value);
    }
    
    [ClientRpc]
    private void RpcSyncStringEvent(string eventActivatedCoroutine, string value)
    {
        //If in Control: Already executed it!
        if (!Maroon.NetworkManager.Instance.IsInControl)
        {
            StartCoroutine(eventActivatedCoroutine, value);
        }
    }
    
    //bool
    protected void SyncEvent(string eventActivatedCoroutine, bool value)
    {
        if(Maroon.NetworkManager.Instance.IsInControl)
            CmdSyncBoolEvent(eventActivatedCoroutine, value);
    }
    
    [Command(requiresAuthority = false)]
    private void CmdSyncBoolEvent(string eventActivatedCoroutine, bool value)
    {
        RpcSyncBoolEvent(eventActivatedCoroutine, value);
    }

    [ClientRpc]
    private void RpcSyncBoolEvent(string eventActivatedCoroutine, bool value)
    {
        //If in Control: Already executed it!
        if (!Maroon.NetworkManager.Instance.IsInControl)
        {
            StartCoroutine(eventActivatedCoroutine, value);
        }
    }
    
    #endregion
    
    //SIMULATION MIRRORING
    private void OnSimulationStart()
    {
        SyncEvent(nameof(StartSimulation));
    }

    private IEnumerator StartSimulation()
    {
        SimulationController.Instance.StartSimulation();
        yield return null;
    }
    
    private void OnSimulationStop()
    {
        SyncEvent(nameof(StopSimulation));
    }

    private IEnumerator StopSimulation()
    {
        SimulationController.Instance.StopSimulation();
        yield return null;
    }
}
