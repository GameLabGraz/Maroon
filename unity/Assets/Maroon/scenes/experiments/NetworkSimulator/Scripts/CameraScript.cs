using TMPro;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class CameraScript : MonoBehaviour {

        enum CameraMode {
            Network,
            InsideDevice,
            Computer
        }

        struct CameraPosition {
            public bool canMove;
            public bool canZoom;
            public bool canPanAndTilt;
            public float minXPos;
            public float maxXPos;
            public float minYPos;
            public float maxYPos;
            public float minZPos;
            public float maxZPos;
            public Quaternion rotationStart;
        }

        private CameraPosition NetworkView = new CameraPosition {
            canMove = true,
            canZoom = true,
            canPanAndTilt = true,
            minXPos = -1.2f,
            maxXPos = 1.2f,
            minYPos = 1.3f,
            maxYPos = 3.5f,
            minZPos = 2.4f,
            maxZPos = 3.6f,
            rotationStart = Quaternion.Euler(45, 0, 0)
        };

        private CameraPosition InsideDeviceView = new CameraPosition {
            canMove = false,
            canZoom = false,
            canPanAndTilt = true,
            minXPos = 0f,
            maxXPos = 0f,
            minYPos = -7f,
            maxYPos = -7f,
            minZPos = 0f,
            maxZPos = 0f,
            rotationStart = Quaternion.Euler(10, 0, 0)
        };


        private Camera mainCamera;
        private float moveSpeed = 1;
        private float scrollSpeed = 0.5f;
        private float panAndTiltIntensity = 4.5f;
        private CameraMode currentCameraMode;
        private CameraPosition currentCameraPosition;
        private Vector3 prevNetworkViewPosition;

        void Start() {
            mainCamera = Camera.main;
            currentCameraMode = CameraMode.Network;
            currentCameraPosition = NetworkView;
        }

        void Update() {
            if(currentCameraPosition.canMove) {
                Move();
            }
            if(currentCameraPosition.canZoom) {
                Zoom();
            }
            if(currentCameraPosition.canPanAndTilt) {
                PanAndTilt();
            }
            ClampPosition();
        }

        public void SetNetworkView() {
            if(currentCameraMode == CameraMode.Network) {
                return;
            }
            currentCameraMode = CameraMode.Network;
            currentCameraPosition = NetworkView;
            mainCamera.transform.position = prevNetworkViewPosition;
        }
        public void SetInsideDeviceView() {
            if(currentCameraMode == CameraMode.InsideDevice) {
                return;
            }
            currentCameraMode = CameraMode.InsideDevice;
            prevNetworkViewPosition = mainCamera.transform.position;
            currentCameraPosition = InsideDeviceView;
        }
        public void SetComputerView(Vector3 computerPosition) {
            if(currentCameraMode == CameraMode.Computer) {
                return;
            }
            currentCameraMode = CameraMode.Computer;
            prevNetworkViewPosition = mainCamera.transform.position;
            currentCameraPosition = new CameraPosition {
                canMove = false,
                canZoom = false,
                canPanAndTilt = false,
                minXPos = computerPosition.x,
                maxXPos = computerPosition.x,
                minYPos = computerPosition.y + 0.3f,
                maxYPos = computerPosition.y + 0.3f,
                minZPos = computerPosition.z - 0.5f,
                maxZPos = computerPosition.z - 0.5f,
                rotationStart = Quaternion.Euler(18, 0, 0)
            };
            mainCamera.transform.rotation = currentCameraPosition.rotationStart;
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
            mainCamera.transform.rotation = Quaternion.Euler(
                currentCameraPosition.rotationStart.eulerAngles.x - (mouse.y - 0.5f) * panAndTiltIntensity,
                currentCameraPosition.rotationStart.eulerAngles.y + (mouse.x - 0.5f) * panAndTiltIntensity,
                0);
        }

        private void ClampPosition() {
            mainCamera.transform.position = new Vector3(
                Mathf.Clamp(mainCamera.transform.position.x, currentCameraPosition.minXPos, currentCameraPosition.maxXPos),
                Mathf.Clamp(mainCamera.transform.position.y, currentCameraPosition.minYPos, currentCameraPosition.maxYPos),
                Mathf.Clamp(mainCamera.transform.position.z, currentCameraPosition.minZPos, currentCameraPosition.maxZPos)
            );
        }
    }
}