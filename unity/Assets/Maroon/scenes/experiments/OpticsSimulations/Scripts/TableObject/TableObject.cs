//
//Author: Alexander Kassil
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject
{
    public class TableObject : MonoBehaviour
    {
        [Header("GameObject Colors")] 
        [SerializeField] private Color standard;
        [SerializeField] private Color selected;
        [SerializeField] private Color hover;
        [SerializeField] private Color dragging;

        private Material _material;
        private bool _isSelected;
        private bool _isDragged;

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

        private void Start()
        {
            _material = GetComponent<Renderer>().material;
            _material.color = standard;
        }

        private void OnMouseEnter()
        {
            if (!_isSelected)
                _material.color = hover;
        }

        private void OnMouseDown()
        {
            OpticalComponentManager.Instance.UnselectAll();
            _material.color = selected;
            _isSelected = true;
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
        }
        
        private void OnMouseExit()
        {
            if (!_isSelected)
                _material.color = standard;
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
}
