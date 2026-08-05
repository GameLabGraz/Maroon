using Maroon.Physics.Optics.TableObject.Handlers;
using Maroon.Physics.Optics.TableObject.OpticalComponent;
using Maroon.Physics.Optics.Util;
using UnityEngine;

namespace Maroon.Physics.Optics.Manager
{
    public class ExperimentManager : MonoBehaviour
    {
        public static ExperimentManager Instance;
        public bool mouseOnUIPanel { get; set; }
        
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
                Debug.LogError("SHOULD NOT OCCUR - Destroyed ExperimentManager");
                Destroy(gameObject);
            }
        }

        // Main Update loop
        private void Update()
        {
            if (mouseOnUIPanel && CanRaycast())
                return;
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
                
                
                if (lc.transform.hasChanged)
                {
                    lc.Origin = lc.transform.localPosition;
                    UIManager.Instance.StoreCurrentPosRot(lc);
                    lc.transform.hasChanged = false;
                }
            }

            // Optical Component Branch
            if (UIManager.Instance.SelectedOc != null)
            {
                var oc = UIManager.Instance.SelectedOc;
                
                if (oc.transform.hasChanged)
                {
                    oc.UpdateProperties();
                    UIManager.Instance.StoreCurrentPosRot(oc);
                    oc.transform.hasChanged = false;
                }
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

        // Removes all OCs and LCs from the table
        public void ClearTable()
        {
            UIManager uim = UIManager.Instance;
            uim.SelectedLc = null;
            uim.SelectedOc = null;
            uim.rayThickness.Value = Constants.BaseRayThicknessInMM;

            foreach (var lc in LightComponentManager.Instance.LightComponents)
                lc.RemoveFromTable();
            LightComponentManager.Instance.LightComponents.Clear();
            uim.DeactivateAllLightControlPanels();

            foreach (var oc in OpticalComponentManager.Instance.OpticalComponents)
                    oc.RemoveFromTable();

            OpticalComponentManager.Instance.OpticalComponents.RemoveAll(oc => oc.OpticalCategory != OpticalCategory.Wall);
            uim.DeactivateAllOpticalControlPanels();
        }

    }
}
