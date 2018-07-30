using System.Collections.Generic;
using UnityEngine;

public class SimpleResetObject : MonoBehaviour, IResetObject
{
    private Vector3 startScale;

    private Vector3 startPos;

    private Quaternion startRot;

    private List<Component> startCompononts;

    private Rigidbody rigidBody;

    void Start ()
    {
        startScale = transform.localScale;
        startPos = transform.position;
        startRot = transform.rotation;

        startCompononts = new List<Component>(GetComponents<Component>());

        rigidBody = GetComponent<Rigidbody>();
    }


    public void resetObject()
    {
        transform.localScale = startScale;
        transform.position = startPos;
        transform.rotation = startRot;

        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;

        foreach (Component component in GetComponents<Component>())
            if(!startCompononts.Contains(component))
                Destroy(component);
    }
}
