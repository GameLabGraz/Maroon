using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent;
using UnityEngine;
using LightType = Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent.LightType;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.Handlers
{
    public class TranslationRotationHandler : MonoBehaviour
    {
        private Plane _translationPlane;
        private Plane _rotationYPlane;
        private Plane _rotationZPlane;

        private Vector3 _initialRotation;
        public Vector3 gizmospoint;
        public bool drawplane;
        
        private Vector3 _objectToMouseTablePosOffset;
        private Vector3 _objectToMouseTablePosLocalOffset;

        private void Awake()
        {
            _translationPlane = new Plane(transform.forward, new Vector3(0, 0, 0));
        }

        private void SetMouseToTableOffsets(Vector3 hitPoint)
        {
            _objectToMouseTablePosOffset = hitPoint - transform.position;
            _objectToMouseTablePosLocalOffset = hitPoint - transform.localPosition;
        }

        public void DoTranslation(Ray mouseRay, Vector3 hitPoint)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _translationPlane.SetNormalAndPosition(transform.forward, hitPoint);
                SetMouseToTableOffsets(hitPoint);
            }
            if (Input.GetMouseButton(0))
            {
                _translationPlane.Raycast(mouseRay, out var dist);
                Vector3 pointOnPlane = mouseRay.GetPoint(dist);
                Vector3 desiredPos = pointOnPlane - _objectToMouseTablePosOffset;
                Vector3 desiredPosLocal = pointOnPlane - _objectToMouseTablePosLocalOffset;
                
                if (Util.Math.CheckTableBounds(desiredPosLocal))
                    transform.position = new Vector3(transform.position.x, desiredPos.y, transform.position.z);
            }
        }

        public void DoYRotation(Ray mouseRay, Vector3 hitPoint)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // _initialRotation = transform.right;
                _rotationYPlane.SetNormalAndPosition(transform.up, hitPoint);
                drawplane = true;
                gizmospoint = hitPoint;
                SetMouseToTableOffsets(hitPoint);
                
                // Debug.Log("arrow center pos: " + transform.Find("RotationArrowY").position.ToString("f3"));
                // Debug.Log("plane        pos: " + hitPoint.ToString("f3"));
            }

            if (Input.GetMouseButton(0))
            {
                // TODO Fix this impementation.. I guess something wrong with angle
                // _rotationYPlane.Raycast(mouseRay, out var dist);
                // Vector3 pointOnPlane = mouseRay.GetPoint(dist);
                // var newLookDirection = (pointOnPlane - transform.position).normalized;
                // var angle = Vector3.Angle(-transform.right, newLookDirection);
                // transform.RotateAround(transform.GetChild(0).position, _rotationYPlane.normal, angle);
                

                // ONLY Y ROT (RESETTING Z ROT)
                _rotationYPlane.Raycast(mouseRay, out var dist);
                Vector3 pointOnPlane = mouseRay.GetPoint(dist);
                pointOnPlane.y = transform.position.y;  // TODO
                var newLookDirection = (pointOnPlane - transform.position).normalized;
                transform.right = -newLookDirection;
            }
        }

        private void OnDrawGizmos()
        {
            // DrawPlane(_rotationYPlane.normal, gizmospoint, 0.3f);
        }
        
        void DrawPlane(Vector3 planeNormal, Vector3 planeCenter, float planeSize)
        {
            Vector3 v0 = planeCenter + Quaternion.AngleAxis(45, planeNormal) * Vector3.right * planeSize;
            Vector3 v1 = planeCenter + Quaternion.AngleAxis(135, planeNormal) * Vector3.right * planeSize;
            Vector3 v2 = planeCenter + Quaternion.AngleAxis(225, planeNormal) * Vector3.right * planeSize;
            Vector3 v3 = planeCenter + Quaternion.AngleAxis(315, planeNormal) * Vector3.right * planeSize;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(v0, v1);
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v3);
            Gizmos.DrawLine(v3, v0);
        }

        public void DoZRotation(Ray mouseRay, Vector3 hitPoint)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _rotationZPlane.SetNormalAndPosition(transform.forward, hitPoint);
                SetMouseToTableOffsets(hitPoint);
            }

            if (Input.GetMouseButton(0))
            {
                _rotationZPlane.Raycast(mouseRay, out var dist);
                Vector3 pointOnPlane = mouseRay.GetPoint(dist);

                pointOnPlane.z = transform.position.z;
                var newLookDirection = (pointOnPlane - transform.position).normalized;
                transform.right = new Vector3(-newLookDirection.x, -newLookDirection.y, 0);
            }
        }

    }
}
