using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CoordCameraController : MonoBehaviour, IResetObject
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 0.5f;
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private float zoomSpeed = 5f;

    [SerializeField] Transform minPosition;
    [SerializeField] private Transform maxPosition;

    private Camera _camera;

    private Vector3 _mouseOrigin;
    private Vector3 _origPos;
    private Quaternion _origRot;

    [SerializeField] private Transform target;
    [SerializeField] private bool fixCameraAtTarget = true;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        UpdateOrigPosition();
        UpdateOrigRotation();
    }
    private void LateUpdate()
    {
        //transform.LookAt(target);

        // Camera Rotation
        if (Input.GetButton("Fire2"))
        {
            /*
            var pos = _camera.ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);
            transform.RotateAround(transform.position, transform.right, -pos.y * rotationSpeed);
            transform.RotateAround(transform.position, Vector3.up, pos.x * rotationSpeed);
            */

            var pos = _camera.ScreenToViewportPoint(_mouseOrigin - Input.mousePosition);
            var move = new Vector3(pos.x * movementSpeed, pos.y * movementSpeed, 0);
            if ((maxPosition.position.x > pos.x || maxPosition.position.y > pos.y) && (minPosition.position.x < pos.x || minPosition.position.y < pos.y))
            {
                transform.Translate(move);
                transform.position = ClampCamPosition(transform.position);
            }
            
        }

        // Camera Rotation
        if (Input.GetButton("Fire1"))
        {
            if (fixCameraAtTarget)
            {
                transform.LookAt(target);
                fixCameraAtTarget = false;
            }
                

            var pos = _camera.ScreenToViewportPoint(Input.mousePosition - _mouseOrigin);
            transform.RotateAround(target.position, transform.right, -pos.y * rotationSpeed);
            transform.RotateAround(target.position, Vector3.up, pos.x * rotationSpeed);
        }
        else
        {
            fixCameraAtTarget = true;
        }

        // Camera Zoom
        var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        var zoom = new Vector3(0, 0, mouseScroll * zoomSpeed);
        transform.Translate(zoom);
        transform.position = ClampCamPosition(transform.position);

        _mouseOrigin = Input.mousePosition;
    }

    private Vector3 ClampCamPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, minPosition.position.x, maxPosition.position.x);
        position.y = Mathf.Clamp(position.y, minPosition.position.y, maxPosition.position.y);
        position.z = Mathf.Clamp(position.z, minPosition.position.z, maxPosition.position.z);

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