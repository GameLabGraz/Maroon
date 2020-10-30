using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AcidTitration))]
public class TitrationController : MonoBehaviour, IResetObject
{
    private AcidTitration _acidTitration;
    
    // Base settings
    private float _baseMol = 0.1f;
    private float _baseMl = 50f;

    // Acid settings
    private float _acidMol = 0.1f;
    private float _acidMl = 20f;

    // Titration settings
    private float _molTitrant = 0.1f;
    private float _mlTitrant = 50f;
    private float _molAnalyte = 0.1f;
    private float _mlAnalyte = 20f;

    [SerializeField] private PipetAnimation pipetAnimation;
    [SerializeField] private DrawGraph drawGraph;
    
    [SerializeField] private Toggle baseToggle;
    [SerializeField] private Toggle acidToggle;
    [SerializeField] private Button startButton;

    private Coroutine _coroutine;

    private void Start () 
    {
        _acidTitration = GetComponent<AcidTitration>();
    }

    public void StartTitration()
    {
        startButton.interactable = false;
        var ready = InitialiseComponents();

        if (ready)
        {
            PlayPipetAnimation();

            // Calculation and Graph
            _acidTitration.Calculation(_molTitrant, _mlTitrant, _molAnalyte, _mlAnalyte);
            drawGraph.Initialise();

            // Draw the Graph when burette is opened
            _coroutine = StartCoroutine(drawGraph.DrawLine());
        }
    }

    private bool InitialiseComponents()
    {
        if (baseToggle.isOn) // if base is titrant
        {
            _molTitrant = _baseMol;
            _mlTitrant = _baseMl;
            _molAnalyte = _acidMol;
            _mlAnalyte = _acidMl;
        }
        else // if acid is titrant
        {
            _molTitrant = _acidMol;
            _mlTitrant = _acidMl;
            _molAnalyte = _baseMol;
            _mlAnalyte = _baseMl;
        }

        if (CheckForZeros(_molTitrant))
            return false;
        if (CheckForZeros(_molAnalyte))
            return false;
        return true;
    }

    public bool CheckForZeros(double value)
    {
        return value == 0.0;
    }

    public void ResetObject()
    {
        _acidTitration.ResetEverything();

        _baseMol = 0.1f;
        _baseMl = 50.0f;

        _acidMol = 0.1f;
        _acidMl = 20.0f;

        baseToggle.isOn = true;
        acidToggle.isOn = false;

        pipetAnimation.ResetPipet(true);
        if(_coroutine != null) StopCoroutine(_coroutine);
        startButton.interactable = true;
    }

    // Animation: Pipet takes fluid and puts it into erlenmeyer flask
    private void PlayPipetAnimation()
    {
        pipetAnimation.ResetTrigger("reset");
        pipetAnimation.SetPipetBool(baseToggle.isOn);
    }

    public void SetBaseToggleTitrant()
    {
        if (!acidToggle.IsActive() && baseToggle.IsActive())
        {
            acidToggle.isOn = !acidToggle.isOn;
            _acidTitration.ChangeAnalyteText(true);
        }
    }

    public void SetAcidToggleTitrant()
    {
        if (!baseToggle.IsActive() && acidToggle.IsActive())
        {
            baseToggle.isOn = !baseToggle.isOn;
            _acidTitration.ChangeAnalyteText(false);
        }
    }

    public bool GetBaseToggleTitrant()
    {
        return baseToggle.isOn;
    }

    public void SetMolBase(float value)
    {
        _baseMol = value;
    }

    public void SetMlBase(float value)
    {
        _baseMl = value;
    }

    public void SetMolAcid(float value)
    {
        _acidMol = value;
    }

    public void SetMlAcid(float value)
    {
        _acidMl = value;
    }

}
