using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using PlatformControls.PC;
using UnityEngine;
using UnityEngine.UI;

public class VanDeGraaffNetworkSync : ExperimentNetworkSync
{
    #region ExperimentInputs

    //Control Buttons
    [SerializeField] private Button _startButton;
    
    [SerializeField] private Button _stopButton;
    
    // Parameters
    [SerializeField] private Toggle _chargeToggle;

    [SerializeField] private Toggle _fieldLineToggle;

    [SerializeField] private Toggle _vectorFieldToggle;

    [SerializeField] private PC_Slider _vectorFieldResolutionSlider;

    [SerializeField] private PC_InputField _vectorFieldResolutionInputField;

    [SerializeField] private Button _efieldFillingButton;
    
    //In Experiment
    [SerializeField] private PC_GrounderMovement _grounder;
    
    #endregion

    [SyncVar(hook = "OnGrounderPositionChanged")]private Vector3 _grounderPosition;
    
    #region ControlHandling

    protected override void OnGetControl()
    {
        _startButton.interactable = true;
        _stopButton.interactable = true;

        _chargeToggle.interactable = true;
        _fieldLineToggle.interactable = true;
        _vectorFieldToggle.interactable = true;
        _vectorFieldResolutionSlider.interactable = true;
        _vectorFieldResolutionInputField.interactable = true;
        _efieldFillingButton.interactable = true;

        if(_grounder != null)
            _grounder.enabled = true;
    }

    protected override void OnLoseControl()
    {
        _startButton.interactable = false;
        _stopButton.interactable = false;

        _chargeToggle.interactable = false;
        _fieldLineToggle.interactable = false;
        _vectorFieldToggle.interactable = false;
        _vectorFieldResolutionSlider.interactable = false;
        _vectorFieldResolutionInputField.interactable = false;
        _efieldFillingButton.interactable = false;

        if(_grounder != null)
            _grounder.enabled = false;
    }

    #endregion

    #region Listeners

    protected override void AddListeners()
    {
        base.AddListeners();
        
        _startButton.onClick.AddListener(OnStartButton);
        _stopButton.onClick.AddListener(OnStopButton);
        
        _chargeToggle.onValueChanged.AddListener(ChargeToggled);
        _fieldLineToggle.onValueChanged.AddListener(FieldLineToggled);
        _vectorFieldToggle.onValueChanged.AddListener(VectorFieldToggled);
        _vectorFieldResolutionSlider.onValueChanged.AddListener(VectorFieldResolutionSliderChanged);
        _vectorFieldResolutionInputField.onEndEdit.AddListener(VectorFieldResolutionInputFieldEndEdit);
        _efieldFillingButton.onClick.AddListener(EfieldFillingButtonClicked);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        
        _startButton.onClick.RemoveListener(OnStartButton);
        _stopButton.onClick.RemoveListener(OnStopButton);
        
        _chargeToggle.onValueChanged.RemoveListener(ChargeToggled);
        _fieldLineToggle.onValueChanged.RemoveListener(FieldLineToggled);
        _vectorFieldToggle.onValueChanged.RemoveListener(VectorFieldToggled);
        _vectorFieldResolutionSlider.onValueChanged.RemoveListener(VectorFieldResolutionSliderChanged);
        _vectorFieldResolutionInputField.onEndEdit.RemoveListener(VectorFieldResolutionInputFieldEndEdit);
        _efieldFillingButton.onClick.RemoveListener(EfieldFillingButtonClicked);
    }

    #endregion

    #region InputHandlers
    
    //Grounder
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _startTime;
    
    private void OnGrounderPositionChanged(Vector3 oldValue, Vector3 newValue)
    {
        _startPosition = oldValue;
        _targetPosition = newValue;
        _startTime = Time.time;
    }

    private void Update()
    {
        if (_grounder == null) //In Balloon Experiment
            return;
        if (!MaroonNetworkManager.Instance.IsInControl)
        {
            if (_grounder.transform.position != _targetPosition)
            {
                float t = (Time.time - _startTime) / syncInterval;
                _grounder.transform.position = Vector3.Lerp(_startPosition, _targetPosition, t);
            }
            return;
        }
        if (_grounderPosition != _grounder.transform.position)
        {
            CmdSetGrounderPosition(_grounder.transform.position);
        }
    }
    
    [Command(ignoreAuthority = true)]
    private void CmdSetGrounderPosition(Vector3 newPosition)
    {
        _grounderPosition = newPosition;
    }

    //Start Button
    private void OnStartButton()
    {
        SyncEvent(nameof(StartButtonActivated));
    }

    private IEnumerator StartButtonActivated()
    {
        _startButton.onClick.Invoke();
        yield return null;
    }
    
    //Stop Button
    private void OnStopButton()
    {
        SyncEvent(nameof(StopButtonActivated));
    }

    private IEnumerator StopButtonActivated()
    {
        _stopButton.onClick.Invoke();
        yield return null;
    }
    
    //Charge Toggle
    private void ChargeToggled(bool value)
    {
        SyncEvent(nameof(ChargeToggleActivated), value);
    }

    private void ChargeToggleActivated(bool value)
    {
        _chargeToggle.onValueChanged.Invoke(value);
        _chargeToggle.isOn = value;
    }
    
    //FieldLine Toggle
    private void FieldLineToggled(bool value)
    {
        SyncEvent(nameof(FieldLineToggleActivated), value);
    }

    private void FieldLineToggleActivated(bool value)
    {
        _fieldLineToggle.onValueChanged.Invoke(value);
        _fieldLineToggle.isOn = value;
    }
    
    //VectorField Toggle
    private void VectorFieldToggled(bool value)
    {
        SyncEvent(nameof(VectorFieldToggleActivated), value);
    }

    private void VectorFieldToggleActivated(bool value)
    {
        _vectorFieldToggle.onValueChanged.Invoke(value);
        _vectorFieldToggle.isOn = value;
    }
    
    //VECTOR FIELD RESOLUTION SLIDER
    private void VectorFieldResolutionSliderChanged(float value)
    {
        SyncEvent(nameof(VectorFieldResolutionSliderActivated), value);
    }

    private void VectorFieldResolutionSliderActivated(float value)
    {
        _vectorFieldResolutionSlider.SetSliderValue(value);
        _vectorFieldResolutionSlider.onValueChanged.Invoke(value);
    }

    //VECTOR FIELD RESOLUTION INPUT FIELD
    private void VectorFieldResolutionInputFieldEndEdit(string valueString)
    {
        SyncEvent(nameof(VectorFieldResolutionInputFieldActivated), valueString);
    }

    private void VectorFieldResolutionInputFieldActivated(string valueString)
    {
        _vectorFieldResolutionInputField.onEndEdit.Invoke(valueString);
    }
    
    //EFILLINGS BUTTON
    private void EfieldFillingButtonClicked()
    {
        SyncEvent(nameof(EfieldFillingButtonActivated));
    }

    private void EfieldFillingButtonActivated()
    {
        _efieldFillingButton.onClick.Invoke();
    }

    #endregion
}
