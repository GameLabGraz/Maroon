using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircuitModeManager : MonoBehaviour
{
    public BlockPlacementManager placementManager;
    public GameObject puzzle1Prefab;
    public GameObject puzzle2Prefab;
    public GameObject puzzle3Prefab;
    public Toggle puzzle1Toggle;
	GameObject currentPuzzleRoot;

    void Start()
    {
        if (puzzle1Toggle != null)
        {
            puzzle1Toggle.SetIsOnWithoutNotify(true);
        }

        LoadSetup(puzzle1Prefab);
    }

    public void LoadPuzzle1(bool selected)
    {
        if (selected == false)
        {
            return;
        }

        LoadSetup(puzzle1Prefab);
    }

    public void LoadPuzzle2(bool selected)
    {
        if (selected == false)
        {
            return;
        }

        LoadSetup(puzzle2Prefab);
    }

    public void LoadPuzzle3(bool selected)
    {
        if (selected == false)
        {
            return;
        }

        LoadSetup(puzzle3Prefab);
    }

    public void LoadFreeBuild(bool selected)
    {
        if (selected == false)
        {
            return;
        }

        LoadSetup(null);
    }

    void LoadSetup(GameObject setupPrefab)
    {
	RemoveCurrentPuzzleRoot();

        placementManager.ClearTable();

        if (setupPrefab == null)
        {
            return;
        }

        Transform blockContainer = placementManager.blockContainer;
        GameObject setupInstance = Instantiate(setupPrefab, blockContainer, false);
        setupInstance.transform.localPosition = Vector3.zero;
        setupInstance.transform.localRotation = Quaternion.identity;
        setupInstance.transform.localScale = Vector3.one;

        CircuitSetup circuitSetup = setupInstance.GetComponent<CircuitSetup>();

        if (circuitSetup == null)
        {
            Destroy(setupInstance);
            return;
        }

        circuitSetup.ConnectBlocks();

        List<Transform> blocks = new List<Transform>();

        for (int i = 0; i < setupInstance.transform.childCount; i++)
        {
            Transform child = setupInstance.transform.GetChild(i);
            Move moveScript = child.GetComponentInChildren<Move>(true);

            if (moveScript != null)
            {
                blocks.Add(child);
            }
        }

        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].SetParent(blockContainer, false);
        }

	currentPuzzleRoot = setupInstance;
        currentPuzzleRoot.transform.SetParent(transform, true);

        Physics.SyncTransforms();
    }

    void RemoveCurrentPuzzleRoot()
    {
        if (currentPuzzleRoot != null)
        {
            currentPuzzleRoot.SetActive(false);
            Destroy(currentPuzzleRoot);
            currentPuzzleRoot = null;
        }
    }
}