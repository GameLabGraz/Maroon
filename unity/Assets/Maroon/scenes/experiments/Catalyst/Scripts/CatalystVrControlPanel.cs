using System.Collections;
using Antares.Evaluation.LearningContent;
using UnityEngine;

public class CatalystVrControlPanel : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 0.2f;
    [SerializeField] Transform playerTransform;

    private bool _isSetupCompleted = false;
    private bool _isMoving = false;
    private Vector3 _center;
    private float _radius = 1f;
    private float _angle;
    private Transform _playerTransform;
    private Vector3 _currentPlayerPosition;
    private Vector3 _previousPlayerPosition;

    public void Setup(float radius)
    {
        _center = transform.position;
        _radius = radius;
        _playerTransform = playerTransform;
        _currentPlayerPosition = playerTransform.position;
        _previousPlayerPosition = playerTransform.position;
        _isSetupCompleted = true;
    }

    private void Update()
    {
        if (!_isSetupCompleted) return;

        if (_isMoving) return;
        
        // check if positions changed
        if (_playerTransform.hasChanged)
            _currentPlayerPosition = _playerTransform.position;

        if (Vector3.Distance(_currentPlayerPosition, _previousPlayerPosition) < 0.1f ) 
           return;

        _previousPlayerPosition = _currentPlayerPosition;

        Vector3 desiredPos = _center + (_currentPlayerPosition - _center).normalized * _radius;
        desiredPos.y = transform.position.y;
        transform.position = desiredPos;
        transform.LookAt(_playerTransform);
        Quaternion rot = transform.rotation;
        transform.rotation = new Quaternion(0.0f, rot.y, 0.0f, rot.w);
        _isMoving = false;
        
        //_isMoving = true;
        //StartCoroutine(MoveObjectTowardsPlayer());
    }

    // not fully working
    /*private IEnumerator MoveObjectTowardsPlayer()
    {
        /*float angle = Vector3.Angle(transform.forward, _currentPlayerPosition);
        float previousDistance = 100f;
        while (_isMoving)
        {
            transform.LookAt(_playerTransform);
            if (angle < 90)
                _angle += rotationSpeed * Time.deltaTime;
            else
                _angle -= rotationSpeed * Time.deltaTime;

            var offset = new Vector3(Mathf.Sin(_angle), 0, Mathf.Cos(_angle)) * _radius;
            transform.position = _center + offset;
            
            float currentDistance = Vector3.Distance(transform.position, _currentPlayerPosition);
            Debug.Log($"currentDistance: {currentDistance}, previousDistance {previousDistance}, difference {currentDistance - previousDistance}");
            if (Mathf.Abs(currentDistance - previousDistance) < 0.03f)
                _isMoving = false;
            else
                previousDistance = currentDistance;
            yield return null;
        }
    }*/
}
