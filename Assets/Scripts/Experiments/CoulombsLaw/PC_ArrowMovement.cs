using System;
using UnityEngine;
using UnityEngine.Events;

public class PC_ArrowMovement : MonoBehaviour, IResetWholeObject
{
    [Header("General Input Objects")]
    [Tooltip("The object that will be moved with the arrows. If null then the object with the script on it will be moved.")]
    public GameObject movingObject = null;
    public bool useMovementOffset = true;

    [Header("Restrictions")]
    public bool restrictXMovement = false;
    public bool restrictYMovement = false;
    public bool restrictZMovement = false;

    public Transform minimumBoundary = null;
    public Transform maximumBoundary = null;
    
    [Header("Visualization Specific Properties")] 
    [Tooltip("When clicking on an arrow, one can see how far the object can be moved. This only worked when Minimum and Maximum Boundary are set.")]
    public bool showMovingLines = false;
    [Tooltip("Hides the arrows when the simulation is running.")]
    public bool hideWhileInRunMode = true;
    //TODO: here
//    [Tooltip("Pauses the simulation when the object is moved. Note: This is only possible if the arrows aren't hidden in run-mode.")]
//    public bool pauseWhileMoving = true;
    
    [Header("Reset Settings")] 
    public bool resetOnReset = false;
    public bool resetOnWholeReset = false;

    public UnityEvent OnMovementFinished;
    
    private Vector3 _localMinBoundary;
    private Vector3 _localMaxBoundary;
    
    private Vector3 _movingDirection =  Vector3.zero;
    private bool _moving = false;
    private Vector3 _movingOffset = Vector3.zero;
    private float _distance;
    
    private LineRenderer _lineRenderer;
    private SimulationController _simController;
    private bool _lastUpdateInRunMode = false;

    private Vector3 _originalPosition;
    
    private GameObject _arrowXPositive;
    private GameObject _arrowXNegative;
    private GameObject _arrowYPositive;
    private GameObject _arrowYNegative;
    private GameObject _arrowZPositive;
    private GameObject _arrowZNegative;
    
    // Start is called before the first frame update
    private void Start()
    {
        //Get the child object via fixed names (is a prefab, shouldn't need change)
        _arrowXPositive = transform.Find("x_right").gameObject;
        _arrowXNegative = transform.Find("x_left").gameObject;
        _arrowYPositive = transform.Find("y_up").gameObject;
        _arrowYNegative = transform.Find("y_down").gameObject;
        _arrowZPositive = transform.Find("z_back").gameObject;
        _arrowZNegative = transform.Find("z_forth").gameObject;
        
        //Get the simulation controller
        var simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            _simController = simControllerObject.GetComponent<SimulationController>();
        _lastUpdateInRunMode = _simController.SimulationRunning;
        
        //Check if we have a moving object or we should just move our own transform
        if (movingObject == null) movingObject = gameObject;

        if (maximumBoundary != null && minimumBoundary != null)
        {
            //Create a linerenderer if none exists and we need one
            if (showMovingLines)
            {
                _lineRenderer = GetComponent<LineRenderer>();
                if (!_lineRenderer) _lineRenderer = gameObject.AddComponent<LineRenderer>();
                _lineRenderer.enabled = false;
            }
            
            _localMinBoundary = movingObject.transform.parent.transform.InverseTransformPoint(minimumBoundary.position);
            _localMaxBoundary = movingObject.transform.parent.transform.InverseTransformPoint(maximumBoundary.position);
        }
        
        //Needed for resetting later
        _originalPosition = movingObject.transform.position;

        //Movement Restrictions -> hide arrows that are not allowed to move
        _arrowXPositive.SetActive(!restrictXMovement); 
        _arrowXNegative.SetActive(!restrictXMovement);
        _arrowYPositive.SetActive(!restrictYMovement);
        _arrowYNegative.SetActive(!restrictYMovement);
        _arrowZPositive.SetActive(!restrictZMovement);
        _arrowZNegative.SetActive(!restrictZMovement);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_simController.SimulationRunning == _lastUpdateInRunMode) return;
        _lastUpdateInRunMode = _simController.SimulationRunning;
        ChangeRunMode();
    }

    private void ChangeRunMode()
    {
        _arrowXPositive.SetActive((!_simController.SimulationRunning || !hideWhileInRunMode) && !restrictXMovement); 
        _arrowXNegative.SetActive((!_simController.SimulationRunning || !hideWhileInRunMode) && !restrictXMovement);
        _arrowYPositive.SetActive((!_simController.SimulationRunning || !hideWhileInRunMode) && !restrictYMovement);
        _arrowYNegative.SetActive((!_simController.SimulationRunning || !hideWhileInRunMode) && !restrictYMovement);
        _arrowZPositive.SetActive((!_simController.SimulationRunning || !hideWhileInRunMode) && !restrictZMovement);
        _arrowZNegative.SetActive((!_simController.SimulationRunning || !hideWhileInRunMode) && !restrictZMovement);
        
        _arrowXPositive.GetComponent<Collider>().enabled = _arrowXNegative.GetComponent<Collider>().enabled = 
            _arrowYPositive.GetComponent<Collider>().enabled = _arrowYNegative.GetComponent<Collider>().enabled = 
                _arrowZPositive.GetComponent<Collider>().enabled = _arrowZNegative.GetComponent<Collider>().enabled = 
                    !_simController.SimulationRunning || hideWhileInRunMode && _arrowXPositive.GetComponent<Collider>().enabled;
    }
    
    public void OnChildMouseDown(GameObject child)
    {
        if (_moving) return;
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (!Physics.Raycast(ray, out hitInfo) || !Input.GetMouseButtonDown(0)) return;
        
        var hitCollider = hitInfo.collider;
        if ((hitCollider == _arrowXPositive.GetComponent<Collider>() || hitCollider == _arrowXNegative.GetComponent<Collider>()) && !restrictXMovement)
            _movingDirection = Vector3.right;
        else if ((hitCollider == _arrowYPositive.GetComponent<Collider>() || hitCollider == _arrowYNegative.GetComponent<Collider>()) && !restrictYMovement)
            _movingDirection = Vector3.up;
        else if ((hitCollider == _arrowZPositive.GetComponent<Collider>() || hitCollider == _arrowZNegative.GetComponent<Collider>()) && !restrictZMovement)
            _movingDirection = Vector3.forward;
        else return;
        
        DrawMovingLines(_movingDirection);
        
        _moving = true;
        _distance = Vector3.Distance(movingObject.transform.position, Camera.main.transform.position);
        var pt = movingObject.transform.parent.transform.InverseTransformPoint(ray.GetPoint(_distance));
        if(useMovementOffset)
            _movingOffset = pt - movingObject.transform.localPosition;
        Debug.Log("pt = " + pt + " -- localPos = " + movingObject.transform.localPosition + " -  offset: " + _movingOffset);
    }

    private void DrawMovingLines(Vector3 drawingMask)
    {
        if (!showMovingLines || maximumBoundary == null || minimumBoundary == null || _lineRenderer == null) return;
        var lineStart = movingObject.transform.localPosition; 
        var lineEnd = movingObject.transform.localPosition;

        if (Math.Abs(drawingMask.x) > 0.0001)
        {
            lineStart.x = Mathf.Min(_localMinBoundary.x, _localMaxBoundary.x);
            lineEnd.x = Mathf.Max(_localMinBoundary.x, _localMaxBoundary.x); 
        }
        if (Math.Abs(drawingMask.y) > 0.0001)
        {
            lineStart.y = Mathf.Min(_localMinBoundary.y, _localMaxBoundary.y);
            lineEnd.y = Mathf.Max(_localMinBoundary.y, _localMaxBoundary.y); 
        }
        if (Math.Abs(drawingMask.z) > 0.0001)
        {
            lineStart.z = Mathf.Min(_localMinBoundary.z, _localMaxBoundary.z);
            lineEnd.z = Mathf.Max(_localMinBoundary.z, _localMaxBoundary.z); 
        }
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPositions(new Vector3[]
        {
            movingObject.transform.parent.transform.TransformPoint(lineStart), 
            movingObject.transform.parent.transform.TransformPoint(lineEnd)
        });
        _lineRenderer.enabled = true;
    }

    public void OnChildMouseDrag()
    {
        if (!_moving) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var pt = movingObject.transform.parent.transform.InverseTransformPoint(ray.GetPoint(_distance));
        var pos = movingObject.transform.localPosition;
        pt -= _movingOffset;

        if (Math.Abs(_movingDirection.x) < 0.001) pt.x = pos.x;
        if (Math.Abs(_movingDirection.y) < 0.001) pt.y = pos.y;
        if (Math.Abs(_movingDirection.z) < 0.001) pt.z = pos.z;

        if (minimumBoundary != null && maximumBoundary != null)
        {
            pt.x = Mathf.Clamp(pt.x, _localMinBoundary.x, _localMaxBoundary.x);
            pt.y = Mathf.Clamp(pt.y, _localMinBoundary.y, _localMaxBoundary.y);
            pt.z = Mathf.Clamp(pt.z, _localMinBoundary.z, _localMaxBoundary.z);
        }
        
        movingObject.transform.localPosition =  pt;
    }

    public void OnChildMouseUp()
    {
        if (!_moving) return;
        _moving = false;
        _movingDirection = Vector3.zero;
        if (_lineRenderer && _lineRenderer.enabled) _lineRenderer.enabled = false;
        
        OnMovementFinished.Invoke();
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
