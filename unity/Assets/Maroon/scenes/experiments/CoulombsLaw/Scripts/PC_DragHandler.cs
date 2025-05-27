using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PC_DragHandler : MonoBehaviour
{
    [Tooltip("The object that will move. If empty, the object where this script is attached will be moved.")]
    public GameObject movingObject;
    
    [Tooltip("If used in conjunction with arrowMovement + rigidbody, this reference should be set, otherwise collisions won't work properly")]
    [SerializeField] private PC_ArrowMovement _arrowMovement = null; 

    [Header("Physics")]
    [Tooltip("For dragging physics objects, the rigidBody reference should be set to avoid interpolation problems")]
    [SerializeField] private Rigidbody _rigidBody = null; 
    private bool _wasKinematicAtDragStart = false;
    [SerializeField] private bool _addVelocityAfterDragEnd = false;
    [SerializeField] private float _maxVelocity = 2;
    private Vector3 _previousDragPos;
    private Vector3 _dragVelocity;

    [Header("Movement Restrictions")]
    public Transform minBoundary;
    public Transform maxBoundary;
    public bool allowZMovement = true;
    
    [Header("Events")]
    [Tooltip("Event that gets triggered when the Object is outside the boundaries when the movement finished. This only gets triggered if the boundaries are set.")]
    public UnityEvent onEndMovingOutsideBoundaries;
    [Tooltip("Event that gets triggered at the end of the movement if the object is within the boundaries (or none are specified).")]
    public UnityEvent onEndMovingInsideBoundaries;

    private bool _dragIsActive = false;
    private bool _isOutsideBoundaries = false;
    private Vector3 _objectPostionAtDragStart;
    private Vector3 _objectToMousePosOffsetAtDragStart;

    // Start is called before the first frame update
    void Start()
    {
        if (movingObject == null) movingObject = gameObject;
    }
    
    public void SetBoundaries(GameObject min, GameObject max)
    {
        minBoundary = min.transform;
        maxBoundary = max.transform;
    }

    public static Vector3 GetMousePointOnPlaneParallelToCamera(Vector3 pointOnPlane)
    {
        Plane movementPlane = new Plane(Camera.main.transform.rotation * Vector3.back, pointOnPlane);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float planeIntersectionDistance = 0.0f;
        movementPlane.Raycast(ray, out planeIntersectionDistance);
        return ray.GetPoint(planeIntersectionDistance);
    }

    private void OnMouseDown()
    {
        if(!movingObject.activeSelf) return;

        if (_arrowMovement != null)
        {
            _arrowMovement.OnChildMouseDown();
            if (_arrowMovement.IsDragActive()) return;
        }
        
        if(_rigidBody != null)
        {
            _wasKinematicAtDragStart = _rigidBody.isKinematic;
            _rigidBody.isKinematic = true;
        }

        _dragIsActive = true;
        _objectPostionAtDragStart = movingObject.transform.position;
        _objectToMousePosOffsetAtDragStart = _objectPostionAtDragStart - GetMousePointOnPlaneParallelToCamera(_objectPostionAtDragStart);
        _previousDragPos = movingObject.transform.position;
        _dragVelocity = Vector3.zero;
    }
    
    private void OnMouseDrag()
    {
        if (_arrowMovement != null && _arrowMovement.IsDragActive())
        {
            _arrowMovement.OnChildMouseDrag();
            return;
        }

        // Note(MartinR): Before merge, check why the distance-check was here, as it causes stuttering durign drag-and-drop on my Machine
        // if (!_moving || Vector3.Distance(_lastMousePos, Input.mousePosition) < 2f) return;
        if (!_dragIsActive) return;

        // Calculate new Position based on Mouse-Pos
        var newPos = GetMousePointOnPlaneParallelToCamera(_objectPostionAtDragStart) + _objectToMousePosOffsetAtDragStart;
        if (!allowZMovement) newPos.z = _objectPostionAtDragStart.z;

        // Calculate drag velocity
        _dragVelocity = (newPos - _previousDragPos) / Time.deltaTime;
        _previousDragPos = newPos;
        
        // Check if point is outside of boundaries
        if (minBoundary != null && maxBoundary != null)
        {
            Vector3 min = Vector3.Min(minBoundary.position, maxBoundary.position);
            Vector3 max = Vector3.Max(minBoundary.position, maxBoundary.position);
            _isOutsideBoundaries = 
                (newPos.x < min.x || newPos.x > max.x) ||
                (newPos.y < min.y || newPos.y > max.y) ||
                (allowZMovement && (newPos.z < min.z || newPos.z > max.z));
        }
        
        // Set new position
        // Note(MartinR): Just setting transform.position causes problems when Physics interpolation is enabled.
        //      _rigidBody.MovePosition also does not seem to do the trick, I guess because it is expected to be called during FixedUpdate?
        if (_rigidBody != null)
        {
            _rigidBody.position = newPos;
        }
        else
        {
            movingObject.transform.position = newPos;
        }
    }
    
    private void OnMouseUp()
    {
        if (_arrowMovement != null && _arrowMovement.IsDragActive())
        {
            _arrowMovement.OnChildMouseUp();
            return;
        }
        
        if(_rigidBody != null && !_wasKinematicAtDragStart)
        {
            _rigidBody.isKinematic = false;

            if (_addVelocityAfterDragEnd)
            {
                var speed = _dragVelocity.magnitude;
                if (speed > _maxVelocity)
                {
                    _dragVelocity = _dragVelocity.normalized * _maxVelocity;
                }
                _rigidBody.velocity = _dragVelocity;
            }
        }

        _dragIsActive = false;
        if (_isOutsideBoundaries) onEndMovingOutsideBoundaries.Invoke();
        else onEndMovingInsideBoundaries.Invoke();
    }
}
