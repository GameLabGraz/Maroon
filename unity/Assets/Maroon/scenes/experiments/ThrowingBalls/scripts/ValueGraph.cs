using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.UI;
using TMPro;

public class ValueGraph : MonoBehaviour
{

    private static ValueGraph _instance;
    public SimpleLineChart simpleLineChart;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadChoosenValue(int choice)
    {
        switch (choice)
        {
            case 0:
                break;
            case 1:
                simpleLineChart.ResetObject();
                PlotX();
                break;
            case 2:
                simpleLineChart.ResetObject();
                PlotY();
                break;
            case 3:
                simpleLineChart.ResetObject();
                PlotZ();
                break;
            case 4:
                simpleLineChart.ResetObject();
                PlotVX();
                break;
            case 5:
                simpleLineChart.ResetObject();
                PlotVY();
                break;
            case 6:
                simpleLineChart.ResetObject();
                PlotVZ();
                break;
            case 7:
                simpleLineChart.ResetObject();
                PlotFX();
                break;
            case 8:
                simpleLineChart.ResetObject();
                PlotFY();
                break;
            case 9:
                simpleLineChart.ResetObject();
                PlotFZ();
                break;
            case 10:
                simpleLineChart.ResetObject();
                PlotP();
                break;
            case 11:
                simpleLineChart.ResetObject();
                PlotEkin();
                break;
            case 12:
                simpleLineChart.ResetObject();
                PlotW();
                break;
            default:
                break;
        }
    }

    private void PlotX()
    {
        simpleLineChart.AddData(Calculation.Instance.GetDataX());
    }

    private void PlotY()
    {
        simpleLineChart.AddData(Calculation.Instance.GetDataY());
    }

    private void PlotZ()
    {
        simpleLineChart.AddData(Calculation.Instance.GetDataZ());
    }

    private void PlotVX()
    {
        simpleLineChart.AddData(Calculation.Instance.GetDataVX());
    }

    private void PlotVY()
    {
        simpleLineChart.AddData(Calculation.Instance.GetDataVY());
    }

    private void PlotVZ()
    {
        simpleLineChart.AddData(Calculation.Instance.GetDataVZ());
    }

    private void PlotFX()
    {
        simpleLineChart.AddData(Calculation.Instance.GetDataFX());
    }

    private void PlotFY()
    {
        simpleLineChart.AddData(Calculation.Instance.GetDataFY());
    }

    private void PlotFZ()
    {
        simpleLineChart.AddData(Calculation.Instance.GetDataFZ());
    }

    private void PlotP()
    {
        simpleLineChart.AddData(Calculation.Instance.GetDataP());
    }

    private void PlotEkin()
    {
        simpleLineChart.AddData(Calculation.Instance.GetDataEkin());
    }

    private void PlotW()
    {
        simpleLineChart.AddData(Calculation.Instance.GetDataW());
    }

    public static ValueGraph Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ValueGraph>();
            return _instance;
        }
    }
}
