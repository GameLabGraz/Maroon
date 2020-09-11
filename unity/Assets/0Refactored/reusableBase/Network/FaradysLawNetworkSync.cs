using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using PlatformControls.PC;
using UnityEngine;
using UnityEngine.UI;

public class FaradysLawNetworkSync : NetworkBehaviour
{
    #region ExperimentInputs
    
    //Control Buttons
    [SerializeField]
    private Button _resetButton;
    
    [SerializeField]
    private Button _playPauseButton;
    
    [SerializeField]
    private Button _stepForwardButton;
    
    //Parameters
    [SerializeField]
    private PC_Slider _ringResistanceSlider;
    
    [SerializeField]
    private PC_InputField _ringResistanceInputField;
    
    [SerializeField]
    private PC_Slider _magneticMomentSlider;
    
    [SerializeField]
    private PC_InputField _magneticMomentInputField;
    
    //Visualisation
    [SerializeField] 
    private Toggle _vectorFieldGridToggle;
    
    [SerializeField]
    private PC_Slider _vectorFieldResolutionSlider;
    
    [SerializeField]
    private PC_InputField _vectorFieldResolutionInputField;
    
    [SerializeField] 
    private Toggle _fieldLinesToggle;
    
    [SerializeField]
    private PC_Slider _lineNumberSlider;
    
    [SerializeField]
    private PC_InputField _lineNumberInputField;
    
    [SerializeField] 
    private Button _ironFillingsButton;

    //In Experiment
    [SerializeField]
    private GameObject Magnet;
    private BoxCollider _magnetCollider;

    #endregion

    [SyncVar(hook = "OnMagnetPositionChanged")]private Vector3 _magnetPosition;

    private void Start()
    {
        //This is only executed if on a running Server!
        SimulationController.Instance.onStartRunning.AddListener(OnSimulationStart);
        SimulationController.Instance.onStopRunning.AddListener(OnSimulationStop);
        
        _resetButton.onClick.AddListener(ResetButtonClicked);
        //Play Pause Button already Mirrored with Simulation Mirroring!
        _stepForwardButton.onClick.AddListener(StepForwardButtonClicked);
        
        _ringResistanceSlider.onValueChanged.AddListener(RingResistanceSliderChanged);
        _ringResistanceInputField.onEndEdit.AddListener(RingResistanceInputFieldEndEdit);
        
        _magneticMomentSlider.onValueChanged.AddListener(MagneticMomentSliderChanged);
        _magneticMomentInputField.onEndEdit.AddListener(MagneticMomentInputFieldEndEdit);
        
        _vectorFieldGridToggle.onValueChanged.AddListener(VectorFieldGridToggled);
        
        _vectorFieldResolutionSlider.onValueChanged.AddListener(VectorFieldResolutionSliderChanged);
        _vectorFieldResolutionInputField.onEndEdit.AddListener(VectorFieldResolutionInputFieldEndEdit);
        
        _fieldLinesToggle.onValueChanged.AddListener(FieldLinesToggled);
        
        _lineNumberSlider.onValueChanged.AddListener(LineNumberSliderChanged);
        _lineNumberInputField.onEndEdit.AddListener(LineNumberInputFieldEndEdit);
        
        _ironFillingsButton.onClick.AddListener(IronFillingsButtonClicked);

        _magnetPosition = Magnet.transform.position;
        _magnetCollider = Magnet.GetComponent<BoxCollider>();
        
        MaroonNetworkManager.Instance.onGetControl.AddListener(OnGetControl);
        MaroonNetworkManager.Instance.onLoseControl.AddListener(OnLoseControl);

        //Needed so Buttons are not randomly reactivated
        SimulationController.Instance.OnStart += DeactivateInteractionIfNotInControl;
        SimulationController.Instance.OnStop += DeactivateInteractionIfNotInControl;
        
        if (MaroonNetworkManager.Instance.IsInControl)
        {
            OnGetControl();
        }
        else
        {
            OnLoseControl();
        }
    }

    #region ControlHandling

    public void DeactivateInteractionIfNotInControl(object sender = null, EventArgs e = null)
    {
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            OnLoseControl();
        }
    }

    public void OnGetControl()
    {
        _resetButton.interactable = true;
        _playPauseButton.interactable = true;
        _stepForwardButton.interactable = true;

        _ringResistanceSlider.interactable = true;
        _ringResistanceInputField.interactable = true;

        _magneticMomentSlider.interactable = true;
        _magneticMomentInputField.interactable = true;

        _vectorFieldGridToggle.interactable = true;

        _vectorFieldResolutionSlider.interactable = true;
        _vectorFieldResolutionInputField.interactable = true;

        _fieldLinesToggle.interactable = true;

        _lineNumberSlider.interactable = true;
        _lineNumberInputField.interactable = true;

        _ironFillingsButton.interactable = true;

        _magnetCollider.enabled = true;
    }
    
    public void OnLoseControl()
    {
        _resetButton.interactable = false;
        _playPauseButton.interactable = false;
        _stepForwardButton.interactable = false;

        _ringResistanceSlider.interactable = false;
        _ringResistanceInputField.interactable = false;

        _magneticMomentSlider.interactable = false;
        _magneticMomentInputField.interactable = false;

        _vectorFieldGridToggle.interactable = false;

        _vectorFieldResolutionSlider.interactable = false;
        _vectorFieldResolutionInputField.interactable = false;

        _fieldLinesToggle.interactable = false;

        _lineNumberSlider.interactable = false;
        _lineNumberInputField.interactable = false;

        _ironFillingsButton.interactable = false;

        _magnetCollider.enabled = false;
    }
    
    #endregion

    #region InputHandlers
    
    //SIMULATION MIRRORING
    public void OnSimulationStart()
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdOnSimulationStart();
    }

    [Command(ignoreAuthority = true)]
    public void CmdOnSimulationStart()
    {
        RpcOnSimulationStart();
    }

    [ClientRpc]
    public void RpcOnSimulationStart()
    {
        if(!MaroonNetworkManager.Instance.IsInControl)
            SimulationController.Instance.StartSimulation();
    }
    
    public void OnSimulationStop()
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdOnSimulationStop();
    }

    [Command(ignoreAuthority = true)]
    public void CmdOnSimulationStop()
    {
        RpcOnSimulationStop();
    }

    [ClientRpc]
    public void RpcOnSimulationStop()
    {
        if(!MaroonNetworkManager.Instance.IsInControl)
            SimulationController.Instance.StopSimulation();
    }

    //MAGNET
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _startTime;
    
    private void OnMagnetPositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        _startPosition = oldPosition;
        _targetPosition = newPosition;
        _startTime = Time.time;
    }

    private void Update()
    {
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            if (Magnet.transform.position != _targetPosition)
            {
                float t = syncInterval / (_startTime - Time.time);
                Magnet.transform.position = Vector3.Lerp(_startPosition, _targetPosition, t);
            }
            return;
        }
        if (_magnetPosition != Magnet.transform.position)
        {
            CmdSetMagnetPosition(Magnet.transform.position);
        }
    }

    [Command(ignoreAuthority = true)]
    private void CmdSetMagnetPosition(Vector3 newPosition)
    {
        _magnetPosition = newPosition;
    }

    //RESET BUTTON
    private void ResetButtonClicked()
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdResetButtonClicked();
    }

    [Command(ignoreAuthority = true)]
    private void CmdResetButtonClicked()
    {
        RpcResetButtonClicked();
    }

    [ClientRpc]
    private void RpcResetButtonClicked()
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            _resetButton.onClick.Invoke();
            OnLoseControl();
        }
    }
    
    //STEP FORWARD BUTTON
    private void StepForwardButtonClicked()
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdStepForwardButtonClicked();
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdStepForwardButtonClicked()
    {
        RpcStepForwardButtonClicked();
    }

    [ClientRpc]
    private void RpcStepForwardButtonClicked()
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            _stepForwardButton.onClick.Invoke();
            _stepForwardButton.interactable = false;
        }
    }

    //RING RESISTANCE SLIDER
    private void RingResistanceSliderChanged(float value)
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdRingResistanceSliderChanged(value);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdRingResistanceSliderChanged(float value)
    {
        RpcRingResistanceSliderChanged(value);
    }

    [ClientRpc]
    private void RpcRingResistanceSliderChanged(float value)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            _ringResistanceSlider.SetSliderValue(value);
            _ringResistanceSlider.onValueChanged.Invoke(value);
        }
    }

    //RING RESISTANCE INPUT FIELD
    private void RingResistanceInputFieldEndEdit(string valueString)
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdRingResistanceInputFieldEndEdit(valueString);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdRingResistanceInputFieldEndEdit(string valueString)
    {
        RpcRingResistanceInputFieldEndEdit(valueString);
    }
    
    [ClientRpc]
    private void RpcRingResistanceInputFieldEndEdit(string valueString)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            _ringResistanceInputField.onEndEdit.Invoke(valueString);
        }
    }
    
    //MAGNETIC MOMENT SLIDER
    private void MagneticMomentSliderChanged(float value)
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdMagneticMomentSliderChanged(value);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdMagneticMomentSliderChanged(float value)
    {
        RpcMagneticMomentSliderChanged(value);
    }

    [ClientRpc]
    private void RpcMagneticMomentSliderChanged(float value)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            _magneticMomentSlider.SetSliderValue(value);
            _magneticMomentSlider.onValueChanged.Invoke(value);
        }
    }

    //MAGNETIC MOMENT INPUT FIELD
    private void MagneticMomentInputFieldEndEdit(string valueString)
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdMagneticMomentInputFieldEndEdit(valueString);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdMagneticMomentInputFieldEndEdit(string valueString)
    {
        RpcMagneticMomentInputFieldEndEdit(valueString);
    }
    
    [ClientRpc]
    private void RpcMagneticMomentInputFieldEndEdit(string valueString)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            _magneticMomentInputField.onEndEdit.Invoke(valueString);
        }
    }

    //VECTOR FIELD GRID TOGGLE
    private void VectorFieldGridToggled(bool value)
    {
        if (MaroonNetworkManager.Instance.IsInControl)
            CmdVectorFieldGridToggled(value);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdVectorFieldGridToggled(bool value)
    {
        RpcVectorFieldGridToggled(value);
    }
    
    [ClientRpc]
    private void RpcVectorFieldGridToggled(bool value)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            _vectorFieldGridToggle.onValueChanged.Invoke(value);
            _vectorFieldGridToggle.isOn = !_vectorFieldGridToggle.isOn;
        }
    }
    
    //VECTOR FIELD RESOLUTION SLIDER
    private void VectorFieldResolutionSliderChanged(float value)
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdVectorFieldResolutionSliderChanged(value);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdVectorFieldResolutionSliderChanged(float value)
    {
        RpcVectorFieldResolutionSliderChanged(value);
    }

    [ClientRpc]
    private void RpcVectorFieldResolutionSliderChanged(float value)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            _vectorFieldResolutionSlider.SetSliderValue(value);
            _vectorFieldResolutionSlider.onValueChanged.Invoke(value);
        }
    }

    //VECTOR FIELD RESOLUTION INPUT FIELD
    private void VectorFieldResolutionInputFieldEndEdit(string valueString)
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdVectorFieldResolutionInputFieldEndEdit(valueString);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdVectorFieldResolutionInputFieldEndEdit(string valueString)
    {
        RpcVectorFieldResolutionInputFieldEndEdit(valueString);
    }
    
    [ClientRpc]
    private void RpcVectorFieldResolutionInputFieldEndEdit(string valueString)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            _vectorFieldResolutionInputField.onEndEdit.Invoke(valueString);
        }
    }
    
    //FIELD LINES TOGGLE
    private void FieldLinesToggled(bool value)
    {
        if (MaroonNetworkManager.Instance.IsInControl)
            CmdFieldLinesToggled(value);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdFieldLinesToggled(bool value)
    {
        RpcFieldLinesToggled(value);
    }
    
    [ClientRpc]
    private void RpcFieldLinesToggled(bool value)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            _fieldLinesToggle.onValueChanged.Invoke(value);
            _fieldLinesToggle.isOn = !_fieldLinesToggle.isOn;
        }
    }
    
    //LINE NUMBER SLIDER
    private void LineNumberSliderChanged(float value)
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdLineNumberSliderChanged(value);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdLineNumberSliderChanged(float value)
    {
        RpcLineNumberSliderChanged(value);
    }

    [ClientRpc]
    private void RpcLineNumberSliderChanged(float value)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            _lineNumberSlider.SetSliderValue(value);
            _lineNumberSlider.onValueChanged.Invoke(value);
        }
    }

    //LINE NUMBER INPUT FIELD
    private void LineNumberInputFieldEndEdit(string valueString)
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdLineNumberInputFieldEndEdit(valueString);
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdLineNumberInputFieldEndEdit(string valueString)
    {
        RpcLineNumberInputFieldEndEdit(valueString);
    }
    
    [ClientRpc]
    private void RpcLineNumberInputFieldEndEdit(string valueString)
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            _lineNumberInputField.onEndEdit.Invoke(valueString);
        }
    }
    
    //IRON FILLINGS BUTTON
    private void IronFillingsButtonClicked()
    {
        if(MaroonNetworkManager.Instance.IsInControl)
            CmdIronFillingsButtonClicked();
    }

    [Command(ignoreAuthority = true)]
    private void CmdIronFillingsButtonClicked()
    {
        RpcIronFillingsButtonClicked();
    }

    [ClientRpc]
    private void RpcIronFillingsButtonClicked()
    {
        //If in Control: Already executed it!
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            _ironFillingsButton.onClick.Invoke();
        }
    }
    
    #endregion
}
