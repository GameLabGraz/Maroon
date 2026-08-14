using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSelectionToggle : MonoBehaviour
{
    public BlockPlacementManager placementManager;
    public GameObject blockPrefab;

    public void SelectionChanged(bool selected)
    {
        if (placementManager == null || blockPrefab == null)
        {
            return;
        }

        if (selected)
        {
            placementManager.SelectBlock(blockPrefab);
        }
        else
        {
            placementManager.UnselectBlock(blockPrefab);
        }
    }
}