using System;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics
{
  [Serializable]
  public class Node
  {
    public string interestingValue = "value";
    //The field below is what makes the serialization data become huge because
    //it introduces a 'class cycle'.
    public List<Node> children = new List<Node>();
  }

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

        private float _oldRopeLength;

        public HingeJoint Joint { get; private set; }

        public float Weight
        {
            get => weight;
            set => weight.Value = value;
        }

        public float RopeLength
        {
            get => ropeLength;
            set
            {
                _oldRopeLength = RopeLength;
                ropeLength.Value = value;
            }
        }

        public float Elongation
        {
            get => elongation;
            set => elongation.Value = value;
        }

        protected override void Start()
        {
            base.Start();
            
            weight.onValueChanged.AddListener((value) =>
            {
              _rigidbody.mass = value;
              _weightObj.transform.localScale = Vector3.one * value;
            });
            ropeLength.onValueChanged.AddListener((value) =>
            {
              var pos = _weightObj.transform.position;
              var moveDirection = (pos - _standRopeJoint.transform.position).normalized;
              _weightObj.transform.position = pos + moveDirection * (value - _oldRopeLength);
            });

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
          Elongation = transform.rotation.x;
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