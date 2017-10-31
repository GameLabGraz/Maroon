using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    private IPath path;

    [SerializeField]
    private bool followPath;

    [SerializeField]
    private float maxForce;

    [SerializeField]
    private float mass;

    [SerializeField]
    private float maxSpeed;

    private Vector3 velocity = Vector3.zero;

    private int currentNode;
	
	private void Update ()
    {
        if (!followPath)
            return;

        Vector3 steering = Vector3.zero;
        steering += PathFollowing();

        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering /= mass;

        velocity = Vector3.ClampMagnitude(velocity + steering, maxSpeed);
        transform.position += velocity;
    }

    private Vector3 PathFollowing()
    {
        if (path == null)
            return new Vector3();

        List<Vector3> nodes = path.GetNodes();
        Vector3 target = nodes[currentNode];

        if (Vector3.Distance(transform.position, target) <= 0.5f)
        {
            currentNode++;
            if (currentNode >= nodes.Count)
                currentNode = nodes.Count - 1;
        }

        return Seek(target);
    }

    private Vector3 Seek(Vector3 target)
    {
        Vector3 desiredVelocity = Vector3.Normalize(target - transform.position) * maxSpeed;
        Vector3 steering = desiredVelocity - velocity;
        return steering;
    }

    public void SetPath(IPath path)
    {
        this.path = path;
    }
}
