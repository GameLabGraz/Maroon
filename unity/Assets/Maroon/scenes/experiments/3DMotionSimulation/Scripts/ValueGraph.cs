using Maroon.Physics;
using Maroon.UI.Charts;
using Maroon.Physics.ThreeDimensionalMotion;
using PlottableValues;

namespace PlottableValues
{
    public enum PlotValue
    {
        None,
        X,
        Y,
        Z,
        Vx,
        Vy,
        Vz,
        Fx,
        Fy,
        Fz,
        Power,
        Ekin,
        Work
    }
}

public class ValueGraph : PausableObject
{

    private static ValueGraph _instance;
    public SimpleLineChart simpleLineChart;

    private int _choice = 0;


    /// <summary>
    /// Update is called every frame
    /// </summary>
    protected override void HandleUpdate()
    {

    }

    /// <summary>
    /// Update is called every frame
    /// </summary>
    protected override void HandleFixedUpdate()
    {
       
    }

    /// <summary>
    /// Loads and displays the choosen value in the LineChart (GUI)
    /// </summary>
    /// <param name="choice">Data to load</param>
    public void LoadChoosenValue(int choice)
    {
        simpleLineChart.ResetObject();
        _choice = choice;

        PlotValue value = (PlotValue)_choice;

        switch (value)
        {
            case PlotValue.None:
                break;
            case PlotValue.X:
                PlotX();
                break;
            case PlotValue.Y:
                PlotY();
                break;
            case PlotValue.Z:
                PlotZ();
                break;
            case PlotValue.Vx:
                PlotVX();
                break;
            case PlotValue.Vy:
                PlotVY();
                break;
            case PlotValue.Vz:
                PlotVZ();
                break;
            case PlotValue.Fx:
                PlotFX();
                break;
            case PlotValue.Fy:
                PlotFY();
                break;
            case PlotValue.Fz:
                PlotFZ();
                break;
            case PlotValue.Power:
                PlotP();
                break;
            case PlotValue.Ekin:
                PlotEkin();
                break;
            case PlotValue.Work:
                PlotW();
                break;
            default:
                break;
        }
    }

    // Methods for plotting the desired values
    // -----------------------------------------------------------------
    private void PlotX()
    {
        simpleLineChart.AddData(MotionCalculation.Instance.GetDataX());
    }

    private void PlotY()
    {
        simpleLineChart.AddData(MotionCalculation.Instance.GetDataY());
    }

    private void PlotZ()
    {
        simpleLineChart.AddData(MotionCalculation.Instance.GetDataZ());
    }

    private void PlotVX()
    {
        simpleLineChart.AddData(MotionCalculation.Instance.GetDataVX());
    }

    private void PlotVY()
    {
        simpleLineChart.AddData(MotionCalculation.Instance.GetDataVY());
    }

    private void PlotVZ()
    {
        simpleLineChart.AddData(MotionCalculation.Instance.GetDataVZ());
    }

    private void PlotFX()
    {
        simpleLineChart.AddData(MotionCalculation.Instance.GetDataFX());
    }

    private void PlotFY()
    {
        simpleLineChart.AddData(MotionCalculation.Instance.GetDataFY());
    }

    private void PlotFZ()
    {
        simpleLineChart.AddData(MotionCalculation.Instance.GetDataFZ());
    }

    private void PlotP()
    {
        simpleLineChart.AddData(MotionCalculation.Instance.GetDataP());
    }

    private void PlotEkin()
    {
        simpleLineChart.AddData(MotionCalculation.Instance.GetDataEkin());
    }

    private void PlotW()
    {
        simpleLineChart.AddData(MotionCalculation.Instance.GetDataW());
    }
    // -----------------------------------------------------------------

    public static ValueGraph Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ValueGraph>();
            return _instance;
        }
    }

    /// <summary>
    /// Resets the object
    /// </summary>
    public void ResetObject()
    {
        LoadChoosenValue(_choice);
    }
}
