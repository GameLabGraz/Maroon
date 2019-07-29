using System;
using UnityEngine;

namespace Maroon.Physics
{
    [RequireComponent(typeof(HingeJoint))]
    public class Pendulum : PausableObject, IResetObject
    {
        [SerializeField]
        private float _weight = 1;

        [SerializeField]
        private GameObject _standRopeJoint;

        [SerializeField]
        private float _ropeLength = 1;

        [SerializeField] private GameObject _weightObj;

        private Vector3 _startPos;

        private Quaternion _startRot;

        private float _startWeight;

        private float _startRopeLength;

        public HingeJoint Joint { get; private set; }

        public float Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                _rigidbody.mass = value;

                _weightObj.transform.localScale = Vector3.one * _weight;
            }
        }

        public float RopeLength
        {
            get => _ropeLength;
            set
            {
                //if (simController.SimulationRunning)
                    //simController.StopSimulation();

                var pos = _weightObj.transform.position;
                var moveDirection = (pos - _standRopeJoint.transform.position).normalized;
                _weightObj.transform.position = pos + moveDirection * (value - _ropeLength);

                _ropeLength = value;
            }
        }

        protected override void Start()
        {
            base.Start();

            Joint = GetComponent<HingeJoint>();

            _rigidbody.mass = Weight;

            _startPos = transform.position;
            _startRot = transform.rotation;
            _startWeight = Weight;
            _startRopeLength = RopeLength;
        }

        protected override void HandleUpdate()
        {

        }
        
        protected override void HandleFixedUpdate()
        {

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

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}