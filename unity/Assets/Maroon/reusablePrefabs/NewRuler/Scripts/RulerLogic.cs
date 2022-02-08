using System;
using Maroon.Physics;
using Maroon.Physics.CoordinateSystem;
using UnityEngine;
using Maroon.GlobalEntities;

namespace Assets.Maroon.reusablePrefabs.NewRuler.Scripts
{
    public class RulerLogic : MonoBehaviour, IResetObject
    {
        public GameObject RulerStart;
        public GameObject RulerEnd;
        public LineRenderer RulerLine;
        private QuantityFloat _pinDistance = 0;
        private Vector3 _resetPosition;
       
        public QuantityFloat PinDistance 
        { 
            get => _pinDistance; 
            set => _pinDistance = value;
        }

        private void Start()
        {
            RulerStart.GetComponent<RulerNotify>().onRulerPinEnable += OnEnableRulerPin;
            RulerStart.GetComponent<RulerNotify>().onRulerPinDisable += OnDisableRulerPin;

            RulerEnd.GetComponent<RulerNotify>().onRulerPinEnable += OnEnableRulerPin;
            RulerEnd.GetComponent<RulerNotify>().onRulerPinDisable += OnDisableRulerPin;

            _resetPosition = transform.position;

            if (RulerLine.positionCount < 2)
            {
                RulerLine.SetPositions(new []{RulerStart.transform.position, RulerEnd.transform.position});
                UpdateRulerLine();
            }

            PinDistance.Value = 0f;
        }
    
        public float CalculateDistance(Unit targetUnit = Unit.m)
        {
            var endPos = CoordSystemHandler.Instance.GetSystemPosition(RulerEnd.transform.position, targetUnit);
            var startPos = CoordSystemHandler.Instance.GetSystemPosition(RulerStart.transform.position, targetUnit);
          
            var calcDist = Vector3.Distance(startPos, endPos);
       
            if (Math.Abs(calcDist - _pinDistance) > 0.0001f)
            {
                _pinDistance = calcDist;
            }

            return _pinDistance;
        }

        public void OnEnableRulerPin()
        {
            if (RulerStart.activeSelf && RulerEnd.activeSelf)
            {
                if (!RulerLine.gameObject.activeSelf)
                    RulerLine.gameObject.SetActive(true);

                UpdateRulerLine();
            }
        }

        public void OnDisableRulerPin()
        {
            if (RulerLine.gameObject.activeSelf)
            {
                RulerLine.gameObject.SetActive(false);
                PinDistance.Value = 0f;
            }
        }

        public void UpdateRulerLine()
        {
            RulerLine.SetPosition(0, RulerStart.transform.position);
            RulerLine.SetPosition(1, RulerEnd.transform.position);
        }

        public void ResetObject()
        {
            transform.position = _resetPosition;
            transform.localRotation = Quaternion.identity;
        }
    }
}
