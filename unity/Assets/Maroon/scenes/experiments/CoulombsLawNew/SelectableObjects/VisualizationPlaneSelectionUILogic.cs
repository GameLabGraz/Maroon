using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class VisualizationPlaneSelectionUILogic : MonoBehaviour
    {
        private VisualizationPlaneLogic visualizationPlane;

        [Header("UI references")]
        [SerializeField] private GuiVector3InputHandler uiPositionInput;
        [SerializeField] private GuiVector3InputHandler uiNormalInput;

        [SerializeField] private GuiBoolInputHandler uiHeatmapToggle;
        [SerializeField] private GuiFloatInputHandler uiHeatmapCutoffKiloVoltSlider;
        [SerializeField] private GuiFloatInputHandler uiHeatmapFalloffSlider;

        [SerializeField] private GuiBoolInputHandler uiEquipotentialToggle;
        [SerializeField] private GuiFloatInputHandler uiEquipotentialMaximumSlider;
        [SerializeField] private GuiFloatInputHandler uiEquipotentialSpacingSlider;

        [SerializeField] private GuiFloatInputHandler uiTransparencySlider;
        [SerializeField] private GuiBoolInputHandler uiFixedPositionToggle;
        [SerializeField] private GuiFloatInputHandler uiVirtualZ;

        [SerializeField] private UnityEngine.UI.Button uiDeleteButton;

        private void Awake()
        {
            visualizationPlane = SelectionSystem.Instance.GetSelectedObject()?.GetComponent<VisualizationPlaneLogic>();
            Debug.Assert(visualizationPlane != null, "UI should only be displayed/spawned when a visualizationPlane is selected");

            // Update ui-elements to show values of selected plane
            uiPositionInput.SetValue(visualizationPlane.position);
            uiNormalInput.SetValue(visualizationPlane.planeNormal);

            uiHeatmapToggle.SetInitialValue(visualizationPlane.heatmapEnabled);
            uiEquipotentialToggle.SetInitialValue(visualizationPlane.equipotentialLinesEnabled);

            // These were used before parameters were global across all visualization planes
            // uiHeatmapCutoffKiloVoltSlider.SetInitialValue(visualizationPlane.heatmapCutoff / 1000.0f);
            // uiHeatmapFalloffSlider.SetInitialValue(visualizationPlane.heatmapFalloff);
            // uiEquipotentialMaximumSlider.SetInitialValue(visualizationPlane.equipotentialLinesMaximum / 1000.0f);
            // uiEquipotentialSpacingSlider.SetInitialValue(visualizationPlane.equipotentialLinesSpacing / 1000.0f);

            uiTransparencySlider.SetInitialValue(visualizationPlane.transparency);
            uiFixedPositionToggle.SetInitialValue(visualizationPlane.fixedPosition);
            uiVirtualZ.SetInitialValue(visualizationPlane.fixedPositionVirtualZ);



            // Register Callbacks
            uiPositionInput.OnEndEdit.AddListener((Vector3 newPos) => 
            { 
                visualizationPlane.position = newPos; visualizationPlane.UpdateMeshAndDraggable(); 
            });
            uiNormalInput.OnEndEdit.AddListener((Vector3 normal) => 
            { 
                if (normal.magnitude < 0.001)
                {
                    normal = Vector3.back;
                }
                normal = normal.normalized;
                visualizationPlane.planeNormal = normal; 
                visualizationPlane.UpdateMeshAndDraggable(); 
            });

            uiHeatmapToggle.OnValueChanged.AddListener((bool enabled) => { visualizationPlane.heatmapEnabled = enabled; });
            uiHeatmapCutoffKiloVoltSlider.OnValueChanged.AddListener((float value) => { VisualizationPlaneLogic.heatmapCutoff = value * 1000.0f; });
            uiHeatmapFalloffSlider.OnValueChanged.AddListener((float value) => { VisualizationPlaneLogic.heatmapFalloff = value; });

            uiEquipotentialToggle.OnValueChanged.AddListener((bool enabled) => { visualizationPlane.equipotentialLinesEnabled = enabled; });
            uiEquipotentialMaximumSlider.OnValueChanged.AddListener((float value) => { VisualizationPlaneLogic.equipotentialLinesMaximum = value * 1000.0f; });
            uiEquipotentialSpacingSlider.OnValueChanged.AddListener((float value) => { VisualizationPlaneLogic.equipotentialLinesSpacing = value * 1000.0f; });

            uiTransparencySlider.OnValueChanged.AddListener((float value) => { visualizationPlane.transparency = value; });
            uiFixedPositionToggle.OnValueChanged.AddListener((bool value) => { visualizationPlane.fixedPosition = value; visualizationPlane.UpdateMeshAndDraggable(); });
            uiVirtualZ.OnValueChanged.AddListener((float value) => { visualizationPlane.fixedPositionVirtualZ = value; });

            visualizationPlane.selectable.OnMovedWithGizmo.AddListener((SelectableObject _unused) => { uiPositionInput.SetValue(visualizationPlane.position); });
            visualizationPlane.draggable.OnMoved.AddListener((DraggableObject _unused) => { uiPositionInput.SetValue(visualizationPlane.position); });

            uiDeleteButton.onClick.AddListener(() => {
                visualizationPlane.gameObject.SetActive(false);
                SelectionSystem.SetSelectedObject(null);
            });
        }
    }
}
