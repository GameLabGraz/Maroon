using UnityEngine;

namespace Maroon.Physics.Motion
{
    public class State
    {
        internal Vector3d position;
        internal Vector3d velocity;
        internal Vector3d acceleration;
        internal Vector3d force;

        internal double t;
        internal double mass;
        internal double kinetic_energy;
        internal double power;
        internal double work;

        private SimulatedEntity entity;

        public double Time { get => t; }
        public Vector3 Position { get => (Vector3) position; }
        public Vector3 Velocity { get => (Vector3) velocity; }
        public Vector3 Acceleration { get => (Vector3) acceleration; }
        public Vector3 Force { get => (Vector3) force; }
        public double Mass { get => mass; }
        public double KineticEnergy { get => kinetic_energy; }
        public double Power { get => power; }
        public double Work { get => work; }

        public Vector3d EvaluateAccelerationAt(double t)
        {
            this.t = t;
            mass = entity.EvaluateMassAt(t);
            force = entity.EvaluateForceAt(t);
            acceleration = force * (1.0 / mass);
            return acceleration;
        }


        public void CalculateEnergyPowerWork(double prev_power, double dt)
        {
            CalculateEnergy();
            CalculatePower();
            CalculateWork(prev_power, dt);
        }

        public void CalculateEnergy()
        {
            kinetic_energy = ((velocity.x * velocity.x) + (velocity.y * velocity.y) + (velocity.z * velocity.z)) * mass * 0.5;
        }

        public void CalculatePower()
        {
            power = (velocity.x * force.x) + (velocity.y * force.y) + (velocity.z * force.z);
        }

        public void CalculateWork(double prev_power, double dt)
        {
            work += 0.5 * (power + prev_power) * dt;
        }

        public State(State state)
        {
            t = state.t;
            position = state.position;
            velocity = state.velocity;
            force = state.force;
            mass = state.mass;
            acceleration = state.acceleration;
            kinetic_energy = state.kinetic_energy;
            power = state.power;
            work = state.work;

            entity = state.entity;
        }

        public State(State state, SimulatedEntity entity) : this(state)
        {
            this.entity = entity;
            this.entity.current_state = this;
        }

        public State(State state, bool update_entity) : this(state)
        {
            if (update_entity && entity != null)
                entity.current_state = this;
        }

        public State(Vector3 position, Vector3 velocity, SimulatedEntity entity)
        {
            this.position = new Vector3d(position);
            this.velocity = new Vector3d(velocity);
            this.entity = entity;
        }


        public State(Vector3d position, Vector3d velocity, SimulatedEntity entity)
        {
            this.position = position;
            this.velocity = velocity;
            this.entity = entity;
        }
    }
}