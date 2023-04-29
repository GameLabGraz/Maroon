using UnityEngine;
using UnityEngine.EventSystems;

public class SortingPlanetDragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform planetTarget; // The specific target for each planet
    public float scaleFactor = 1f; // The scale factor to adjust the object size based on the target

    private Vector3 startPosition;
    private Transform startParent;

    void Start()
    {
        startPosition = transform.position;
        startParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<Collider>().enabled = false; // Disable the collider while dragging to avoid any conflicts
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 screenPoint = new Vector3(eventData.position.x, eventData.position.y, 10f);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPoint);
        transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float distanceToTarget = Vector3.Distance(transform.position, planetTarget.position);

        // Check if the object is dropped near the target
        if (distanceToTarget < 1f) // You can adjust this value based on your desired snapping distance
        {
            transform.position = planetTarget.position;
            transform.localScale = scaleFactor * planetTarget.localScale;
        }
        else
        {
            // Snap back to the original position
            transform.position = startPosition;
            transform.SetParent(startParent);
        }

        GetComponent<Collider>().enabled = true; // Re-enable the collider after dragging
    }
}
