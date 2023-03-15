using System.Linq;
using UnityEngine;

public class DraggableNetworkComponentScript : MonoBehaviour {
    private Collider collider;
    private Plane plane;
    private Vector3 offset;
    private bool fromKit = true;
    private Vector3 kitPosition;

    void Start() {
        collider = GetComponent<Collider>();
        plane = new Plane(Vector3.up, transform.position);
        kitPosition = transform.position;
    }

    private void OnMouseDown() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out var distance);
        offset = transform.position - ray.GetPoint(distance);
    }

    void OnMouseDrag() {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out var distance);
        transform.position = ray.GetPoint(distance) + offset;
    }

    private void OnMouseUp() {
        if(fromKit) {
            Instantiate(this, kitPosition, Quaternion.identity, transform.parent);
            transform.parent = FindObjectsOfType<Transform>().Where(t => t.name == "Network").First();
            fromKit = false;
        }
    }
}
