using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PC_DragHandler : MonoBehaviour
{
    [Tooltip("The object that will move. If empty, the object where this script is attached will be moved.")]
    public GameObject movingObject;
    
    [Tooltip("If used in conjunction with arrowMovement, this reference should be set")]
    public PC_ArrowMovement arrowMovement = null; 

    [Header("Movement Restrictions")]
    public Transform minBoundary;
    public Transform maxBoundary;
    
    public bool allowedXMovement = true;
    public bool allowedYMovement = true;
    public bool allowedZMovement = true;
    public bool useLocalCoordinates = false;

    [Header("Movement Restrictions Appearances")]
    [Tooltip("Boundaries need to be set for this")]
    public List<GameObject> changeMaterialIfOutside;
    [Tooltip("The materials must support transparency for this.")]
    public float outsideTransparency = 0.7f;
    
    [Header("Events")]
    [Tooltip("Event that gets triggered when the Object starts to move.")]
    public UnityEvent onStartedMoving;
    public UnityEvent onMove;
    [Tooltip("Event that gets triggered when the Object is outside the boundaries when the movement finished. This only gets triggered if the boundaries are set.")]
    public UnityEvent onEndMovingOutsideBoundaries;
    [Tooltip("Event that gets triggered at the end of the movement if the object is within the boundaries (or none are specified).")]
    public UnityEvent onEndMovingInsideBoundaries;
    
    [Tooltip("Event that gets triggered when the object is enabled.")]
    public UnityEvent onEnabled;
    [Tooltip("Event that gets triggered when the object is disabled.")]
    public UnityEvent onDisabled;

    private bool _moving = false;
    private bool _isOutsideBoundaries = false;
    private Vector3 _objectPostionAtDragStart;
    private Vector3 _objectToMousePosOffsetAtDragStart;
    private bool _wasKinematicAtDragStart = false;

    // Start is called before the first frame update
    void Start()
    {
        if (movingObject == null) movingObject = gameObject;

        var rb = GetComponent<Rigidbody>();
    }
    
    public void SetBoundaries(GameObject min, GameObject max)
    {
        minBoundary = min.transform;
        maxBoundary = max.transform;
    }

    private static Vector3 getMousePointOnPlaneParallelToCamera(Vector3 pointOnPlane)
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

        if (arrowMovement != null)
        {
            arrowMovement.OnChildMouseDown();
            if (arrowMovement.IsDragActive()) return;
        }
        
        var rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            _wasKinematicAtDragStart = rb.isKinematic;
            rb.isKinematic = true;
        }

        _moving = true;
        _objectPostionAtDragStart = movingObject.transform.position;
        _objectToMousePosOffsetAtDragStart = _objectPostionAtDragStart - getMousePointOnPlaneParallelToCamera(_objectPostionAtDragStart);

        onStartedMoving.Invoke();
    }
    
    private void OnMouseDrag()
    {
        if (arrowMovement != null && arrowMovement.IsDragActive())
        {
            arrowMovement.OnChildMouseDrag();
            return;
        }

        // Note(MartinR): Before merge, check why the distance-check was here, as it causes stuttering durign drag-and-drop on my Machine
        // if (!_moving || Vector3.Distance(_lastMousePos, Input.mousePosition) < 2f) return;
        if (!_moving) return;

        // Calculate new Position based on Mouse-Pos
        var newPos = getMousePointOnPlaneParallelToCamera(_objectPostionAtDragStart) + _objectToMousePosOffsetAtDragStart;
        if (!allowedXMovement) newPos.x = _objectPostionAtDragStart.x;
        if (!allowedYMovement) newPos.y = _objectPostionAtDragStart.y;
        if (!allowedZMovement) newPos.z = _objectPostionAtDragStart.z;
        
        // Check if point is outside of boundaries
        var outside = false;
        if (minBoundary != null && maxBoundary != null)
        {
            var minPosition = useLocalCoordinates ? minBoundary.localPosition : minBoundary.position;
            var maxPosition = useLocalCoordinates ? maxBoundary.localPosition : maxBoundary.position;
            var checkPos = useLocalCoordinates? minBoundary.parent.InverseTransformPoint(newPos) : newPos;

            Debug.Assert(minBoundary.parent == maxBoundary.parent);
            Vector3 min = Vector3.Min(minPosition, maxPosition);
            Vector3 max = Vector3.Max(minPosition, maxPosition);
            // Note(MartinR): Tolerance was previously 0.2f, not sure where/why this was used
            const float tolerance = 0.0f;

            outside = 
                (allowedXMovement && checkPos.x + tolerance < min.x || checkPos.x - tolerance > max.x) ||
                (allowedYMovement && checkPos.y + tolerance < min.y || checkPos.y - tolerance > max.y) ||
                (allowedZMovement && checkPos.z + tolerance < min.z || checkPos.z - tolerance > max.z);
        }

        // Change material transparency if object was moved between inside/outside of boundaries
        if (outside != _isOutsideBoundaries)
        {
            _isOutsideBoundaries = outside;

            foreach(var obj in changeMaterialIfOutside)
            {
                if(!obj.activeSelf) continue;
                foreach (var mat in obj.GetComponent<MeshRenderer>().materials)
                {
                    var col = mat.color;
                    col.a = outside ? outsideTransparency : 1f;
                    mat.color = col;
                }
            }
        }
        
        // Set new position
        movingObject.transform.position = newPos;
        onMove.Invoke();
    }
    
    private void OnMouseUp()
    {
        if (arrowMovement != null && arrowMovement.IsDragActive())
        {
            arrowMovement.OnChildMouseUp();
            return;
        }
        
        var rb = GetComponent<Rigidbody>();
        if(rb != null && !_wasKinematicAtDragStart)
        {
            rb.isKinematic = false;
        }

        _moving = false;
        if (_isOutsideBoundaries) onEndMovingOutsideBoundaries.Invoke();
        else onEndMovingInsideBoundaries.Invoke();
    }

    public void RestrictMovement(bool allowX, bool allowY, bool allowZ)
    {
        allowedXMovement = allowX;
        allowedYMovement = allowY;
        allowedZMovement = allowZ;
    }


    private void OnDisable()
    {
        onDisabled.Invoke();
    }

    private void OnEnable()
    {
        onEnabled.Invoke();
    }

    public void SetUseLocalCoordinates(bool useLocal)
    {
        useLocalCoordinates = useLocal;
    }
}
