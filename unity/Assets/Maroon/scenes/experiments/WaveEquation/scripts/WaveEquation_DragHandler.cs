using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PlatformControls.BaseControls;
using UnityEngine.Assertions;
using UnityStandardAssets.Water;

public class WaveEquation_DragHandler : MonoBehaviour
{
    [Tooltip("The object that will move. If empty, the object where this script is attached will be moved.")]
    public GameObject movingObject;

    public WaveEquationWaterPlane waterPlane;

    [Header("Movement Restrictions")]
    public Transform minBoundary;
    public Transform maxBoundary;

    public bool allowedXMovement = true;
    public bool allowedYMovement = true;
    public bool allowedZMovement = true;
    
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


    private bool _moving;
    private bool _isOutsideBoundaries;
    private float _distance;


    // Start is called before the first frame update
    void Start()
    {
        if (movingObject == null) movingObject = gameObject;

        waterPlane = FindObjectOfType<WaveEquationWaterPlane>();
        Assert.IsNotNull(waterPlane);
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
        _distance = Vector3.Distance(movingObject.transform.position, Camera.main.transform.position);
        onStartedMoving.Invoke();
    }
    
    private void OnMouseDrag()
    {
        if (!_moving) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var pt = ray.GetPoint(_distance);
        var pos = movingObject.transform.position;

        if (!allowedXMovement) pt.x = pos.x;
        if (!allowedYMovement) pt.y = pos.y;
        if (!allowedZMovement) pt.z = pos.z;

        var outside = false;
        if (minBoundary != null && maxBoundary != null)
        {
            if (allowedXMovement && (pt.x < minBoundary.position.x || maxBoundary.position.x < pt.x))
                outside = true;
            else if (allowedYMovement && (pt.y < Mathf.Min(minBoundary.position.y, maxBoundary.position.y) 
                                          || Mathf.Max(minBoundary.position.y, maxBoundary.position.y) < pt.y))
                outside = true;
            else if (allowedZMovement && (pt.z < minBoundary.position.z || maxBoundary.position.z < pt.z))
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
        waterPlane.UpdateParameterAndPosition();
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
}
