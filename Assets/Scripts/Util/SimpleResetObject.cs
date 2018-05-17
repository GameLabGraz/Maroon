using UnityEngine;

public class SimpleResetObject : MonoBehaviour, IResetObject
{
    private Vector3 startPos;

    private Quaternion startRot;

    private Rigidbody rigidBody;

    void Start ()
    {
        startPos = transform.position;
        startRot = transform.rotation;

        rigidBody = GetComponent<Rigidbody>();
    }


    public void resetObject()
    {
        transform.position = startPos;
        transform.rotation = startRot;

        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
    }
}
