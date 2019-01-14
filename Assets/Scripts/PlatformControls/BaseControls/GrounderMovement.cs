using UnityEngine;

namespace PlatformControls.BaseControls
{
    public abstract class GrounderMovement : MonoBehaviour
    {
        [SerializeField]
        protected float MaxMovementLeft = 0;

        [SerializeField]
        protected float MaxMovementRight = 0;

        [SerializeField]
        protected float MovementSpeed = 1;

        private Vector3 _initialPosition;
        private float _lastDistance;

        protected virtual void Start()
        {
            _initialPosition = transform.position;
        }

        public void Move(Vector3 direction, float maxMovement)
        {
            var translateVector = direction * Time.deltaTime * MovementSpeed;
            var newPosition = transform.position + transform.TransformDirection(translateVector);
		
            var distance = Vector3.Distance(_initialPosition, newPosition);
            if(distance < maxMovement || distance < _lastDistance)
            {
                transform.Translate(translateVector);
                _lastDistance = distance;
            }
        }
    }
}
