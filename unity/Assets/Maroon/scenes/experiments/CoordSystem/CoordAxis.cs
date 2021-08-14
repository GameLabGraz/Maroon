using Maroon.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using TMPro;
using UnityEngine.Purchasing;

public class CoordAxis : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Axis _axisID;
    [SerializeField]
    private float _axisWorldLength;
    [SerializeField]
    public GameObject _axisMarkerPrefab;

    [SerializeField]
    private float _axisLocalLength;
    [SerializeField]
    private Unit _lengthUnit;

    [SerializeField]
    private float _axisSubdivision;
    [SerializeField]
    private Unit _divisionUnit;

    public float AxisWorldLength {get { return _axisWorldLength;} set { _axisWorldLength = value; } }

    private float _fontSize = 0.5f;
    private List<GameObject> _axisMarkers = new List<GameObject>();
    private Vector3 _direction;

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
        _ = _divisionUnit == 0 ? throw new NullReferenceException("The unit of the subdivision is not set!(mm,cm,m)") : 0;
        _ = _lengthUnit == 0 ? throw new NullReferenceException("The length unit of the axis is not set! (mm,cm,m)") : 0;
        
        SetupAxis();
    }

    void Start()
    {
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
        var numberOfMarkers = CalculateNumberOfMarkers(out bool isDivisionRestless);
        var worldSpaceDistance = _axisWorldLength / numberOfMarkers;

        if(!isDivisionRestless)
        {
            var text = _axisID == Axis.Y ? $"- {_axisLocalLength} {_lengthUnit.ToString()}" : $"{_axisLocalLength} {_lengthUnit.ToString()}\n |";
            InstantiateMarker(_direction * _axisWorldLength, text);
        }


        for (var count = 1; count <= numberOfMarkers; count++)
        {
            var translate = _direction * worldSpaceDistance * count;
            var unitText = (_divisionUnit != Unit.none && _lengthUnit != Unit.none) ? _divisionUnit.ToString() : "";
            var valueText = (count * _axisSubdivision).ToString(CultureInfo.CurrentCulture);
            var completeText = _axisID == Axis.Y ? $"- {valueText} {unitText}" : $"{valueText} {unitText}\n |";

            InstantiateMarker(translate, completeText);
        }
    }

    private int CalculateNumberOfMarkers(out bool isDivisionRestless)
    {
        int numberOfMarkers = 0;
        if (_lengthUnit == Unit.none || _divisionUnit == Unit.none)
        {
            isDivisionRestless = _axisLocalLength % _axisSubdivision == 0 ? true : false;
            numberOfMarkers = (int)Math.Floor(_axisLocalLength / _axisSubdivision);
        }
        else
        {
            var actualLocalLength = _axisLocalLength * Mathf.Pow(10, (float)_lengthUnit);
            var subdivisionLength = _axisSubdivision * Mathf.Pow(10, (float)_divisionUnit);

            isDivisionRestless = actualLocalLength % subdivisionLength == 0 ? true : false;
            numberOfMarkers = (int)Math.Floor(actualLocalLength / subdivisionLength);
        }

        return numberOfMarkers;
    }

    private GameObject InstantiateMarker(Vector3 translation, string text)
    {
        var marker = Instantiate(_axisMarkerPrefab, this.transform);
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
        _direction = _axisID is Axis.X ? new Vector3(1, 0, 0) : _axisID is Axis.Y ? new Vector3(0, 1, 0) : new Vector3(0, 0, 1);
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

    /*
    private void UpdateAxisSubdivision(Axis id, int value)
    {
        if (id != _axisID)
            return;

       
    }*/

}
