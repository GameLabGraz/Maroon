using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Util;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.Handlers
{
    public class SelectionMovementHandler : MonoBehaviour
    {
        [SerializeField] private GameObject component;
        [SerializeField] private GameObject arrowUp;
        
        [Header("Colors")] 
        [SerializeField] private Color standard;
        [SerializeField] private Color selected;
        [SerializeField] private Color hover;
        [SerializeField] private Color dragging;
        
        private Material _material;
        private bool _isSelected;
        private ComponentType _componentType;
        
        private Collider _collider;
        private UnityEngine.Camera _cam;
        private Vector3 _objectToMouseTablePosOffset;
        private Vector3 _objectToMouseTablePosLocalOffset;
        private Plane _movementPlane;

        private void Awake()
        {
            _componentType = GetComponent<TableObject>().ComponentType;
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
                UIManager.Instance.ActivateOpticalControlPanel((OpticalComponent.OpticalComponent)GetComponent<TableObject>());
            }
            else
            {
                LightComponentManager.Instance.UnselectAll();
                UIManager.Instance.SelectLightComponent((LightComponent.LightComponent)GetComponent<TableObject>());
            }
                
            _material.color = selected;
            _isSelected = true;
            
            // Set the movement plain to the current world space mouse position (somewhere on the table object)
            Ray camMouseRay = _cam.ScreenPointToRay(Input.mousePosition);
            _collider.Raycast(camMouseRay, out var hit, Mathf.Infinity);
            _movementPlane.SetNormalAndPosition(Vector3.up, hit.point);

            _objectToMouseTablePosOffset = hit.point - transform.position;
            _objectToMouseTablePosLocalOffset = hit.point - transform.localPosition;
        }

        public void OnColliderMouseUp()
        {
            if (_componentType != ComponentType.Wall)
            {
                _material.color = selected;
                _isSelected = true;
            }
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
            if (CheckTableBounds(desiredPosLocal))
                transform.position = desiredPos;
        }

        private bool CheckTableBounds(Vector3 desiredPos)
        {
            return !(desiredPos.x < Constants.MinPositionTable.x) &&
                   !(desiredPos.z < Constants.MinPositionTable.z) &&
                   !(desiredPos.x > Constants.MaxPositionTable.x) &&
                   !(desiredPos.z > Constants.MaxPositionTable.z);
        }

        public void Unselect()
        {
            _isSelected = false;
            if (_componentType != ComponentType.Wall)
                _material.color = standard;
        }

    }
}