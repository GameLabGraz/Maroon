using System;
using Maroon.Physics;
using Maroon.Physics.CoordinateSystem;
using UnityEngine;

namespace Assets.Maroon.reusablePrefabs.NewRuler.Scripts
{
    public class RulerLogic : MonoBehaviour, IResetWholeObject
    {
        public GameObject RulerStart;
        public GameObject RulerEnd;
        public LineRenderer RulerLine;

        [SerializeField] private bool useBiggestDistanceUnit = true;

        private QuantityFloat _pinDistance = 0;

        public QuantityFloat PinDistance 
        { 
            get => _pinDistance; 
            set => _pinDistance = value;
        }
        private Vector3 _resetPosition; 

        void Start()
        {
            _resetPosition = transform.position;

            if (RulerLine.positionCount < 2)
            {
                RulerLine.SetPositions(new []{RulerStart.transform.position, RulerEnd.transform.position});
            }

            PinDistance.Value = 0f;
        }

        // Change this to a event system??
        void Update()
        {
            if (RulerStart.activeSelf && RulerEnd.activeSelf)
            {
                if (!RulerLine.gameObject.activeSelf)
                    RulerLine.gameObject.SetActive(true);

                RulerLine.SetPosition(0, RulerStart.transform.position);
                RulerLine.SetPosition(1, RulerEnd.transform.position);
            }
            else if (RulerLine.gameObject.activeSelf)
            {
                RulerLine.gameObject.SetActive(false);
                PinDistance.Value = 0f;
                
            }
        }

        public float CalculateDistance(Unit targetUnit = Unit.cm)
        {
            var startPosition = RulerStart.transform.position;
            var endPosition = RulerEnd.transform.position;
            var calcDist = 0f;

            if (CoordSystem.Instance == null)
            {
                calcDist = Vector3.Distance(startPosition, endPosition);
            }
            else
            {
                var localStartPos = CoordSystem.Instance.GetPositionInAxisUnits(startPosition, targetUnit);
                var localEndPos = CoordSystem.Instance.GetPositionInAxisUnits(endPosition, targetUnit);

                calcDist = Vector3.Distance(localStartPos, localEndPos);
            }
            
            if (Math.Abs(calcDist - _pinDistance) > 0.0001f)
            {
                _pinDistance = calcDist;
            }

            return _pinDistance;
        }

       /* private void ConvertToCommonUnit()
        {
            var xUnit = CoordSystem.Instance.AxisDictionary[Axis.X].AxisLengthUnit;
            var yUnit = CoordSystem.Instance.AxisDictionary[Axis.Y].AxisLengthUnit;
            var zUnit = CoordSystem.Instance.AxisDictionary[Axis.Z].AxisLengthUnit;
        }*/

        public void ResetObject()
        {
            
        }

        public void ResetWholeObject()
        {
            transform.position = _resetPosition;
            transform.localRotation = Quaternion.identity;
        }
    }
}
