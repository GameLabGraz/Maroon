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
    private bool _enableNegativeDirection = false;
    
    [SerializeField]
    private bool _enableThirdDimension = false;

    [SerializeField]
    private bool _lengthUniform = true;

    [SerializeField]
    private float _uniformWorldAxisLength = 1;

    [SerializeField]
    private float _markerFontSize;

    public float FontSize
    {
        get => _markerFontSize;
        set 
        {
            _markerFontSize = value;
            SystemChangeHandler.Instance.FontSizeChanged(_markerFontSize); 
        }
    }

    [SerializeField]
    private List<CoordAxis> _axisList = new List<CoordAxis>(3){};

    public bool EnableNegativeDirection { get => _enableNegativeDirection;
        set => _enableNegativeDirection = value;
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
        if (_lengthUniform)
        {
            SetAxisWorldLengthUniform(_uniformWorldAxisLength);
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
            axis.AxisWorldLength = _uniformWorldAxisLength;
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
