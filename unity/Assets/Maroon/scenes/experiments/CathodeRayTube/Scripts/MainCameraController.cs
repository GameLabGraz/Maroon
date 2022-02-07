using Maroon.Physics.CathodeRayTube;
using UnityEngine;
using UnityEngine.EventSystems;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MainCameraController : MonoBehaviour, IResetObject
{
    [SerializeField] private CRTController crtController;
    [SerializeField] private GameObject furniture;

    private const float MovementSpeed = 3f;
    private const float RotationSpeed = 150f;
    private const float ZoomSpeed = 1f;

    private Vector3 _minPosition;
    private Vector3 _maxPosition;

    private Vector3 _mouseOrigin;
    private Vector3 _origPos;
    private Quaternion _origRot;

    private Vector3 _target;
    private bool _fixCameraAtTarget = true;

    private void Awake()
    {
        Vector3 crtStart = crtController.GetCRTStart();
        _target = new Vector3(0, crtStart.y, crtStart.z);
        transform.LookAt(_target);

        _minPosition = crtController.GetCRTStart() + new Vector3(-0.5f, 0, -1);
        _maxPosition = crtController.GetCRTStart() + new Vector3(crtController.GetCRTDist() + 0.5f, 0.5f, 0);

        _origPos = transform.position;
        _origRot = transform.rotation;

        furniture.SetActive(true);
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
            var move = new Vector3(pos.x * MovementSpeed, pos.y * MovementSpeed, 0);
            if ((_maxPosition.x > pos.x || _maxPosition.y > pos.y) &&
                (_minPosition.x < pos.x || _minPosition.y < pos.y))
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
            transform.RotateAround(_target, transform.right, -pos.y * RotationSpeed);
            transform.RotateAround(_target, Vector3.up, pos.x * RotationSpeed);
        }
        else
        {
            _fixCameraAtTarget = true;
        }

        // Camera Zoom
        var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        var zoom = new Vector3(0, 0, mouseScroll * ZoomSpeed);
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

    public void TwoDimensionView()
    {
        Vector3 sideView = _target + new Vector3(0, -2, -0.6f);
        transform.position = ClampCamPosition(sideView);
        transform.rotation = Quaternion.identity;
    }

    public void ResetObject()
    {
        transform.position = _origPos;
        transform.rotation = _origRot;
    }
}