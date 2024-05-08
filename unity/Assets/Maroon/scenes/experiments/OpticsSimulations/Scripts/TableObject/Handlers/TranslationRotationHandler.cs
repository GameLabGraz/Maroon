using Maroon.Physics.Optics.TableObject.OpticalComponent;
using UnityEngine;

namespace Maroon.Physics.Optics.TableObject.Handlers
{
    public class TranslationRotationHandler : MonoBehaviour
    {
        private Plane _translationPlane;
        private Plane _rotationYPlane;
        private Plane _rotationZPlane;
        private Vector3 _initialRotation;

        private TableObject _tableObject;
        
        private Vector3 _objectToMouseTablePosOffset;
        private Vector3 _objectToMouseTablePosLocalOffset;

        private void Awake()
        {
            _tableObject = GetComponent<TableObject>();
            _translationPlane = new Plane(transform.forward, Vector3.zero);
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
              var transformUp = transform.up;
              if (Input.GetMouseButtonDown(0))
              {
                  // We need to adjust the collider hitPoint by projecting it onto the plane defined by: transform.up, transform.position
                  // So the arrow is now "hit" at the middle of its thickness and not on the mantle
                  Vector3 projHitPoint = ProjectPointOntoPlane(hitPoint, transform.GetChild(0).position, transformUp);
                  
                  _rotationYPlane.SetNormalAndPosition(transformUp, projHitPoint);
                  SetMouseToTableOffsets(projHitPoint);
              }
              if (Input.GetMouseButton(0))
              {
                  _rotationYPlane.Raycast(mouseRay, out var dist);
                  Vector3 pointOnPlane = mouseRay.GetPoint(dist);
                  var newLookDirection = (pointOnPlane - transform.GetChild(0).position).normalized;
                  var angle = Vector3.SignedAngle(GetAngleMeasureVector(), newLookDirection, transform.up);
                  transform.RotateAround(transform.GetChild(0).position, transformUp, angle);
              }
        }  
        
        public void DoZRotation(Ray mouseRay, Vector3 hitPoint)
        {
            var transformForward = transform.forward;
            if (Input.GetMouseButtonDown(0))
            {
                // We need to adjust the collider hitPoint by projecting it onto the plane defined by: transform.up, transform.position
                // So the arrow is now "hit" at the middle of its thickness and not on the mantle
                Vector3 projHitPoint = ProjectPointOntoPlane(hitPoint, transform.GetChild(0).position, transformForward);
                _rotationZPlane.SetNormalAndPosition(transformForward, projHitPoint);
                SetMouseToTableOffsets(projHitPoint);
            }

            if (Input.GetMouseButton(0))
            {
                _rotationZPlane.Raycast(mouseRay, out var dist);
                Vector3 pointOnPlane = mouseRay.GetPoint(dist);
                var newLookDirection = (pointOnPlane - transform.GetChild(0).position).normalized;
                var angle = Vector3.SignedAngle(GetAngleMeasureVector(), newLookDirection, transformForward);
                transform.RotateAround(transform.GetChild(0).position, transformForward, angle);
                
            }
        }
        
        Vector3 ProjectPointOntoPlane(Vector3 point, Vector3 planePoint, Vector3 planeNormal)
        {
            Vector3 planeToHit = point - planePoint;
            float distance = Vector3.Dot(planeToHit, planeNormal);
            return point - distance * planeNormal;
        }

        Vector3 GetAngleMeasureVector()
        {
            if (_tableObject.ComponentType == ComponentType.OpticalComponent)
                if ( ((OpticalComponent.OpticalComponent) _tableObject).OpticalCategory == OpticalCategory.Mirror )
                    if (((OpticalComponent.Mirror)_tableObject).flipped)
                        return transform.right;
            
            return -transform.right;
        }

        // bool IsMirror()
        // {
        //     return _tableObject.ComponentType == ComponentType.OpticalComponent &&
        //            ((OpticalComponent.OpticalComponent)_tableObject).OpticalType == OpticalType.Mirror;
        // }

    }
}
