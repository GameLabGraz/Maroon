using UnityEngine;
using UnityEngine.EventSystems;

public class SortingPlanetDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private Transform originalParent;
    private Transform placeholderTransform;
    public float snapDistance = 1.0f;

    void Start()
    {
        placeholderTransform = GameObject.Find("Placeholder").transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        originalParent = transform.parent;

        // Make the current object a child of the scene so that it can move freely
        transform.SetParent(placeholderTransform.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 screenPoint = new Vector3(eventData.position.x, eventData.position.y, 10f);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPoint);
        transform.position = new Vector3(worldPosition.x, worldPosition.y, 0f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bool snapped = false;

        for (int i = 0; i < placeholderTransform.childCount; i++)
        {
            Transform child = placeholderTransform.GetChild(i);

            if (Vector3.Distance(child.position, transform.position) <= snapDistance)
            {
                transform.SetParent(child);
                transform.position = child.position;
                snapped = true;
                break;
            }
        }

        if (!snapped)
        {
            // If the object is not dropped in the right position, snap it back to its original position
            transform.SetParent(originalParent);
            transform.position = originalPosition;
        }
    }
}
