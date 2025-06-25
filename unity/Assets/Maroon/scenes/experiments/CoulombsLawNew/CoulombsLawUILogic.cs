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
        [SerializeField] private VisualizationPlaneLogic prefabVisualizationPlane;
        [SerializeField] private Transform parentForNewObjects;

        [Header("UI-Element References")]
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
            });
            dragIconChargedRod.OnDragFinished.AddListener((Vector3 pos) =>
            {
                var newRod = GameObject.Instantiate(prefabChargedRod, pos, Quaternion.identity, parentForNewObjects);
                newRod.SetRodParameters(pos, Vector3.up);
            });
            dragIconChargedPlane.OnDragFinished.AddListener((Vector3 pos) =>
            {
                var newPlane = GameObject.Instantiate(prefabChargedPlane, pos, Quaternion.identity, parentForNewObjects);
                newPlane.SetPlaneParameters(pos, Vector3.right);
            });
            dragIconPlaneVisualization.OnDragFinished.AddListener((Vector3 pos) =>
            {
                var plane = GameObject.Instantiate(prefabVisualizationPlane, pos, Quaternion.identity, parentForNewObjects);
                plane.position = pos;
                plane.planeNormal = Vector3.back;
                plane.fixedPosition = !CameraController.Instance.In3DMode;
                plane.UpdateMeshAndDraggable();
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
