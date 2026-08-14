using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockCategoryMenu : MonoBehaviour
{
    public BlockPlacementManager placementManager;
    public GameObject logicGatesGrid;
    public GameObject inputsOutputsGrid;
    public Toggle logicGatesTab;

    void Start()
    {
        logicGatesTab.SetIsOnWithoutNotify(true);
        ShowCategory(logicGatesGrid);
    }

    public void ShowLogicGates(bool selected)
    {
        if (selected == false)
        {
            return;
        }

        ShowCategory(logicGatesGrid);
    }

    public void ShowInputsOutputs(bool selected)
    {
        if (selected == false)
        {
            return;
        }

        ShowCategory(inputsOutputsGrid);
    }

    void ShowCategory(GameObject selectedGrid)
    {
        ClearBlockSelection();

        logicGatesGrid.SetActive(false);
        inputsOutputsGrid.SetActive(false);

        selectedGrid.SetActive(true);
    }

    void ClearBlockSelection()
    {
        if (placementManager != null)
        {
            placementManager.ClearSelection();
        }

        TurnOffToggles(logicGatesGrid);
        TurnOffToggles(inputsOutputsGrid);
    }

    void TurnOffToggles(GameObject grid)
    {
        Toggle[] toggles = grid.GetComponentsInChildren<Toggle>(true);

        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].SetIsOnWithoutNotify(false);
        }
    }
}