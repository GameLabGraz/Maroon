using Maroon.Physics.Optics.Manager;
using UnityEngine;

namespace Maroon.Physics.Optics.TableObject.Handlers
{
    public class SelectionHandler : MonoBehaviour
    {
        [Header("Colors")] 
        [SerializeField] private Color standard;
        [SerializeField] private Color selected;
        [SerializeField] private Color hover;
        [SerializeField] private Color dragging;
        
        private Material _material;
        private bool _isSelected;

        private TableObject _tableObject;
        private ComponentType _componentType;
        
        private Collider _collider;
        private UnityEngine.Camera _cam;
        private Vector3 _objectToMouseTablePosOffset;
        private Vector3 _objectToMouseTablePosLocalOffset;
        private Plane _movementPlane;

        private void Awake()
        {
            _tableObject = GetComponent<TableObject>();
            _componentType = _tableObject.ComponentType;
            _cam = UnityEngine.Camera.main;
            if (_componentType != ComponentType.Wall)
            {
                _material = transform.GetComponentInChildren<Renderer>().material;
                _material.color = standard;
            }
            
            _collider = GetComponentInChildren<Collider>();
            if (_collider == null)
                Debug.LogError($"Collider of TableObject {this.name} missing!");

            _movementPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        }

        
        public void OnColliderMouseEnter()
        {
            if (!_isSelected && _componentType != ComponentType.Wall)
                _material.color = hover;
        }
        
        public void OnColliderMouseExit()
        {
            if (!_isSelected && _componentType != ComponentType.Wall)
                _material.color = standard;
        }

        public void OnColliderMouseDown()
        {
            if (_componentType == ComponentType.Wall)
                return;

            if (_componentType == ComponentType.OpticalComponent)
            {
                OpticalComponentManager.Instance.UnselectAll();
                UIManager.Instance.ActivateOpticalControlPanel(GetComponent<OpticalComponent.OpticalComponent>());
            }
            else
            {
                LightComponentManager.Instance.UnselectAll();
                UIManager.Instance.ActivateLightControlPanel(GetComponent<LightComponent.LightComponent>());
            }
                
            Select();
            
            // Set the movement plain to the current world space mouse position (somewhere on the table object)
            Ray camMouseRay = _cam.ScreenPointToRay(Input.mousePosition);
            _collider.Raycast(camMouseRay, out var hit, Mathf.Infinity);
            _movementPlane.SetNormalAndPosition(Vector3.up, hit.point);

            _objectToMouseTablePosOffset = hit.point - transform.position;
            _objectToMouseTablePosLocalOffset = hit.point - transform.localPosition;
        }

        public void OnColliderMouseUp()
        {
            Select();
        }

        public void OnColliderMouseDrag()
        {
            if (_componentType == ComponentType.Wall)
                return;
            
            _material.color = dragging;

            // Next 3 lines taken from old DragLaserObject.cs implementation
            Ray camPlaneRay = _cam.ScreenPointToRay(Input.mousePosition);
            _movementPlane.Raycast(camPlaneRay, out var dist);
            Vector3 pointOnPlane = camPlaneRay.GetPoint(dist);
            
            Vector3 desiredPos = pointOnPlane - _objectToMouseTablePosOffset;
            Vector3 desiredPosLocal = pointOnPlane - _objectToMouseTablePosLocalOffset;
            if (Util.Math.CheckTableBounds(desiredPosLocal))
                transform.position = desiredPos;
        }

        public void Select()
        {
            if (_componentType != ComponentType.Wall)
            {
                _material.color = selected;
                _isSelected = true;
                
                _tableObject.SetArrowsActive(true);
                
            }
        }

        public void Unselect()
        {
            if (_componentType != ComponentType.Wall)
            {
                _material.color = standard;
                _isSelected = false;
                _tableObject.SetArrowsActive(false);
            }
        }

    }
}