//
//Authors: Alexander Kassil
//

using System;
using System.Collections;
using Maroon.scenes.experiments.OpticsSimulations.Scripts;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager;
using UnityEngine;

namespace Maroon.PlatformControls.PC
{
    public class CameraControls : MonoBehaviour
    {
        [SerializeField] private bool topView;
        [SerializeField] private float moveSpeed = 3.0f;
        [SerializeField] private float zoomSpeed = 1.0f;
        
        private Vector3 _newCameraPos;
        private Camera _cam;
        private Transform _camTransform;
        private Vector3 _camPos;
        private Quaternion _camRot;
        private bool _isMoving;
        private float _moveFactor;
        
        private readonly Vector3 _camTopPos = new Vector3(0, 3f, 2.5f);
        private readonly Quaternion _camTopRot = Quaternion.Euler(90, 0, 0);

        private void Start()
        {
            _cam = GetComponent<Camera>();
            _camTransform = _cam.transform;
        }

        void Update()
        {
            if (!topView && !_isMoving)
            {
                Ray ray = new Ray(_camTransform.position, _camTransform.forward);

                if (UnityEngine.Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << Constants.MouseColliderMaskIndex))
                    _moveFactor = hit.distance + 1;
                    
                    
                if (Input.GetMouseButton(1))
                {
                    float cameraOffsetX = -Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
                    float cameraOffsetZ = Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;

                    _newCameraPos = transform.position + new Vector3(cameraOffsetX, 0, cameraOffsetZ);

                    if (CheckPointInCuboid(_newCameraPos, Constants.MinPositionCamera, Constants.MaxPositionCamera))
                    {
                        transform.position += new Vector3(cameraOffsetX, 0, cameraOffsetZ) * _moveFactor;
                    }
                }

                _newCameraPos = transform.position + transform.forward * (zoomSpeed * Input.GetAxis("Mouse ScrollWheel"));
                
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
                _camPos = _cam.transform.position;
                _camRot = _cam.transform.rotation;
                StartCoroutine(ChangeCameraView(_camTopPos, _camTopRot));
            }
            else
            {
                StartCoroutine(ChangeCameraView(_camPos, _camRot));
            }
            topView = !topView;
        }
        
        private IEnumerator ChangeCameraView(Vector3 targetPos, Quaternion targetRot)
        {
            _isMoving = true;

            float startTime = Time.time;
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            while (Time.time - startTime < 1f)
            {
                float t = (Time.time - startTime) / 1f;
                transform.position = Vector3.Lerp(startPos, targetPos, t * 1.5f);
                transform.rotation = Quaternion.Slerp(startRot, targetRot, t * 1.5f);
                yield return null;
            }
            transform.position = targetPos;
            transform.rotation = targetRot;

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