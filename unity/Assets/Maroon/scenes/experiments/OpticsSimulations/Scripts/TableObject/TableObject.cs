using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager;
using UnityEngine;


namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject
{
    public class TableObject : MonoBehaviour
    {
        [SerializeField] private Type type;
        
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
        private Vector3 _tableOffset;
        private Vector3 _tableOffsetLocal;

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
            _material = transform.GetComponent<Renderer>().material;
            _material.color = standard;
            
            
            _collider = GetComponent<Collider>();
            if (_collider == null)
                Debug.LogError($"Collider of TableObject {this.name} missing!");
        }

        // private void Start()
        // {
        //     _collider = GetComponent<Collider>();
        //     if (_collider == null)
        //         Debug.LogError($"Collider of TableObject {this.name} missing!");
        // }

        private void OnMouseEnter()
        {
            if (!_isSelected)
                _material.color = hover;
        }

        private void OnMouseDown()
        {
            if (type == Type.OpticalComponent)
                OpticalComponentManager.Instance.UnselectAll();
            else
                LightSourceManager.Instance.UnselectAll();
                
            _material.color = selected;
            _isSelected = true;
            
            _tableOffset = transform.position - GetMousePlainPosition();
            _tableOffsetLocal = transform.localPosition - GetMousePlainPosition();
        }

        private void OnMouseUp()
        {
            if (_isDragged)
            {
                _material.color = selected;
                _isDragged = false;
                _isSelected = true;
            }
        }

        private void OnMouseDrag()
        {
            _material.color = dragging;
            _isDragged = true;

            Vector3 mousePos = GetMousePlainPosition();
            Vector3 desiredPos = mousePos + _tableOffset;
            Vector3 desiredPosLocal = mousePos + _tableOffsetLocal;
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

        private void OnMouseExit()
        {
            if (!_isSelected)
                _material.color = standard;
        }


        private void DraggingHandler()
        {
            
        }
        
        private Vector3 GetMousePlainPosition()
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (UnityEngine.Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << Constants.MouseColliderMaskIndex))
                return hit.point;
            
            Debug.LogError("Mouse did not hit plane. Should not occur!");
            return new Vector3(0,0,0);
        }

        public void Unselect()
        {
            _isSelected = false;
            _material.color = standard;
        }

        private void SelectionHandler()
        {
            
        }
        
        private void RotationHandler()
        {
            
        }
        
        private void TranslationHandler()
        {
            
        }

    }
    
    public enum Type
    {
        LightSource = 0,
        OpticalComponent = 1
    }
}
