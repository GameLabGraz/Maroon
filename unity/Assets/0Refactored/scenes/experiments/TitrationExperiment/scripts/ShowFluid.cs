using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum colorEnum { Colorless, Red, Yellow, Blue, Green, Orange };

public class Indicator
{
    public colorEnum LowColor { get; set; }
    public colorEnum MidColor { get; set; }
    public colorEnum HighColor { get; set; }
    public float StartNumber { get; set; }
    public float EndNumber { get; set; }

    public Indicator(colorEnum newLowColor, colorEnum newMidColor, colorEnum newHighColor, float newStartNumber, float newEndnumber)
    {
        LowColor = newLowColor;
        MidColor = newMidColor;
        HighColor = newHighColor;
        StartNumber = newStartNumber;
        EndNumber = newEndnumber;
    }
}
public class ShowFluid : MonoBehaviour
{
    // Indicators
    private readonly Dictionary<int, Indicator> indicators = new Dictionary<int, Indicator>
    {
         {0, new Indicator(colorEnum.Colorless, colorEnum.Red, colorEnum.Red, 8.3f, 10f)}, // Phenolphtalein
         {1, new Indicator(colorEnum.Yellow, colorEnum.Green, colorEnum.Blue, 6f, 7.6f)},  // Bromothymol blue
         {2, new Indicator(colorEnum.Red, colorEnum.Orange, colorEnum.Yellow, 3.1f, 4.5f)} // Mehtyl orange
    };
 
    private static Indicator _currentInd;

    [Header("Fluid Materials")]
    public Material FluidWaterBlue;
    public Material FluidWaterColorless;
    public Material FluidWaterOrange;
    public Material FluidWaterRed;
    public Material FluidWaterYellow;
    public Material FluidWaterGreen;

    [SerializeField] private GameObject fluid;

    private MeshRenderer _meshRend;
    [SerializeField] private AcidTitration _acidTitrationScript;
    private Dictionary<double, double> _result = new Dictionary<double, double>();
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
    private void Start()
    {
        _meshRend = fluid.GetComponent<MeshRenderer>();
        _meshRend.enabled = false;

        _currentInd = indicators[0];
    }

    // Show fluid of the analyte in erlenmeyer flask at beginning -- for Animator
    public void EnableMeshRenderer()
    {
        if (_acidTitrationScript.analyteText == "HNO3")
            ChangeMeshMaterial(colorEnum.Yellow);
        _meshRend.enabled = !_meshRend.enabled;
    }

    public void DisableMeshRenderer()
    {
        _meshRend.enabled = false;
    }

    // Change the material of analyte during titration
    public void ChangeMeshMaterial(colorEnum color)
    {
        switch (color)
        {
            case colorEnum.Colorless:
                _meshRend.material = FluidWaterColorless;
                break;
            case colorEnum.Blue:
                _meshRend.material = FluidWaterBlue;
                break;
            case colorEnum.Red:
                _meshRend.material = FluidWaterRed;
                break;
            case colorEnum.Yellow:
                _meshRend.material = FluidWaterYellow;
                break;
            case colorEnum.Green:
                _meshRend.material = FluidWaterGreen;
                break;
            case colorEnum.Orange:
                _meshRend.material = FluidWaterOrange;
                break;
            default:
                break;
        }
    }

    // Show fluid of the analyte with indicator in erlenmeyer flask 
    // activated in Animator
    public void ActivateIndicatorColor()
    {
        _result = _acidTitrationScript.GetResultDictionary();
        if (_result.Count > 0)
            DetermineAnalyteColor((float)_result.Values.First());
    }

    public void SetCurrentIndicator(int value)
    {
        _currentInd = indicators[value];
    }

    public void DetermineAnalyteColor(float phValue)
    {
        if (phValue >= _currentInd.StartNumber && phValue <= _currentInd.EndNumber)
        {
            ChangeMeshMaterial(_currentInd.MidColor);
        }
        if (phValue > _currentInd.EndNumber)
        {
            ChangeMeshMaterial(_currentInd.HighColor);
        }
        if (phValue < _currentInd.StartNumber)
        {
            ChangeMeshMaterial(_currentInd.LowColor);
        }
    }
}
