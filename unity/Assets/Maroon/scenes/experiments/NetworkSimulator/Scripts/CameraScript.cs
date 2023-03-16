using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class CameraScript : MonoBehaviour {

        private Camera mainCamera;
        private float moveSpeed = 1;
        private float scrollSpeed = 0.5f;
        private float panAndTiltIntensity = 4.5f;
        private float minXPos = -1.2f;
        private float maxXPos = 1.2f;
        private float minYPos = 1.3f;
        private float maxYPos = 3.5f;
        private float minZPos = 1.9f;
        private float maxZPos = 3.8f;
        private Quaternion rotationStart;

        void Start() {
            mainCamera = Camera.main;
            rotationStart = mainCamera.transform.rotation;
        }

        void Update() {
            Move();
            Zoom();
            PanAndTilt();
            ClampPosition();
        }

        private void Move() {
            var posChange = Vector3.zero;
            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
                posChange += new Vector3(0, 0, 1);
            }
            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
                posChange += new Vector3(-1, 0, 0);
            }
            if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
                posChange += new Vector3(0, 0, -1);
            }
            if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
                posChange += new Vector3(1, 0, 0);
            }
            var movement = posChange.normalized * Time.deltaTime * moveSpeed;
            mainCamera.transform.Translate(movement, Space.World);
        }

        private void Zoom() {
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            mainCamera.transform.Translate(0, 0, scroll * scrollSpeed);
        }

        private void PanAndTilt() {
            var mouse = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            mainCamera.transform.rotation = Quaternion.Euler(rotationStart.eulerAngles.x - (mouse.y - 0.5f) * panAndTiltIntensity, rotationStart.eulerAngles.y + (mouse.x - 0.5f) * panAndTiltIntensity, 0);
        }

        private void ClampPosition() {
            mainCamera.transform.position = new Vector3(
                Mathf.Clamp(mainCamera.transform.position.x, minXPos, maxXPos),
                Mathf.Clamp(mainCamera.transform.position.y, minYPos, maxYPos),
                Mathf.Clamp(mainCamera.transform.position.z, minZPos, maxZPos)
            );
        }
    }
}