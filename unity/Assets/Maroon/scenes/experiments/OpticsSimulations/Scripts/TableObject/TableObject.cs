using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager;
using UnityEngine;


namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject
{
    public class TableObject : MonoBehaviour
    {
        [SerializeField] private ComponentType componentType;
        
        [Header("Colors")] 
        [SerializeField] private Color standard;
        [SerializeField] private Color selected;
        [SerializeField] private Color hover;
        [SerializeField] private Color dragging;

        private Material _material;
        private bool _isSelected;
        private bool _isDragged;

        private Collider _collider;
        private Camera _cam;
        private Vector3 _objectToMouseTablePosOffset;
        private Vector3 _objectToMouseTablePosLocalOffset;
        private Plane _movementPlane;

        public ComponentType ComponentType => ComponentType;
        public Color Standard
        {
            get => standard;
            set => standard = value;
        }
        public Color Selected
        {
            get => selected;
            set => selected = value;
        }
        public Color Hover
        {
            get => hover;
            set => hover = value;
        }
        public Color Dragging
        {
            get => dragging;
            set => dragging = value;
        }

        private void Awake()
        {
            _cam = Camera.main;
            if (componentType != ComponentType.Wall)
            {
                _material = transform.GetComponent<Renderer>().material;
                _material.color = standard;
            }
            
            _collider = GetComponent<Collider>();
            if (_collider == null)
                Debug.LogError($"Collider of TableObject {this.name} missing!");

            _movementPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        }

        private void OnMouseEnter()
        {
            if (!_isSelected && componentType != ComponentType.Wall)
                _material.color = hover;
        }

        private void OnMouseDown()
        {
            if (componentType == ComponentType.Wall)
                return;
            
            if (componentType == ComponentType.OpticalComponent)
                OpticalComponentManager.Instance.UnselectAll();
            else
                LightComponentManager.Instance.UnselectAll();
                
            _material.color = selected;
            _isSelected = true;
            
            // Set the movement plain to the current world space mouse position (somewhere on the table object)
            Ray camMouseRay = _cam.ScreenPointToRay(Input.mousePosition);
            _collider.Raycast(camMouseRay, out var hit, Mathf.Infinity);
            _movementPlane.SetNormalAndPosition(Vector3.up, hit.point);

            _objectToMouseTablePosOffset = hit.point - transform.position;
            _objectToMouseTablePosLocalOffset = hit.point - transform.localPosition;
        }

        private void OnMouseUp()
        {
            if (_isDragged && componentType != ComponentType.Wall)
            {
                _material.color = selected;
                _isDragged = false;
                _isSelected = true;
            }
        }

        private void OnMouseDrag()
        {
            if (componentType == ComponentType.Wall)
                return;
            
            _material.color = dragging;
            _isDragged = true;
            
            // Next 3 lines taken from old DragLaserObject.cs implementation
            Ray camPlaneRay = _cam.ScreenPointToRay(Input.mousePosition);
            _movementPlane.Raycast(camPlaneRay, out var dist);
            Vector3 pointOnPlane = camPlaneRay.GetPoint(dist);
            
            Vector3 desiredPos = pointOnPlane - _objectToMouseTablePosOffset;
            Vector3 desiredPosLocal = pointOnPlane - _objectToMouseTablePosLocalOffset;
            if (CheckTableBounds(desiredPosLocal))
                transform.position = desiredPos;        // TODO create updatePosition virtual functions
        }

        private bool CheckTableBounds(Vector3 desiredPos)
        {
            return !(desiredPos.x < Constants.MinPositionTable.x) &&
                   !(desiredPos.z < Constants.MinPositionTable.z) &&
                   !(desiredPos.x > Constants.MaxPositionTable.x) &&
                   !(desiredPos.z > Constants.MaxPositionTable.z);
        }

        private void OnMouseExit()
        {
            if (!_isSelected && componentType != ComponentType.Wall)
                _material.color = standard;
        }

        public void Unselect()
        {
            _isSelected = false;
            if (componentType != ComponentType.Wall)
                _material.color = standard;
        }

        private void RotationHandler()
        {
            
        }
        
        private void TranslationHandler()
        {
            
        }

    }
    
    public enum ComponentType
    {
        LightSource = 0,
        OpticalComponent = 1,
        Wall = 2,
    }
}
