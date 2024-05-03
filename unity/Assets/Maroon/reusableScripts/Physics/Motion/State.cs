﻿using UnityEngine;

namespace Maroon.Physics.Motion
{
    public class State
    {
        private double _t = 0.0;
        private Vector3d _position;
        private Vector3d _velocity;
        private Vector3d _acceleration;
        private Vector3d _force;
        private double _mass;

        private SimulationEntity _reference;

        public Vector3d position { get { return _position; } set { _position = value; } }
        public Vector3d velocity { get { return _velocity; } set { _velocity = value; } }
        public Vector3d acceleration { get { return _acceleration; } }

        public Vector3d Acceleration(double t)
        {
            _t = t;
            _mass = _reference.EvaluateMassAt(t);
            _force = _reference.EvaluateForceAt(t);
            _acceleration = _force * (1.0 / _mass);
            return _acceleration;
        }

        public State(Vector3d position, Vector3d velocity)
        {
            _position = position;
            _velocity = velocity;
        }

        public State(State state)
        {
            _position = state.position;
            _velocity = state.velocity;
        }

        public State(Vector3d position, Vector3d velocity, SimulationEntity refernce) : this (position, velocity)
        {
            _reference = refernce;
        }
    }
}