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
        private ChargedParticle selectedParticle = null;

        [Header("References to Scene-Objects")] 
        [SerializeField] private ParticleController _particleController = null;
        [SerializeField] private Transform _minBoundary = null;
        [SerializeField] private Transform _maxBoundary = null;

        [Header("Selection-Highlight")] 
        [SerializeField] private GameObject _selectionHighlightMarker = null;
        [SerializeField] private Color _selectionColor;
        [SerializeField] private Color _outOfBoundsSelectionColor;
        [Range(1.0f, 1.5f)] [SerializeField] private float _selectionScaleMultiplier = 1.2f;

        [Header("UI-Element References")] 
        [SerializeField] private PC_InputParser_Float_TMP _textfieldPosX; 
        [SerializeField] private PC_InputParser_Float_TMP _textfieldPosY;
        [SerializeField] private PC_InputParser_Float_TMP _textfieldPosZ;

        [SerializeField] private PC_Slider _electricChargeSlider;
        [SerializeField] private PC_InputParser_Float_TMP _electricChargeTextField;
        [SerializeField] private Button _buttonAddDeleteParticle;
        [SerializeField] private LocalizedTMP _buttonAddDeleteParticleText;
        [SerializeField] private Toggle _fixPositionToggle;
        [SerializeField] private UIItemDragHandlerSimple _uiParticleDragHandler;

        [Header("Particle Preview Images")]
        [SerializeField] private Image BackgroundImage;
        [SerializeField] private Image MinusImage;
        [SerializeField] private Image PlusImage;

        private void UpdateParticleCoordinate(float value, int dimension)
        {
            if (selectedParticle == null) return;
            var pos = CoordSystemHandler.Instance.GetSystemPosition(selectedParticle.transform.position, Unit.m);
            pos[dimension] = value;
            pos = CoordSystem.Instance.GetPositionInWorldSpace(pos);

            // Note(MartinR): Just setting transform.position causes problems when Physics interpolation is enabled.
            //      _rigidBody.MovePosition also does not seem to do the trick, I guess because it is expected to be called during FixedUpdate?
            var rigidBody = selectedParticle.GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                rigidBody.position = pos;
            }
            else
            {
                rigidBody.transform.position = pos;
            }
        }

        private void UpdateChargeValue(float newValue)
        {
            // Update Text-Field
            float textFieldValue = _electricChargeTextField.GetValue();
            if (textFieldValue != newValue)
            {
                _electricChargeTextField.SetValue(newValue);
            }

            // Update Slider
            if (_electricChargeSlider.value != newValue)
            {
                _electricChargeSlider.value = newValue;
            }

            // Update selected particle
            if (selectedParticle != null)
            {
                selectedParticle.SetCharge(newValue * 1e-6f);
            }

            // Update Particle preview
            bool positive = newValue >= 0;
            PlusImage.gameObject.SetActive(positive);
            MinusImage.gameObject.SetActive(!positive);
            BackgroundImage.color = ChargedParticle.ChargeValueToColor(newValue * 1e-6f);
        }

        // Initialize UI values and set up callbacks for UI-Interactions
        // Note(MartinR): I would like to do the initialization in the Awake method, but
        //      using the CoordSystem in Awake axis-controller causes an exception in the WebGL build, so now the initialization
        //      is done on Start(), I assume it has something to do with the initialization order the order in which Awake is called on Objects...
        private void Start()
        {
            // Initialize UI-Element limits
            float max_charge = ChargedParticle.MAX_ABSOLUTE_CHARGE * 1e6f; // Units in UI are in micro-Coulomb 
            _electricChargeSlider.maxValue = max_charge;
            _electricChargeSlider.minValue = -max_charge;
            _electricChargeTextField.maximum = max_charge;
            _electricChargeTextField.minimum = -max_charge;

            Vector3 min = CoordSystem.Instance.GetPositionInAxisUnits(_minBoundary.transform.position, Unit.m);
            Vector3 max = CoordSystem.Instance.GetPositionInAxisUnits(_maxBoundary.transform.position, Unit.m);

            _textfieldPosX.minimum = min.x;
            _textfieldPosX.maximum = max.x;
            _textfieldPosY.minimum = min.y;
            _textfieldPosY.maximum = max.y;
            _textfieldPosZ.minimum = min.z;
            _textfieldPosZ.maximum = max.z;

            // Set initial values
            _textfieldPosX.SetValue(1.0f);
            _textfieldPosY.SetValue(1.0f);
            _textfieldPosZ.SetValue(1.0f);
            _fixPositionToggle.isOn = false;
            UpdateChargeValue(0.0f);

            // Add callbacks/logic to UI-Elements
            _fixPositionToggle.onValueChanged.AddListener((fixPosition) =>
            {
                if (selectedParticle != null) selectedParticle.SetPositionLocked(fixPosition);
            });

            _textfieldPosX?.onValueChangedFloat.AddListener((value) =>
            {
                UpdateParticleCoordinate(value, 0);
            });

            _textfieldPosY?.onValueChangedFloat.AddListener((value) =>
            {
                UpdateParticleCoordinate(value, 1);
            });

            _textfieldPosZ?.onValueChangedFloat.AddListener((value) =>
            {
                UpdateParticleCoordinate(value, 2);
            });

            _electricChargeSlider.onValueChanged.AddListener((newValue) =>
            {
                UpdateChargeValue(newValue);
            });

            _electricChargeTextField.onValueChangedFloat.AddListener((newValue) =>
            {
                UpdateChargeValue(newValue);
            });

            _buttonAddDeleteParticle.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (selectedParticle == null)
                {
                    var inputPos = new Vector3(_textfieldPosX.GetValue(), _textfieldPosY.GetValue(), _textfieldPosZ.GetValue());
                    var worldPos = CoordSystem.Instance.GetPositionInWorldSpace(inputPos);
                    // Note(MartinR): Slider shows Value in micro-coulomb, and particle stores charge in Coulomb
                    _particleController.CreateChargedParticle(worldPos, _electricChargeSlider.value * 1e-6f, _fixPositionToggle.isOn);
                }
                else
                {
                    _particleController.RemoveChargedParticle(selectedParticle);
                    selectedParticle = null;
                }
            });

            _uiParticleDragHandler.SetBoundaries(_minBoundary, _maxBoundary);
            _uiParticleDragHandler.OnDragFinished.AddListener((Vector3 position) =>
            {
                _particleController.CreateChargedParticle(position, _electricChargeSlider.value, _fixPositionToggle.isOn);
            });
        }

        // Update checks if we clicked on a charged particle (Raycasts the scene), and updates the X/Y/Z labels if position has changed
        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;

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

        // UI-text is updated every frame, as things like particle-positions can change every frame anyway, so setting up
        //      a event-system with listeners isn't usefull
        private void LateUpdate()
        {
            // Update Particle Highlight-Marker
            _selectionHighlightMarker.SetActive(selectedParticle != null);
            if (selectedParticle != null)
            {
                Vector3 pos = selectedParticle.transform.position;
                Vector3 max = Vector3.Max(_minBoundary.position, _maxBoundary.position);
                Vector3 min = Vector3.Min(_minBoundary.position, _maxBoundary.position);
                bool outsideBounds = pos.x < min.x || pos.y < min.y || pos.x > max.x || pos.y > max.y;
                _selectionHighlightMarker.GetComponent<SpriteRenderer>().color = outsideBounds ? _outOfBoundsSelectionColor : _selectionColor;

                float scale = ChargedParticle.RADIUS * _selectionScaleMultiplier * 2;
                _selectionHighlightMarker.transform.localScale = new Vector3(scale, scale, scale);
                _selectionHighlightMarker.transform.position = selectedParticle.transform.position;
            }

            // Update UI-Element Text
            _buttonAddDeleteParticleText.Key = selectedParticle == null ? "Button_Add" : "Button_Delete";
            if (selectedParticle != null)
            {
                var systemPos = CoordSystemHandler.Instance.GetSystemPosition(selectedParticle.transform.position, Unit.m);
                _textfieldPosX.SetValue(systemPos.x);
                _textfieldPosY.SetValue(systemPos.y);
                _textfieldPosZ.SetValue(systemPos.z);

                _electricChargeSlider.value = selectedParticle.GetCharge() * 1e6f;
                _fixPositionToggle.isOn = selectedParticle.GetPositionLocked();
            }
        }
    }
}
