using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ExperimentNetworkSync : NetworkBehaviour
{
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (MaroonNetworkManager.Instance.IsInControl)
        {
            OnGetControl();
        }
        else
        {
            OnLoseControl();
        }
    }

    private void OnEnable()
    {
        AddListeners();
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    protected virtual void OnGetControl() { }
    
    protected virtual void OnLoseControl() { }

    protected virtual void AddListeners()
    {
        MaroonNetworkManager.Instance.onGetControl.AddListener(OnGetControl);
        MaroonNetworkManager.Instance.onLoseControl.AddListener(OnLoseControl);

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
        MaroonNetworkManager.Instance.onGetControl.RemoveListener(OnGetControl);
        MaroonNetworkManager.Instance.onLoseControl.RemoveListener(OnLoseControl);

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
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            OnLoseControl();
        }
    }

    #region SyncronizeEvents

    //No parameters
    protected void SyncEvent(string eventActivatedCoroutine)
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdSyncEvent(eventActivatedCoroutine);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdSyncEvent(string eventActivatedCoroutine)
    {
        RpcSyncEvent(eventActivatedCoroutine);
    }
    
    [ClientRpc]
    private void RpcSyncEvent(string eventActivatedCoroutine)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            StartCoroutine(eventActivatedCoroutine);
        }
    }
    
    //float
    protected void SyncEvent(string eventActivatedCoroutine, float value)
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdSyncFloatEvent(eventActivatedCoroutine, value);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdSyncFloatEvent(string eventActivatedCoroutine, float value)
    {
        RpcSyncFloatEvent(eventActivatedCoroutine, value);
    }
    
    [ClientRpc]
    private void RpcSyncFloatEvent(string eventActivatedCoroutine, float value)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            StartCoroutine(eventActivatedCoroutine, value);
        }
    }
    
    //int
    protected void SyncEvent(string eventActivatedCoroutine, int value)
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdSyncIntEvent(eventActivatedCoroutine, value);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdSyncIntEvent(string eventActivatedCoroutine, int value)
    {
        RpcSyncIntEvent(eventActivatedCoroutine, value);
    }
    
    [ClientRpc]
    private void RpcSyncIntEvent(string eventActivatedCoroutine, int value)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            StartCoroutine(eventActivatedCoroutine, value);
        }
    }
    
    //string
    protected void SyncEvent(string eventActivatedCoroutine, string value)
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdSyncStringEvent(eventActivatedCoroutine, value);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdSyncStringEvent(string eventActivatedCoroutine, string value)
    {
        RpcSyncStringEvent(eventActivatedCoroutine, value);
    }
    
    [ClientRpc]
    private void RpcSyncStringEvent(string eventActivatedCoroutine, string value)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            StartCoroutine(eventActivatedCoroutine, value);
        }
    }
    
    //bool
    protected void SyncEvent(string eventActivatedCoroutine, bool value)
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdSyncBoolEvent(eventActivatedCoroutine, value);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdSyncBoolEvent(string eventActivatedCoroutine, bool value)
    {
        RpcSyncBoolEvent(eventActivatedCoroutine, value);
    }

    [ClientRpc]
    private void RpcSyncBoolEvent(string eventActivatedCoroutine, bool value)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
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
