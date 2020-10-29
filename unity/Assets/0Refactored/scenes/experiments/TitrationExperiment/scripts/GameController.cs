using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour, IResetObject
{

    private AcidTitration acidTitrationScript;
    private DrawGraph drawGraphScript;

    // Base settings
    private double baseMol = 0.1f;
    private double baseMl = 50f;

    // Acid settings
    private double acidMol = 0.1f;
    private double acidMl = 20f;

    // Titration settings
    private double molTitrant = 0.1f;
    private double mlTitrant = 50f;
    private double molAnalyte = 0.1f;
    private double mlAnalyte = 20f;

    [SerializeField] private GameObject pipet;
    [SerializeField] private Toggle baseToggle;
    [SerializeField] private Toggle acidToggle;
    [SerializeField] private Button startButton;

    private PipetAnimation pipetAnimation;
    private Coroutine theCoroutine;

    void Start () 
    {
        acidTitrationScript = gameObject.GetComponent<AcidTitration>();
        drawGraphScript = GameObject.Find("LineRenderer").GetComponent<DrawGraph>();
        pipetAnimation = pipet.GetComponent<PipetAnimation>();
    }

    public void StartTitration()
    {
        startButton.interactable = false;
        bool ready = InitialiseComponents();

        if (ready)
        {
            PlayPipetAnimation();

            // Calculation and Graph
            acidTitrationScript.Calculation(molTitrant, mlTitrant, molAnalyte, mlAnalyte);
            drawGraphScript.Initialise();

            // Draw the Graph when burette is opened
            theCoroutine = StartCoroutine(drawGraphScript.DrawLine());
        }
    }

    bool InitialiseComponents()
    {
        if (baseToggle.isOn) // if base is titrant
        {
            molTitrant = baseMol;
            mlTitrant = baseMl;
            molAnalyte = acidMol;
            mlAnalyte = acidMl;
        }
        else // if acid is titrant
        {
            molTitrant = acidMol;
            mlTitrant = acidMl;
            molAnalyte = baseMol;
            mlAnalyte = baseMl;
        }

        if (CheckForZeros(molTitrant))
            return false;
        if (CheckForZeros(molAnalyte))
            return false;
        return true;
    }

    public bool CheckForZeros(double value)
    {
        if (value == 0.0)
            return true;
        else
            return false;
    }

    public void ResetObject()
    {
        // handle of burette is not interactable
        //pipetAnimation.analyte.GetComponent<SetBuretteInteractive>().disableBuretteTap();
        acidTitrationScript.ResetEverything();

        baseMol = 0.1f;
        baseMl = 50.0f;

        acidMol = 0.1f;
        acidMl = 20.0f;

        baseToggle.isOn = true;
        acidToggle.isOn = false;

        pipetAnimation.ResetPipet(true);
        StopCoroutine(theCoroutine);
        startButton.interactable = true;
    }

    // Animation: Pipet takes fluid and puts it into erlenmeyer flask
    void PlayPipetAnimation()
    {
        pipetAnimation.ResetTrigger("reset");
        pipetAnimation.SetPipetBool(baseToggle.isOn);
    }

    public void SetBaseToggleTitrant()
    {
        if (!acidToggle.IsActive() && baseToggle.IsActive())
        {
            acidToggle.isOn = !acidToggle.isOn;
            acidTitrationScript.ChangeAnalyteText(true);
        }
    }

    public void SetAcidToggleTitrant()
    {
        if (!baseToggle.IsActive() && acidToggle.IsActive())
        {
            baseToggle.isOn = !baseToggle.isOn;
            acidTitrationScript.ChangeAnalyteText(false);
        }
    }

    public bool GetBaseToggleTitrant()
    {
        return baseToggle.isOn;
    }


    public void SetMolBase(float value)
    {
        baseMol = (double)value;
    }

    public void SetMlBase(float value)
    {
        baseMl = (double)value;
    }

    public void SetMolAcid(float value)
    {
        acidMol = (double)value;
    }

    public void SetMlAcid(float value)
    {
        acidMl = (double)value;
    }

}
