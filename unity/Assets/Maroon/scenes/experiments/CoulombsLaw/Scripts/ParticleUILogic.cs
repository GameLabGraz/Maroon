using GEAR.Localization.Text;
using Maroon.GlobalEntities;
using Maroon.Physics.CoordinateSystem;
using PlatformControls.PC;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.Experiments.CoulombsLaw
{
    public class ParticleUILogic : MonoBehaviour 
    {
        // Selection Data
        private ChargedParticle selectedParticle = null;
        [SerializeField] private ChargedParticle particlePrefab = null;
        [SerializeField] private Transform _particleParentObject = null;

        // UI-Element References
        [SerializeField] private PC_InputParser_Float_TMP _textfieldPosX; 
        [SerializeField] private PC_InputParser_Float_TMP _textfieldPosY;
        [SerializeField] private PC_InputParser_Float_TMP _textfieldPosZ;

        [SerializeField] private PC_Slider _electricChargeSlider;
        [SerializeField] private PC_InputParser_Float_TMP _electricChargeTextField;
        [SerializeField] private Button _buttonAddDeleteParticle;
        [SerializeField] private LocalizedTMP _buttonAddDeleteParticleText;
        [SerializeField] private Toggle _fixPositionToggle;

        private void Start()
        {
            UpdateAddDeleteButtonText();

            float max_charge = ChargedParticle.MAX_ABSOLUTE_CHARGE * 1e6f; // Units in UI are in micro-Coulomb 
            _electricChargeSlider.maxValue = max_charge;
            _electricChargeSlider.minValue = -max_charge;
            _electricChargeTextField.maximum = max_charge; 
            _electricChargeTextField.minimum = -max_charge;
            _fixPositionToggle.isOn = false;

            _fixPositionToggle.onValueChanged.AddListener((fixPosition) =>
            {
                if (selectedParticle != null) selectedParticle.SetFixPosition(fixPosition);
            });

            _textfieldPosX?.onValueChangedFloat.AddListener((endVal) =>
            {
                MoveToNewPosition(new Vector3(endVal, _textfieldPosY.GetValue(), _textfieldPosZ.GetValue()));
            });

            _textfieldPosY?.onValueChangedFloat.AddListener((endVal) =>
            {
                MoveToNewPosition(new Vector3(_textfieldPosY.GetValue(), endVal , _textfieldPosY.GetValue()));
            });

            _textfieldPosZ?.onValueChangedFloat.AddListener((endVal) =>
            {
                MoveToNewPosition(new Vector3(_textfieldPosZ.GetValue(), _textfieldPosZ.GetValue(), endVal));
            });

            _electricChargeSlider.onValueChanged.AddListener((newValue) =>
            {
                if (selectedParticle == null) return;
                selectedParticle.electricCharge = newValue;
                selectedParticle.UpdateParticleColor();
            });

            _buttonAddDeleteParticle.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (selectedParticle != null)
                {
                    GameObject.Destroy(selectedParticle.gameObject);
                    selectedParticle = null;
                }
                else
                {
                    var pos = new Vector3(_textfieldPosX.GetValue(), _textfieldPosY.GetValue(), _textfieldPosZ.GetValue());
                    pos = CoordSystemHandler.Instance.GetWorldPosition(pos);
                    var particle = GameObject.Instantiate(particlePrefab, pos, Quaternion.identity, _particleParentObject);
                    particle.electricCharge = _electricChargeSlider.value * 1e-6f; // Slider shows Value in micro-coulomb, and particle stores coulomb
                    particle.SetFixPosition(_fixPositionToggle.isOn);
                    particle.UpdateParticleColor();
                }
                UpdateAddDeleteButtonText();
            });
        }

        // Update checks if we clicked on a charged particle (Raycasts the scene), and updates the X/Y/Z labels if position has changed
        private void Update()
        {
            // Check if we selected/deselected a Particle
            if (Input.GetMouseButtonDown(0))
            {
                // Check if we clicked on particle, and update selection if we did
                Camera cam = Camera.main;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                bool raycastHitParticle = false;
                if (UnityEngine.Physics.Raycast(ray, out hitInfo))
                {
                    var hitObject = hitInfo.collider.gameObject;
                    var clickedParticle = hitObject.GetComponent<ChargedParticle>();
                    if (clickedParticle != null)
                    {
                        raycastHitParticle = true;
                        selectedParticle = clickedParticle;
                    }
                }

                // Find out if we clicked on UI (Don't deselect the selected charge if we clicked on UI)
                bool clickedOnUIElement = false;
                {
                    // Note(MartinR): There may be a better way to do this, but for now we raycast the UI to check if we hit anything
                    var eventSystem = UnityEngine.EventSystems.EventSystem.current;
                    var eventData = new UnityEngine.EventSystems.PointerEventData(eventSystem);
                    eventData.position = Input.mousePosition;
                    var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
                    eventSystem.RaycastAll(eventData, results);
                    if (results.Count > 0)
                    {
                        clickedOnUIElement = true;
                    }
                }

                // Deselect current particle if we clicked on something else that wasn't UI
                if (!raycastHitParticle && !clickedOnUIElement)
                {
                    selectedParticle = null;
                }
            }

            // Update UI elements based on current selection
            UpdatePositionText();
            UpdateAddDeleteButtonText();
            if (selectedParticle != null)
            {
                _electricChargeSlider.value = selectedParticle.electricCharge * 1e6f;
                _fixPositionToggle.isOn = selectedParticle.GetFixPosition();
            }
        }

        private void UpdateAddDeleteButtonText()
        {
            _buttonAddDeleteParticleText.Key = selectedParticle == null ? "Add Charge" : "Delete Charge";
        }

        private void UpdatePositionText()
        {
            if (selectedParticle == null) return;

            var systemPos = CoordSystemHandler.Instance.GetSystemPosition(selectedParticle.transform.position, Unit.m);
            _textfieldPosX.SetValue(systemPos.x);
            _textfieldPosY.SetValue(systemPos.y);
            _textfieldPosZ.SetValue(systemPos.z);
        }

        private void MoveToNewPosition(Vector3 transform)
        {
            if (selectedParticle == null) return;
            selectedParticle.transform.position = CoordSystemHandler.Instance.GetWorldPosition(transform);
        }
    }
}
