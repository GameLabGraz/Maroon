using System;
using System.Collections;
using Maroon.scenes.experiments.OpticsSimulations.Scripts;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Util;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Camera
{
    public class CameraControls : MonoBehaviour
    {
        [SerializeField] private bool topView;
        [SerializeField] private float moveSpeed = 8.0f;
        [SerializeField] private float zoomSpeed = 15.0f;

        private const float MinFOV = 1f;
        private const float MaxFOV = 70f;
        
        private Vector3 _newCameraPos;
        private UnityEngine.Camera _cam;
        private Transform _camTransform;
        private Vector3 _camPos;
        private Quaternion _camRot;
        private float _camFOV;
        private bool _isMoving;
        private float _moveFactor;
        
        private void Start()
        {
            _cam = GetComponent<UnityEngine.Camera>();
            _camTransform = _cam.transform;
        }

        void Update()
        {
            if (ExperimentManager.Instance.mouseOnUIPanel || _isMoving)
                return;
            
            if (Input.GetMouseButton(1))
            {
                float cameraOffsetX = -Input.GetAxis("Mouse X");
                float cameraOffsetZ = Input.GetAxis("Mouse Y");
                float adjustedSpeed = moveSpeed * Mathf.Lerp(0.1f, 1f, (_cam.fieldOfView - MinFOV) / (MaxFOV - MinFOV));

                float moveFactor = adjustedSpeed * Time.deltaTime;
                _newCameraPos = transform.position + new Vector3(cameraOffsetX, 0, cameraOffsetZ) * moveFactor;

                MoveCamera(_newCameraPos);
            }
            _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, MinFOV, MaxFOV);
        }

        void MoveCamera(Vector3 p)
        {
            Vector3 newCameraPos = new Vector3
            {
                x = Mathf.Clamp(p.x, Constants.MinPositionCamera.x, Constants.MaxPositionCamera.x),
                y = Mathf.Clamp(p.y, Constants.MinPositionCamera.y, Constants.MaxPositionCamera.y),
                z = Mathf.Clamp(p.z, Constants.MinPositionCamera.z, Constants.MaxPositionCamera.z)
            };
            transform.position = newCameraPos;
        }

        public void ToggleTopView()
        {
            if (!topView)
            {
                _camPos = _camTransform.position;
                _camRot = _camTransform.rotation;
                _camFOV = _cam.fieldOfView;
                StartCoroutine(ChangeCameraView(Constants.CamTopPos, Constants.CamTopRot, Constants.BaseCameraFOV));
            }
            else
                StartCoroutine(ChangeCameraView(_camPos, _camRot, _camFOV));
            topView = !topView;
        }
        
        private IEnumerator ChangeCameraView(Vector3 targetPos, Quaternion targetRot, float targetFOV)
        {
            _isMoving = true;

            float startTime = Time.time;
            Transform trans = transform;
            Vector3 startPos = trans.position;
            Quaternion startRot = trans.rotation;
            float startFOV = _cam.fieldOfView;

            while (Time.time - startTime < 1f)
            {
                float t = (Time.time - startTime) / 1f;
                trans.position = Vector3.Lerp(startPos, targetPos, t * 1.5f);
                trans.rotation = Quaternion.Slerp(startRot, targetRot, t * 1.5f);
                _cam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t * 1.5f);
                yield return null;
            }
            trans.position = targetPos;
            trans.rotation = targetRot;
            _cam.fieldOfView = targetFOV;

            _isMoving = false;
        }

    }
}