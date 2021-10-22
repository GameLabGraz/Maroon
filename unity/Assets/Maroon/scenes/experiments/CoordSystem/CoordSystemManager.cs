﻿using Maroon.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public enum Axis { X = 1, Y = 2, Z = 3, NX = -1, NY = -2, NZ = -3}
public enum Unit { none = -999,nm = -9, µm = -6, mm = -3, cm = -2, dm = -1, m = 0, km = 3}

public class CoordSystemManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private bool _enableNegativeDirection = false;
    
    [SerializeField] private bool _enableThirdDimension = false;

    [SerializeField] private bool _lengthUniform = true;

    [SerializeField] private float _uniformWorldAxisLength = 1;

    [SerializeField] private float _markerFontSize;

    [SerializeField] private bool _enableVisualIndicator;

    [SerializeField] private GameObject _spaceIndicator;
   
    [SerializeField]
    private List<CoordAxis> _axisList = new List<CoordAxis>(6);

    private Dictionary<Axis, CoordAxis> _axisDictionary;
    public Dictionary<Axis, CoordAxis> GetAxisDictionary() => _axisDictionary;

    public float FontSize
    {
        get => _markerFontSize;
        set 
        {
            _markerFontSize = value;
            SystemChangeHandler.Instance.FontSizeChanged(_markerFontSize); 
        }
    }

    public bool EnableNegativeDirection 
    { 
        get => _enableNegativeDirection;
        set => _enableNegativeDirection = value;
    }

    public float UniformWorldAxisLength
    {
        get => _uniformWorldAxisLength;
        set
        {
            _uniformWorldAxisLength = value;
            SetAxisWorldLengthUniform();
        }
    }

    private void Awake()
    {
        _axisDictionary = DictionaryFromAxisList();

        foreach (var axis in _axisList)
        {
            _ = axis ?? throw new NullReferenceException();
        }
        
        if (_lengthUniform)
            SetAxisWorldLengthUniform();
    }

    private void Start()
    {
        ToggleThirdDimension(_enableThirdDimension);
        ToggleNegativeAxisVisibility(_enableNegativeDirection);
        ToggleSpaceIndicatorVisibility(_enableVisualIndicator);
        UpdateAxisFontSize();
    }

    public void SetAxisWorldLengthUniform()
    {
        foreach (var axis in _axisList)
        {
            axis.AxisWorldLength = _uniformWorldAxisLength;
            axis.SetupAxis();
        }
    }
    
    public void ToggleNegativeAxisVisibility(bool active)
    {
        _axisList.ElementAt(FindInList(Axis.NX)).gameObject.SetActive(active);
        _axisList.ElementAt(FindInList(Axis.NY)).gameObject.SetActive(active);
        
        if (_enableThirdDimension)
            _axisList.ElementAt(FindInList(Axis.NZ)).gameObject.SetActive(active);
    }

    public void ToggleSpaceIndicatorVisibility(bool active)
    {
        var axisWorldLengths = GetWorldLengthsOfDirection(true);
        var negativeAxisWorldLengths = GetWorldLengthsOfDirection(false);

        PlaceIndicators(axisWorldLengths, negativeAxisWorldLengths);
        ScaleIndicators(axisWorldLengths, negativeAxisWorldLengths);

        _spaceIndicator.gameObject.SetActive(active);
    }
    
    public void UpdateAxisFontSize()
    {
        foreach (var axis in _axisList)
        {
            axis?.UpdateFontSize(FontSize);
        }
    }

    public void ToggleThirdDimension(bool active)
    {
        _axisList.ElementAt(FindInList(Axis.Z)).gameObject.SetActive(active);
        
        if(_enableNegativeDirection)
           _axisList.ElementAt(FindInList(Axis.NZ)).gameObject.SetActive(active);
    }

    private void PlaceIndicators(Vector3 axisWorldLengths, Vector3 negativeAxisWorldLengths)
    {
        if (_enableNegativeDirection)
        {
            var xPos = (gameObject.transform.position.x + axisWorldLengths.x - negativeAxisWorldLengths.x) / 2.0f;
            var yPos = (gameObject.transform.position.y + axisWorldLengths.y - negativeAxisWorldLengths.y) / 2.0f;
            var zPos = (gameObject.transform.position.z + axisWorldLengths.z - negativeAxisWorldLengths.z) / 2.0f;

            _spaceIndicator.gameObject.transform.position = _enableThirdDimension
                ? new Vector3(xPos, yPos, zPos)
                : new Vector3(xPos, yPos, gameObject.transform.position.z);
        }
        else
        {
            _spaceIndicator.gameObject.transform.position = _enableThirdDimension ?
                axisWorldLengths * 0.5f : new Vector3(axisWorldLengths.x * 0.5f, axisWorldLengths.y * 0.5f, gameObject.transform.position.z);
        }
    }

    private void ScaleIndicators(Vector3 axisWorldLengths, Vector3 negativeAxisWorldLengths)
    {
        if (_enableNegativeDirection)
        {
            var buffer = .5f;
            var xLength = axisWorldLengths.x + negativeAxisWorldLengths.x + buffer;
            var yLength = axisWorldLengths.y + negativeAxisWorldLengths.y + buffer;
            var zLength = axisWorldLengths.z + negativeAxisWorldLengths.z + buffer;

            _spaceIndicator.gameObject.transform.localScale = _enableThirdDimension
                ? new Vector3(xLength, yLength, zLength)
                : new Vector3(xLength, yLength, 0.2f);
        }
        else
        {
            _spaceIndicator.gameObject.transform.localScale = _enableThirdDimension
                ? new Vector3(axisWorldLengths.x, axisWorldLengths.y, axisWorldLengths.z)
                : new Vector3(axisWorldLengths.x, axisWorldLengths.y, 0.2f);
        }
    }
    
    public Vector3 GetWorldLengthsOfDirection(bool positive)
    {
        if (positive)
        {
            var x_worldLength = _axisList.ElementAt(FindInList(Axis.X)).AxisWorldLength;
            var y_worldLength = _axisList.ElementAt(FindInList(Axis.Y)).AxisWorldLength;
            var z_worldLength = _axisList.ElementAt(FindInList(Axis.Z)).AxisWorldLength;

            return new Vector3(x_worldLength, y_worldLength, z_worldLength); 
        }

        var nx_worldLength = _axisList.ElementAt(FindInList(Axis.NX)).AxisWorldLength;
        var ny_worldLength = _axisList.ElementAt(FindInList(Axis.NY)).AxisWorldLength;
        var nz_worldLength = _axisList.ElementAt(FindInList(Axis.NZ)).AxisWorldLength;

        return new Vector3(nx_worldLength, ny_worldLength, nz_worldLength);
    }

    
    private Dictionary<Axis, CoordAxis> DictionaryFromAxisList()
    {
        return new Dictionary<Axis, CoordAxis>()
        {
            {Axis.X, _axisList.ElementAt(FindInList(Axis.X))},
            {Axis.Y, _axisList.ElementAt(FindInList(Axis.Y))},
            {Axis.Z, _axisList.ElementAt(FindInList(Axis.Z))},
            {Axis.NX, _axisList.ElementAt(FindInList(Axis.NX))},
            {Axis.NY, _axisList.ElementAt(FindInList(Axis.NY))},
            {Axis.NZ, _axisList.ElementAt(FindInList(Axis.NZ))}
        };
    }

    private int FindInList(Axis direction)
    {
        for (var index = 0; index < _axisList.Count; index++)
        {
            if (direction.ToString() == _axisList.ElementAt(index).AxisID.ToString())
                return index;
        }

        throw new NullReferenceException($"There was no axis to be found {direction.ToString()}");
    }
}
