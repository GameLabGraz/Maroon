using System;
using GameLabGraz.VRInteraction;
using PlatformControls.BaseControls;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace PlatformControls.VR
{
    public class VR_GrounderMovement : MonoBehaviour
    {
        [SerializeField]
        protected float MaxMovementLeft = 0;

        [SerializeField]
        protected float MaxMovementRight = 0;

        [SerializeField] 
        protected bool invertInputAngle = false;
        
        [SerializeField]
        protected float MovementSpeedPerAngle = 0.05f; //moves 0.05m/s per one degree

        [SerializeField] 
        protected VRCircularDrive circularDrive;

        public UnityEvent onLeftEndReached;
        public UnityEvent onRightEndReached;
        public UnityEvent onBelowEndAgain;
        public UnityEvent onMove;
        
        private Vector3 _initialPosition;
        private float _currentAngle;
        private float _lastDistance;

        private bool _endReached = false;

        protected void Start()
        {
            _currentAngle = circularDrive.startAngle;
            _initialPosition = transform.position;
        }
        
        public void OnDegreeChanged()
        {
            Debug.Log("OnDegreeChanged" + circularDrive.outAngle);
            var newAngle = circularDrive.outAngle;
            var movementAngles = Mathf.Abs(_currentAngle - newAngle);
            
            if (newAngle < _currentAngle)
            {
                //move left
                Debug.Log("Move left");
                MoveLeft(invertInputAngle ? Vector3.right : Vector3.left, movementAngles);
            }
            else if (newAngle > _currentAngle)
            {
                //move right
                Debug.Log("Move right");
                MoveRight(invertInputAngle ? Vector3.left : Vector3.right, movementAngles);
            }

            _currentAngle = newAngle;
        }

        public void ResetCurrentAngle()
        {
            _currentAngle = circularDrive.outAngle;
        }
        
        protected void MoveLeft(Vector3 direction, float angles)
        {
            Move(direction, angles, MaxMovementLeft, onLeftEndReached);
        }
        
        protected void MoveRight(Vector3 direction, float angles)
        {
            Move(direction, angles, MaxMovementRight, onRightEndReached);
        }

        private void Move(Vector3 direction, float angles, float maxMovement, UnityEvent endReachedEvent)
        {
            var translateVector = direction * Time.deltaTime * MovementSpeedPerAngle * angles;
            var newPosition = transform.position + transform.TransformDirection(translateVector);

            var distance = Vector3.Distance(_initialPosition, newPosition);
            Debug.Log("Distance: " + distance);
            if (distance >= maxMovement && !_endReached)
            {
                endReachedEvent?.Invoke();
                _endReached = true;

                newPosition = _initialPosition + direction * maxMovement;
                distance = Vector3.Distance(_initialPosition, newPosition);
                Debug.Assert(distance <= maxMovement);
            }

            if (distance <= maxMovement || distance < _lastDistance)
            {
                transform.Translate(translateVector);
                _lastDistance = distance;
                
                onMove?.Invoke();
            }
            
            if (_endReached && (distance < maxMovement || distance < _lastDistance))
            {
                onBelowEndAgain.Invoke();
                _endReached = false;
            }
        }
    }
}
