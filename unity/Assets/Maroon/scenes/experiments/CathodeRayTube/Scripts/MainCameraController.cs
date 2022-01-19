using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Maroon.Physics.CathodeRayTube;
using UnityEngine;
using UnityEngine.EventSystems;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MainCameraController : MonoBehaviour, IResetObject
{
    [SerializeField] private CRTController _crtController; 
    
    private float _movementSpeed = 3f;
    private float _rotationSpeed = 150f;
    private float _zoomSpeed = 1f;

    private Vector3 _minPosition;
    private Vector3 _maxPosition;

    private Vector3 _mouseOrigin;
    private Vector3 _origPos;
    private Quaternion _origRot;

    private Vector3 _target;
    private bool _fixCameraAtTarget = true;

    private void Awake()
    {
        Vector3 crtStart = _crtController.GetCRTStart();
        _target = new Vector3(0, crtStart.y, crtStart.z);
        transform.LookAt(_target);

        _minPosition = _crtController.GetCRTStart() + new Vector3(-0.5f, 0, -1);
        _maxPosition = _crtController.GetCRTStart() + new Vector3(_crtController.GetCRTDist() + 0.5f, 0.5f, 0);

        _origPos = transform.position;
        _origRot = transform.rotation;
    }

    /// <summary>
    /// Adapted functionality of the original CameraController-Script
    /// </summary>
    private void LateUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        // Camera Movement
        if (Input.GetButton("Fire2"))
        {
            var pos = GetComponent<Camera>().ScreenToViewportPoint(_mouseOrigin - Input.mousePosition);
            var move = new Vector3(pos.x * _movementSpeed, pos.y * _movementSpeed, 0);
            if ((_maxPosition.x > pos.x || _maxPosition.y > pos.y) && (_minPosition.x < pos.x || _minPosition.y < pos.y))
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
                

            var pos = GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);
            transform.RotateAround(_target, transform.right, -pos.y * _rotationSpeed);
            transform.RotateAround(_target, Vector3.up, pos.x * _rotationSpeed);
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
        position.x = Mathf.Clamp(position.x, _minPosition.x, _maxPosition.x);
        position.y = Mathf.Clamp(position.y, _minPosition.y, _maxPosition.y);
        position.z = Mathf.Clamp(position.z, _minPosition.z, _maxPosition.z);

        return position;
    }

    public void ResetObject()
    {
        transform.position = _origPos;
        transform.rotation = _origRot;
    }
}
