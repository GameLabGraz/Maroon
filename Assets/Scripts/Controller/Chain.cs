using System;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour, IPath, IResetObject
{
    [SerializeField]
    private Vector3 chainAxis = Vector3.up;

    [SerializeField]
    private Vector3 linkScale = Vector3.one;

    [SerializeField]
    private Vector3 linkOffset = Vector3.zero;

    [SerializeField]
    private Vector3 jointAxis = Vector3.right;

    [SerializeField]
    private Vector3 jointSwingAxis = Vector3.up;

    [SerializeField]
    private float jointSwingLimit = 0;

    [SerializeField]
    private int linkCount = 0;

    [SerializeField]
    private GameObject customLinkObject;

    [SerializeField, HideInInspector]
    private List<GameObject> LinkObjects = new List<GameObject>();

    private List<Vector3> startLinkPositions = new List<Vector3>();
    private List<Quaternion> startLinkRotations = new List<Quaternion>();

    private void Start()
    {
        foreach(GameObject linkObject in LinkObjects)
        {
            startLinkPositions.Add(linkObject.transform.position);
            startLinkRotations.Add(linkObject.transform.rotation);
        }
    }


    public void UpdateNumberOfLinks()
    {
        if (linkCount > LinkObjects.Count)
            CreateLinkObjects(linkCount - LinkObjects.Count);
        else if (linkCount < LinkObjects.Count)
            RemoveLinkObjects(LinkObjects.Count - linkCount);
    }

    private void UpdateLinkPosition(GameObject linkObject, GameObject previousLink = null)
    {
        if (LinkObjects.Count == 0)
            linkObject.transform.localPosition = Vector3.zero;

        if (!previousLink)
        {
            int linkIndex = LinkObjects.IndexOf(linkObject);

            if (linkIndex <= 0)
                return;

            previousLink = LinkObjects[linkIndex - 1];
        }

        Vector3 size = GetLinkSize(linkObject);
        linkObject.transform.position = previousLink.transform.position - size * 0.5f - linkOffset;
    }

    private Vector3 GetLinkSize(GameObject linkObject, bool localScale = true)
    {
        Vector3 size = Vector3.Scale(linkObject.GetComponent<MeshFilter>().sharedMesh.bounds.size, chainAxis);
        if (localScale)
            size = Vector3.Scale(size, linkObject.transform.localScale);
        return size;
    }

    public void UpdateCustomLinkObjects()
    {
        ClearLinkObjects();
        CreateLinkObjects(linkCount);
    }

    public void UpdateSettings()
    {
        foreach (GameObject linkObject in LinkObjects)
        {
            linkObject.transform.localScale = linkScale;
            UpdateLinkPosition(linkObject);
            UpdateJointSettings(linkObject);
        }
    }

    private void UpdateJointSettings(GameObject linkObject)
    {
        CharacterJoint joint = linkObject.GetComponent<CharacterJoint>();

        joint.anchor = Vector3.Scale(chainAxis, GetLinkSize(linkObject, false)) * 0.25f + linkOffset * 0.5f;
        joint.axis = jointAxis;
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = -joint.anchor;
        joint.swingAxis = jointSwingAxis;
        SoftJointLimit limit = new SoftJointLimit()
        {
            limit = this.jointSwingLimit,
            bounciness = 0,
            contactDistance = 0
        };
        joint.swing1Limit = limit;
        joint.swing2Limit = limit;
    }

    private void RemoveLinkObjects(int removeCount)
    {
        for (int i = 0; i < removeCount; i++)
        {
            if (LinkObjects.Count == 0)
                return;

            GameObject toRemove = LinkObjects[LinkObjects.Count - 1];
            LinkObjects.Remove(toRemove);
            DestroyImmediate(toRemove);
        }
    }

    private void CreateLinkObjects(int createCount)
    {
        GameObject previousLink = null;
        if (LinkObjects.Count > 0)
            previousLink = LinkObjects[LinkObjects.Count - 1];

        for (int i = 0; i < createCount; i++)
        {
            GameObject linkObject;
            if (customLinkObject != null)
                linkObject = GameObject.Instantiate(customLinkObject);
            else
                linkObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);

            linkObject.name = string.Format("Link{0}", LinkObjects.Count);
            linkObject.transform.parent = transform;
            linkObject.transform.localScale = linkScale;
            UpdateLinkPosition(linkObject, previousLink);

            CharacterJoint joint = linkObject.AddComponent<CharacterJoint>();
            UpdateJointSettings(linkObject);

            if (previousLink)
                joint.connectedBody = previousLink.GetComponent<Rigidbody>();

            LinkObjects.Add(linkObject);
            previousLink = linkObject;
        }
    }

    private void ClearLinkObjects()
    {
        while (LinkObjects.Count > 0)
        {
            GameObject toRemove = LinkObjects[0];
            LinkObjects.Remove(toRemove);
            DestroyImmediate(toRemove);
        }
    }

    public List<Vector3> GetNodes(bool reverseOrder = false)
    {
        List<Vector3> path = new List<Vector3>();

        foreach (GameObject linkObject in LinkObjects)
            path.Add(linkObject.transform.position);

        if (reverseOrder)
            path.Reverse();

        return path;
    }

    public void resetObject()
    {
        for(int i = 0; i < LinkObjects.Count; i++)
        {
            LinkObjects[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
            LinkObjects[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            LinkObjects[i].transform.position = startLinkPositions[i];
            LinkObjects[i].transform.rotation = startLinkRotations[i];
        }  
    }
}
