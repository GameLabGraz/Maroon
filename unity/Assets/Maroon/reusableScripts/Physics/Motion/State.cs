using UnityEngine;

namespace Maroon.Physics.Motion
{
    public class State
    {
        private double _t;
        private Vector3d _position;
        private Vector3d _velocity;
        private Vector3d _acceleration;
        private Vector3d _force;
        private double _mass;
        private double _kinetic_energy;
        private double _power;
        private double _work;

        private SimulatedEntity entity;


        public double t { get { return _t; } }
        public Vector3d position { get { return _position; } set { _position = value; } }
        public Vector3d velocity { get { return _velocity; } set { _velocity = value; } }
        public Vector3d acceleration { get { return _acceleration; } }
        public Vector3d force { get { return _force; } }
        public double mass { get { return _mass; } }
        public double kinetic_energy { get { return ((velocity.x * velocity.x) + (velocity.y * velocity.y) + (velocity.z * velocity.z)) * mass * 0.5; } }
        public double power { get { return (velocity.x * force.x) + (velocity.y * force.y) + (velocity.z * force.z); } }

        public Vector3d Acceleration(double t)
        {
            _t = t;
            _mass = entity.EvaluateMassAt(t);
            _force = entity.EvaluateForceAt(t);
            _acceleration = force * (1.0 / mass);
            return acceleration;
        }

        public void UpdateEnergies(double prev_power, double dt)
        {
            _kinetic_energy = ((velocity.x * velocity.x) + (velocity.y * velocity.y) + (velocity.z * velocity.z)) * mass * 0.5;
            _power = (velocity.x * force.x) + (velocity.y * force.y) + (velocity.z * force.z);
            _work += 0.5 * (_power + prev_power) * dt;
        }


        public State(Vector3d position, Vector3d velocity) 
        {
            _position = position;
            _velocity = velocity;
        }

        public State(State state)
        {
            _t = state.t;
            _position = state.position;
            _velocity = state.velocity;
            _force = state.force;
            _mass = state.mass;
            _acceleration = state.acceleration;

            entity = state.entity;
        }

        public State(State state, SimulatedEntity entity) : this(state)
        {
            this.entity = entity;
            this.entity.state = this;
        }

        public State(State state, bool update_entity) : this(state)
        {
            if (update_entity && entity != null)
                entity.state = this;
        }

        public State(Vector3d position, Vector3d velocity, SimulatedEntity entity) : this (position, velocity)
        {
            this.entity = entity;
        }

    }
}