using UnityEngine;

namespace Maroon.Chemistry.Catalyst.VR
{
    public class CatalystVrControlPanel : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform centerTransform;
        [SerializeField] private GameObject stepWiseButtonGameObject;
        [SerializeField] private GameObject GloveSpawnObject;

        private bool _isSetupCompleted;
        private bool _isMoving;
        private Vector3 _center;
        private float _radius = 1f;
        private float _angle;
        private Transform _playerTransform;
        private Vector3 _currentPlayerPosition;
        private Vector3 _previousPlayerPosition;
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        public void Setup(float radius, bool enableStepWiseButton)
        {
            var pos = transform.position;
            var rot = transform.rotation;
            _initialPosition = new Vector3(pos.x, pos.y, pos.z);
            _initialRotation = new Quaternion(rot.x, rot.y, rot.z, rot.w);

            _center = centerTransform.position;
            _radius = radius;
            _playerTransform = playerTransform;
            _currentPlayerPosition = playerTransform.position;
            _previousPlayerPosition = playerTransform.position;
            stepWiseButtonGameObject.SetActive(enableStepWiseButton);
            GloveSpawnObject.SetActive(true);
            _isSetupCompleted = true;
        }

        public void ResetToInitialPosition()
        {
            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
        }

        public void UpdatePositionToPlayer()
        {
            if (!_isSetupCompleted) return;

            if (_isMoving) return;

            // check if positions changed
            if (_playerTransform.hasChanged)
                _currentPlayerPosition = _playerTransform.position;

            if (Vector3.Distance(_currentPlayerPosition, _previousPlayerPosition) < 0.1f)
                return;

            _previousPlayerPosition = _currentPlayerPosition;

            var desiredPos = _center + (_currentPlayerPosition - _center).normalized * _radius;
            desiredPos.y = transform.position.y;
            transform.position = desiredPos;
            var rot = Quaternion.LookRotation(desiredPos - _center);
            transform.rotation = new Quaternion(0.0f, rot.y, 0.0f, rot.w);
            _isMoving = false;
        }
    }
}
