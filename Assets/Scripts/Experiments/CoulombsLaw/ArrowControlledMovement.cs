using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

public class ArrowControlledMovement : MonoBehaviour
{
    [Header("General Input Objects")]
    public GameObject ArrowXPositive;
    public GameObject ArrowXNegative;
    public GameObject ArrowYPositive;
    public GameObject ArrowYNegative;
    public GameObject ArrowZPositive;
    public GameObject ArrowZNegative;

    public GameObject MovingObject;

    [Header("General Settings")] public bool InUse = true;
    public bool ShowMovingLines = false;
    public bool HideWhileInRunMode = true;
    public bool RestrictXMovement = false;
    public bool RestrictYMovement = false;
    public bool RestrictZMovement = false;

    public Transform MinimumBoundary;
    public Transform MaximumBoundary;

    private Vector3 _localMinBoundary;
    private Vector3 _localMaxBoundary;
    
    private Vector3 _movingDirection =  Vector3.zero;
    private bool _moving = false;
    private float _distance;
    
    private LineRenderer _lineRenderer;
    private SimulationController _simController;
    private bool _lastUpdateInRunMode = false;

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
        
        _localMinBoundary = MovingObject.transform.parent.transform.InverseTransformPoint(MinimumBoundary.position);
        _localMaxBoundary = MovingObject.transform.parent.transform.InverseTransformPoint(MaximumBoundary.position);
        
        ArrowXPositive.SetActive(!RestrictXMovement); 
        ArrowXNegative.SetActive(!RestrictXMovement);
        ArrowYPositive.SetActive(!RestrictYMovement);
        ArrowYNegative.SetActive(!RestrictYMovement);
        ArrowZPositive.SetActive(!RestrictZMovement);
        ArrowZNegative.SetActive(!RestrictZMovement);
    }

    private void Update()
    {
        if (_simController.SimulationRunning != _lastUpdateInRunMode)
        {
            _lastUpdateInRunMode = _simController.SimulationRunning;
            ChangeRunMode();
        }
    }

    private void ChangeRunMode()
    {
        if (!_simController.SimulationRunning)
        {
            ArrowXPositive.GetComponent<Collider>().enabled =  
                ArrowXNegative.GetComponent<Collider>().enabled = 
                    ArrowYPositive.GetComponent<Collider>().enabled = 
                        ArrowYNegative.GetComponent<Collider>().enabled = 
                            ArrowZPositive.GetComponent<Collider>().enabled = 
                                ArrowZNegative.GetComponent<Collider>().enabled = true;
            ArrowXPositive.SetActive(!RestrictXMovement); 
            ArrowXNegative.SetActive(!RestrictXMovement);
            ArrowYPositive.SetActive(!RestrictYMovement);
            ArrowYNegative.SetActive(!RestrictYMovement);
            ArrowZPositive.SetActive(!RestrictZMovement);
            ArrowZNegative.SetActive(!RestrictZMovement);
        }
        else
        {
            if (HideWhileInRunMode)
            {
                ArrowXPositive.SetActive(false); 
                ArrowXNegative.SetActive(false);
                ArrowYPositive.SetActive(false);
                ArrowYNegative.SetActive(false);
                ArrowZPositive.SetActive(false);
                ArrowZNegative.SetActive(false);
            }
            else
            {
                ArrowXPositive.GetComponent<Collider>().enabled =  
                ArrowXNegative.GetComponent<Collider>().enabled = 
                ArrowYPositive.GetComponent<Collider>().enabled = 
                ArrowYNegative.GetComponent<Collider>().enabled = 
                ArrowZPositive.GetComponent<Collider>().enabled = 
                ArrowZNegative.GetComponent<Collider>().enabled = false;
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
        if(!InUse)
            return false;
        var mousePos = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hitInfo;
        if (!Physics.Raycast(ray, out hitInfo) || !Input.GetMouseButtonDown(0)) return false;
        
        var hitCollider = hitInfo.collider;
        Debug.Log("Hit: " + hitCollider.name);
        var lineStart = MovingObject.transform.localPosition;
        var lineEnd = MovingObject.transform.localPosition;
        Color color; 
        if ((hitCollider == ArrowXPositive.GetComponent<Collider>() || hitCollider == ArrowXNegative.GetComponent<Collider>()) && !RestrictXMovement)
        {
            _movingDirection = Vector3.right;
            lineStart.x = _localMinBoundary.x;
            lineEnd.x = _localMaxBoundary.x;
            color = ArrowXPositive.GetComponent<Renderer>().material.color;
        }
        else if ((hitCollider == ArrowYPositive.GetComponent<Collider>() || hitCollider == ArrowYNegative.GetComponent<Collider>()) && !RestrictYMovement)
        {
            _movingDirection = Vector3.up;
            lineStart.y = _localMinBoundary.y;
            lineEnd.y = _localMaxBoundary.y; 
            color = ArrowYPositive.GetComponent<Renderer>().material.color;
        }
        else if ((hitCollider == ArrowZPositive.GetComponent<Collider>() || hitCollider == ArrowZNegative.GetComponent<Collider>()) && !RestrictZMovement)
        {
            _movingDirection = Vector3.forward;
            lineStart.z = _localMinBoundary.z;
            lineEnd.z = _localMaxBoundary.z;
            color = ArrowZPositive.GetComponent<Renderer>().material.color;
        }
        else return false;

        if (ShowMovingLines)
        {
            color.a = 0.5f;
//            _lineRenderer.startColor = _lineRenderer.endColor = color;
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPositions(new Vector3[]
            {
                MovingObject.transform.parent.transform.TransformPoint(lineStart), 
                MovingObject.transform.parent.transform.TransformPoint(lineEnd)
            });
            _lineRenderer.enabled = true;
        }

        
        _moving = true;
        _distance = Vector3.Distance(MovingObject.transform.position, Camera.main.transform.position);
        
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
        var pt = MovingObject.transform.parent.transform.InverseTransformPoint(ray.GetPoint(_distance));
        var pos = MovingObject.transform.localPosition;
        
        if (Math.Abs(_movingDirection.x) < 0.001) pt.x = pos.x;
        if (Math.Abs(_movingDirection.y) < 0.001) pt.y = pos.y;
        if (Math.Abs(_movingDirection.z) < 0.001) pt.z = pos.z;
        
        if (pt.x < _localMinBoundary.x) pt.x = _localMinBoundary.x;
        else if (pt.x > _localMaxBoundary.x) pt.x = _localMaxBoundary.x;
        if (pt.y < _localMinBoundary.y) pt.y = _localMinBoundary.y;
        else if (pt.y > _localMaxBoundary.y) pt.y = _localMaxBoundary.y;
        if (pt.z < _localMinBoundary.z) pt.z = _localMinBoundary.z;
        else if (pt.z > _localMaxBoundary.z) pt.z = _localMaxBoundary.z;
        
        MovingObject.transform.localPosition = pt;
    }
    
    private void OnMouseUp()
    {
        MouseUp();
    }

    public void MouseUp()
    {
        _movingDirection = Vector3.zero;
        _lineRenderer.enabled = false;
    }
}
