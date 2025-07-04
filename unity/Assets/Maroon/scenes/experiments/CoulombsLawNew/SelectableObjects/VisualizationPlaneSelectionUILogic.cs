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

        [SerializeField] private GuiDropdownInputHandler uiHeatmapDropdown;
        [SerializeField] private GuiBoolInputHandler uiEquipotentialToggle;
        [SerializeField] private GuiFloatInputHandler uiEquipotentialSpacingSlider;
        [SerializeField] private GuiFloatInputHandler uiTransparencySlider;

        [SerializeField] private UnityEngine.UI.Button uiDeleteButton;

        private void Awake()
        {
            visualizationPlane = SelectionSystem.Instance.GetSelectedObject()?.GetComponent<VisualizationPlaneLogic>();
            Debug.Assert(visualizationPlane != null, "UI should only be displayed/spawned when a visualizationPlane is selected");

            // Update ui-elements to show values of selected plane
            uiPositionInput.SetValue(visualizationPlane.position);
            uiNormalInput.SetValue(visualizationPlane.planeNormal);

            uiHeatmapDropdown.SetSelectedIndex(visualizationPlane.heatmapMode);
            uiEquipotentialToggle.SetInitialValue(visualizationPlane.equipotentialLinesEnabled);
            uiEquipotentialSpacingSlider.SetInitialValue(visualizationPlane.equipotentialLinesSpacing / 1000.0f);

            uiTransparencySlider.SetInitialValue(visualizationPlane.transparency);



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

            uiHeatmapDropdown.OnValueChanged.AddListener((int heatmapMode) => { visualizationPlane.heatmapMode = heatmapMode; });
            uiEquipotentialToggle.OnValueChanged.AddListener((bool enabled) => { visualizationPlane.equipotentialLinesEnabled = enabled; });
            uiEquipotentialSpacingSlider.OnValueChanged.AddListener((float value) => { visualizationPlane.equipotentialLinesSpacing = value * 1000.0f; });
            uiTransparencySlider.OnValueChanged.AddListener((float value) => { visualizationPlane.transparency = value; });

            visualizationPlane.selectable.OnMoved.AddListener((SelectableObject _unused) => { uiPositionInput.SetValue(visualizationPlane.position); });

            uiDeleteButton.onClick.AddListener(() => {
                visualizationPlane.gameObject.SetActive(false);
                SelectionSystem.SetSelectedObject(null);
            });
        }
    }
}
