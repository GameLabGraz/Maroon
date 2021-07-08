using System.Collections;
using System.Collections.Generic;
using Mirror;
using PlatformControls.PC;
using UnityEngine;
using UnityEngine.UI;

public class HuygensNetworkSync : ExperimentNetworkSync
{
    #region ExperimentInputs

    //Control
    //Control Buttons
    [SerializeField] private Button _resetButton;
    
    [SerializeField] private Button _playPauseButton;
    
    [SerializeField] private Button _stepForwardButton;
    
    //parameters
    [SerializeField] private PC_Slider _amplitudeSlider;
    
    [SerializeField] private PC_InputField _amplitudeInputField;
    
    [SerializeField] private PC_Slider _lengthSlider;
    
    [SerializeField] private PC_InputField _lengthInputField;
    
    [SerializeField] private PC_Slider _frequencySlider;
    
    [SerializeField] private PC_InputField _frequencyInputField;

    [SerializeField] private PC_LocalizedDropDown _propagationModeDropDown;
    
    [SerializeField] private PC_Slider _slitNumberSlider;
    
    [SerializeField] private PC_InputField _slitNumberInputField;
    
    [SerializeField] private PC_Slider _slitWidthSlider;
    
    [SerializeField] private PC_InputField _slitWidthInputField;
    
    //Visualization
    [SerializeField] private PC_Slider _peakColorSlider;
    
    [SerializeField] private PC_Slider _troughColorSlider;
    
    //Experiment
    [SerializeField] private PC_MouseMovement _plateMovement;

    #endregion
    
    [SyncVar(hook = "OnPlatePositionChanged")]private Vector3 _platePosition;

    #region ControlHandling

    protected override void OnGetControl()
    {
        _resetButton.interactable = true;
        _playPauseButton.interactable = true;
        _stepForwardButton.interactable = true;

        _amplitudeSlider.interactable = true;
        _amplitudeInputField.interactable = true;
        _lengthSlider.interactable = true;
        _lengthInputField.interactable = true;
        _frequencySlider.interactable = true;
        _frequencyInputField.interactable = true;
        _propagationModeDropDown.interactable = true;
        _slitNumberSlider.interactable = true;
        _slitNumberInputField.interactable = true;
        _slitWidthSlider.interactable = true;
        _slitWidthInputField.interactable = true;

        _peakColorSlider.interactable = true;
        _troughColorSlider.interactable = true;

        _plateMovement.enabled = true;
    }

    protected override void OnLoseControl()
    {
        _resetButton.interactable = false;
        _playPauseButton.interactable = false;
        _stepForwardButton.interactable = false;

        _amplitudeSlider.interactable = false;
        _amplitudeInputField.interactable = false;
        _lengthSlider.interactable = false;
        _lengthInputField.interactable = false;
        _frequencySlider.interactable = false;
        _frequencyInputField.interactable = false;
        _propagationModeDropDown.interactable = false;
        _slitNumberSlider.interactable = false;
        _slitNumberInputField.interactable = false;
        _slitWidthSlider.interactable = false;
        _slitWidthInputField.interactable = false;

        _peakColorSlider.interactable = false;
        _troughColorSlider.interactable = false;

        _plateMovement.enabled = false;
    }

    #endregion
    
    #region Listeners

    protected override void AddListeners()
    {
        base.AddListeners();
        
        _resetButton.onClick.AddListener(ResetButtonClicked);
        //Play Pause Button already Mirrored with Simulation Mirroring!
        _stepForwardButton.onClick.AddListener(StepForwardButtonClicked);
        
        _amplitudeSlider.onValueChanged.AddListener(AmplitudeSliderChanged);
        _amplitudeInputField.onEndEdit.AddListener(AmplitudeInputFieldEndEdit);
        
        _lengthSlider.onValueChanged.AddListener(LengthSliderChanged);
        _lengthInputField.onEndEdit.AddListener(LengthInputFieldEndEdit);
        
        _frequencySlider.onValueChanged.AddListener(FrequencySliderChanged);
        _frequencyInputField.onEndEdit.AddListener(FrequencyInputFieldEndEdit);
        
        _propagationModeDropDown.onValueChanged.AddListener(PropagationModeDropDownChanged);
        
        _slitNumberSlider.onValueChanged.AddListener(SlitNumberSliderChanged);
        _slitNumberInputField.onEndEdit.AddListener(SlitNumberInputFieldEndEdit);
        
        _slitWidthSlider.onValueChanged.AddListener(SlitWidthSliderChanged);
        _slitWidthInputField.onEndEdit.AddListener(SlitWidthInputFieldEndEdit);
        
        _peakColorSlider.onValueChanged.AddListener(PeakColorSliderChanged);
        _troughColorSlider.onValueChanged.AddListener(TroughColorSliderChanged);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        
        _resetButton.onClick.RemoveListener(ResetButtonClicked);
        //Play Pause Button already Mirrored with Simulation Mirroring!
        _stepForwardButton.onClick.RemoveListener(StepForwardButtonClicked);
        
        _amplitudeSlider.onValueChanged.RemoveListener(AmplitudeSliderChanged);
        _amplitudeInputField.onEndEdit.RemoveListener(AmplitudeInputFieldEndEdit);
        
        _lengthSlider.onValueChanged.RemoveListener(LengthSliderChanged);
        _lengthInputField.onEndEdit.RemoveListener(LengthInputFieldEndEdit);
        
        _frequencySlider.onValueChanged.RemoveListener(FrequencySliderChanged);
        _frequencyInputField.onEndEdit.RemoveListener(FrequencyInputFieldEndEdit);
        
        _propagationModeDropDown.onValueChanged.RemoveListener(PropagationModeDropDownChanged);
        
        _slitNumberSlider.onValueChanged.RemoveListener(SlitNumberSliderChanged);
        _slitNumberInputField.onEndEdit.RemoveListener(SlitNumberInputFieldEndEdit);
        
        _slitWidthSlider.onValueChanged.RemoveListener(SlitWidthSliderChanged);
        _slitWidthInputField.onEndEdit.RemoveListener(SlitWidthInputFieldEndEdit);
        
        _peakColorSlider.onValueChanged.RemoveListener(PeakColorSliderChanged);
        _troughColorSlider.onValueChanged.RemoveListener(TroughColorSliderChanged);
    }

    #endregion

    #region InputHandlers
    
    //Plate Position
    private void OnPlatePositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        if (Maroon.NetworkManager.Instance.IsInControl)
            return;

        StartCoroutine(UpdatePlatePosition(newPosition));
    }

    private IEnumerator UpdatePlatePosition(Vector3 newPosition)
    {
        _plateMovement.transform.position = newPosition;
        
        //Skip one frame, because plate position lags behind handle position
        yield return null;
        _plateMovement.onMovement.Invoke();
    }
    
    private void Update()
    {
        if (!Maroon.NetworkManager.Instance.IsInControl)
            return;
        
        if (_platePosition != _plateMovement.transform.position)
        {
            CmdSetPlatePosition(_plateMovement.transform.position);
        }
    }
    
    [Command(requiresAuthority = false)]
    private void CmdSetPlatePosition(Vector3 newPosition)
    {
        _platePosition = newPosition;
    }

    //RESET BUTTON
    private void ResetButtonClicked()
    {
        SyncEvent(nameof(ResetButtonActivated));
    }

    private IEnumerator ResetButtonActivated()
    {
        _resetButton.onClick.Invoke();
        OnLoseControl();
        yield return null;
    }
    
    //STEP FORWARD BUTTON
    private void StepForwardButtonClicked()
    {
        SyncEvent(nameof(StepForwardButtonActivated));
    }

    private IEnumerator StepForwardButtonActivated()
    {
        _stepForwardButton.onClick.Invoke();
        _stepForwardButton.interactable = false;
        yield return null;
    }
    
    //AMPLITUDE
    private void AmplitudeSliderChanged(float value)
    {
        SyncEvent(nameof(AmplitudeSliderActivated), value);
    }

    private void AmplitudeSliderActivated(float value)
    {
        _amplitudeSlider.SetSliderValue(value);
        _amplitudeSlider.onValueChanged.Invoke(value);
    }

    private void AmplitudeInputFieldEndEdit(string valueString)
    {
        SyncEvent(nameof(AmplitudeInputFieldActivated), valueString);
    }

    private void AmplitudeInputFieldActivated(string valueString)
    {
        _amplitudeInputField.onEndEdit.Invoke(valueString);
    }
    
    //LENGTH
    private void LengthSliderChanged(float value)
    {
        SyncEvent(nameof(LengthSliderActivated), value);
    }

    private void LengthSliderActivated(float value)
    {
        _lengthSlider.SetSliderValue(value);
        _lengthSlider.onValueChanged.Invoke(value);
    }
    
    private void LengthInputFieldEndEdit(string valueString)
    {
        SyncEvent(nameof(LengthInputFieldActivated), valueString);
    }

    private void LengthInputFieldActivated(string valueString)
    {
        _lengthInputField.onEndEdit.Invoke(valueString);
    }
    
    //FREQUENCY
    private void FrequencySliderChanged(float value)
    {
        SyncEvent(nameof(FrequencySliderActivated), value);
    }

    private void FrequencySliderActivated(float value)
    {
        _frequencySlider.SetSliderValue(value);
        _frequencySlider.onValueChanged.Invoke(value);
    }
    
    private void FrequencyInputFieldEndEdit(string valueString)
    {
        SyncEvent(nameof(FrequencyInputFieldActivated), valueString);
    }

    private void FrequencyInputFieldActivated(string valueString)
    {
        _frequencyInputField.onEndEdit.Invoke(valueString);
    }

    private void PropagationModeDropDownChanged(int value)
    {
        SyncEvent(nameof(PropagationModeDropDownActivated), value);
    }

    private void PropagationModeDropDownActivated(int value)
    {
        _propagationModeDropDown.value = value;
        _propagationModeDropDown.onValueChanged.Invoke(value);
    }
    
    //SlitNumber
    private void SlitNumberSliderChanged(float value)
    {
        SyncEvent(nameof(SlitNumberSliderActivated), value);
    }

    private void SlitNumberSliderActivated(float value)
    {
        _slitNumberSlider.SetSliderValue(value);
        _slitNumberSlider.onValueChanged.Invoke(value);
    }

    private void SlitNumberInputFieldEndEdit(string valueString)
    {
        SyncEvent(nameof(SlitNumberInputFieldActivated), valueString);
    }

    private void SlitNumberInputFieldActivated(string valueString)
    {
        _slitNumberInputField.onEndEdit.Invoke(valueString);
    }
    
    //SlitWidth
    private void SlitWidthSliderChanged(float value)
    {
        SyncEvent(nameof(SlitWidthSliderActivated), value);
    }

    private void SlitWidthSliderActivated(float value)
    {
        _slitWidthSlider.SetSliderValue(value);
        _slitWidthSlider.onValueChanged.Invoke(value);
    }

    private void SlitWidthInputFieldEndEdit(string valueString)
    {
        SyncEvent(nameof(SlitWidthInputFieldActivated), valueString);
    }

    private void SlitWidthInputFieldActivated(string valueString)
    {
        _slitWidthInputField.onEndEdit.Invoke(valueString);
    }
    
    //PeakColor
    private void PeakColorSliderChanged(float value)
    {
        SyncEvent(nameof(PeakColorSliderActivated), value);
    }

    private void PeakColorSliderActivated(float value)
    {
        _peakColorSlider.SetSliderValue(value);
        _peakColorSlider.onValueChanged.Invoke(value);
    }
    
    //TroughColor
    private void TroughColorSliderChanged(float value)
    {
        SyncEvent(nameof(TroughColorSliderActivated), value);
    }

    private void TroughColorSliderActivated(float value)
    {
        _troughColorSlider.SetSliderValue(value);
        _troughColorSlider.onValueChanged.Invoke(value);
    }

    #endregion
}
