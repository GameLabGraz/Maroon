using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.Handlers;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Util;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class ExperimentManager : MonoBehaviour
    {
        public static ExperimentManager Instance;
        
        private UnityEngine.Camera _cam;
        private Ray _mouseRay;
        private RaycastHit _hit;

        private Transform _currentHit;
        private bool _isHovered;
        private bool _isDragging;
        private bool _isTranslating;
        private bool _isYRotating;
        private bool _isZRotating;
        
        private void Awake()
        {
            _cam = UnityEngine.Camera.main;
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("SHOULD NOT OCCUR - Destroyed LightSourceManager");
                Destroy(gameObject);
            }
        }
        // Main Update loop
        private void Update()
        {
            // Handle Table object selection, dragging, y-translation, y-rotation, z-rotation
            _mouseRay = _cam.ScreenPointToRay(Input.mousePosition);
            RayCollisionLogic();
            DraggingLogic();
            TranslationLogic();
            YRotationLogic();
            ZRotationLogic();
            
            // Light Source Branch
            if (UIManager.Instance.SelectedLc != null)
            {
                var lc = UIManager.Instance.SelectedLc;
                
                lc.ChangeWavelengthAndIntensity(
                    UIManager.Instance.selectedWavelength.Value, 
                    UIManager.Instance.selectedIntensity.Value);
                
                if (lc.transform.hasChanged)
                {
                    lc.Origin = lc.transform.localPosition;
                    lc.RecalculateLightRoute();
                    lc.transform.hasChanged = false;
                }
            }

            // Optical Component Branch
            if (UIManager.Instance.SelectedOc != null)
            {
                var oc = UIManager.Instance.SelectedOc;
                UIManager.Instance.UpdateOpticalComponentValues();
                
                if (oc.transform.hasChanged)
                {
                    oc.UpdateProperties();
                    oc.transform.hasChanged = false;
                }
                LightComponentManager.Instance.CheckOpticalComponentHit(oc);
            }
        }

        private bool CanRaycast()
        {
            return !_isDragging && !_isTranslating && !_isYRotating && !_isZRotating;
        }

        private void RayCollisionLogic()
        {
            if (CanRaycast() && UnityEngine.Physics.Raycast(_mouseRay, out _hit, Mathf.Infinity, Constants.TableObjectLayer))
            {
                ColliderLogic();
            }
            else if (_isHovered)
            {
                _currentHit.parent.GetComponent<SelectionHandler>().OnColliderMouseExit();
                _isHovered = false;
            }
        }
        
        private void ColliderLogic()
        {
            if (_hit.collider == null)
                return;
            
            _currentHit = _hit.transform;
            var translationRotationHandler = _currentHit.parent.GetComponent<TranslationRotationHandler>();
                
            switch (_hit.collider.tag)
            {
                case Constants.TagTranslationArrowY:
                    translationRotationHandler.DoTranslation(_mouseRay, _hit.point);
                    _isTranslating = true;
                    break;
                
                case Constants.TagRotationArrowY:
                    translationRotationHandler.DoYRotation(_mouseRay, _hit.point);
                    _isYRotating = true;
                    break;
                
                case Constants.TagRotationArrowZ:
                    translationRotationHandler.DoZRotation(_mouseRay, _hit.point);
                    _isZRotating = true;
                    break;
                
                default:
                    _isHovered = true;
                    var selectionHandler = _currentHit.parent.GetComponent<SelectionHandler>();
                    selectionHandler.OnColliderMouseEnter();
                    
                    if (Input.GetMouseButtonDown(0))
                    {
                        _isDragging = true;
                        selectionHandler.OnColliderMouseDown();
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        selectionHandler.OnColliderMouseUp();
                    }
                    break;
            }
        }

        private void DraggingLogic()
        {
            if (_isDragging && Input.GetMouseButton(0))
                _currentHit.parent.GetComponent<SelectionHandler>().OnColliderMouseDrag();
            else
                _isDragging = false;
        }

        private void TranslationLogic()
        {
            if (_isTranslating && Input.GetMouseButton(0))
                _currentHit.parent.GetComponent<TranslationRotationHandler>().DoTranslation(_mouseRay, _hit.point);
            else
                _isTranslating = false;
        }

        private void YRotationLogic()
        {
            if (_isYRotating && Input.GetMouseButton(0))
                _currentHit.parent.GetComponent<TranslationRotationHandler>().DoYRotation(_mouseRay, _hit.point);
            else
                _isYRotating = false;
        }
        
        private void ZRotationLogic()
        {
            if (_isZRotating && Input.GetMouseButton(0))
                _currentHit.parent.GetComponent<TranslationRotationHandler>().DoZRotation(_mouseRay, _hit.point);
            else
                _isZRotating = false;
        }
        
    }
}
