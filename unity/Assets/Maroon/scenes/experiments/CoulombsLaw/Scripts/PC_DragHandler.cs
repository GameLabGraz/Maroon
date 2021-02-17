using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PC_DragHandler : MonoBehaviour
{
    [Tooltip("The object that will move. If empty, the object where this script is attached will be moved.")]
    public GameObject movingObject;
    
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

    [Header("Additional Object References")]
    public PC_ArrowMovement ArrowMovement = null;
    
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
    private Vector3 _lastMousePos = Vector3.zero;
    private bool _isOutsideBoundaries = false;
    private float _distance;

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

    private void OnMouseDown()
    {
        if(!movingObject.activeSelf) return;
        if (!Input.GetMouseButtonDown(0)) return;
        
        _moving = true;

        var main = Camera.main;
        Debug.Assert(main != null);
        _lastMousePos = Input.mousePosition;
        _distance = Vector3.Distance( movingObject.transform.position, main.transform.position);
        
        onStartedMoving.Invoke();
    }
    
    private void OnMouseDrag()
    {
        if (!_moving || Vector3.Distance(_lastMousePos, Input.mousePosition) < 2f) return;

        _lastMousePos = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(_lastMousePos);
        var pt = ray.GetPoint(_distance);
        var pos = movingObject.transform.position;

        if (!allowedXMovement) pt.x = pos.x;
        if (!allowedYMovement) pt.y = pos.y;
        if (!allowedZMovement) pt.z = pos.z;

        
        var outside = false;
        if (minBoundary != null && maxBoundary != null)
        {
            var minPosition = useLocalCoordinates ? minBoundary.localPosition : minBoundary.position;
            var maxPosition = useLocalCoordinates ? maxBoundary.localPosition : maxBoundary.position;
            var checkPt = useLocalCoordinates? minBoundary.parent.InverseTransformPoint(pt) : pt;

            Debug.Assert(minBoundary.parent == maxBoundary.parent);
            if (allowedXMovement && (checkPt.x + 0.2f < Mathf.Min(minPosition.x, maxPosition.x)
                                     || Mathf.Max(minPosition.x, maxPosition.x) < checkPt.x - 0.2f))
                outside = true;
            else if (allowedYMovement && (checkPt.y + 0.2f < Mathf.Min(minPosition.y, maxPosition.y) 
                                          || Mathf.Max(minPosition.y, maxPosition.y) < checkPt.y - 0.2f))
                outside = true;
            else if (allowedZMovement && (checkPt.z + 0.2f < Mathf.Min(minPosition.z, maxPosition.z) 
                                          || Mathf.Max(minPosition.z, maxPosition.z)  < checkPt.z - 0.2f))
                outside = true;
        }

        // ReSharper disable once RedundantCheckBeforeAssignment
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
        
        movingObject.transform.position = pt;
        onMove.Invoke();
    }
    
    private void OnMouseUp()
    {
        if (!Input.GetMouseButtonUp(0)) return;
        
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
