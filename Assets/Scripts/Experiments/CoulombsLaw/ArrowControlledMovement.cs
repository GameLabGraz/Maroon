using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

public class ArrowControlledMovement : MonoBehaviour, IResetWholeObject
{
    [Header("General Input Objects")]
    public GameObject arrowXPositive;
    public GameObject arrowXNegative;
    public GameObject arrowYPositive;
    public GameObject arrowYNegative;
    public GameObject arrowZPositive;
    public GameObject arrowZNegative;
    
    [Tooltip("The object that will be moved with the arrows. If null then the object with the script on it will be moved.")]
    public GameObject movingObject = null;

    [Header("General Settings")] 
    [Tooltip("Tells whether the arrows are currently used (hence visible) or not.")]
    public bool inUse = true;
    [Tooltip("When clicking on an arrow, one can see how far the object can be moved. This only worked when Minimum and Maximum Boundary are set.")]
    public bool showMovingLines = false;
    [Tooltip("Hides the arrows when the simulation is running.")]
    public bool hideWhileInRunMode = true;
    [Tooltip("Pauses the simulation when the object is moved. Note: This is only possible if the arrows aren't hidden in run-mode.")]
    public bool pauseWhileMoving = true;
    
    [Header("Restrictions")]
    public bool restrictXMovement = false;
    public bool restrictYMovement = false;
    public bool restrictZMovement = false;

    public Transform minimumBoundary = null;
    public Transform maximumBoundary = null;

    [Header("Reset Settings")] 
    public bool resetOnReset = false;
    public bool resetOnWholeReset = false;
    
    private Vector3 _localMinBoundary;
    private Vector3 _localMaxBoundary;
    
    private Vector3 _movingDirection =  Vector3.zero;
    private bool _moving = false;
    private float _distance;
    
    private LineRenderer _lineRenderer;
    private SimulationController _simController;
    private bool _lastUpdateInRunMode = false;

    private Vector3 _originalPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        var simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            _simController = simControllerObject.GetComponent<SimulationController>();

        _lastUpdateInRunMode = _simController.SimulationRunning;
        
        _lineRenderer = GetComponent<LineRenderer>();
        if(!_lineRenderer) _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.enabled = false;

        if (movingObject == null) movingObject = gameObject;
        
        if(minimumBoundary != null)
            _localMinBoundary = movingObject.transform.parent.transform.InverseTransformPoint(minimumBoundary.position);
        if(maximumBoundary != null)
            _localMaxBoundary = movingObject.transform.parent.transform.InverseTransformPoint(maximumBoundary.position);
        
        arrowXPositive.SetActive(!restrictXMovement); 
        arrowXNegative.SetActive(!restrictXMovement);
        arrowYPositive.SetActive(!restrictYMovement);
        arrowYNegative.SetActive(!restrictYMovement);
        arrowZPositive.SetActive(!restrictZMovement);
        arrowZNegative.SetActive(!restrictZMovement);
        

        _originalPosition = movingObject.transform.position;
    }

    private void Update()
    {
        if (_simController.SimulationRunning == _lastUpdateInRunMode) return;
        _lastUpdateInRunMode = _simController.SimulationRunning;
        ChangeRunMode();
    }

    private void ChangeRunMode()
    {
        if (!_simController.SimulationRunning)
        {
            arrowXPositive.GetComponent<Collider>().enabled =  
                arrowXNegative.GetComponent<Collider>().enabled = 
                    arrowYPositive.GetComponent<Collider>().enabled = 
                        arrowYNegative.GetComponent<Collider>().enabled = 
                            arrowZPositive.GetComponent<Collider>().enabled = 
                                arrowZNegative.GetComponent<Collider>().enabled = true;
            arrowXPositive.SetActive(!restrictXMovement); 
            arrowXNegative.SetActive(!restrictXMovement);
            arrowYPositive.SetActive(!restrictYMovement);
            arrowYNegative.SetActive(!restrictYMovement);
            arrowZPositive.SetActive(!restrictZMovement);
            arrowZNegative.SetActive(!restrictZMovement);
        }
        else
        {
            if (hideWhileInRunMode)
            {
                arrowXPositive.SetActive(false); 
                arrowXNegative.SetActive(false);
                arrowYPositive.SetActive(false);
                arrowYNegative.SetActive(false);
                arrowZPositive.SetActive(false);
                arrowZNegative.SetActive(false);
            }
            else
            {
                arrowXPositive.GetComponent<Collider>().enabled =  
                arrowXNegative.GetComponent<Collider>().enabled = 
                arrowYPositive.GetComponent<Collider>().enabled = 
                arrowYNegative.GetComponent<Collider>().enabled = 
                arrowZPositive.GetComponent<Collider>().enabled = 
                arrowZNegative.GetComponent<Collider>().enabled = false;
            }
        }
    }
    
    private void OnMouseDown()
    {
        MouseDown();
    }
    
    public bool MouseDown()
    {
        Debug.Log("Mouse Down ArrowControlledMovement");
        if(!inUse)
            return false;
        var mousePos = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hitInfo;
        if (!Physics.Raycast(ray, out hitInfo) || !Input.GetMouseButtonDown(0)) return false;
        
        var hitCollider = hitInfo.collider;
//        Debug.Log("Hit: " + hitCollider.name);
        var lineStart = movingObject.transform.localPosition;
        var lineEnd = movingObject.transform.localPosition;
        Color color; 
        if ((hitCollider == arrowXPositive.GetComponent<Collider>() || hitCollider == arrowXNegative.GetComponent<Collider>()) && !restrictXMovement)
        {
            _movingDirection = Vector3.right;
            lineStart.x = _localMinBoundary.x;
            lineEnd.x = _localMaxBoundary.x;
            color = arrowXPositive.GetComponent<Renderer>().material.color;
        }
        else if ((hitCollider == arrowYPositive.GetComponent<Collider>() || hitCollider == arrowYNegative.GetComponent<Collider>()) && !restrictYMovement)
        {
            _movingDirection = Vector3.up;
            lineStart.y = _localMinBoundary.y;
            lineEnd.y = _localMaxBoundary.y; 
            color = arrowYPositive.GetComponent<Renderer>().material.color;
        }
        else if ((hitCollider == arrowZPositive.GetComponent<Collider>() || hitCollider == arrowZNegative.GetComponent<Collider>()) && !restrictZMovement)
        {
            _movingDirection = Vector3.forward;
            lineStart.z = _localMinBoundary.z;
            lineEnd.z = _localMaxBoundary.z;
            color = arrowZPositive.GetComponent<Renderer>().material.color;
        }
        else return false;

        if (showMovingLines && maximumBoundary != null && minimumBoundary != null)
        {
            color.a = 0.5f;
//            _lineRenderer.startColor = _lineRenderer.endColor = color;
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPositions(new Vector3[]
            {
                movingObject.transform.parent.transform.TransformPoint(lineStart), 
                movingObject.transform.parent.transform.TransformPoint(lineEnd)
            });
            _lineRenderer.enabled = true;
        }

        
        _moving = true;
        _distance = Vector3.Distance(movingObject.transform.position, Camera.main.transform.position);
        
        return true;
    }

    private void OnMouseDrag()
    {
        MouseDrag();
    }

    public void MouseDrag()
    { 
        if (!_moving) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var pt = movingObject.transform.parent.transform.InverseTransformPoint(ray.GetPoint(_distance));
        var pos = movingObject.transform.localPosition;
        
        if (Math.Abs(_movingDirection.x) < 0.001) pt.x = pos.x;
        if (Math.Abs(_movingDirection.y) < 0.001) pt.y = pos.y;
        if (Math.Abs(_movingDirection.z) < 0.001) pt.z = pos.z;

        if (minimumBoundary != null && maximumBoundary != null)
        {
            if (pt.x < _localMinBoundary.x) pt.x = _localMinBoundary.x;
            else if (pt.x > _localMaxBoundary.x) pt.x = _localMaxBoundary.x;
            if (pt.y < _localMinBoundary.y) pt.y = _localMinBoundary.y;
            else if (pt.y > _localMaxBoundary.y) pt.y = _localMaxBoundary.y;
            if (pt.z < _localMinBoundary.z) pt.z = _localMinBoundary.z;
            else if (pt.z > _localMaxBoundary.z) pt.z = _localMaxBoundary.z;
        }

        movingObject.transform.localPosition = pt;
    }
    
    private void OnMouseUp()
    {
        MouseUp();
    }

    public void MouseUp()
    {
        _movingDirection = Vector3.zero;
        if(_lineRenderer)
            _lineRenderer.enabled = false;
    }

    public void ResetObject()
    {
        if (resetOnReset) movingObject.transform.position = _originalPosition;
    }

    public void ResetWholeObject()
    {
        if (resetOnWholeReset) movingObject.transform.position = _originalPosition;
    }
}
