using System;
using Maroon.Physics;
using Maroon.Physics.CoordinateSystem;
using UnityEngine;
using Maroon.GlobalEntities;

namespace Assets.Maroon.reusablePrefabs.NewRuler.Scripts
{
    public class RulerLogic : MonoBehaviour, IResetWholeObject
    {
        public GameObject RulerStart;
        public GameObject RulerEnd;
        public LineRenderer RulerLine;
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

        public float CalculateDistance(Unit targetUnit = Unit.m)
        {
            var calcDist = CoordSystemHelper.Instance.GetSystemDistance(RulerStart.transform.position, RulerEnd.transform.position, targetUnit);
       
            if (Math.Abs(calcDist - _pinDistance) > 0.0001f)
            {
                _pinDistance = calcDist;
            }

            return _pinDistance;
        }

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
