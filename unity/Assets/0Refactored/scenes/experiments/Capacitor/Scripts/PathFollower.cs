using UnityEngine;

public class PathFollower : PausableObject
{
    private IPath path;

    [SerializeField]
    public float maxForce;

    [SerializeField]
    public float mass;

    [SerializeField]
    public float maxSpeed;

    [SerializeField]
    public bool reverseOrder = false;

    [SerializeField]
    public bool followPath = true;

    private Vector3 velocity = Vector3.zero;

    private int currentNode = 0;

    private Vector3 previousTarget;
    private Vector3 currentTarget;

    private Vector3 PathFollowing()
    {
        if (path == null)
            return new Vector3();

        var nodes = path.GetNodes(reverseOrder);

        previousTarget = currentTarget;
        currentTarget = nodes[currentNode];

        if (Vector3.Distance(transform.position, currentTarget) <= 0.01f)
        {
            if (SimulationController.Instance.SimulationRunning)
                currentNode++;

            if (currentNode >= nodes.Count)
                followPath = false;
        }

        return Seek(currentTarget);
    }

    private Vector3 Seek(Vector3 target)
    {
        var desiredVelocity = Vector3.Normalize(target - transform.position) * maxSpeed;
        var steering = desiredVelocity - velocity;
        return steering;
    }

    public void SetPath(IPath path)
    {
        this.path = path;
        currentNode = 0;
    }
    protected override void Update()
    {
        HandleUpdate();
    }

    protected override void HandleUpdate()
    {
        if (!followPath)
            return;

        var steering = Vector3.zero;
        steering += PathFollowing();

        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering /= mass;

        if(SimulationController.Instance.SimulationRunning)
        {
            velocity = Vector3.ClampMagnitude(velocity + steering, maxSpeed);
            transform.position += velocity;
        }
        else
        {
            var direction = currentTarget - previousTarget;
            transform.position += direction;
        }
    }

    protected override void HandleFixedUpdate()
    {
        // not implemented
    }
}
