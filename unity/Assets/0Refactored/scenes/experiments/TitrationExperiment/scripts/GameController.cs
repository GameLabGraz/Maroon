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

	[Space(10)]
    public GameObject pipet;
    private PipetAnimation pipetAnimation;

	public Toggle baseToggle;
	public Toggle acidToggle;
	public Button startButton;

    Coroutine theCoroutine;

	void Start () 
	{
		acidTitrationScript = gameObject.GetComponent<AcidTitration>();
		drawGraphScript = GameObject.Find("LineRenderer").GetComponent<DrawGraph>();
        pipetAnimation = pipet.GetComponent<PipetAnimation>();
	}

	public void startTitration()
	{
		startButton.interactable = false;
		bool ready = initialiseComponents();

        if (ready)
        {
            playPipetAnimation();

            // Calculation and Graph
            acidTitrationScript.calculation(molTitrant, mlTitrant, molAnalyte, mlAnalyte);
            drawGraphScript.initialise();

            // Draw the Graph when burette is opened
            theCoroutine = StartCoroutine(drawGraphScript.DrawLine());
        }
    }

    bool initialiseComponents()
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

		if (checkForZeros(molTitrant))
            return false;
        if (checkForZeros(molAnalyte))
            return false;
        return true;


	}

    public bool checkForZeros(double value)
    {
        if (value == 0.0)
            return true;
        else
            return false;
    }

	public void ResetObject()
	{
		// handle of burette is not interactable
		pipetAnimation.analyte.GetComponent<SetBuretteInteractive>().disableBuretteTap();
        acidTitrationScript.resetEverything();

		baseMol = 0.1f;
		baseMl = 50.0f;

		acidMol = 0.1f;
		acidMl = 20.0f;

		baseToggle.isOn = true;
		acidToggle.isOn = false;

		pipetAnimation.resetPipet(true);
        StopCoroutine(theCoroutine);
		startButton.interactable = true;
	}

    // Animation: Pipet takes fluid and puts it into erlenmeyer flask
    void playPipetAnimation()
    {
		pipetAnimation.resetTrigger("reset");
		pipetAnimation.setPipetBool(baseToggle.isOn);
    }

	public void setBaseToggleTitrant(bool value)
	{
		if (!acidToggle.IsActive())
			acidToggle.isOn = !acidToggle.isOn;
	}

	public void setAcidToggleTitrant(bool value)
	{
		if (!baseToggle.IsActive())
			baseToggle.isOn = !baseToggle.isOn;
	}

	public bool getBaseToggleTitrant()
	{
		return baseToggle.isOn;
	}


	public void setMolBase(float value)
	{
		baseMol = (double)value;
	}

	public void setMlBase(float value)
	{
		baseMl = (double)value;
	}

	public void setMolAcid(float value)
	{
		acidMol = (double)value;
	}

	public void setMlAcid(float value)
	{
		acidMl = (double)value;
	}

}
