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
        public float dragSpringStrength;
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

        [SerializeField] private GUIBoolInputLogic     uiShowTrajectoriesToggle;
        [SerializeField] private GUIBoolInputLogic     uiEnableBoundaryToggle;
        [SerializeField] private GUIFloatInputLogic    uiDragSlider;
        [SerializeField] private GUIFloatInputLogic    uiBouncinessSlider;
        [SerializeField] private GUIFloatInputLogic    uiFrictionSlider;
        [SerializeField] private GUIFloatInputLogic    uiSpringStrengthSlider;
        [SerializeField] private GUIDropdownInputLogic uiScenarioDropdown;

        // Force Visualization
        [SerializeField] private GUIBoolInputLogic  uiForceVectorsEnabledToggle;
        [SerializeField] private GUIFloatInputLogic uiForceVectorsScalingSlider;
        [SerializeField] private GUIFloatInputLogic uiForceVectorsMaxLengthSlider;

        // Somewhat hacky, but all ChargedPoints need to know if they should display force-vectors or not
        public static float forceVectorsScaling = 0.3f;
        public static float forceVectorsMaxLength = 0.6f;

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
                newParticle.SetCharge(ChargedPoint.MAX_ABSOLUTE_CHARGE);
                newParticle.GetComponent<SphereCollider>().sharedMaterial = physicMaterial;
                newParticle.SetGenerateTrail(uiShowTrajectoriesToggle.GetValue());
                newParticle.displayForceArrow = uiForceVectorsEnabledToggle.GetValue();
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
            });



            // Force Vector setup
            {
                // Initialize static variables again (So that leaving/re-entering the experiment resets these values)
                forceVectorsScaling = 0.4f;
                forceVectorsMaxLength = 0.8f;

                // Initialize ui-element values and callbacks
                uiForceVectorsEnabledToggle.SetValue(false);
                uiForceVectorsScalingSlider.SetValue(forceVectorsScaling);
                uiForceVectorsMaxLengthSlider.SetValue(forceVectorsMaxLength);
                uiForceVectorsEnabledToggle.OnValueChanged.AddListener((bool newValue) =>
                {
                    foreach (ChargedPoint point in ElectricField.Instance.chargedPoints)
                    {
                        point.displayForceArrow = newValue;
                    }
                });
                uiForceVectorsScalingSlider.OnValueChanged.AddListener((float newValue) =>
                {
                    forceVectorsScaling = newValue;
                });
                uiForceVectorsMaxLengthSlider.OnValueChanged.AddListener((float newValue) =>
                {
                    forceVectorsMaxLength = newValue;
                });
            }



            // Simulation window setup
            simulationSettings.bounciness = 0.0f;
            simulationSettings.friction = 0.03f;
            simulationSettings.boundaryEnabled = true;
            simulationSettings.drag = 0.3f;
            simulationSettings.dragSpringStrength = 3.0f;
            ApplySimulationSettings(simulationSettings);

            uiShowTrajectoriesToggle.OnValueChanged.AddListener((bool newValue) =>
            {
                foreach (ChargedPoint point in ElectricField.Instance.chargedPoints)
                {
                    point.SetGenerateTrail(newValue);
                }
            });
            uiEnableBoundaryToggle.OnValueChanged.AddListener((bool newValue) =>
            {
                simulationSettings.boundaryEnabled = newValue;
                ApplySimulationSettings(simulationSettings);
            });
            uiDragSlider.OnValueChanged.AddListener((float newValue) =>
            {
                simulationSettings.drag = newValue;
                ApplySimulationSettings(simulationSettings);
            });
            uiBouncinessSlider.OnValueChanged.AddListener((float newValue) =>
            {
                simulationSettings.bounciness = newValue;
                ApplySimulationSettings(simulationSettings);
            });
            uiFrictionSlider.OnValueChanged.AddListener((float newValue) =>
            {
                simulationSettings.friction = newValue;
                ApplySimulationSettings(simulationSettings);
            });
            uiSpringStrengthSlider.OnValueChanged.AddListener((float newValue) =>
            {
                simulationSettings.dragSpringStrength = newValue;
                ApplySimulationSettings(simulationSettings);
            });

            uiScenarioDropdown.OnValueChanged.AddListener((int newValue) =>
            {
                SimulationController.Instance.StopSimulation();

                // Update UI
                simulationStartButton.gameObject.SetActive(true);
                simulationPauseButton.gameObject.SetActive(false);
                simulationResetButton.gameObject.SetActive(false);

                // Load scenario
                Configuration configuration = new Configuration();
                configuration.chargedPoints = new ChargedPointData[0];
                configuration.chargedRods   = new ChargedRodData[0];
                configuration.chargedPlanes = new ChargedPlaneData[0];
                SimulationSettings settings = new SimulationSettings();
                settings.bounciness = 0.0f;
                settings.boundaryEnabled = true;
                settings.drag = 0.2f;
                settings.friction = 1.0f;
                settings.dragSpringStrength = 3.0f;

                const int POINT_COUNT_1D = 4;
                switch (newValue)
                {
                    case 0: // Empty-Simulation
                        break;
                    case 1: // 2 Points
                        configuration.chargedPoints = new ChargedPointData[2];
                        configuration.chargedPoints[0] = new ChargedPointData(new Vector3(-0.5f, 0, 0), -ChargedPoint.MAX_ABSOLUTE_CHARGE);
                        configuration.chargedPoints[1] = new ChargedPointData(new Vector3( 0.5f, 0, 0), ChargedPoint.MAX_ABSOLUTE_CHARGE);
                        configuration.chargedPoints[0].generateTrail = true;
                        configuration.chargedPoints[1].generateTrail = true;
                        settings.bounciness = 0.0f;
                        settings.drag = .1f;
                        break;
                    case 2: // 2 Points orbiting
                        configuration.chargedPoints = new ChargedPointData[2];
                        configuration.chargedPoints[0] = new ChargedPointData(
                            new Vector3(0, 0, 0), Vector3.zero, ChargedPoint.MAX_ABSOLUTE_CHARGE, false, 1.0f, true, true, true, false, true, false);
                        configuration.chargedPoints[1] = new ChargedPointData(
                            new Vector3(0.5f, 0, 0), Vector3.up, -2.0f * ChargedPoint.MAX_ABSOLUTE_CHARGE, false, 1.0f, true, false, true, false, true, false);
                        settings.bounciness = 0;
                        settings.drag = 0;
                        settings.friction = 0;
                        break;
                    case 3: // Multi orbit
                        configuration.chargedPoints = new ChargedPointData[4];
                        configuration.chargedPoints[0] = new ChargedPointData(
                            new Vector3(0, 0, 0), Vector3.zero, 3.0f * ChargedPoint.MAX_ABSOLUTE_CHARGE, 
                            false, 1.0f, false, true, true, false, false, false);
                        configuration.chargedPoints[1] = new ChargedPointData(
                            new Vector3(0.5f, 0, 0), Vector3.up, -ChargedPoint.MAX_ABSOLUTE_CHARGE, 
                            false, 1.0f, true, false, true, false, true, false);
                        configuration.chargedPoints[2] = new ChargedPointData(
                            new Vector3(-0.5f, 0, 0), Vector3.down, -ChargedPoint.MAX_ABSOLUTE_CHARGE, 
                            false, 1.0f, true, false, true, false, true, false);
                        configuration.chargedPoints[3] = new ChargedPointData(
                            new Vector3(0.0f, -.5f, 0.0f), Vector3.forward, -ChargedPoint.MAX_ABSOLUTE_CHARGE, 
                            false, 1.0f, true, false, true, false, true, false);
                        settings.bounciness = 0.8f;
                        settings.drag = 0;
                        settings.friction = 0;
                        break;
                    case 4: // 3 Bodies
                        configuration.chargedPoints = new ChargedPointData[3];
                        configuration.chargedPoints[0] = new ChargedPointData(
                            new Vector3(0, 0, 0), Vector3.up * 0.1f, ChargedPoint.MAX_ABSOLUTE_CHARGE, 
                            false, 1.0f, false, false, true, false, true, false);
                        configuration.chargedPoints[1] = new ChargedPointData(
                            new Vector3(0.5f, 0, 0), Vector3.up * 0.1f, -0.5f * ChargedPoint.MAX_ABSOLUTE_CHARGE, 
                            false, 1.0f, false, false, true, false, true, false);
                        configuration.chargedPoints[2] = new ChargedPointData(
                            new Vector3(-0.5f, 0, 0), Vector3.down * 0.1f, -0.5f * ChargedPoint.MAX_ABSOLUTE_CHARGE, 
                            false, 1.0f, false, false, true, false, true, false);
                        settings.bounciness = 0.8f;
                        settings.drag = 0;
                        settings.friction = 0;
                        break;
                    case 5: // Charged Transfer single particle
                        configuration.chargedPoints = new ChargedPointData[1];
                        configuration.chargedPoints[0] = new ChargedPointData(
                            new Vector3(0, 0, 0), Vector3.zero, ChargedPoint.MAX_ABSOLUTE_CHARGE, 
                            false, 1.0f, true, false, true, true, false, false);
                        configuration.chargedPlanes = new ChargedPlaneData[2];
                        configuration.chargedPlanes[0] = new ChargedPlaneData(
                            new Vector3(-0.6f, 0, 0), Vector3.left, -ChargedPlane.MAX_CHARGE_DENSITY, false, true);
                        configuration.chargedPlanes[1] = new ChargedPlaneData(
                            new Vector3(0.6f, 0, 0), Vector3.left, ChargedPlane.MAX_CHARGE_DENSITY, false, true);
                        settings.bounciness = 0.3f;
                        settings.drag = 0;
                        settings.friction = 0;
                        break;
                    case 6: // Charge Transfer multiple particles
                        configuration.chargedPlanes = new ChargedPlaneData[2];
                        configuration.chargedPlanes[0] = new ChargedPlaneData(
                            new Vector3(-0.6f, 0, 0), Vector3.left, -ChargedPlane.MAX_CHARGE_DENSITY, false, true);
                        configuration.chargedPlanes[1] = new ChargedPlaneData(
                            new Vector3(0.6f, 0, 0), Vector3.left, 0.7f * ChargedPlane.MAX_CHARGE_DENSITY, false, true);

                        configuration.chargedPoints = new ChargedPointData[POINT_COUNT_1D * POINT_COUNT_1D];
                        for (int x = 0; x < POINT_COUNT_1D; x++)
                        {
                            for (int y = 0; y < POINT_COUNT_1D; y++)
                            {
                                float tX = x / (float)(POINT_COUNT_1D - 1);
                                float tY = y / (float)(POINT_COUNT_1D - 1);
                                float chargeSign = (x + y) % 2 == 0 ? 1.0f : -1.0f;

                                float MIN_COORD = -.8f;
                                configuration.chargedPoints[x + y * POINT_COUNT_1D] = new ChargedPointData(
                                    new Vector3(0, MIN_COORD + tX * (-2 * MIN_COORD), MIN_COORD + tY * (-2 * MIN_COORD)), 
                                    Vector3.zero, chargeSign * ChargedPoint.MAX_ABSOLUTE_CHARGE, false, 1.0f, true, false, true, true, false, false);
                            }
                        }

                        settings.bounciness = 0.4f;
                        settings.drag = 0;
                        settings.friction = 0;
                        break;
                    case 7: // Ballpit
                        configuration.chargedPoints = new ChargedPointData[POINT_COUNT_1D * POINT_COUNT_1D * POINT_COUNT_1D];
                        for (int x = 0; x < POINT_COUNT_1D; x++)
                        {
                            for (int y = 0; y < POINT_COUNT_1D; y++)
                            {
                                for (int z = 0; z < POINT_COUNT_1D; z++)
                                {
                                    float tX = x / (float)(POINT_COUNT_1D - 1);
                                    float tY = y / (float)(POINT_COUNT_1D - 1);
                                    float tZ = z / (float)(POINT_COUNT_1D - 1);
                                    bool conductive = (x + y + z) % 3 == 1;
                                    float chargeSign = (x + y + z) % 2 == 0 ? 1.0f : -1.0f;

                                    float MIN_COORD = -.8f;
                                    configuration.chargedPoints[x + y * POINT_COUNT_1D + z * POINT_COUNT_1D * POINT_COUNT_1D] = new ChargedPointData(
                                        new Vector3(MIN_COORD + tX * (-2 * MIN_COORD), MIN_COORD + tY * (-2 * MIN_COORD), MIN_COORD + tZ * (-2 * MIN_COORD)), 
                                        Vector3.zero, chargeSign * Random.Range(0, ChargedPoint.MAX_ABSOLUTE_CHARGE), 
                                        false, 1.0f, true, false, true, conductive, false, false);
                                }
                            }
                        }

                        settings.bounciness = 0.4f;
                        settings.drag = 0;
                        settings.friction = 0;
                        break;
                    case 8: // Circle formation
                        int CIRCLE_POINTS = 10;
                        float CIRCLE_RADIUS = 0.9f;
                        configuration.chargedPoints = new ChargedPointData[CIRCLE_POINTS + 1];
                        for (int i = 0; i < CIRCLE_POINTS; i++)
                        {
                            float angle = i / (float) CIRCLE_POINTS * 2 * Mathf.PI;
                            Vector3 pos = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0.0f) * CIRCLE_RADIUS;
                            configuration.chargedPoints[i] = new ChargedPointData(
                                pos, Vector3.zero, ChargedPoint.MAX_ABSOLUTE_CHARGE, 
                                false, 1.0f, true, true, true, false, false, false);
                        }

                        // One point slightly offset from center
                        configuration.chargedPoints[CIRCLE_POINTS] = new ChargedPointData(
                            new Vector3(0.2f, 0.1f, 0.0f), Vector3.up * 0.7f, ChargedPoint.MAX_ABSOLUTE_CHARGE, 
                            false, 1.0f, true, false, true, false, true, false);

                        settings.bounciness = 0.5f;
                        settings.drag = 0;
                        settings.friction = 0;
                        break;
                }

                ElectricFieldSerializer.RestoreConfiguration(
                    configuration, parentForNewObjects, prefabChargedPoint, prefabChargedRod, prefabChargedPlane);
                ApplySimulationSettings(settings);
            });

            // Note(MartinR): Not sure where to put this, I hope this doesn't mess with any other simulations,
            //  but in the Coulombs-Law-Experiment the default value for the bounceThreshold (2) results in bad collision handling
            //  https://stackoverflow.com/questions/54656220/physics-object-doesnt-bounce-correctly-at-low-speed-in-unity
            UnityEngine.Physics.bounceThreshold = 0.15f;


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
                uiCameraMaxFramerateInput.SetInteractable(!newValue, newValue);
            });

            Application.targetFrameRate = 120;
            uiCameraMaxFramerateInput.SetInteractable(QualitySettings.vSyncCount == 0, QualitySettings.vSyncCount != 0);
            uiCameraMaxFramerateInput.SetValue(Application.targetFrameRate);
            uiCameraMaxFramerateInput.OnValueChanged.AddListener((int newValue) =>
            {
                Application.targetFrameRate = newValue;
            });



            // Initialize simulation start/pause/reset buttions
            simulationStartButton.gameObject.SetActive(true);
            simulationPauseButton.gameObject.SetActive(false);
            simulationResetButton.gameObject.SetActive(false);

            simulationStartButton.onClick.AddListener(() =>
            {
                // We either start the simulation or continue from pause when play is pressed
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
                SelectionSystem.SetSelectedObject(null);
            });

            simulationPauseButton.onClick.AddListener(() =>
            {
                // Update UI
                simulationStartButton.gameObject.SetActive(true);
                simulationPauseButton.gameObject.SetActive(false);
            });

            // Note: Reset could be pressed while simulation is paused or while it's running
            simulationResetButton.onClick.AddListener(() =>
            {
                // Update UI
                simulationStartButton.gameObject.SetActive(true);
                simulationPauseButton.gameObject.SetActive(false);
                simulationResetButton.gameObject.SetActive(false);

                // Restore simulation state to how it was when play was pressed
                ElectricFieldSerializer.RestoreConfiguration(
                    configurationOnSimulationStart, parentForNewObjects, prefabChargedPoint, prefabChargedRod, prefabChargedPlane);

                ApplySimulationSettings(settingsAtSimulationStart);
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
            // Note(MartinR): This isn't currently working for moving vertically, but it's good enough for now
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

        private void ApplySimulationSettings(SimulationSettings settings)
        {
            this.simulationSettings = settings;
            particleSimulationBoundary.SetActive(settings.boundaryEnabled);
            physicMaterial.bounciness = settings.bounciness;
            physicMaterial.staticFriction = settings.friction;
            physicMaterial.dynamicFriction = settings.friction;
            SelectionSystem.Instance.dragSpringCoefficient = settings.dragSpringStrength;

            uiEnableBoundaryToggle.SetValue(particleSimulationBoundary.activeSelf);
            uiDragSlider.SetValue(settings.drag);
            uiBouncinessSlider.SetValue(physicMaterial.bounciness);
            uiFrictionSlider.SetValue(physicMaterial.staticFriction);
            uiSpringStrengthSlider.SetValue(settings.dragSpringStrength);

            foreach (var chargedPoint in ElectricField.Instance.chargedPoints)
            {
                chargedPoint.GetComponent<Rigidbody>().drag = settings.drag;
                chargedPoint.GetComponent<SphereCollider>().sharedMaterial = physicMaterial;
            }
        }
    }
}
