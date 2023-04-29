using UnityEngine;

public class PlanetDragAndDrop : MonoBehaviour
{
    public float distanceToCamera = 1.0f;
    public float snapDistance = 1.0f;
    //public  sortingPlanetTarget;

    private bool dragging = false;
    private Vector3 initialPosition;
    private Camera mainCamera;

    private void Start()
    {
        initialPosition = transform.position;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (dragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = distanceToCamera;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            transform.position = worldPos;
        }
    }

    private void OnMouseDown()
    {
        dragging = true;
    }

    private void OnMouseUp()
    {
        dragging = false;
        //sortingPlanetTarget.SnapToClosestPlaceholder(this.gameObject, snapDistance);
    }


}
