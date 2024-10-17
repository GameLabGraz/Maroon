using System.Collections;
using Maroon.Physics.Optics.Manager;
using Maroon.Physics.Optics.Util;
using Maroon.Utils;
using UnityEngine;
using Newtonsoft.Json;

namespace Maroon.Physics.Optics.Camera
{
    public class CameraControls : MonoBehaviour
    {
        [SerializeField] private bool isTopView;
        [SerializeField] private QuantityFloat moveSpeed = 4.0f;
        [SerializeField] private float zoomSpeed = 15.0f;

        private UnityEngine.Camera _cam;
        private CameraSetting _currentView;
        private CameraSetting _topView;
        private CameraSetting _baseView;
        
        private bool _isMoving;
        private float _moveFactor;
        
        public bool IsTopView => isTopView;

        [System.Serializable]
        public struct CameraSetting
        {
            public SerializableVector3 Position;
            public SerializableQuaternion Rotation;
            public float FOV;

            public CameraSetting(Vector3 position, Quaternion rotation, float fov)
            {
                Position = new SerializableVector3(position);
                Rotation = new SerializableQuaternion(rotation);
                FOV = fov;
            }
        }
        
        private void Start()
        {
            _cam = GetComponent<UnityEngine.Camera>();
            _currentView = new CameraSetting(Constants.BaseCamPos, Constants.BaseCamRot, Constants.BaseCamFOV);
            _baseView = new CameraSetting(Constants.BaseCamPos, Constants.BaseCamRot, Constants.BaseCamFOV);
            _topView = new CameraSetting(Constants.TopCamPos, Constants.TopCamRot, Constants.TopCamFOV);
        }

        private void Update()
        {
            if (ExperimentManager.Instance.mouseOnUIPanel || _isMoving)
                return;
            
            if (Input.GetMouseButton(1))
            {
                float cameraOffsetX = -Input.GetAxis("Mouse X");
                float cameraOffsetZ = Input.GetAxis("Mouse Y");
                float adjustedSpeed = moveSpeed.Value * 0.33f * Mathf.Lerp(0.1f, 1f, (_cam.fieldOfView - Constants.MinFOV) / (Constants.MaxFOV - Constants.MinFOV));

                float moveFactor = adjustedSpeed * Time.deltaTime;
                Vector3 newCamPos = transform.position + new Vector3(cameraOffsetX, 0, cameraOffsetZ) * moveFactor;

                _currentView.Position.Set(new Vector3
                {
                    x = Mathf.Clamp(newCamPos.x, Constants.MinPositionCamera.x, Constants.MaxPositionCamera.x),
                    y = Mathf.Clamp(newCamPos.y, Constants.MinPositionCamera.y, Constants.MaxPositionCamera.y),
                    z = Mathf.Clamp(newCamPos.z, Constants.MinPositionCamera.z, Constants.MaxPositionCamera.z)
                });
                UpdateCamera(_currentView);
            }
            _currentView.FOV = Mathf.Clamp(_cam.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, Constants.MinFOV, Constants.MaxFOV);
            _cam.fieldOfView = _currentView.FOV;
        }

        private void UpdateCamera(CameraSetting cs)
        {
            _cam.transform.position = cs.Position;
            _cam.transform.rotation = cs.Rotation;
            _cam.fieldOfView = cs.FOV;

            if (!isTopView)   CopyCameraSetting(_currentView, out _baseView);
            else CopyCameraSetting(_currentView, out _topView);
        }

        public void ResetCameras()
        {
            _baseView = new CameraSetting(Constants.BaseCamPos, Constants.BaseCamRot, Constants.BaseCamFOV);
            _topView = new CameraSetting(Constants.TopCamPos, Constants.TopCamRot, Constants.TopCamFOV);
            isTopView = false;
            StartCoroutine(ChangeCameraView(_baseView));
        }

        public void ToggleTopView()
        {
            if (!isTopView) StartCoroutine(ChangeCameraView(_topView));
            else StartCoroutine(ChangeCameraView(_baseView));
            isTopView = !isTopView;
        }
        
        private IEnumerator ChangeCameraView(CameraSetting cs)
        {
            _isMoving = true;

            float startTime = Time.time;
            Transform camTransform = _cam.transform;

            while (Time.time - startTime < 1f)
            {
                float t = (Time.time - startTime) / 1f;
                camTransform.position = Vector3.Lerp(_currentView.Position, cs.Position, t * 1.5f);
                camTransform.rotation = Quaternion.Slerp(_currentView.Rotation, cs.Rotation, t * 1.5f);
                _cam.fieldOfView = Mathf.Lerp(_currentView.FOV, cs.FOV, t * 1.5f);
                yield return null;
            }
            camTransform.position = cs.Position;
            camTransform.rotation = cs.Rotation;
            _cam.fieldOfView = cs.FOV;
            CopyCameraSetting(cs, out _currentView);

            _isMoving = false;
        }
        
        public void SetPresetCameras(CameraSetting presetBaseView, CameraSetting presetTopView)
        {
            CopyCameraSetting(presetBaseView, out _baseView);
            CopyCameraSetting(presetTopView, out _topView);
            
            if (!isTopView) StartCoroutine(ChangeCameraView(_baseView));
            else StartCoroutine(ChangeCameraView(_topView));
        }
        
        private void CopyCameraSetting(CameraSetting a, out CameraSetting b)
        {
            b = new CameraSetting (a.Position, a.Rotation, a.FOV);
        }
    }
}