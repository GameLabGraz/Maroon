using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockPlacementManager : MonoBehaviour
{
    public Camera sceneCamera;
    public BoxCollider tableCollider;
    public Transform blockContainer;
    public Toggle deleteToggle;
    public Transform blockToggleContainer;

    public float distanceAboveTable = 0.005f;
    public float tableEdgePadding = 0.01f;
    public float overlapPadding = 0.001f;

    public BlockInfoPanel infoPanel;

    GameObject selectedBlockPrefab;
    bool deleteMode = false;

    void Start()
    {
        if (sceneCamera == null)
        {
            sceneCamera = Camera.main;
        }

        if (blockContainer == null)
        {
            blockContainer = transform;
        }

        if (tableCollider == null)
        {
            Debug.LogError("No table collider assigned to BlockPlacementManager.");
        }
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0) == false)
        {
            return;
        }

        if (EventSystem.current != null)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
        }

	if (deleteMode)
        {
            DeleteClickedBlock();
            return;
        }

        if (selectedBlockPrefab != null)
        {
            PlaceSelectedBlock();
        }
    }

    public void SelectBlock(GameObject blockPrefab)
    {
        selectedBlockPrefab = blockPrefab;

	deleteMode = false;
        if (deleteToggle != null)
        {
            deleteToggle.SetIsOnWithoutNotify(false);
        }
	
	if (infoPanel != null)
        {
            infoPanel.ShowBlockInfo(blockPrefab);
        }
    }

    public void UnselectBlock(GameObject blockPrefab)
    {
        if (selectedBlockPrefab == blockPrefab)
        {
            selectedBlockPrefab = null;
	
	    if (infoPanel != null)
            {
                infoPanel.ShowGeneralInfo();
            }
        }
    }

    void PlaceSelectedBlock()
    {
        if (sceneCamera == null || tableCollider == null)
        {
            return;
        }

        Ray mouseRay = sceneCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit closestHit;

        if (Physics.Raycast(mouseRay, out closestHit, Mathf.Infinity, -1, QueryTriggerInteraction.Collide))
        {
            if (closestHit.collider != tableCollider)
            {
                if (IsInsideBlockContainer(closestHit.collider.transform))
                {
                    return;
                }
            }
        }

        RaycastHit tableHit;

        if (tableCollider.Raycast(mouseRay, out tableHit, Mathf.Infinity) == false)
        {
            return;
        }

        Vector3 tableNormal = tableCollider.transform.up.normalized;

        if (Vector3.Dot(tableHit.normal, tableNormal) < 0.5f)
        {
            return;
        }

        Vector3 localHitPosition = tableCollider.transform.InverseTransformPoint(tableHit.point);

        localHitPosition.y = tableCollider.center.y + tableCollider.size.y * 0.5f;

        Vector3 surfacePosition = tableCollider.transform.TransformPoint(localHitPosition);

        GameObject newBlock = Instantiate(selectedBlockPrefab, blockContainer, false);

        newBlock.name = selectedBlockPrefab.name;
        newBlock.transform.position = surfacePosition;

        Physics.SyncTransforms();

        BoxCollider[] blockColliders = newBlock.GetComponentsInChildren<BoxCollider>();

        if (HasEnabledCollider(blockColliders) == false)
        {
            Debug.LogError(newBlock.name + " has no enabled BoxCollider.");
            newBlock.SetActive(false);
            Destroy(newBlock);
            return;
        }

	CenterBlockOnMouse(newBlock, blockColliders, surfacePosition);
        MoveBlockAboveTable(newBlock, blockColliders, surfacePosition, tableNormal);

	Physics.SyncTransforms();

        if (IsInsideTable(blockColliders) == false)
        {
            Debug.Log("The " + newBlock.name + " block would be outside the table.");
            newBlock.SetActive(false);
            Destroy(newBlock);
            return;
        }

        if (OverlapsAnotherBlock(newBlock, blockColliders))
        {
            Debug.Log("The " + newBlock.name + " block would overlap another block.");
            newBlock.SetActive(false);
            Destroy(newBlock);
            return;
        }
    }

    bool HasEnabledCollider(BoxCollider[] blockColliders)
    {
        for (int i = 0; i < blockColliders.Length; i++)
        {
            if (blockColliders[i].enabled && blockColliders[i].gameObject.activeInHierarchy)
            {
                return true;
            }
        }

        return false;
    }

    void CenterBlockOnMouse(GameObject newBlock, BoxCollider[] blockColliders, Vector3 surfacePosition)
    {
    	bool foundCorner = false;

    	float minimumX = 0f;
    	float maximumX = 0f;
    	float minimumZ = 0f;
    	float maximumZ = 0f;

        for (int i = 0; i < blockColliders.Length; i++)
    	{
            BoxCollider blockCollider = blockColliders[i];

            if (blockCollider.enabled == false || blockCollider.gameObject.activeInHierarchy == false)
            {
                continue;
            }

            for (int x = -1; x <= 1; x = x + 2)
            {
                for (int y = -1; y <= 1; y = y + 2)
                {
                    for (int z = -1; z <= 1; z = z + 2)
                    {
                        Vector3 corner = GetColliderCorner(blockCollider, x, y, z);

                        Vector3 localCorner = tableCollider.transform.InverseTransformPoint(corner);

                        if (foundCorner == false)
                        {
                            minimumX = localCorner.x;
                            maximumX = localCorner.x;
                            minimumZ = localCorner.z;
                            maximumZ = localCorner.z;

                            foundCorner = true;
                        }
                        else
                        {
                            minimumX = Mathf.Min(minimumX, localCorner.x);
                            maximumX = Mathf.Max(maximumX, localCorner.x);
                            minimumZ = Mathf.Min(minimumZ, localCorner.z);
                            maximumZ = Mathf.Max(maximumZ, localCorner.z);
                        }
                    }
                }
            }
        }

        if (foundCorner == false)
        {
            return;
        }

        float colliderCenterX = (minimumX + maximumX) * 0.5f;

        float colliderCenterZ = (minimumZ + maximumZ) * 0.5f;

        Vector3 localMousePosition = tableCollider.transform.InverseTransformPoint(surfacePosition);

        Vector3 localMovement = new Vector3(localMousePosition.x - colliderCenterX, 0f, localMousePosition.z - colliderCenterZ);

        Vector3 worldMovement = tableCollider.transform.TransformVector(localMovement);

        newBlock.transform.position = newBlock.transform.position + worldMovement;
    }

    void MoveBlockAboveTable(GameObject newBlock, BoxCollider[] blockColliders, Vector3 surfacePosition, Vector3 tableNormal)
    {
        bool foundCorner = false;
        float lowestDistance = 0f;

        for (int i = 0; i < blockColliders.Length; i++)
        {
            BoxCollider blockCollider = blockColliders[i];

            if (blockCollider.enabled == false || blockCollider.gameObject.activeInHierarchy == false)
            {
                continue;
            }

            for (int x = -1; x <= 1; x = x + 2)
            {
                for (int y = -1; y <= 1; y = y + 2)
                {
                    for (int z = -1; z <= 1; z = z + 2)
                    {
                        Vector3 corner = GetColliderCorner(blockCollider, x, y, z);

                        float distance = Vector3.Dot(corner - surfacePosition, tableNormal);

                        if (foundCorner == false || distance < lowestDistance)
                        {
                            lowestDistance = distance;
                            foundCorner = true;
                        }
                    }
                }
            }
        }

        if (foundCorner)
        {
            float movement = distanceAboveTable - lowestDistance;

            newBlock.transform.position = newBlock.transform.position + tableNormal * movement;
        }
    }

    bool IsInsideTable(BoxCollider[] blockColliders)
    {
        Vector3 tableMinimum = tableCollider.center - tableCollider.size * 0.5f;
        Vector3 tableMaximum = tableCollider.center + tableCollider.size * 0.5f;

        tableMinimum.x = tableMinimum.x + tableEdgePadding;
        tableMinimum.z = tableMinimum.z + tableEdgePadding;
        tableMaximum.x = tableMaximum.x - tableEdgePadding;
        tableMaximum.z = tableMaximum.z - tableEdgePadding;

        for (int i = 0; i < blockColliders.Length; i++)
        {
            BoxCollider blockCollider = blockColliders[i];

            if (blockCollider.enabled == false || blockCollider.gameObject.activeInHierarchy == false)
            {
                continue;
            }

            for (int x = -1; x <= 1; x = x + 2)
            {
                for (int y = -1; y <= 1; y = y + 2)
                {
                    for (int z = -1; z <= 1; z = z + 2)
                    {
                        Vector3 corner = GetColliderCorner(blockCollider, x, y, z);

                        Vector3 localCorner = tableCollider.transform.InverseTransformPoint(corner);

                        if (localCorner.x < tableMinimum.x || localCorner.x > tableMaximum.x 
			    || localCorner.z < tableMinimum.z || localCorner.z > tableMaximum.z)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    bool OverlapsAnotherBlock(GameObject newBlock, BoxCollider[] blockColliders)
    {
        for (int i = 0; i < blockColliders.Length; i++)
        {
            BoxCollider blockCollider = blockColliders[i];

            if (blockCollider.enabled == false || blockCollider.gameObject.activeInHierarchy == false)
            {
                continue;
            }

            Vector3 colliderCenter = blockCollider.transform.TransformPoint(blockCollider.center);

            Vector3 scale = blockCollider.transform.lossyScale;

            scale.x = Mathf.Abs(scale.x);
            scale.y = Mathf.Abs(scale.y);
            scale.z = Mathf.Abs(scale.z);

            Vector3 halfSize = Vector3.Scale(blockCollider.size * 0.5f, scale);

            halfSize.x = Mathf.Max(0.0001f, halfSize.x - overlapPadding);
            halfSize.y = Mathf.Max(0.0001f, halfSize.y - overlapPadding);
            halfSize.z = Mathf.Max(0.0001f, halfSize.z - overlapPadding);

            Collider[] overlappingColliders = Physics.OverlapBox(colliderCenter, halfSize,
		blockCollider.transform.rotation, -1, QueryTriggerInteraction.Collide);

            for (int j = 0; j < overlappingColliders.Length; j++)
            {
                Collider otherCollider = overlappingColliders[j];

                if (otherCollider == tableCollider)
                {
                    continue;
                }

                if (otherCollider.transform == newBlock.transform)
                {
                    continue;
                }

                if (otherCollider.transform.IsChildOf(newBlock.transform))
                {
                    continue;
                }

                if (IsInsideBlockContainer(otherCollider.transform))
                {
                    return true;
                }
            }
        }

        return false;
    }

    Vector3 GetColliderCorner(BoxCollider blockCollider, int x, int y, int z)
    {
        Vector3 halfSize = blockCollider.size * 0.5f;

        Vector3 localCorner = blockCollider.center + new Vector3(halfSize.x * x, halfSize.y * y, halfSize.z * z);

        return blockCollider.transform.TransformPoint(localCorner);
    }

    bool IsInsideBlockContainer(Transform objectTransform)
    {
        if (objectTransform == blockContainer)
        {
            return true;
        }

        if (objectTransform.IsChildOf(blockContainer))
        {
            return true;
        }

        return false;
    }

    public void ClearSelection()
    {
        selectedBlockPrefab = null;

	deleteMode = false;
        if (deleteToggle != null)
        {
            deleteToggle.SetIsOnWithoutNotify(false);
        }

	if (infoPanel != null)
	{
    	    infoPanel.ShowGeneralInfo();
	}
    }

    public void SetDeleteMode(bool selected)
    {
        deleteMode = selected;

        if (selected)
        {
            selectedBlockPrefab = null;
            TurnOffBlockSelectionToggles();
            ResetConnectionClicks();
        }

	if (infoPanel != null)
	{
    	    if (selected)
    	    {
        	infoPanel.ShowDeleteInfo();
    	    }

            else
    	    {
        	infoPanel.ShowGeneralInfo();
    	    }
	}
     }

    void TurnOffBlockSelectionToggles()
    {
        if (blockToggleContainer == null)
        {
            return;
        }

        BlockSelectionToggle[] blockToggles = blockToggleContainer.GetComponentsInChildren<BlockSelectionToggle>(true);

        for (int i = 0; i < blockToggles.Length; i++)
        {
            Toggle blockToggle = blockToggles[i].GetComponent<Toggle>();

            if (blockToggle != null)
            {
                blockToggle.SetIsOnWithoutNotify(false);
            }
        }
    }

    void DeleteClickedBlock()
    {
        if (sceneCamera == null)
        {
            return;
        }

        Ray mouseRay = sceneCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(mouseRay, out hit, Mathf.Infinity, -1, QueryTriggerInteraction.Collide) == false)
        {
            return;
        }

        GameObject blockToDelete = GetClickedBlock(hit.collider.transform);

        if (blockToDelete == null)
        {
            return;
        }

        RemoveConnections(blockToDelete);
        ResetConnectionClicks();
        blockToDelete.SetActive(false);
        Destroy(blockToDelete);
    }

    GameObject GetClickedBlock(Transform clickedObject)
    {
        if (clickedObject == null || blockContainer == null)
        {
            return null;
        }

        Transform currentObject = clickedObject;

        while (currentObject.parent != null && currentObject.parent != blockContainer)
        {
            currentObject = currentObject.parent;
        }

        if (currentObject.parent != blockContainer)
        {
            return null;
        }

        Move moveScript = currentObject.GetComponentInChildren<Move>(true);
        if (moveScript == null)
        {
            return null;
        }

        return currentObject.gameObject;
    }

    void RemoveConnections(GameObject blockToDelete)
    {
        Output[] outputs = blockContainer.GetComponentsInChildren<Output>(true);

        for (int i = 0; i < outputs.Length; i++)
        {
            Output output = outputs[i];
            bool outputIsOnDeletedBlock = BelongsToBlock(output.transform, blockToDelete);
            bool connectedInputIsOnDeletedBlock = false;

            if (output.ConnectedTo != null)
            {
                connectedInputIsOnDeletedBlock = BelongsToBlock(output.ConnectedTo.transform, blockToDelete);
            }

            if (outputIsOnDeletedBlock || connectedInputIsOnDeletedBlock)
            {
                DisconnectOutput(output);
            }
        }

        InputS[] inputs = blockContainer.GetComponentsInChildren<InputS>(true);

        for (int i = 0; i < inputs.Length; i++)
        {
            InputS input = inputs[i];
            bool inputIsOnDeletedBlock = BelongsToBlock(input.transform, blockToDelete);
            bool connectedOutputIsOnDeletedBlock = false;

            if (input.ConnectedTo != null)
            {
                connectedOutputIsOnDeletedBlock = BelongsToBlock(input.ConnectedTo.transform,blockToDelete);
            }

            if (inputIsOnDeletedBlock || connectedOutputIsOnDeletedBlock)
            {
                DisconnectInput(input);
            }
        }
    }

    void DisconnectOutput(Output output)
    {
        if (output.ConnectedTo != null)
        {
            InputS connectedInput = output.ConnectedTo.GetComponent<InputS>();

            if (connectedInput != null)
            {
                connectedInput.Connected = false;
                connectedInput.ConnectedTo = null;
                connectedInput.InputValue = 0;
            }
        }
        output.Connected = false;
        output.ConnectedTo = null;
    }

    void DisconnectInput(InputS input)
    {
        if (input.ConnectedTo != null)
        {
            Output connectedOutput = input.ConnectedTo.GetComponent<Output>();

            if (connectedOutput != null)
            {
                connectedOutput.Connected = false;
                connectedOutput.ConnectedTo = null;
            }
        }

        input.Connected = false;
        input.ConnectedTo = null;
        input.InputValue = 0;
    }

    bool BelongsToBlock(Transform objectTransform, GameObject block)
    {
        if (objectTransform == null || block == null)
        {
            return false;
        }

        if (objectTransform == block.transform)
        {
            return true;
        }

        return objectTransform.IsChildOf(block.transform);
    }

    void ResetConnectionClicks()
    {
        GameObject clickManagerObject = GameObject.Find("InputOutputClickManager");

        if (clickManagerObject == null)
        {
            return;
        }

        ClickManager clickManager = clickManagerObject.GetComponent<ClickManager>();

        if (clickManager == null)
        {
            return;
        }

        clickManager.ClickedInput = null;
        clickManager.ClickedOutput = null;
        clickManager.CounterObject = null;
        clickManager.Counter = 0;
    }

    public void ClearTable()
    {
        selectedBlockPrefab = null;
        deleteMode = false;

        if (deleteToggle != null)
        {
            deleteToggle.SetIsOnWithoutNotify(false);
        }

        TurnOffBlockSelectionToggles();
        ResetConnectionClicks();

        Move[] moveScripts = blockContainer.GetComponentsInChildren<Move>(true);
        List<GameObject> blocks =new List<GameObject>();

        for (int i = 0; i < moveScripts.Length; i++)
        {
            GameObject block = GetClickedBlock(moveScripts[i].transform);

            if (block != null && blocks.Contains(block) == false)
            {
                blocks.Add(block);
            }
        }

        for (int i = 0; i < blocks.Count; i++)
        {
            RemoveConnections(blocks[i]);
        }

        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].SetActive(false);
            Destroy(blocks[i]);
        }

        Physics.SyncTransforms();
    }
}