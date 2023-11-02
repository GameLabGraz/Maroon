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
            // Check if mouse over TableObject 
            _mouseRay = _cam.ScreenPointToRay(Input.mousePosition);
            if (!_isDragging && UnityEngine.Physics.Raycast(_mouseRay, out _hit, Mathf.Infinity, Constants.TableObjectLayer))
            {
                if (_hit.collider != null)
                {
                    _isHovered = true;
                    _currentHit = _hit.transform;
                    _currentHit.parent.GetComponent<SelectionMovementHandler>().OnColliderMouseEnter();
                
                    if (Input.GetMouseButtonDown(0))
                    {
                        Debug.Log("DOWN " + Time.time);
                        _isDragging = true;
                        _currentHit.parent.GetComponent<SelectionMovementHandler>().OnColliderMouseDown();
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        Debug.Log("UP " + Time.time);
                        _currentHit.parent.GetComponent<SelectionMovementHandler>().OnColliderMouseUp();
                    }
                }
            }
            else if (_isHovered)
            {
                _currentHit.parent.GetComponent<SelectionMovementHandler>().OnColliderMouseExit();
                _isHovered = false;
            }

            if (_isDragging && Input.GetMouseButton(0))
                _currentHit.parent.GetComponent<SelectionMovementHandler>().OnColliderMouseDrag();
            else
                _isDragging = false;
            
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
    }
}
