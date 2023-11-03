using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.Handlers
{
    public class TranslationRotationHandler : MonoBehaviour
    {
        private Plane _translationPlane;
        private Vector3 _objectToMouseTablePosOffset;
        private Vector3 _objectToMouseTablePosLocalOffset;

        private void Awake()
        {
            _translationPlane = new Plane(transform.forward, new Vector3(0, 0, 0));
        }

        public void DoTranslation(Ray mouseRay, Vector3 hitPoint)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _translationPlane.SetNormalAndPosition(transform.forward, hitPoint);
                _objectToMouseTablePosOffset = hitPoint - transform.position;
                _objectToMouseTablePosLocalOffset = hitPoint - transform.localPosition;
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

    }
}
