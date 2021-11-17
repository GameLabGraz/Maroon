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

public class ValueGraph : PausableObject, IResetObject
{

    private static ValueGraph _instance;
    public SimpleLineChart simpleLineChart;
    public MotionCalculation motionCalculation;

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
        PlotValue value = (PlotValue)choice;

        switch (value)
        {
            case PlotValue.None:
                break;
            case PlotValue.X:
                simpleLineChart.ResetObject();
                PlotX();
                break;
            case PlotValue.Y:
                simpleLineChart.ResetObject();
                PlotY();
                break;
            case PlotValue.Z:
                simpleLineChart.ResetObject();
                PlotZ();
                break;
            case PlotValue.Vx:
                simpleLineChart.ResetObject();
                PlotVX();
                break;
            case PlotValue.Vy:
                simpleLineChart.ResetObject();
                PlotVY();
                break;
            case PlotValue.Vz:
                simpleLineChart.ResetObject();
                PlotVZ();
                break;
            case PlotValue.Fx:
                simpleLineChart.ResetObject();
                PlotFX();
                break;
            case PlotValue.Fy:
                simpleLineChart.ResetObject();
                PlotFY();
                break;
            case PlotValue.Fz:
                simpleLineChart.ResetObject();
                PlotFZ();
                break;
            case PlotValue.Power:
                simpleLineChart.ResetObject();
                PlotP();
                break;
            case PlotValue.Ekin:
                simpleLineChart.ResetObject();
                PlotEkin();
                break;
            case PlotValue.Work:
                simpleLineChart.ResetObject();
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
        simpleLineChart.AddData(motionCalculation.GetDataX());
    }

    private void PlotY()
    {
        simpleLineChart.AddData(motionCalculation.GetDataY());
    }

    private void PlotZ()
    {
        simpleLineChart.AddData(motionCalculation.GetDataZ());
    }

    private void PlotVX()
    {
        simpleLineChart.AddData(motionCalculation.GetDataVX());
    }

    private void PlotVY()
    {
        simpleLineChart.AddData(motionCalculation.GetDataVY());
    }

    private void PlotVZ()
    {
        simpleLineChart.AddData(motionCalculation.GetDataVZ());
    }

    private void PlotFX()
    {
        simpleLineChart.AddData(motionCalculation.GetDataFX());
    }

    private void PlotFY()
    {
        simpleLineChart.AddData(motionCalculation.GetDataFY());
    }

    private void PlotFZ()
    {
        simpleLineChart.AddData(motionCalculation.GetDataFZ());
    }

    private void PlotP()
    {
        simpleLineChart.AddData(motionCalculation.GetDataP());
    }

    private void PlotEkin()
    {
        simpleLineChart.AddData(motionCalculation.GetDataEkin());
    }

    private void PlotW()
    {
        simpleLineChart.AddData(motionCalculation.GetDataW());
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
        simpleLineChart.ResetObject();
        motionCalculation.ResetObject();
    }
}
