//
//Author: Tobias Stöckl
//
using UnityEngine;
using Maroon.PlatformControls.PC;

[RequireComponent(typeof(Collider))]
//handles everything needed for dragging & dropping the lasers
public class DragLaserObject : MonoBehaviour
{
    private Camera _mainCamera;
    private Plane _movementPlane = new Plane(Vector3.up, new Vector3(0.0f, 0.0f, 0.0f));
    private Material _laserMat;
    private Color _ogColor;

    private LPProperties _laserProperties;

    [SerializeField]
    private Color _hoverColor = Color.red;
    [SerializeField]
    private Color _draggingColor = Color.green;
    [SerializeField]
    private Color _activeColor = Color.grey;


    private bool _currentlyDragging;

    private MeshRenderer _handleRenderer;
    private Collider _handleCollider;
    private LaserSelectionHandler _laserHandler;

    private Collider _thisColl;

    private Vector3 _offset = Vector3.zero;


    private void Start()
    {
        _mainCamera = Camera.main;
        _thisColl = GetComponent<Collider>();
        _laserMat = GetComponent<Renderer>().material;

        _ogColor = _laserMat.color;
        _currentlyDragging = false;

        _hoverColor = Color.red;
        _draggingColor = Color.green;
        _activeColor = Color.grey;

        _laserProperties = GetComponent<LPProperties>();
        _laserHandler = GameObject.FindGameObjectWithTag("LaserSelectionHandler").GetComponent<LaserSelectionHandler>();

        _handleRenderer = gameObject.transform.GetChild(0).GetComponent<MeshRenderer>();
        _handleCollider = gameObject.transform.GetChild(0).GetComponent<Collider>();
        _handleCollider.enabled = false;
        _handleRenderer.enabled = false;
    }


    private void OnMouseDown()
    {
        _currentlyDragging = true;
        Ray objIntersectRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
        _thisColl.Raycast(objIntersectRay, out var thisHit, Mathf.Infinity);

        _movementPlane.SetNormalAndPosition(Vector3.up, thisHit.point);

        _offset = thisHit.point - transform.position;
        MakeActive();
    }

    private void OnMouseEnter()
    {
        if(!_currentlyDragging) _laserMat.color = _hoverColor;
    }

    private void OnMouseExit()
    {
        if (_currentlyDragging) return;
        
        if (IsActiveLP())
        {
            MakeActive();
        }
        else
        {
            _laserMat.color = _ogColor;
        }
        
    }

    private void OnMouseUp()
    {
        _currentlyDragging = false;
        _laserMat.color = _hoverColor;
    }

    private void OnMouseDrag()
    {
        Ray distRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
        _movementPlane.Raycast(distRay, out var dist);

        Vector3 pointOnPlane = distRay.GetPoint(dist);
        transform.position = pointOnPlane - _offset;
        _laserMat.color = _draggingColor;
    }


    private bool IsActiveLP()
    {
        GameObject activeLaser = _laserHandler.CurrActiveLaser;

        if (activeLaser == null)
        {
            return false;
        }
        return ReferenceEquals(gameObject, activeLaser);
    }

    public void MakeActive()
    {
        _laserHandler.SetActiveIntensityAndWavelength(_laserProperties.LaserIntensity, _laserProperties.LaserWavelength);
        _laserMat.color = _activeColor;
        _handleRenderer.enabled = true;
        _handleCollider.enabled = true;
    }
    
    public void MakeInactive()
    {
        _laserMat.color = _ogColor;
        _handleRenderer.enabled = false;
        _handleCollider.enabled = false;
    }

}
   
