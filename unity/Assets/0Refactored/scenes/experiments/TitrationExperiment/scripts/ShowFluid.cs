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
    private readonly Dictionary<int, Indicator> indicators = new Dictionary<int, Indicator>() {
         {0, new Indicator(colorEnum.colorless, colorEnum.red, colorEnum.red, 8.3f, 10f)}, // Phenolphtalein
         {1, new Indicator(colorEnum.yellow, colorEnum.green, colorEnum.blue, 6f, 7.6f)},  // Bromothymol blue
         {2, new Indicator(colorEnum.red, colorEnum.orange, colorEnum.yellow, 3.1f, 4.5f)} }; // Mehtyl orange
 
    private static Indicator currentInd;

    [Header("Fluid Materials")]
    public Material FluidWaterBlue;
    public Material FluidWaterColorless;
    public Material FluidWaterOrange;
    public Material FluidWaterRed;
    public Material FluidWaterYellow;
    public Material FluidWaterGreen;

    [SerializeField] private GameObject fluid;
    private MeshRenderer meshRend;
    private AcidTitration acidTitrationScript;
    private Dictionary<double, double> result = new Dictionary<double, double>();
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

    // Show fluid of the analyte in erlenmeyer flask at beginning -- for Animator
    public void EnableMeshRenderer()
    {
        if (acidTitrationScript.analyteText == "HNO3")
            ChangeMeshMaterial(colorEnum.yellow);
        meshRend.enabled = !meshRend.enabled;
    }

    public void DisableMeshRenderer()
    {
        meshRend.enabled = false;
    }

    // Change the material of analyte during titration
    public void ChangeMeshMaterial(colorEnum color)
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
    public void ActivateIndicatorColor()
    {
        result = acidTitrationScript.GetResultDictionary();
        if (result.Count > 0)
            DetermineAnalyteColor((float)result.Values.First());
    }

    public void SetCurrentIndicator(int value)
    {
        currentInd = indicators[value];
    }

    public void DetermineAnalyteColor(float phValue)
    {
        if (phValue >= currentInd.startNumber && phValue <= currentInd.endNumber)
        {
            ChangeMeshMaterial(currentInd.midColor);
        }
        if (phValue > currentInd.endNumber)
        {
            ChangeMeshMaterial(currentInd.highColor);
        }
        if (phValue < currentInd.startNumber)
        {
            ChangeMeshMaterial(currentInd.lowColor);
        }
    }
}
