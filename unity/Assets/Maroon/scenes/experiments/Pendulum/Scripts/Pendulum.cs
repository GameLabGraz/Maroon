using System;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Physics.Pendulum
{
    [RequireComponent(typeof(HingeJoint))]
    public class Pendulum : PausableObject, IResetObject
    {
        public QuantityFloat weight = 1.0f;

        public QuantityFloat ropeLength = 0.3f;

        public QuantityFloat elongation = 0.0f;
		
        [SerializeField]
        private GameObject _standRopeJoint;
    
        [SerializeField]
        private GameObject _weightObj;

        private Vector3 _startPos;

        private Quaternion _startRot;

        private float _startWeight;

        private float _startRopeLength;
        private Vector3 _startRopePosition;

        private bool _pendulumRelease = false;

        enum ElongationMode
        {
            EM_Increasing,
            EM_Decreasing,
            EM_StandingStill
        }
        
        private float _previousElongation = 0f;
        private float _currentElongation = 0f;
        private ElongationMode _elongationMode = ElongationMode.EM_StandingStill;

        public HingeJoint Joint { get; private set; }

        public UnityEvent onPendulumRotationMaximumPoint = new UnityEvent();
        public UnityEvent onPendulumRotationNullPoint = new UnityEvent();
        public UnityEvent onPendulumRotationStopped = new UnityEvent();
        
        public float Weight
        {
            get => weight;
            set
            {
                weight.Value = value;
                UpdatePendulum();
            }
        }
        
        public float RopeLength
        {
            get => ropeLength;
            set
            {
                ropeLength.Value = value;
                UpdatePendulum();
            }
        }

        public void UpdatePendulum()
        {
            //weight
            _rigidBody.mass = weight.Value;
            _weightObj.transform.localScale = Vector3.one * weight.Value;
            
            //rope len
            var pos = _weightObj.transform.position;
            var moveDirection = (_startRopePosition - _standRopeJoint.transform.position).normalized;
            _weightObj.transform.position = _startRopePosition + moveDirection * (ropeLength.Value - _startRopeLength);
        }
        
        public float Elongation
        {
            get => elongation;
            set => elongation.Value = value;
        }

        protected override void Start()
        {
            base.Start();
            Joint = GetComponent<HingeJoint>();

            _rigidBody.mass = Weight;

            _startPos = transform.position;
            _startRot = transform.rotation;
            _startWeight = Weight;
            _startRopeLength = RopeLength;
            _startRopePosition = _weightObj.transform.position;
        }

        protected override void HandleUpdate()
        {

        }
        
        protected override void HandleFixedUpdate()
        {
            if (!_pendulumRelease) return;
            
            _previousElongation = _currentElongation;
            _currentElongation = transform.rotation.x;

            if (Mathf.Abs(_previousElongation) <= 0.000001f && Mathf.Abs(Elongation) < 0.000001f &&
                _elongationMode != ElongationMode.EM_StandingStill)
            {
                Elongation = 0f;
                _elongationMode = ElongationMode.EM_StandingStill;
                onPendulumRotationStopped.Invoke();
                _pendulumRelease = false;
                Debug.Log("Pendulum Stopped");
                return;
            }
            
            if (_elongationMode == ElongationMode.EM_StandingStill && Mathf.Abs(_currentElongation) > 0.000001f)
            {
                _elongationMode = _currentElongation > 0 ? ElongationMode.EM_Decreasing : ElongationMode.EM_Increasing;
            }
            else
            {
                if ((_previousElongation <= 0 && _currentElongation >= 0) ||
                    (_previousElongation >= 0 && _currentElongation <= 0))
                {
                    Debug.Log("Null Point: " + _previousElongation + " - " + _currentElongation);
                    Elongation = 0f;
                    onPendulumRotationNullPoint.Invoke();
                }

                if (_elongationMode == ElongationMode.EM_Decreasing && _previousElongation > _currentElongation)
                {
                    _elongationMode = ElongationMode.EM_Increasing;
                    Elongation = _previousElongation;
                    onPendulumRotationMaximumPoint.Invoke();
                    Debug.Log("Pendulum Maximum" + _currentElongation + " - " + _previousElongation);
                }
                else if(_elongationMode == ElongationMode.EM_Increasing && _currentElongation > _previousElongation)
                {
                    _elongationMode = ElongationMode.EM_Decreasing;
                    Elongation = _previousElongation;
                    onPendulumRotationMaximumPoint.Invoke();
                    Debug.Log("Pendulum Maximum" + _currentElongation + " - " + _previousElongation);
                }
            }
        }

        public float GetDeflection()
        {
            var x = transform.localEulerAngles.x;
            x = x < 180 ? x : x - 360;
            return (float)Math.Round(x, 1);
        }

        public void GetDeflectionByReference(MessageArgs args)
        {
            args.value = GetDeflection();
        }

        public void GetWeightByReference(MessageArgs args)
        {
            args.value = Weight;
        }

        public void GetRopeLengthByReference(MessageArgs args)
        {
            args.value = RopeLength;
        }

        public void ResetObject()
        {
            Weight = _startWeight;
            RopeLength = _startRopeLength;

            transform.position = _startPos;
            transform.rotation = _startRot;

            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;

            Elongation = 0f;

            UpdatePendulum();
        }

        public void PendulumReleased()
        {
            _pendulumRelease = true;
        }

        public void UpdateElongation()
        {
            Elongation = transform.rotation.x;
        }
    }
}