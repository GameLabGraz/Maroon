using System.Collections;
using Mirror;
using PlatformControls.PC;
using UnityEngine;
using UnityEngine.UI;
using InputField = GameLabGraz.UI.InputField;
using Slider = GameLabGraz.UI.Slider;

public class FaradysLawNetworkSync : ExperimentNetworkSync
{
    #region ExperimentInputs
    
    //Control Buttons
    [SerializeField]
    private Button _resetButton;
    
    [SerializeField]
    private Button _playButton;

    [SerializeField]
    private Button _pauseButton;

    [SerializeField]
    private Button _stepForwardButton;
    
    //Parameters
    [SerializeField]
    private Slider _ringResistanceSlider;
    
    [SerializeField]
    private InputField _ringResistanceInputField;
    
    [SerializeField]
    private Slider _magneticMomentSlider;
    
    [SerializeField]
    private InputField _magneticMomentInputField;
    
    //Visualisation
    [SerializeField] 
    private Toggle _vectorFieldGridToggle;
    
    [SerializeField]
    private Slider _vectorFieldResolutionSlider;
    
    [SerializeField]
    private InputField _vectorFieldResolutionInputField;
    
    [SerializeField] 
    private Toggle _fieldLinesToggle;
    
    [SerializeField]
    private Slider _lineNumberSlider;
    
    [SerializeField]
    private InputField _lineNumberInputField;
    
    [SerializeField] 
    private Button _ironFillingsButton;

    //In Experiment
    [SerializeField]
    private GameObject Magnet;

    #endregion

    [SyncVar(hook = "OnMagnetPositionChanged")]private Vector3 _magnetPosition;

    #region ControlHandling

    protected override void OnGetControl()
    {
        _resetButton.interactable = true;
        _playButton.interactable = true;
        _pauseButton.interactable = true;
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

        if(Magnet != null)
            Magnet.GetComponent<PC_MouseMovement>().enabled = true;
    }
    
    protected override void OnLoseControl()
    {
        _resetButton.interactable = false;
        _playButton.interactable = false;
        _pauseButton.interactable = false;
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

        if(Magnet != null)
            Magnet.GetComponent<PC_MouseMovement>().enabled = false;
    }
    
    #endregion

    #region Listeners

    protected override void AddListeners()
    {
        base.AddListeners();
        
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
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        
        _resetButton.onClick.RemoveListener(ResetButtonClicked);
        //Play Pause Button already Mirrored with Simulation Mirroring!
        _stepForwardButton.onClick.RemoveListener(StepForwardButtonClicked);
        
        _ringResistanceSlider.onValueChanged.RemoveListener(RingResistanceSliderChanged);
        _ringResistanceInputField.onEndEdit.RemoveListener(RingResistanceInputFieldEndEdit);
        
        _magneticMomentSlider.onValueChanged.RemoveListener(MagneticMomentSliderChanged);
        _magneticMomentInputField.onEndEdit.RemoveListener(MagneticMomentInputFieldEndEdit);
        
        _vectorFieldGridToggle.onValueChanged.RemoveListener(VectorFieldGridToggled);
        
        _vectorFieldResolutionSlider.onValueChanged.RemoveListener(VectorFieldResolutionSliderChanged);
        _vectorFieldResolutionInputField.onEndEdit.RemoveListener(VectorFieldResolutionInputFieldEndEdit);
        
        _fieldLinesToggle.onValueChanged.RemoveListener(FieldLinesToggled);
        
        _lineNumberSlider.onValueChanged.RemoveListener(LineNumberSliderChanged);
        _lineNumberInputField.onEndEdit.RemoveListener(LineNumberInputFieldEndEdit);
        
        _ironFillingsButton.onClick.RemoveListener(IronFillingsButtonClicked);
    }

    #endregion

    #region InputHandlers

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
        if (Magnet == null) //In Falling Coil Experiment
            return;
        if (!Maroon.NetworkManager.Instance.IsInControl)
        {
            if (Magnet.transform.position != _targetPosition)
            {
                float t = (Time.time - _startTime) / syncInterval;
                Magnet.transform.position = Vector3.Lerp(_startPosition, _targetPosition, t);
            }
            return;
        }
        if (_magnetPosition != Magnet.transform.position)
        {
            CmdSetMagnetPosition(Magnet.transform.position);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdSetMagnetPosition(Vector3 newPosition)
    {
        _magnetPosition = newPosition;
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
    
    //RING RESISTANCE SLIDER
    private void RingResistanceSliderChanged(float value)
    {
        SyncEvent(nameof(RingResistanceSliderActivated), value);
    }

    private void RingResistanceSliderActivated(float value)
    {
        _ringResistanceSlider.SetSliderValue(value);
        _ringResistanceSlider.onValueChanged.Invoke(value);
    }

    //RING RESISTANCE INPUT FIELD
    private void RingResistanceInputFieldEndEdit(string valueString)
    {
        SyncEvent(nameof(RingResistanceInputFieldActivated), valueString);
    }

    private void RingResistanceInputFieldActivated(string valueString)
    {
        _ringResistanceInputField.onEndEdit.Invoke(valueString);
    }
    
    //MAGNETIC MOMENT SLIDER
    private void MagneticMomentSliderChanged(float value)
    {
        SyncEvent(nameof(MagneticMomentSliderActivated), value);
    }

    private void MagneticMomentSliderActivated(float value)
    {
        _magneticMomentSlider.SetSliderValue(value);
        _magneticMomentSlider.onValueChanged.Invoke(value);
    }
    
    //MAGNETIC MOMENT INPUT FIELD
    private void MagneticMomentInputFieldEndEdit(string valueString)
    {
        SyncEvent(nameof(MagneticMomentInputFieldActivated), valueString);
    }
    
    private void MagneticMomentInputFieldActivated(string valueString)
    {
        _magneticMomentInputField.onEndEdit.Invoke(valueString);
    }

    //VECTOR FIELD GRID TOGGLE
    private void VectorFieldGridToggled(bool value)
    {
        SyncEvent(nameof(VectorFieldGridToggleActivated), value);
    }

    private void VectorFieldGridToggleActivated(bool value)
    {
        _vectorFieldGridToggle.onValueChanged.Invoke(value);
        _vectorFieldGridToggle.isOn = value;
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
    
    //FIELD LINES TOGGLE
    private void FieldLinesToggled(bool value)
    {
        SyncEvent(nameof(FieldLinesToggleActivated), value);
    }

    private void FieldLinesToggleActivated(bool value)
    {
        _fieldLinesToggle.onValueChanged.Invoke(value);
        _fieldLinesToggle.isOn = value;
    }
    
    //LINE NUMBER SLIDER
    private void LineNumberSliderChanged(float value)
    {
        SyncEvent(nameof(LineNumberSliderActivated), value);
    }

    private void LineNumberSliderActivated(float value)
    {
        _lineNumberSlider.SetSliderValue(value);
        _lineNumberSlider.onValueChanged.Invoke(value);
    }

    //LINE NUMBER INPUT FIELD
    private void LineNumberInputFieldEndEdit(string valueString)
    {
        SyncEvent(nameof(LineNumberInputFieldActivated), valueString);
    }

    private void LineNumberInputFieldActivated(string valueString)
    {
        _lineNumberInputField.onEndEdit.Invoke(valueString);
    }

    //IRON FILLINGS BUTTON
    private void IronFillingsButtonClicked()
    {
        SyncEvent(nameof(IronFillingsButtonActivated));
    }

    private void IronFillingsButtonActivated()
    {
        _ironFillingsButton.onClick.Invoke();
    }
    
    #endregion
}
