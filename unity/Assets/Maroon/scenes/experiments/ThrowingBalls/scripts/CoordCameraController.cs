using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CoordCameraController : MonoBehaviour, IResetObject
{
    [Header("Movement Settings")]
    private float _movementSpeed = 10f;
    private float _rotationSpeed = 300f;
    [SerializeField] private float _zoomSpeed = 5f;

    [SerializeField] private Transform _minPosition;
    [SerializeField] private Transform _maxPosition;

    private Camera _camera;

    private Vector3 _mouseOrigin;
    private Vector3 _origPos;
    private Quaternion _origRot;

    [SerializeField] private Transform _target;
    [SerializeField] private bool _fixCameraAtTarget = true;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        transform.LookAt(_target);

        UpdateOrigPosition();
        UpdateOrigRotation();
    }

    /// <summary>
    /// Adapted functionality of the original CameraController-Script
    /// </summary>
    private void LateUpdate()
    {
        // Camera Movement
        if (Input.GetButton("Fire2"))
        {
            var pos = _camera.ScreenToViewportPoint(_mouseOrigin - Input.mousePosition);
            var move = new Vector3(pos.x * _movementSpeed, pos.y * _movementSpeed, 0);
            if ((_maxPosition.position.x > pos.x || _maxPosition.position.y > pos.y) && (_minPosition.position.x < pos.x || _minPosition.position.y < pos.y))
            {
                transform.Translate(move);
                transform.position = ClampCamPosition(transform.position);
            }
        }

        // Camera Rotation
        if (Input.GetButton("Fire1"))
        {
            if (_fixCameraAtTarget)
            {
                transform.LookAt(_target);
                _fixCameraAtTarget = false;
            }
                

            var pos = _camera.ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);
            transform.RotateAround(_target.position, transform.right, -pos.y * _rotationSpeed);
            transform.RotateAround(_target.position, Vector3.up, pos.x * _rotationSpeed);
        }
        else
        {
            _fixCameraAtTarget = true;
        }

        // Camera Zoom
        var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        var zoom = new Vector3(0, 0, mouseScroll * _zoomSpeed);
        transform.Translate(zoom);
        transform.position = ClampCamPosition(transform.position);

        _mouseOrigin = Input.mousePosition;
    }

    private Vector3 ClampCamPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, _minPosition.position.x, _maxPosition.position.x);
        position.y = Mathf.Clamp(position.y, _minPosition.position.y, _maxPosition.position.y);
        position.z = Mathf.Clamp(position.z, _minPosition.position.z, _maxPosition.position.z);

        return position;
    }

    public void UpdateOrigPosition()
    {
        _origPos = transform.position;

    }

    public void UpdateOrigRotation()
    {
        _origRot = transform.rotation;
    }

    public void ResetObject()
    {
        transform.position = _origPos;
        transform.rotation = _origRot;
    }
}