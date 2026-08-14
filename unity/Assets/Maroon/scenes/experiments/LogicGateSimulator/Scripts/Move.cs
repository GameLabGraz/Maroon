using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    static List<Move> moveScripts = new List<Move>();
    static Move draggedBlock;

    public Transform objectToMove;
    public BoxCollider movementCollider;
    public bool includeChildBoxColliders = true;

    public TableSurface tableSurface;
    public Camera inputCamera;

    public LayerMask selectableLayers = -1;
    public LayerMask obstacleLayers = -1;
    public bool onlyBlockAgainstOtherMovableBlocks = true;
    public float collisionSkin = 0.005f;

    Plane dragPlane;
    Vector3 mouseOffset;
    bool isDragging = false;
    bool showedError = false;
    BoxCollider[] blockColliders;

    // Start is called before the first frame update
    void Start()
    {
        FindReferences();
    }

    void OnEnable()
    {
        if (!moveScripts.Contains(this))
        {
            moveScripts.Add(this);
        }
    }

    void OnDisable()
    {
        moveScripts.Remove(this);

        if (draggedBlock == this)
        {
            draggedBlock = null;
        }

        isDragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartMoving();
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            MoveBlock();
        }

        if (isDragging && Input.GetMouseButtonUp(0))
        {
            StopMoving();
        }
    }

    void FindReferences()
    {
        if (objectToMove == null)
        {
            if (transform.name == "Base" && transform.parent != null)
            {
                objectToMove = transform.parent;
            }
            else
            {
                objectToMove = transform;
            }
        }

        if (movementCollider == null)
        {
            movementCollider = FindLargestCollider();
        }

        if (includeChildBoxColliders)
        {
            blockColliders = objectToMove.GetComponentsInChildren<BoxCollider>(true);
        }
        else
        {
            blockColliders = new BoxCollider[1];
            blockColliders[0] = movementCollider;
        }

        if (tableSurface == null)
        {
            tableSurface = FindObjectOfType<TableSurface>();
        }

        if (inputCamera == null)
        {
            inputCamera = Camera.main;
        }

        if (collisionSkin < 0f)
        {
            collisionSkin = 0f;
        }
    }

    void StartMoving()
    {
        if (draggedBlock != null)
        {
            return;
        }

        FindReferences();

        if (!ReferencesAreValid())
        {
            return;
        }

        Ray mouseRay = inputCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        bool hitSomething = Physics.Raycast(mouseRay, out hit, Mathf.Infinity,
	    selectableLayers, QueryTriggerInteraction.Ignore);

        if (!hitSomething)
        {
            return;
        }

        if (!ColliderBelongsToThisBlock(hit.collider))
        {
            return;
        }

        Vector3 tableNormal = tableSurface.GetSurfaceNormal();

        dragPlane = new Plane(tableNormal, objectToMove.position);

        float distanceToPlane;

        if (!dragPlane.Raycast(mouseRay, out distanceToPlane))
        {
            return;
        }

        Vector3 mousePositionOnPlane = mouseRay.GetPoint(distanceToPlane);

        mouseOffset = objectToMove.position - mousePositionOnPlane;

        isDragging = true;
        draggedBlock = this;
    }

    void MoveBlock()
    {
        Ray mouseRay = inputCamera.ScreenPointToRay(Input.mousePosition);

        float distanceToPlane;

        if (!dragPlane.Raycast(mouseRay, out distanceToPlane))
        {
            return;
        }

        Vector3 mousePositionOnPlane = mouseRay.GetPoint(distanceToPlane);

        Vector3 wantedPosition = mousePositionOnPlane + mouseOffset;

        Vector3 wantedMovement = wantedPosition - objectToMove.position;

        Vector3 tableNormal = tableSurface.GetSurfaceNormal();

        wantedMovement = Vector3.ProjectOnPlane(wantedMovement, tableNormal);

        wantedMovement = tableSurface.ClampMovement(blockColliders, wantedMovement);

        wantedMovement = StopAtOtherBlocks(wantedMovement);

        if (wantedMovement.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        objectToMove.position = objectToMove.position + wantedMovement;

        Physics.SyncTransforms();
    }

    void StopMoving()
    {
        isDragging = false;

        if (draggedBlock == this)
        {
            draggedBlock = null;
        }
    }

    Vector3 StopAtOtherBlocks(Vector3 wantedMovement)
    {
        float wantedDistance = wantedMovement.magnitude;

        if (wantedDistance <= Mathf.Epsilon)
        {
            return Vector3.zero;
        }

        Vector3 direction = wantedMovement / wantedDistance;

        float allowedDistance = wantedDistance;

        foreach (BoxCollider boxCollider in blockColliders)
        {
            if (!ColliderCanBeUsed(boxCollider))
            {
                continue;
            }

            Vector3 center = boxCollider.transform.TransformPoint(boxCollider.center);

            Vector3 scale = MakePositive(boxCollider.transform.lossyScale);

            Vector3 halfSize = Vector3.Scale(boxCollider.size * 0.5f, scale);

            RaycastHit[] hits = Physics.BoxCastAll(center, halfSize, direction,
                boxCollider.transform.rotation, allowedDistance + collisionSkin,
                obstacleLayers, QueryTriggerInteraction.Collide);

            foreach (RaycastHit hit in hits)
            {
                if (ColliderBlocksMovement(hit.collider))
                {
                    float distanceBeforeCollider = hit.distance - collisionSkin;

                    if (distanceBeforeCollider < 0f)
                    {
                        distanceBeforeCollider = 0f;
                    }

                    if (distanceBeforeCollider < allowedDistance)
                    {
                        allowedDistance = distanceBeforeCollider;
                    }
                }
            }
        }
        return direction * allowedDistance;
    }

    bool ColliderBlocksMovement(Collider otherCollider)
    {
        if (otherCollider == null)
        {
            return false;
        }

        if (ColliderBelongsToThisBlock(otherCollider))
        {
            return false;
        }

        if (tableSurface != null)
        {
            if (otherCollider == tableSurface.surfaceCollider)
            {
                return false;
            }
        }

        if (!onlyBlockAgainstOtherMovableBlocks)
        {
            return true;
        }

        foreach (Move otherMoveScript in moveScripts)
        {
            if (otherMoveScript == null)
            {
                continue;
            }

            if (otherMoveScript == this)
            {
                continue;
            }

            if (otherMoveScript.objectToMove == objectToMove)
            {
                continue;
            }

            if (otherMoveScript.ColliderBelongsToThisBlock(otherCollider))
            {
                return true;
            }
        }

        return false;
    }

    bool ColliderBelongsToThisBlock(Collider colliderToCheck)
    {
        if (colliderToCheck == null)
        {
            return false;
        }

        Transform colliderTransform = colliderToCheck.transform;

        if (colliderTransform == objectToMove)
        {
            return true;
        }

        if (colliderTransform.IsChildOf(objectToMove))
        {
            return true;
        }

        return false;
    }

    bool ReferencesAreValid()
    {
        bool valid = true;

        if (objectToMove == null)
        {
            valid = false;
        }

        if (movementCollider == null)
        {
            valid = false;
        }

        if (blockColliders == null)
        {
            valid = false;
        }
        else if (blockColliders.Length == 0)
        {
            valid = false;
        }

        if (tableSurface == null)
        {
            valid = false;
        }
        else if (tableSurface.surfaceCollider == null)
        {
            valid = false;
        }

        if (inputCamera == null)
        {
            valid = false;
        }

        if (!valid && !showedError)
        {
            Debug.LogError("Move needs a block BoxCollider, a " +
                "TableSurface with a BoxCollider, and " +
                "a camera tagged MainCamera.", this);

            showedError = true;
        }

        return valid;
    }

    bool ColliderCanBeUsed(BoxCollider boxCollider)
    {
        if (boxCollider == null)
        {
            return false;
        }

        if (!boxCollider.enabled)
        {
            return false;
        }

        if (!boxCollider.gameObject.activeInHierarchy)
        {
            return false;
        }

        if (!ColliderBelongsToThisBlock(boxCollider))
        {
            return false;
        }

        return true;
    }

    BoxCollider FindLargestCollider()
    {
        BoxCollider[] colliders = objectToMove.GetComponentsInChildren<BoxCollider>(true);

        BoxCollider largestCollider = null;
        float largestVolume = -1f;

        foreach (BoxCollider boxCollider in colliders)
        {
            if (boxCollider.isTrigger)
            {
                continue;
            }

            Vector3 scale = MakePositive(boxCollider.transform.lossyScale);

            Vector3 size = Vector3.Scale(boxCollider.size, scale);

            float volume = size.x * size.y * size.z;

            if (volume > largestVolume)
            {
                largestCollider = boxCollider;
                largestVolume = volume;
            }
        }

        return largestCollider;
    }

    Vector3 MakePositive(Vector3 value)
    {
        Vector3 positiveValue = new Vector3();

        positiveValue.x = Mathf.Abs(value.x);
        positiveValue.y = Mathf.Abs(value.y);
        positiveValue.z = Mathf.Abs(value.z);

        return positiveValue;
    }
}