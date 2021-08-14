using Maroon.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum Axis { X = 1, Y = 2, Z = 3 }
public enum Unit { none = -999, mm = -3, cm = -2, dm = -1, m = 0, km = 3}

public class CoordSystemManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private bool enableNegativeDirection = false;
    
    [SerializeField]
    private bool enableThirdDimension = false;

    [SerializeField]
    private bool isWorldLengthUniform = true;

    [SerializeField]
    private float uniformWorldAxisLength = 1;

    [SerializeField]
    private float markerFontSize;

    public float FontSize
    {
        get => markerFontSize;
        set 
        {
            markerFontSize = value;
            SystemChangeHandler.Instance.FontSizeChanged(markerFontSize); 
        }
    }

    [SerializeField]
    private List<CoordAxis> _axisList = new List<CoordAxis>(3){};

    public bool EnableNegativeDirection { get => enableNegativeDirection;
        set => enableNegativeDirection = value;
    }

    private void Awake()
    {
        foreach (var axis in _axisList)
        {
            _ = axis ?? throw new NullReferenceException();
        }
    }

    private void Start()
    {
        if (isWorldLengthUniform)
        {
            SetAxisWorldLengthUniform(uniformWorldAxisLength);
        }
       
    }

    public void ToggleThirdDimension(bool active)
    {
       _axisList.ElementAt(2).gameObject.SetActive(active);
    }

    public void SetAxisWorldLengthUniform(float value)
    {
        foreach (var axis in _axisList)
        {
            axis.AxisWorldLength = uniformWorldAxisLength;
            axis.SetupAxis();
        }
    }

    public void UpdateAxisFontSize()
    {
        foreach (var axis in _axisList)
        {
            axis?.UpdateFontSize(FontSize);
        }
    }
}
