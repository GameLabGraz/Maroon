using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIItemDragHandlerSimple : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
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
    
    private GameObject _item;
    private Transform _parent;

    protected void Start()
    {
        if (childObject == null)
            childObject = generatedObject;

        _parent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _item = Instantiate(gameObject, parentCanvas.transform);
        var recTransformOrig = gameObject.GetComponent<RectTransform>();
        var recTransform = _item.GetComponent<RectTransform>();

        recTransform.sizeDelta = recTransformOrig.sizeDelta;
        recTransform.localScale = recTransformOrig.localScale;


        if (changeLayerObject)
        {
            changeLayerObject.layer = 0;
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        //_item.transform.parent = parentCanvas.transform;

        var screenPoint = Input.mousePosition;
        var finish = parentCanvas.worldCamera.ScreenToWorldPoint(screenPoint);
        finish.z = 0f;
        _item.transform.position = finish;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, 100f);

        foreach (var hit in hits)
        {
            if(!hit.transform.CompareTag("VectorField"))
                continue;

            ShowObject(hit.point, hit.transform.parent);
        }

        if (changeLayerObject)
        {
            changeLayerObject.layer = 2; //IgnoreRaycast
        }

        Destroy(_item);
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

        childObject.transform.parent = parent;
        generatedObject.transform.position = position;
        if(resetRotation)
            generatedObject.transform.localRotation = Quaternion.identity;
        generatedObject.SetActive(false);
        generatedObject.SetActive(true);
    }
}
