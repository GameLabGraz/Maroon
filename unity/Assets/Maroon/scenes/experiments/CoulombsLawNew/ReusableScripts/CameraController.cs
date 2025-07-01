using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public enum CameraMode2D
    {
        FRONT,
        BACK,
        LEFT,
        RIGHT,
        TOP,
        BOTTOM
    }

    public class CameraController : MonoBehaviour
    {
        // Note(MartinR): Since maroon has the player object, we should use this for camera movement,
        //      although I'm not sure if we should query this by name or if there is a singleton to access it, so
        //      right now the reference should be set by any experiment that uses this CameraController
        [SerializeField] GameObject _cameraObject = null;

        [SerializeField] private bool _in3DMode = false;
        public bool In3DMode { get { return _in3DMode; } }

        [SerializeField] private CameraMode2D _cameraMode2D = CameraMode2D.FRONT;
        public CameraMode2D CameraMode2D { get { return _cameraMode2D; } }

        public UnityEngine.Events.UnityEvent OnCameraModeChanged;

        // Orbit Camera variables (Angles in Degree)
        private float orbitInclineAngle = 0.0f;
        private float orbitRotationAngle = 0.0f;
        private float orbitDistanceToCenter = 3.0f;
        private bool orbitDragActive = false;

        public void SetIn3DMode(bool in3D)
        {
            if (in3D == _in3DMode) return;
            _in3DMode = in3D;
            OnCameraModeChanged.Invoke();
            UpdateCamera();
            OnCameraModeChanged.Invoke();
        }

        public void SetCameraMode2D(CameraMode2D mode2D)
        {
            if (_cameraMode2D == mode2D) return;
            _cameraMode2D = mode2D;
            if (_in3DMode) return;
            UpdateCamera();
            OnCameraModeChanged.Invoke();
        }

        private void UpdateCamera()
        {
            if (_cameraObject == null) return;
            var camera = Camera.main;
            camera.orthographic = !_in3DMode;
            camera.orthographicSize = 1.25f; // Half-size of orthographic height (width is determined by screen aspect ratio)

            var simBox = SimulationBox.Instance.Bounds;
            var transform = _cameraObject.transform;
            if (_in3DMode)
            {
                transform.rotation = Quaternion.Euler(new Vector3(orbitInclineAngle, orbitRotationAngle, 0));
                transform.position = simBox.center - transform.rotation * Vector3.forward * orbitDistanceToCenter;
            }
            else
            {
                // TODO: Set position and rotation according to 2D mode
                transform.rotation = Quaternion.identity;
                float cameraDistance = 2.0f;
                transform.position = simBox.center - transform.rotation * Vector3.forward * cameraDistance;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.C))
            {
                _in3DMode = !_in3DMode;
                UpdateCamera();
                OnCameraModeChanged.Invoke();
            }

            if (_cameraObject == null || !_in3DMode) return;
            var camera = Camera.main;

            bool lastDragActive = orbitDragActive;
            orbitDragActive = _in3DMode && Input.GetMouseButton(1) && Application.isFocused && !SelectionSystem.IsMouseOverVisibleUIElement();

            // Active/Deactive cursor
            if (lastDragActive != orbitDragActive)
            {
                UnityEngine.Cursor.lockState = orbitDragActive ? CursorLockMode.Locked : CursorLockMode.None;
                UnityEngine.Cursor.visible = !orbitDragActive;
            }

            // Update camera rotation
            if (orbitDragActive)
            {
                // Update orbit camera coordinates based on mouse delta
                Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                float maxScreenDimension = Mathf.Max(Screen.currentResolution.width, Screen.currentResolution.height);
                float sensitivity = 3.0f * 360 / maxScreenDimension; // 3 Rotation for every 1 screen of mouse movement

                orbitRotationAngle += mouseDelta.x * sensitivity;
                orbitInclineAngle += mouseDelta.y * sensitivity;

                // Clamp spherical coordinates
                orbitRotationAngle = orbitRotationAngle % 360.0f;
                orbitInclineAngle = Mathf.Clamp(orbitInclineAngle, -45.0f, 75.0f);
            }
            // Update Camera zoom
            if (Application.isFocused && !SelectionSystem.IsMouseOverVisibleUIElement())
            {
                const float MOUSE_WHEEL_SENSITIVITY = 0.3f;
                orbitDistanceToCenter -= Input.mouseScrollDelta.y * MOUSE_WHEEL_SENSITIVITY;
                orbitDistanceToCenter = Mathf.Clamp(orbitDistanceToCenter, 1.0f, 3.0f);
            }

            UpdateCamera();
        }

        // Note(MartinR): static helper method for drag and drop calculation, not sure where else I should put it
        public static Vector3 GetMousePointOnPlaneParallelToCamera(Vector3 pointOnPlane)
        {
            Plane movementPlane = new Plane(Camera.main.transform.rotation * Vector3.back, pointOnPlane);
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float planeIntersectionDistance = 0.0f;
            movementPlane.Raycast(ray, out planeIntersectionDistance);
            return ray.GetPoint(planeIntersectionDistance);
        }



        // Singleton Pattern (See SimulationBox.cs for comments about the implementation)
        private static CameraController _instance;

        public static CameraController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<CameraController>();
                    if (_instance == null) _instance = new GameObject("CameraController").AddComponent<CameraController>();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                _instance.orbitDistanceToCenter = this.orbitDistanceToCenter;
                _instance.orbitInclineAngle = this.orbitInclineAngle;
                _instance.orbitRotationAngle = this.orbitRotationAngle;
                _instance.SetIn3DMode(this.In3DMode);
                _instance.SetCameraMode2D(this.CameraMode2D);
                _instance.UpdateCamera();
                
                Destroy(this.gameObject);
                return;
            }
            _instance = this;

            UpdateCamera();
        }

        private void OnDestroy()
        {
            if (this == _instance) { _instance = null; }
        }
    }
}
