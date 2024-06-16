using System.Collections.Generic;
using UnityEngine;
using System;

namespace Maroon.Physics.Motion
{
    /// <summary>
    /// Simulates the movement of entities over time through space
    /// </summary>
    public class Solver
    {
        private double _dt;
        private double _t0;
        private double _t;
        private int _steps;
        private int current_step = 0;
        private IIntegrator integrator;
        private Bounds _bounds = new Bounds();

        private bool isInitialized = false;

        private List<MotionEntity> entities = new List<MotionEntity>();

        public double dt { get => _dt; set { _dt = value; } }
        public double t { get => _t; }
        public double t0 { get => _t0;  set { _t0 = value; } }
        public int steps { get => _steps; set { _steps = value; } }
        public Bounds bounds { get => _bounds; }

        public Solver()
        {
            integrator = new RungeKutta4();
        }

        public Solver(IIntegrator integrator)
        {
            this.integrator = integrator;
        }

        public Solver(IIntegrator integrator, double dt, double t0, int steps) : this(integrator)
        {
            _dt = dt;
            _t0 = t0;
            _steps = steps;
        }

        public Solver(IIntegrator integrator, double dt, double t0, double duration) : this(integrator)
        {
            _dt = dt;
            _t0 = t0;
            this.steps = (int) (duration / dt);
        }

        public void AddEntity(MotionEntity entity)
        {
            entities.Add(entity);
        }

        public void Initialize()
        {
            _t = t0;

            foreach (MotionEntity entity in entities)
            {
                entity.Initialize(t0, dt);
            }

            this.isInitialized = true;
        }

        public void Solve(int steps)
        {
            if (!isInitialized)
                Initialize();

            int start = current_step;

            while(current_step < (start + steps)) {
                foreach (MotionEntity entity in entities)
                {
                    entity.current_state = integrator.Integrate(entity.current_state, t, dt);
                    entity.SaveState();
                }

                current_step++;
                _t = current_step * dt;
            }

            foreach (MotionEntity entity in entities) {
                _bounds.Encapsulate(entity.Bounds);
            }

            this.steps = Math.Max(this.steps, current_step);
        }

        public void Solve()
        {
            Initialize();
            Solve(this.steps);
        }
    }
}