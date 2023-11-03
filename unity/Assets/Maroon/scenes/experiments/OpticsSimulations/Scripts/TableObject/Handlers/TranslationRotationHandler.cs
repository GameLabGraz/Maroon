using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.Handlers
{
    public class TranslationRotationHandler : MonoBehaviour
    {
        private Plane _translationPlane;
        private Plane _rotationYPlane;
        private Plane _rotationZPlane;
        
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
                _rotationYPlane.SetNormalAndPosition(transform.up, hitPoint);
                SetMouseToTableOffsets(hitPoint);
            }

            if (Input.GetMouseButton(0))
            {
                _rotationYPlane.Raycast(mouseRay, out var dist);
                Vector3 pointOnPlane = mouseRay.GetPoint(dist);

                pointOnPlane.y = transform.position.y;
                var newLookDirection = (pointOnPlane - transform.position).normalized;
                transform.right = new Vector3(-newLookDirection.x, 0, -newLookDirection.z);
            }
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
