using System.Collections.Generic;
using UnityEngine;
using System;

namespace Maroon.Physics.Motion
{
    public class Simulation
    {
        private double _dt;
        private double _t0;
        private double _t;
        private int _steps;
        private int current_step = 0;
        private IIntegrator integrator;
        private Bounds _bounds = new Bounds();

        private List<SimulatedEntity> entities = new List<SimulatedEntity>();

        public double dt { get { return _dt; } set { _dt = value; } }
        public double t { get { return _t; } }
        public double t0 { get { return _t0; } set { _t0 = value; } }
        public int steps { get { return _steps; } set { _steps = value; } }
        public Bounds bounds { get { return _bounds; } }

        public Simulation()
        {
            integrator = new RungeKutta4();
        }

        public Simulation(IIntegrator integrator)
        {
            this.integrator = integrator;
        }

        public Simulation(IIntegrator integrator, double dt, double t0, int steps) : this(integrator)
        {
            _dt = dt;
            _t0 = t0;
            _steps = steps;
        }

        public Simulation(IIntegrator integrator, double dt, double t0, double duration) : this(integrator)
        {
            _dt = dt;
            _t0 = t0;
            this.steps = (int) (duration / dt);
        }

        public void AddEntity(SimulatedEntity entity)
        {
            entities.Add(entity);
        }

        public void InitializeEntities()
        {
            foreach (SimulatedEntity entity in entities)
            {
                entity.Initialize(t0, dt);
            }
        }

        public void Solve(int steps)
        {
            int initial_step = current_step;

            while(current_step++ < (initial_step + steps)) { 
                foreach (SimulatedEntity entity in entities)
                {
                    entity.state = integrator.Integrate(entity.state, t, dt);
                    entity.SaveState();
                }

                _t = current_step * dt;
            }


            foreach (SimulatedEntity entity in entities) {
                _bounds.Encapsulate(entity.bounds);
            }
        }

        public void Run()
        {
            _t = t0;

            InitializeEntities();
            Solve(this.steps);
        }
    }
}