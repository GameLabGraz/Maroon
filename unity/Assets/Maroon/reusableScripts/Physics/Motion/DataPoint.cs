namespace Maroon.Physics.Motion
{
    public class DataPoint
    {
        public double t { get; }
        public Vector3d position { get; }
        public Vector3d velocity { get; }
        public Vector3d acceleration { get; }
        public Vector3d force { get; }
        public double mass { get; }

        public double kinetic_energy { get; }
        public double power { get; }

        public DataPoint(double t, Vector3d position, Vector3d velocity, Vector3d force, double mass)
        {
            this.t = t;
            this.position = position;
            this.velocity = velocity;
            this.force = force;
            this.mass = mass;

            acceleration = force * (1.0 / mass);
            kinetic_energy = ((velocity.x * velocity.x) + (velocity.y * velocity.y) + (velocity.z * velocity.z)) * mass * 0.5;
            power = (velocity.x * force.x) + (velocity.y * force.y) + (velocity.z * force.z);
            // TODO: add work calculation
        }
    }
}