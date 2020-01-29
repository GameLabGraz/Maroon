using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector3 = UnityEngine.Vector3;

public class UIItemDragHandlerSimple : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("General Settings")]
    public Canvas parentCanvas;
    public float returnTime = 1f;
    public GameObject generatedObject;
    [Tooltip("The object which is set as the new child, if empty the generated Object will be used as childObject.")]
    public GameObject childObject = null;
    
    [Header("Other Affected GameObjects")]
    public GameObject ChangeLayerObject;
    
    private Vector3 _initialPosition;
    private Vector3 _initialMousePosition;

    private bool _slowlyReturnToOrigin = false;
    private Vector3 _returnDirection;
    private float _time;
    
    private float _currentCharge = 0f;
    private bool _fixedPosition = false;

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
        _initialPosition = transform.position;
        if (ChangeLayerObject)
        {
            ChangeLayerObject.layer = 0; //Default Layer
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
            if(!hit.transform.CompareTag("VectorField"))
                continue;

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
        
        if (ChangeLayerObject)
        {
            ChangeLayerObject.layer = 2; //Ignore Raycast
        }
    }
    
    public virtual void SetObjectToOrigin()
    {
        var vecFields = GameObject.FindGameObjectsWithTag("VectorField"); //should be 2 -> one 2d and one 3d, where only one should be active at a time

        foreach (var vecField in vecFields)
        {
            if(!vecField.activeInHierarchy) continue;
            ShowObject(vecField.transform.position, vecField.transform.parent);
        }
    }

    protected virtual void ShowObject(Vector3 position, Transform parent)
    {
        if (generatedObject == null) return;
//        var obj = Instantiate(generatedObject, position, Quaternion.identity, parent);

        childObject.transform.parent = parent;
        generatedObject.transform.position = position;
        generatedObject.SetActive(false);
        generatedObject.SetActive(true);
    }
}
