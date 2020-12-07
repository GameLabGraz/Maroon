using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector3 = UnityEngine.Vector3;

public class UISourceDragHandlerSimple : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("General Settings")]
    public Canvas parentCanvas;
    public float returnTime = 1f;
    public GameObject generatedObject;
    [Tooltip("The object which is set as the new child, if empty the generated Object will be used as childObject.")]
    public GameObject childObject = null;
    public bool resetRotation = true;
    
    [Header("Other Affected GameObjects")]
    public GameObject changeLayerObject;
    
    private Vector3 _initialPosition;
    private Vector3 _initialMousePosition;

    private bool _slowlyReturnToOrigin = false;
    private Vector3 _returnDirection;
    private float _time;
    
    private void Start()
    {
        if (childObject == null)
            childObject = generatedObject;
    }

    private void Update()
    {
        if (_slowlyReturnToOrigin)
        {
            transform.position += Time.deltaTime * _returnDirection / returnTime;
            _time += Time.deltaTime;

            if (_time >= returnTime)
            {
                transform.position = _initialPosition;
                _slowlyReturnToOrigin = false;
            }
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("OnBegin Drag: " + changeLayerObject);
        _initialPosition = transform.position;
        if (changeLayerObject)
        {
            // Debug.Log("Change Layer: " + changeLayerObject);

            changeLayerObject.layer = 0; //Default Layer
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        var screenPoint = Input.mousePosition;
        var finish = parentCanvas.worldCamera.ScreenToWorldPoint(screenPoint);
        finish.z = 0f;
        transform.position = finish;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, 100f);

        var hitVectorField = false;
        foreach (var hit in hits)
        {
            // Debug.Log("Hit " + hit.transform.tag + " - " + hit.transform.gameObject.name);
            if(!hit.transform.CompareTag("WaterPlane"))
                continue;

           // Debug.Log("" + hit.point.x + ": " + hit.transform.parent.position.x);
            hitVectorField = true;
            ShowObject(hit.point, hit.transform.parent);
            transform.position = _initialPosition;
        }

        if (!hitVectorField)
        {
            _slowlyReturnToOrigin = true;
            _time = 0f;
            _returnDirection = _initialPosition - transform.position;
        }
        
        if (changeLayerObject)
        {
            changeLayerObject.layer = 2; //IgnoreRaycast
        }
    }

    protected virtual void ShowObject(Vector3 position, Transform parent)
    {
        if (generatedObject == null) return;

        childObject.transform.parent = parent;
        generatedObject.transform.position = position;
        if(resetRotation)
            generatedObject.transform.localRotation = Quaternion.identity;
        generatedObject.SetActive(false);
        generatedObject.SetActive(true);
    }
}
