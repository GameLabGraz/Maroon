//
//Authors: Tobias Stöckl, Alexander Kassil
//

using System;
using System.Collections;
using UnityEngine;

namespace Maroon.PlatformControls.PC
{
    public class CameraControls : MonoBehaviour
    {
        [SerializeField] private bool topView;
        [SerializeField] private float moveSpeed = 3.0f;
        [SerializeField] private float zoomSpeed = 1.0f;
        
        public Vector3 minPosition = new Vector3(-2.0f,1.4f,0.5f);
        public Vector3 maxPosition = new Vector3(2.0f, 3.0f, 3.5f);
        private Vector3 _newCameraPos;

        private Camera cam;
        private Vector3 _camPos;
        private Quaternion _camRot;
        private bool _isMoving;
        
        private readonly Vector3 _camTopPos = new Vector3(0, 3f, 2.5f);
        private readonly Quaternion _camTopRot = Quaternion.Euler(90, 0, 0);

        private void Start()
        {
            cam = GetComponent<Camera>();
        }

        void Update()
        {
            if (!topView && !_isMoving)
            {
                if (Input.GetMouseButton(1))
                {
                    float cameraOffsetX = -Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
                    float cameraOffsetZ = Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;

                    _newCameraPos = transform.position + new Vector3(cameraOffsetX, 0, cameraOffsetZ);

                    if (CheckPointInCuboid(_newCameraPos, minPosition, maxPosition))
                    {
                        transform.position += new Vector3(cameraOffsetX, 0, cameraOffsetZ);
                    }
                }

                _newCameraPos = transform.position + transform.forward * (zoomSpeed * Input.GetAxis("Mouse ScrollWheel"));
                
                if (CheckPointInCuboid(_newCameraPos, minPosition, maxPosition))
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
                _camPos = cam.transform.position;
                _camRot = cam.transform.rotation;
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

            Vector3 center = (minPosition + maxPosition) * 0.5f;
            Vector3 size = new Vector3(Mathf.Abs(minPosition.x - maxPosition.x), Mathf.Abs(minPosition.y - maxPosition.y), Mathf.Abs(minPosition.z - maxPosition.z));

            Gizmos.DrawWireCube(center, size);
        }
        
    }
}