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
        
        [SerializeField] private float baseFOV = 60f;
        [SerializeField] private float minFOV = 1f;
        [SerializeField] private float maxFOV = 70f;
        
        private Vector3 _newCameraPos;
        private UnityEngine.Camera _cam;
        private Transform _camTransform;
        private Vector3 _camPos;
        private Quaternion _camRot;
        private float _camFOV;
        private bool _isMoving;
        private float _moveFactor;
        
        private readonly Vector3 _camTopPos = new Vector3(0, 3f, 2.5f);
        private readonly Quaternion _camTopRot = Quaternion.Euler(90, 0, 0);

        private void Start()
        {
            _cam = GetComponent<UnityEngine.Camera>();
            _camTransform = _cam.transform;
        }

        void Update()
        {
            if (!topView && !_isMoving)
            {
                if (Input.GetMouseButton(1))
                {
                    float cameraOffsetX = -Input.GetAxis("Mouse X");
                    float cameraOffsetZ = Input.GetAxis("Mouse Y");

                    float adjustedSpeed = moveSpeed * Mathf.Lerp(0.1f, 1f, (_cam.fieldOfView - minFOV) / (maxFOV - minFOV));
                    // Debug.Log("Adj speed: " + adjustedSpeed);

                    float moveFactor = adjustedSpeed * Time.deltaTime;
                    _newCameraPos = transform.position + new Vector3(cameraOffsetX, 0, cameraOffsetZ) * moveFactor;

                    if (CheckPointInCuboid(_newCameraPos, Constants.MinPositionCamera, Constants.MaxPositionCamera))
                    {
                        transform.position = _newCameraPos;
                    }
                    else
                    {
                        // TODO Set camera back in cube
                    }
                }

                _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, minFOV, maxFOV);
                
                if (CheckPointInCuboid(_newCameraPos, Constants.MinPositionCamera, Constants.MaxPositionCamera))
                    transform.position = _newCameraPos;

            }
        }

        bool CheckPointInCuboid(Vector3 p, Vector3 minCorner, Vector3 maxCorner)
        {
            return p.x >= minCorner.x && p.x <= maxCorner.x &&
                   p.y >= minCorner.y && p.y <= maxCorner.y &&
                   p.z >= minCorner.z && p.z <= maxCorner.z;
        }

        public void ToggleTopView()
        {
            if (!topView)
            {
                _camPos = _camTransform.position;
                _camRot = _camTransform.rotation;
                _camFOV = _cam.fieldOfView;
                StartCoroutine(ChangeCameraView(_camTopPos, _camTopRot, baseFOV));
            }
            else
            {
                StartCoroutine(ChangeCameraView(_camPos, _camRot, _camFOV));
            }
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
        
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Vector3 center = (Constants.MinPositionCamera + Constants.MaxPositionCamera) * 0.5f;
            Vector3 size = new Vector3(
                Mathf.Abs(Constants.MinPositionCamera.x - Constants.MaxPositionCamera.x), 
                Mathf.Abs(Constants.MinPositionCamera.y - Constants.MaxPositionCamera.y), 
                Mathf.Abs(Constants.MinPositionCamera.z - Constants.MaxPositionCamera.z)
                );

            Gizmos.DrawWireCube(center, size);
        }
        
    }
}