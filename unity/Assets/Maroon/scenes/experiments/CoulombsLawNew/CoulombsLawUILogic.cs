using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public struct SimulationSettings
    {
        public float bounciness;
        public float drag;
        public float friction;
        public bool boundaryEnabled;
    }

    public class CoulombsLawUILogic : MonoBehaviour
    {
        private Configuration configurationOnSimulationStart;

        [SerializeField] private ChargedPoint prefabChargedPoint;
        [SerializeField] private ChargedRod prefabChargedRod;
        [SerializeField] private ChargedPlane prefabChargedPlane;
        [SerializeField] private Transform parentForNewObjects;

        [SerializeField] private VisualizationPlaneLogic visualizationPlane;
        [SerializeField] private PhysicMaterial physicMaterial;
        [SerializeField] private GameObject particleSimulationBoundary;

        private SimulationSettings simulationSettings;
        private SimulationSettings settingsAtSimulationStart;

        [Header("UI-Element References")]
        [SerializeField] private GUIBoolInputLogic vectorFieldEnabledToggle;
        [SerializeField] private GUIBoolInputLogic isoSurfaceEnabledToggle;

        [SerializeField] private GuiIconTo3DObjectDrag dragIconParticle;
        [SerializeField] private GuiIconTo3DObjectDrag dragIconChargedRod;
        [SerializeField] private GuiIconTo3DObjectDrag dragIconChargedPlane;
        [SerializeField] private GuiIconTo3DObjectDrag dragIconPlaneVisualization;

        [SerializeField] private GUIBoolInputLogic  uiCamera3DModeToggle;
        [SerializeField] private GUIFloatInputLogic uiCameraSensitivitySlider;
        [SerializeField] private GUIBoolInputLogic  uiCameraVSynchToggle;
        [SerializeField] private GUIIntInputLogic   uiCameraMaxFramerateInput;

        [SerializeField] private GUIBoolInputLogic  uiEnableBoundaryToggle;
        [SerializeField] private GUIFloatInputLogic uiDragSlider;
        [SerializeField] private GUIFloatInputLogic uiBouncinessSlider;
        [SerializeField] private GUIFloatInputLogic uiFrictionSlider;

        // Note(MartinR): I'm overwritting the normal Maroon button behavior in Awake
        //      so that all UI-elements in the whole scene are handled in a uniform manner
        [SerializeField] private UnityEngine.UI.Button simulationStartButton;
        [SerializeField] private UnityEngine.UI.Button simulationPauseButton;
        [SerializeField] private UnityEngine.UI.Button simulationResetButton;

        private void Awake()
        {
            // Setup for DraggableIcon logic
            dragIconParticle.OnDragFinished.AddListener((Vector3 pos) =>
            {
                var newParticle = GameObject.Instantiate(prefabChargedPoint, pos, Quaternion.identity, parentForNewObjects);
                newParticle.SetCharge(0.001f * 1e-6f); // HACK so that visualization plane doesn't have 0 equipotential surface everywhere
                newParticle.GetComponent<SphereCollider>().sharedMaterial = physicMaterial;
            });
            dragIconChargedRod.OnDragFinished.AddListener((Vector3 pos) =>
            {
                var newRod = GameObject.Instantiate(prefabChargedRod, pos, Quaternion.identity, parentForNewObjects);
                newRod.SetRodParameters(pos, Vector3.forward);
            });
            dragIconChargedPlane.OnDragFinished.AddListener((Vector3 pos) =>
            {
                var newPlane = GameObject.Instantiate(prefabChargedPlane, pos, Quaternion.identity, parentForNewObjects);
                newPlane.SetPlaneParameters(pos, Vector3.right);
            });
            dragIconPlaneVisualization.OnDragFinished.AddListener((Vector3 pos) =>
            {
                bool isActive = visualizationPlane.gameObject.activeSelf;
                visualizationPlane.position = pos;
                if (!isActive)
                {
                    visualizationPlane.gameObject.SetActive(true);
                    visualizationPlane.planeNormal = Vector3.back;
                }
                visualizationPlane.UpdateMeshAndDraggable();

                // Only one transparent object can be active at the same time
                vectorFieldEnabledToggle.SetValue(false);
                isoSurfaceEnabledToggle.SetValue(false);
            });
            vectorFieldEnabledToggle.OnValueChanged.AddListener((bool newValue) =>
            {
                // Only one transparent object can be active at the same time
                if (newValue)
                {
                    visualizationPlane.gameObject.SetActive(false);
                    isoSurfaceEnabledToggle.SetValue(false);
                }
            });
            isoSurfaceEnabledToggle.OnValueChanged.AddListener((bool newValue) =>
            {
                // Only one transparent object can be active at the same time
                if (newValue)
                {
                    visualizationPlane.gameObject.SetActive(false);
                    vectorFieldEnabledToggle.SetValue(false);
                }
            });



            // Simulation window setup
            simulationSettings.bounciness = 0.0f;
            simulationSettings.friction = 0.6f;
            simulationSettings.boundaryEnabled = true;
            simulationSettings.drag = 1.0f;
            SetSimulationSettings(simulationSettings);

            uiEnableBoundaryToggle.OnValueChanged.AddListener((bool newValue) => 
            {
                simulationSettings.boundaryEnabled = newValue;
                SetSimulationSettings(simulationSettings);
            });
            uiDragSlider.OnValueChanged.AddListener((float newValue) =>
            {
                simulationSettings.drag = newValue;
                SetSimulationSettings(simulationSettings);
            });
            uiBouncinessSlider.OnValueChanged.AddListener((float newValue) =>
            {
                simulationSettings.bounciness = newValue;
                SetSimulationSettings(simulationSettings);
            });
            uiFrictionSlider.OnValueChanged.AddListener((float newValue) =>
            {
                simulationSettings.friction = newValue;
                SetSimulationSettings(simulationSettings);
            });



            // Camera window setup
            var cameraController = CameraController.Instance;
            uiCamera3DModeToggle.SetValue(cameraController.In3DMode);
            uiCamera3DModeToggle.OnValueChanged.AddListener((bool newValue) =>
            {
                CameraController.Instance.SetIn3DMode(newValue);
            });

            uiCameraSensitivitySlider.SetValue(cameraController.cameraSensitivity);
            uiCameraSensitivitySlider.OnValueChanged.AddListener((float newValue) =>
            {
                cameraController.cameraSensitivity = newValue;
            });

            // Note(Martin): I think VSynch should in general be turned on in Maroon
            QualitySettings.vSyncCount = 1;
            // See https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Application-targetFrameRate.html
            //  to know what these values do
            uiCameraVSynchToggle.SetValue(QualitySettings.vSyncCount != 0);
            uiCameraVSynchToggle.OnValueChanged.AddListener((bool newValue) =>
            {
                if (newValue)
                {
                    QualitySettings.vSyncCount = 1;
                }
                else
                {
                    QualitySettings.vSyncCount = 0;
                }
                uiCameraMaxFramerateInput.SetInteractable(!newValue, !newValue);
            });

            Application.targetFrameRate = 120;
            uiCameraMaxFramerateInput.SetInteractable(QualitySettings.vSyncCount == 0, QualitySettings.vSyncCount == 0);
            uiCameraMaxFramerateInput.SetValue(Application.targetFrameRate);
            uiCameraMaxFramerateInput.OnValueChanged.AddListener((int newValue) =>
            {
                Application.targetFrameRate = newValue;
            });



            // Initialize simulation start/pause/reset buttions
            simulationStartButton.onClick.RemoveAllListeners();
            simulationStartButton.gameObject.SetActive(true);
            simulationPauseButton.onClick.RemoveAllListeners();
            simulationPauseButton.gameObject.SetActive(false);
            simulationResetButton.onClick.RemoveAllListeners();
            simulationResetButton.gameObject.SetActive(false);

            simulationStartButton.onClick.AddListener(() =>
            {
                SimulationController.Instance.StartSimulation();

                bool newSimulationStart = !simulationResetButton.gameObject.activeSelf;

                // Update UI
                simulationStartButton.gameObject.SetActive(false);
                simulationPauseButton.gameObject.SetActive(true);
                simulationResetButton.gameObject.SetActive(true);

                // Store current configuration for reset
                if (newSimulationStart)
                {
                    configurationOnSimulationStart = ElectricFieldSerializer.CreateConfigurationForCurrentSetup();
                    settingsAtSimulationStart = simulationSettings;
                }

                // Configure objects for simulation start
                SelectionSystem.SetSelectedObject(null);
                foreach (var chargedPoint in ElectricField.Instance.chargedPoints)
                {
                    if (chargedPoint.lockPosition) continue;
                    chargedPoint.rigidBody.isKinematic = false;
                    chargedPoint.rigidBody.velocity = newSimulationStart ? chargedPoint.initialVelocity : chargedPoint.storedVelocity;
                }
            });

            simulationPauseButton.onClick.AddListener(() =>
            {
                SimulationController.Instance.StopSimulation();

                // Update UI
                simulationStartButton.gameObject.SetActive(true);
                simulationPauseButton.gameObject.SetActive(false);

                // Stop object movement
                foreach (var chargedPoint in ElectricField.Instance.chargedPoints)
                {
                    chargedPoint.storedVelocity = chargedPoint.rigidBody.velocity;
                    chargedPoint.rigidBody.isKinematic = true;
                }
            });

            // Note: Reset could be pressed while simulation is paused or while it's running
            simulationResetButton.onClick.AddListener(() =>
            {
                SimulationController.Instance.StopSimulation();

                // Update UI
                simulationStartButton.gameObject.SetActive(true);
                simulationPauseButton.gameObject.SetActive(false);
                simulationResetButton.gameObject.SetActive(false);

                // Restore simulation state to how it was when play was pressed
                ElectricFieldSerializer.RestoreConfiguration(
                    configurationOnSimulationStart, parentForNewObjects, prefabChargedPoint, prefabChargedRod, prefabChargedPlane);

                SetSimulationSettings(settingsAtSimulationStart);
            });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                var cameraController = CameraController.Instance;
                cameraController.SetIn3DMode(!cameraController.In3DMode);
                uiCamera3DModeToggle.SetValue(cameraController.In3DMode);
            }

            // Enable tab movement in UI, see https://discussions.unity.com/t/tab-between-input-fields/547817/10
            // Slightly changed so it only works with InputFields for now...
            // Note(MartinR): Sometimes this isn't working right, but it's good enough for now
            UnityEngine.EventSystems.EventSystem system = UnityEngine.EventSystems.EventSystem.current;
            bool forward = !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
            if (Input.GetKeyDown(KeyCode.Tab) && 
                system.currentSelectedGameObject != null && 
                system.currentSelectedGameObject.GetComponent<TMPro.TMP_InputField>() != null)
            {
                // Find next selectable that is also an TMP_InputField
                const int MAX_SEARCH_STEPS = 3;
                UnityEngine.UI.Selectable current = system.currentSelectedGameObject.GetComponent<UnityEngine.UI.Selectable>();
                TMPro.TMP_InputField found = null;
                for (int i = 0; i < MAX_SEARCH_STEPS && current != null && found == null; i++)
                {
                    current = forward ? current.FindSelectableOnRight() : current.FindSelectableOnLeft();
                    if (current == null) break;
                    if (!current.isActiveAndEnabled) continue;
                    found = current.GetComponent<TMPro.TMP_InputField>();
                }

                if (found != null)
                {
                    found.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(system)); //if it's an input field, also set the text caret
                    system.SetSelectedGameObject(found.gameObject, new UnityEngine.EventSystems.BaseEventData(system));
                }
            }
        }

        private void SetSimulationSettings(SimulationSettings settings)
        {
            this.simulationSettings = settings;
            particleSimulationBoundary.SetActive(settings.boundaryEnabled);
            physicMaterial.bounciness = settings.bounciness;
            physicMaterial.staticFriction = settings.friction;
            physicMaterial.dynamicFriction = settings.friction;

            uiEnableBoundaryToggle.SetValue(particleSimulationBoundary.activeSelf);
            uiDragSlider.SetValue(settings.drag);
            uiBouncinessSlider.SetValue(physicMaterial.bounciness);
            uiFrictionSlider.SetValue(physicMaterial.staticFriction);

            foreach (var chargedPoint in ElectricField.Instance.chargedPoints)
            {
                chargedPoint.GetComponent<Rigidbody>().drag = settings.drag;
                chargedPoint.GetComponent<SphereCollider>().sharedMaterial = physicMaterial;
            }
        }
    }
}
