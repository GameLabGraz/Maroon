using System.Collections;
using Maroon.Physics;
using Mirror;
using PlatformControls.PC;
using UnityEngine;
using UnityEngine.UI;
using InputField = Maroon.UI.InputField;
using Slider = Maroon.UI.Slider;


public class PendulumNetworkSync : ExperimentNetworkSync
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
    private Slider _weightSlider;
    
    [SerializeField]
    private InputField _weightInputField;
    
    [SerializeField]
    private Slider _lengthSlider;
    
    [SerializeField]
    private InputField _lengthInputField;
    
    //Watch
    [SerializeField] 
    private Button _startSlowMotionButton;
    
    [SerializeField] 
    private Button _stopSlowMotionButton;

    //In Experiment
    [SerializeField] 
    private GameObject _pendulumObject; 
    
    #endregion

    [SyncVar(hook = "OnPendulumAngleChanged")] private float _pendulumAngle;

    private Pendulum _pendulum;
    protected override void Start()
    {
        _pendulum = _pendulumObject.GetComponentInChildren<Pendulum>();
        
        base.Start();
    }
    
    #region ControlHandling

    protected override void OnGetControl()
    {
        _resetButton.interactable = true;
        _playButton.interactable = true;
        _pauseButton.interactable = true;
        _stepForwardButton.interactable = true;

        _weightSlider.interactable = true;
        _weightInputField.interactable = true;
        _lengthSlider.interactable = true;
        _lengthInputField.interactable = true;

        _startSlowMotionButton.interactable = true;
        _stopSlowMotionButton.interactable = true;

        _pendulumObject.GetComponentInChildren<PC_SwingPendulum>().enabled = true;

        if (_grabbed)
        {
            StartCoroutine(PendulumReleased(_pendulum.Joint.angle));
            OnReleasePendulum();
        }
    }
    
    protected override void OnLoseControl()
    {
        _resetButton.interactable = false;
        _playButton.interactable = false;
        _pauseButton.interactable = false;
        _stepForwardButton.interactable = false;

        _weightSlider.interactable = false;
        _weightInputField.interactable = false;
        _lengthSlider.interactable = false;
        _lengthInputField.interactable = false;

        _startSlowMotionButton.interactable = false;
        _stopSlowMotionButton.interactable = false;

        _pendulumObject.GetComponentInChildren<PC_SwingPendulum>().enabled = false;
    }
    
    #endregion
    
    #region Listeners

    protected override void AddListeners()
    {
        base.AddListeners();
        
        _resetButton.onClick.AddListener(ResetButtonClicked);
        //Play Pause Button already Mirrored with Simulation Mirroring!
        _stepForwardButton.onClick.AddListener(StepForwardButtonClicked);
        
        _weightSlider.onValueChanged.AddListener(WeightSliderChanged);
        _weightInputField.onEndEdit.AddListener(WeightInputFieldEndEdit);
        
        _lengthSlider.onValueChanged.AddListener(LengthSliderChanged);
        _lengthInputField.onEndEdit.AddListener(LengthInputFieldEndEdit);
        
        _startSlowMotionButton.onClick.AddListener(StartSlowMotionButtonClicked);
        _stopSlowMotionButton.onClick.AddListener(StopSlowMotionButtonClicked);
        
        _pendulumObject.GetComponentInChildren<PC_SwingPendulum>().OnGrab.AddListener(OnGrabPendulum);
        _pendulumObject.GetComponentInChildren<PC_SwingPendulum>().OnRelease.AddListener(OnReleasePendulum);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        
        _resetButton.onClick.RemoveListener(ResetButtonClicked);
        //Play Pause Button already Mirrored with Simulation Mirroring!
        _stepForwardButton.onClick.RemoveListener(StepForwardButtonClicked);
        
        _weightSlider.onValueChanged.RemoveListener(WeightSliderChanged);
        _weightInputField.onEndEdit.RemoveListener(WeightInputFieldEndEdit);
        
        _lengthSlider.onValueChanged.RemoveListener(LengthSliderChanged);
        _lengthInputField.onEndEdit.RemoveListener(LengthInputFieldEndEdit);
        
        _startSlowMotionButton.onClick.RemoveListener(StartSlowMotionButtonClicked);
        _stopSlowMotionButton.onClick.RemoveListener(StopSlowMotionButtonClicked);
        
        _pendulumObject.GetComponentInChildren<PC_SwingPendulum>().OnGrab.RemoveListener(OnGrabPendulum);
        _pendulumObject.GetComponentInChildren<PC_SwingPendulum>().OnRelease.RemoveListener(OnReleasePendulum);
    }

    #endregion

    #region InputHandlers
    
    //PENDULUM
    private bool _grabbed;
    private void OnGrabPendulum()
    {
        _grabbed = true;
        SyncEvent(nameof(PendulumGrabbed));
    }
    
    private IEnumerator PendulumGrabbed()
    {
        _pendulum.GetComponent<Rigidbody>().WakeUp();
        _pendulum.Joint.useLimits = true;
        _grabbed = true;
        yield return null;
    }
    
    private void OnReleasePendulum()
    {
        _grabbed = false;
        SyncEvent(nameof(PendulumReleased), _pendulum.Joint.angle);
    }

    private IEnumerator PendulumReleased(float value)
    {
        _pendulum.Joint.limits = new JointLimits
        {
            min = value,
            max = value + 0.0001f
        };
        yield return null;
        
        _pendulum.Joint.useLimits = false;
        _grabbed = false;
        yield return null;
    }
    
    private void OnPendulumAngleChanged(float oldAngle, float newAngle)
    {
        if (Maroon.NetworkManager.Instance.IsInControl)
            return;
        
        _pendulum.Joint.limits = new JointLimits
        {
            min = newAngle,
            max = newAngle + 0.0001f
        };
    }


    private void Update()
    {
        if (!_grabbed && SimulationController.Instance.SimulationRunning)
            return;
        if (Mathf.Abs(_pendulum.Joint.angle - _pendulumAngle) > 0.0001)
        {
            CmdSetPendulumAngle(_pendulum.Joint.angle);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdSetPendulumAngle(float value)
    {
        _pendulumAngle = value;
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
    
    //WEIGHT SLIDER
    private void WeightSliderChanged(float value)
    {
        SyncEvent(nameof(WeightSliderActivated), value);
    }

    private void WeightSliderActivated(float value)
    {
        _weightSlider.SetSliderValue(value);
        _weightSlider.onValueChanged.Invoke(value);
    }

    //WEIGHT INPUT FIELD
    private void WeightInputFieldEndEdit(string valueString)
    {
        SyncEvent(nameof(WeightInputFieldActivated), valueString);
    }

    private void WeightInputFieldActivated(string valueString)
    {
        _weightInputField.onEndEdit.Invoke(valueString);
    }
    
    //LENGTH SLIDER
    private void LengthSliderChanged(float value)
    {
        SyncEvent(nameof(LengthSliderActivated), value);
    }

    private void LengthSliderActivated(float value)
    {
        _lengthSlider.SetSliderValue(value);
        _lengthSlider.onValueChanged.Invoke(value);
    }

    //LENGTH INPUT FIELD
    private void LengthInputFieldEndEdit(string valueString)
    {
        SyncEvent(nameof(LengthInputFieldActivated), valueString);
    }

    private void LengthInputFieldActivated(string valueString)
    {
        _lengthInputField.onEndEdit.Invoke(valueString);
    }

    //SLOW MOTION BUTTON
    private void StartSlowMotionButtonClicked()
    {
        SyncEvent(nameof(StartSlowMotionButtonActivated));
    }

    private IEnumerator StartSlowMotionButtonActivated()
    {
        _startSlowMotionButton.onClick.Invoke();
        _startSlowMotionButton.interactable = false;
        yield return null;
    }
    
    private void StopSlowMotionButtonClicked()
    {
        SyncEvent(nameof(StopSlowMotionButtonActivated));
    }

    private IEnumerator StopSlowMotionButtonActivated()
    {
        _stopSlowMotionButton.onClick.Invoke();
        _stopSlowMotionButton.interactable = false;
        yield return null;
    }

    #endregion
}
