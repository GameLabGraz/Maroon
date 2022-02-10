using GEAR.Gadgets.Attribute;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics.CoordinateSystem
{
    public enum Axis
    {
        X = 1,
        Y = 2,
        Z = 3,
        NX = -1,
        NY = -2,
        NZ = -3
    }

    public enum Unit
    {
        [StringValue("respective")] respective = -1000,
        [StringValue("none")] none = -999,
        [StringValue("femto")] femto = -15,
        [StringValue("pico")] pico = -12,
        [StringValue("nm")] nm = -9,
        [StringValue("µm")] µm = -6,
        [StringValue("mm")] mm = -3,
        [StringValue("cm")] cm = -2,
        [StringValue("dm")] dm = -1,
        [StringValue("m")] m = 0,
        [StringValue("km")] km = 3
    }

    public class AxisController : MonoBehaviour
    {
        [SerializeField] private bool _enableNegativeDirection = false;

        [SerializeField] private bool _enableThirdDimension = false;

        [SerializeField] private bool _lengthUniform = true;

        [SerializeField] private float _uniformWorldAxisLength = 1;

        [SerializeField] private float _markerFontSize;

        [SerializeField] private bool _enableVisualIndicator;

        [SerializeField] private GameObject _spaceIndicator;

        [SerializeField] private List<CoordAxis> _axisList = new List<CoordAxis>(6);

        private Dictionary<Axis, CoordAxis> _axisDictionary = new Dictionary<Axis, CoordAxis>();
        public Dictionary<Axis, CoordAxis> GetAxisDictionary() => _axisDictionary;

        #region Accessors & Mutators

        public float FontSize
        {
            get => _markerFontSize;
            set
            {
                _markerFontSize = value;
                UpdateAxisFontSize();
            }
        }

        public bool EnableNegativeDirection
        {
            get => _enableNegativeDirection;
            set
            {
                _enableNegativeDirection = value;
                ToggleNegativeAxisVisibility();
            }
        }

        public bool EnableThirdDimension
        {
            get => _enableThirdDimension;
            set
            {
                _enableThirdDimension = value;
                ToggleThirdDimension();
            }
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

        public List<CoordAxis> PositveAxis => new List<CoordAxis> { FindInList(Axis.X), 
            FindInList(Axis.Y), 
            FindInList(Axis.Z) };
        public List<CoordAxis> NegativeAxis => new List<CoordAxis> { FindInList(Axis.NX),
            FindInList(Axis.NY),
            FindInList(Axis.NZ) };

        public Vector3 PositiveWorldLengths => new Vector3(FindInList(Axis.X).AxisWorldLength,
            FindInList(Axis.Y).AxisWorldLength,
            FindInList(Axis.Z).AxisWorldLength);

        public Vector3 NegativeWorldLengths => new Vector3(FindInList(Axis.NX).AxisWorldLength,
            FindInList(Axis.NY).AxisWorldLength,
            FindInList(Axis.NZ).AxisWorldLength);


        #endregion

        private void Awake()
        {
            foreach (var axis in _axisList)
            {
                _ = axis ?? throw new NullReferenceException();
                _axisDictionary[axis.AxisID] = axis;
            }

            if (_lengthUniform)
                SetAxisWorldLengthUniform();
        }

        private void Start()
        {
            ToggleThirdDimension();
            ToggleNegativeAxisVisibility();
            ToggleSpaceIndicatorVisibility();
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

        public void ToggleNegativeAxisVisibility()
        {
            var negativeAxis = NegativeAxis;

            if (_enableNegativeDirection)
            {
                var positiveAxis = PositveAxis;
                negativeAxis[0].CopyAxisValuesFrom(positiveAxis[0], true);
                negativeAxis[1].CopyAxisValuesFrom(positiveAxis[1], true);
                negativeAxis[2].CopyAxisValuesFrom(positiveAxis[2], true);
            }

            negativeAxis[0].gameObject.SetActive(_enableNegativeDirection);
            negativeAxis[1].gameObject.SetActive(_enableNegativeDirection);
           
            if (_enableThirdDimension)
                negativeAxis[2].gameObject.SetActive(_enableNegativeDirection);     
        }

        public void ToggleSpaceIndicatorVisibility()
        {
            var axisWorldLengths = PositiveWorldLengths;
            var negativeAxisWorldLengths = NegativeWorldLengths;

            PlaceIndicators(axisWorldLengths, negativeAxisWorldLengths);
            ScaleIndicators(axisWorldLengths, negativeAxisWorldLengths);

            _spaceIndicator.gameObject.SetActive(_enableVisualIndicator);
        }

        public void UpdateAxisFontSize()
        {
            foreach (var axis in _axisList)
            {
                axis?.UpdateFontSize(FontSize);
            }
        }

        private void UpdateNegativeAxisDimensions(CoordAxis pAxis, CoordAxis nAxis)
        {
            nAxis.AxisWorldLength = pAxis.AxisWorldLength;
            nAxis.AxisLocalLength = pAxis.AxisLocalLength;
            nAxis.AxisLengthUnit = pAxis.AxisLengthUnit;
            nAxis.AxisSubdivisionUnit = pAxis.AxisSubdivisionUnit;
        }

        public void ToggleThirdDimension()
        {
            FindInList(Axis.Z).gameObject.SetActive(_enableThirdDimension);

            if (_enableNegativeDirection)
                FindInList(Axis.NZ).gameObject.SetActive(_enableThirdDimension);
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
                _spaceIndicator.gameObject.transform.position = _enableThirdDimension
                    ? axisWorldLengths * 0.5f
                    : new Vector3(axisWorldLengths.x * 0.5f, axisWorldLengths.y * 0.5f,
                        gameObject.transform.position.z);
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

        public List<Unit> GetAxisUnits() => new List<Unit>()
        {
            FindInList(Axis.X).AxisLengthUnit,
            FindInList(Axis.Y).AxisLengthUnit,
            FindInList(Axis.Z).AxisLengthUnit
        };

        public List<Unit> GetAxisSubdivisionUnits() => new List<Unit>()
        {
            FindInList(Axis.X).AxisSubdivisionUnit,
            FindInList(Axis.Y).AxisSubdivisionUnit,
            FindInList(Axis.Z).AxisSubdivisionUnit
        };
     

        private CoordAxis FindInList(Axis direction) => _axisList.Find(axis => axis.AxisID == direction);
    }
}