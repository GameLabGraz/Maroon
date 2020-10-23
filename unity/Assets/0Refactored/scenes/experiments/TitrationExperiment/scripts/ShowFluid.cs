using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public enum colorEnum { colorless, red, yellow, blue, green, orange };

public class Indicator
{
    public colorEnum lowColor { get; set; }
    public colorEnum midColor { get; set; }
    public colorEnum highColor { get; set; }
    public float startNumber { get; set; }
    public float endNumber { get; set; }

    public Indicator(colorEnum newLowColor, colorEnum newMidColor, colorEnum newHighColor, float newStartNumber, float newEndnumber)
    {
        this.lowColor = newLowColor;
        this.midColor = newMidColor;
        this.highColor = newHighColor;
        this.startNumber = newStartNumber;
        this.endNumber = newEndnumber;
    }
}
public class ShowFluid : MonoBehaviour
{
    // Indicators
    Dictionary<int, Indicator> indicators = new Dictionary<int, Indicator>() {
         {0, new Indicator(colorEnum.colorless, colorEnum.red, colorEnum.red, 8.3f, 10f)}, // Phenolphtalein
         {1, new Indicator(colorEnum.yellow, colorEnum.green, colorEnum.blue, 6f, 7.6f)},  // Bromothymol blue
         {2, new Indicator(colorEnum.red, colorEnum.orange, colorEnum.yellow, 3.1f, 4.5f)} }; // Mehtyl orange
 

    private static Indicator currentInd;
    private Dictionary<double, double> result = new Dictionary<double, double>();

    [Header("Fluid Materials")]
    public Material FluidWaterBlue;
    public Material FluidWaterColorless;
    public Material FluidWaterOrange;
    public Material FluidWaterRed;
    public Material FluidWaterYellow;
    public Material FluidWaterGreen;

    [Space(10)]
    public GameObject fluid;

    [HideInInspector]
    public MeshRenderer meshRend;
    private AcidTitration acidTitrationScript;

    private static ShowFluid _instance;
    public static ShowFluid Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ShowFluid>();
            return _instance;
        }
    }

    // Use this for initialization
    void Start()
    {
        meshRend = fluid.GetComponent<MeshRenderer>();
        meshRend.enabled = false;
        acidTitrationScript = GameObject.Find("TitrationController").GetComponent<AcidTitration>();

        currentInd = indicators[0];
    }

    // Show fluid of the analyte in erlenmeyer flask at beginning
    public void enableMeshRenderer()
    {
        if (acidTitrationScript.analyteText == "HNO3")
            changeMeshMaterial(colorEnum.yellow);
        meshRend.enabled = !meshRend.enabled;
    }

    // Change the material of analyte during titration
    public void changeMeshMaterial(colorEnum color)
    {
        switch (color)
        {
            case colorEnum.colorless:
                meshRend.material = FluidWaterColorless;
                break;
            case colorEnum.blue:
                meshRend.material = FluidWaterBlue;
                break;
            case colorEnum.red:
                meshRend.material = FluidWaterRed;
                break;
            case colorEnum.yellow:
                meshRend.material = FluidWaterYellow;
                break;
            case colorEnum.green:
                meshRend.material = FluidWaterGreen;
                break;
            case colorEnum.orange:
                meshRend.material = FluidWaterOrange;
                break;
            default:
                break;
        }
    }

    // Show fluid of the analyte with indicator in erlenmeyer flask 
    // activated in Animator
    public void activateIndicatorColor()
    {
        result = acidTitrationScript.getResultDictionary();
        if (result.Count > 0)
            determineAnalyteColor((float)result.Values.First());
    }

    public void setCurrentIndicator(int value)
    {
        currentInd = indicators[value];
    }

    public void determineAnalyteColor(float phValue)
    {
        if (phValue >= currentInd.startNumber && phValue <= currentInd.endNumber)
        {
            changeMeshMaterial(currentInd.midColor);
        }
        if (phValue > currentInd.endNumber)
        {
            changeMeshMaterial(currentInd.highColor);
        }
        if (phValue < currentInd.startNumber)
        {
            changeMeshMaterial(currentInd.lowColor);
        }
    }
}
