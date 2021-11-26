using System;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics.CoordinateSystem
{
    public class CoordSystem : MonoBehaviour
    {
        [SerializeField] private Transform origin;
        [SerializeField] private CoordSystemManager systemManager;

        private Dictionary<Axis, CoordAxis> _axisDictionary;

        private static CoordSystem _instance;

        public static CoordSystem Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<CoordSystem>();
                return _instance;
            }
        }

        private void Start()
        {
            _ = origin ?? throw new NullReferenceException();
            _ = systemManager ?? throw new NullReferenceException();

            _axisDictionary = systemManager.GetAxisDictionary();
        }

        public Vector3 GetPositionInAxisUnits(Vector3 objectTransform)
        {
            var systemSpaceCoordinates = WorldCoordinatesToSystemSpace(objectTransform);

            var xValue = _axisDictionary[Axis.X].GetValueFromAxisPoint(systemSpaceCoordinates.x);
            var yValue = _axisDictionary[Axis.Y].GetValueFromAxisPoint(systemSpaceCoordinates.y);
            var zValue = _axisDictionary[Axis.Z].GetValueFromAxisPoint(systemSpaceCoordinates.z);

            return new Vector3(xValue, yValue, zValue);
        }

        public Vector3 GetPositionInWorldSpace(Vector3 localSpacePosition, Unit[] respectiveUnits)
        {
            var xAxis = _axisDictionary[Axis.X];
            var yAxis = _axisDictionary[Axis.Y];
            var zAxis = _axisDictionary[Axis.Z];

            var xValue = xAxis.GetAxisPointFromValue(localSpacePosition.x, respectiveUnits[0]) * xAxis.AxisWorldLength;
            var yValue = yAxis.GetAxisPointFromValue(localSpacePosition.y, respectiveUnits[1]) * yAxis.AxisWorldLength;
            var zValue = zAxis.GetAxisPointFromValue(localSpacePosition.z, respectiveUnits[2]) * zAxis.AxisWorldLength;

            var localSpaceVector = new Vector3(xValue, yValue, zValue);
            return origin.TransformDirection(localSpaceVector);
        }

        public Vector3 WorldCoordinatesToSystemSpace(Vector3 objectTransform)
        {
            var axisLengths = systemManager.GetWorldLengthsOfDirection(true);
            var objectPosition = origin.InverseTransformDirection(objectTransform);

            var objectPositionRelativeToAxisLength = new Vector3(objectPosition.x / axisLengths.x,
                objectPosition.y / axisLengths.y,
                objectPosition.z / axisLengths.z);

            Debug.Log($"POSITION OF TEST CUBE: {objectPositionRelativeToAxisLength}");
            return objectPositionRelativeToAxisLength;
        }
    }
}
