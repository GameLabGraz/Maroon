using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class CoulombsLawUILogic : MonoBehaviour
    {
        private Configuration configurationOnSimulationStart;

        [SerializeField] private ChargedPoint prefabChargedPoint;
        [SerializeField] private ChargedRod prefabChargedRod;
        [SerializeField] private ChargedPlane prefabChargedPlane;
        [SerializeField] private Transform parentForNewObjects;

        [SerializeField] private VisualizationPlaneLogic visualizationPlane;

        [Header("UI-Element References")]
        [SerializeField] private GUIBoolInputLogic vectorFieldEnabledToggle;
        [SerializeField] private GUIBoolInputLogic isoSurfaceEnabledToggle;

        [SerializeField] private GuiIconTo3DObjectDrag dragIconParticle;
        [SerializeField] private GuiIconTo3DObjectDrag dragIconChargedRod;
        [SerializeField] private GuiIconTo3DObjectDrag dragIconChargedPlane;
        [SerializeField] private GuiIconTo3DObjectDrag dragIconPlaneVisualization;

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
                    visualizationPlane.transparency = 0.8f;
                    visualizationPlane.heatmapMode = 1;
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

                // Update UI
                simulationStartButton.gameObject.SetActive(false);
                simulationPauseButton.gameObject.SetActive(true);
                simulationResetButton.gameObject.SetActive(true);

                // Store current configuration for reset
                configurationOnSimulationStart = ElectricFieldSerializer.CreateConfigurationForCurrentSetup();

                // Configure objects for simulation start
                SelectionSystem.SetSelectedObject(null);
                foreach (var chargedPoint in ElectricField.Instance.chargedPoints)
                {
                    chargedPoint.rigidBody.isKinematic = false;
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
            });
        }
    }
}
