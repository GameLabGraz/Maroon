using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.UI;
using TMPro;

public class ValueGraph : MonoBehaviour
{

    private static ValueGraph _instance;
    public SimpleLineChart simplelinechart;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadChoosenValue(int choice)
    {
        switch (choice)
        {
            case 0:
                break;
            case 1:
                simplelinechart.ResetObject();
                plotX();
                break;
            case 2:
                simplelinechart.ResetObject();
                plotY();
                break;
            case 3:
                simplelinechart.ResetObject();
                plotZ();
                break;
            case 4:
                simplelinechart.ResetObject();
                plotVX();
                break;
            case 5:
                simplelinechart.ResetObject();
                plotVY();
                break;
            case 6:
                simplelinechart.ResetObject();
                plotVZ();
                break;
            case 7:
                simplelinechart.ResetObject();
                plotFX();
                break;
            case 8:
                simplelinechart.ResetObject();
                plotFY();
                break;
            case 9:
                simplelinechart.ResetObject();
                plotFZ();
                break;
            case 10:
                simplelinechart.ResetObject();
                plotP();
                break;
            case 11:
                simplelinechart.ResetObject();
                plotEkin();
                break;
            case 12:
                simplelinechart.ResetObject();
                plotW();
                break;
            default:
                break;
        }
    }

    private void plotX()
    {
        simplelinechart.AddData(Calculation.Instance.getDataX());
    }

    private void plotY()
    {
        simplelinechart.AddData(Calculation.Instance.getDataY());
    }

    private void plotZ()
    {
        simplelinechart.AddData(Calculation.Instance.getDataZ());
    }

    private void plotVX()
    {
        simplelinechart.AddData(Calculation.Instance.getDataVX());
    }

    private void plotVY()
    {
        simplelinechart.AddData(Calculation.Instance.getDataVY());
    }

    private void plotVZ()
    {
        simplelinechart.AddData(Calculation.Instance.getDataVZ());
    }

    private void plotFX()
    {
        simplelinechart.AddData(Calculation.Instance.getDataFX());
    }

    private void plotFY()
    {
        simplelinechart.AddData(Calculation.Instance.getDataFY());
    }

    private void plotFZ()
    {
        simplelinechart.AddData(Calculation.Instance.getDataFZ());
    }

    private void plotP()
    {
        simplelinechart.AddData(Calculation.Instance.getDataP());
    }

    private void plotEkin()
    {
        simplelinechart.AddData(Calculation.Instance.getDataEkin());
    }

    private void plotW()
    {
        simplelinechart.AddData(Calculation.Instance.getDataW());
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
