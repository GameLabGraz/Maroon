using System.Linq;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class NetworkDevice : MonoBehaviour {
        [SerializeField]
        private BoxCollider networkAreaCollider;
        [SerializeField]
        private float clickVsDragThreshold = 0.001f;

        private Plane plane;
        private Vector3 offset;
        private bool fromKit = true;
        private Vector3 kitPosition;
        private Vector3 dragStartPosition;
        private Vector3 clickStartPosition;

        void Start() {
            plane = new Plane(Vector3.up, transform.position);
            kitPosition = transform.position;
        }

        private void OnMouseDown() {
            clickStartPosition = Input.mousePosition;
            dragStartPosition = transform.position;
            var ray = Camera.main.ScreenPointToRay(clickStartPosition);
            plane.Raycast(ray, out var distance);
            offset = transform.position - ray.GetPoint(distance);
        }

        void OnMouseDrag() {
            var newMousePosition = Input.mousePosition;
            if((clickStartPosition - newMousePosition).sqrMagnitude < clickVsDragThreshold) {
                return;
            }
            var ray = Camera.main.ScreenPointToRay(newMousePosition);
            plane.Raycast(ray, out var distance);
            var newPosition = ray.GetPoint(distance) + offset;
            if(fromKit) {
                transform.position = newPosition;
            }
            else {
                var closestNetworkPoint = networkAreaCollider.ClosestPoint(newPosition);
                var isInNetworkArea = (closestNetworkPoint - newPosition).sqrMagnitude < float.Epsilon;
                if(isInNetworkArea) {
                    transform.position = newPosition;
                }
                else {
                    transform.position = closestNetworkPoint;
                }
            }
        }

        private void OnMouseUp() {
            var isInNetworkArea = (networkAreaCollider.ClosestPoint(transform.position) - transform.position).sqrMagnitude < float.Epsilon;
            if(fromKit) {
                if(isInNetworkArea) {
                    Instantiate(this, kitPosition, Quaternion.identity, transform.parent).name = name;
                    transform.parent = networkAreaCollider.transform;
                    fromKit = false;
                }
                else {
                    transform.position = kitPosition;
                }
            }
            else {
                var newMousePosition = Input.mousePosition;
                if((clickStartPosition - newMousePosition).sqrMagnitude < clickVsDragThreshold) {
                    UIController.ShowDeviceOptions();
                    return;
                }
                if(!isInNetworkArea) {
                    transform.position = dragStartPosition;
                }
            }
        }
    }
}