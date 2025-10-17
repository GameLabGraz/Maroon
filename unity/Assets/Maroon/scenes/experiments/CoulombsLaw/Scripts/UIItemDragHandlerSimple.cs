using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIItemDragHandlerSimple : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Canvas _parentCanvas = null;

    [Tooltip("To determine at which Depth objects are spawned/placed, a reference position + offset can be used")]
    [SerializeField] private Transform _referencePosition = null;
    [Tooltip("To determine at which Depth objects are spawned/placed, a reference position + offset can be used")]
    [SerializeField] private float _zOffset = 0;

    [SerializeField] private Transform _minBoundary = null;
    [SerializeField] private Transform _maxBoundary = null;

    public UnityEngine.Events.UnityEvent<Vector3> OnDragFinished; 
    private GameObject _placeholderObject; // The 2D object used as placeholder until the drag has finished

    private void Awake()
    {
        _parentCanvas = GetComponentInParent<Canvas>();
        Debug.Assert(_parentCanvas != null, "UIItemDragHandler should only be used on UI-objects");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _placeholderObject = Instantiate(gameObject, _parentCanvas.transform);
        var recTransformOrig = gameObject.GetComponent<RectTransform>();
        var recTransform = _placeholderObject.GetComponent<RectTransform>();
        recTransform.sizeDelta = recTransformOrig.sizeDelta;
        recTransform.localScale = recTransformOrig.localScale;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        var screenPoint = Input.mousePosition;
        var finish = _parentCanvas.worldCamera.ScreenToWorldPoint(screenPoint);
        finish.z = 0f;
        _placeholderObject.transform.position = finish;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(_placeholderObject);

        // Note(MartinR): Currently I'm assuming that we spawn everything in 2D-Mode
        Vector3 referencePoint = new Vector3(0, 0, _zOffset);
        if (_referencePosition != null)
        {
            referencePoint += _referencePosition.position;
        }
        Vector3 dragEndPoint = PC_DragHandler.GetMousePointOnPlaneParallelToCamera(referencePoint);

        // Early exit if we placed something out of bounds
        if (_minBoundary != null && _maxBoundary != null)
        {
            Vector3 min = Vector3.Min(_minBoundary.position, _maxBoundary.position);
            Vector3 max = Vector3.Max(_minBoundary.position, _maxBoundary.position);
            if (dragEndPoint.x < min.x || dragEndPoint.x > max.x ||
                dragEndPoint.y < min.y || dragEndPoint.y > max.y)
            {
                return;
            }
        }

        OnDragFinished.Invoke(dragEndPoint);
    }

    public void SetBoundaries(Transform min, Transform max)
    {
        _minBoundary = min;
        _maxBoundary = max;
    }
}
