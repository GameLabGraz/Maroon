using Maroon.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEditor;
using TMPro;

public class CoordAxis : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Axis _axisID;
    [SerializeField]
    private float _axisWorldLength;
    [SerializeField]
    private GameObject _axisMarkerPrefab;

    [SerializeField]
    private float _axisLocalLength;
    [SerializeField]
    private Unit _lengthUnit;

    [SerializeField]
    private float _axisSubdivision;
    [SerializeField]
    private Unit _divisionUnit;

    private Vector3 _direction;
    private float _fontSize = 0.5f;
    private List<GameObject> _axisMarkers = new List<GameObject>();

    #region Accessors & Mutators

    public float AxisWorldLength
    {
        get => _axisWorldLength;
        set => _axisWorldLength = value;
    }

    public float AxisLocalLength
    {
        get => _axisLocalLength;
        set => _axisLocalLength = value;
    }

    public Unit AxisLengthUnit
    {
        get => _lengthUnit;
        set => _lengthUnit = value;
    }

    public Unit AxisSubdivisionUnit
    {
        get => _divisionUnit;
        set => _divisionUnit = value;
    }

    public Axis AxisID
    {
        get => _axisID;
        set => _axisID = value;
    }

    #endregion
    
    private void OnEnable()
    {
        SystemChangeHandler.Instance.OnUniformWorldLength += UniformWorldSpaceLengthUpdate;
    }

    private void OnDisable()
    {
        SystemChangeHandler.Instance.OnUniformWorldLength -= UniformWorldSpaceLengthUpdate;
    }

    private void Awake()
    {
        _ = _axisMarkerPrefab ?? throw new NullReferenceException("Missing axis marker prefab!");
        _ = _axisID == 0 ? throw new NullReferenceException("Axis id is not set") : 0;
        _ = _divisionUnit == Unit.none ? throw new NullReferenceException("The unit of the subdivision is not set!(mm,cm,m)") : 0;
        _ = _lengthUnit == Unit.none ? throw new NullReferenceException("The length unit of the axis is not set! (mm,cm,m)") : 0;
    }

    void Start()
    {
        SetupAxis();
        
        //SystemChangeHandler.Instance.onWorldSpaceAxisScaleChange += UpdateWorldSpaceLength;
        //SystemChangeHandler.Instance.onLocalSpaceAxisLengthChange += UpdateLocalSpaceLength;
        //SystemChangeHandler.Instance.onEnableNegativeDirectionChange += UpdateNegativeAxisDirection;
    }

    public void UpdateFontSize(float value)
    {
        _fontSize = value;
        foreach(var marker in GetComponentsInChildren<TextMeshPro>())
        {
            marker.fontSize = value;
        }
    }

    public void SetupAxis()
    {
        ResetAxis();
        DetermineAxisDirection();
        GenerateMarkers();
    }

    private void GenerateMarkers()
    {
        var numberOfMarkers = CalculateNumberOfMarkers();
        var worldSpaceDistance = _axisWorldLength / numberOfMarkers;

        for (var count = 1; count <= numberOfMarkers; count++)
        {
            var translate = _direction * (worldSpaceDistance * count);
            var unitText = (_divisionUnit != Unit.none && _lengthUnit != Unit.none) ? _divisionUnit.ToString() : "";
            var valueText = (count * _axisSubdivision).ToString(CultureInfo.CurrentCulture);
            var completeText = _axisID == Axis.Y || _axisID == Axis.NY ? $"- {valueText} {unitText}" : $"{valueText} {unitText}\n |";

            InstantiateMarker(translate, completeText);
        }
    }

    private int CalculateNumberOfMarkers()
    {
        int numberOfMarkers = 0;
        if (_lengthUnit == Unit.none || _divisionUnit == Unit.none)
        {
            numberOfMarkers = (int)Math.Floor(_axisLocalLength / _axisSubdivision);
        }
        else
        {
            var actualLocalLength = _axisLocalLength * Mathf.Pow(10, (float)_lengthUnit);
            var subdivisionLength = _axisSubdivision * Mathf.Pow(10, (float)_divisionUnit);

            numberOfMarkers = (int)Math.Floor(actualLocalLength / subdivisionLength);
        }

        return numberOfMarkers;
    }

    private GameObject InstantiateMarker(Vector3 translation, string text)
    {
        var marker = Instantiate(_axisMarkerPrefab, gameObject.transform);
        marker.transform.Translate(translation);
        
        var textMeshText = marker.transform.Find("Marker_Text").GetComponent<TextMeshPro>();
        textMeshText.text = text;
        textMeshText.fontSize = _fontSize;
        
        _axisMarkers.Add(marker);

        return marker;
    }

    private void ResetAxis()
    {
        foreach (var marker in this.GetComponentsInChildren<TextMeshPro>())
        {
            if(marker != null)
                GameObject.DestroyImmediate(marker.transform.parent.gameObject);
        }

        _axisMarkers.Clear();
    }

    public void DetermineAxisDirection()
    {
        switch (_axisID)
        {
            case Axis.X:
                _direction = new Vector3(1, 0, 0);
                break;
            case Axis.Y:
                _direction = new Vector3(0, 1, 0);
                break;
            case Axis.Z:
                _direction = new Vector3(0, 0, 1);
                break;
            case Axis.NX:
                _direction = new Vector3(-1, 0, 0);
                break;
            case Axis.NY:
                _direction = new Vector3(0, -1, 0);
                break;
            case Axis.NZ:
                _direction = new Vector3(0, 0, -1);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        
        /*
        _direction = (_axisID) switch
        {
            Axis.X => new Vector3(1, 0, 0),
            Axis.Y => new Vector3(0, 1, 0),
            Axis.Z => new Vector3(0, 0, 1),
            Axis.NX => new Vector3(-1, 0, 0),
            Axis.NY => new Vector3(0, -1, 0),
            Axis.NZ => new Vector3(0, 0, -1),
            _ => new Vector3(0, 0, 0)
        };*/
    }

    
    /*
    private void UpdateNegativeAxisDirection(Axis id)
    {
        if (id != _axisID)
            return;
    }

    private void UpdateLocalSpaceLength(Axis id, float value)
    {
        if (id != _axisID)
            return;
       
    }
    */
    private void UniformWorldSpaceLengthUpdate(float value)
    {
        this._axisWorldLength = value; 
        SetupAxis();
    }

    public float GetValueFromAxisPoint(float point)
    {
        //TODO FIX
        var pointOnAxis =  _axisLocalLength * point;
        var unitConversion = Mathf.Pow(10, (float) _lengthUnit) / Mathf.Pow(10, (float) _divisionUnit);

        return (pointOnAxis * unitConversion);
    }

    /*
    private void UpdateAxisSubdivision(Axis id, int value)
    {
        if (id != _axisID)
            return;  
    }*/

}
