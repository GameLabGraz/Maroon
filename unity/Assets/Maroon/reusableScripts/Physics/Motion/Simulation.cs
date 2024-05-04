using System.Collections.Generic;
using UnityEngine;
using System;

namespace Maroon.Physics.Motion
{
    public class Simulation
    {
        private double dt;
        private double t;
        private int steps;
        private int current_step = 0;
        private IIntegrator integrator;

        private List<SimulatedEntity> entities = new List<SimulatedEntity>();

        private Bounds bounds;
        private String log;

        public Simulation(double dt, double t, int steps, IIntegrator integrator)
        {
            this.dt = dt;
            this.t = t;
            this.steps = steps;
            this.integrator = integrator;
        }

        public Simulation(double dt, double t, double duration, IIntegrator integrator)
        {
            this.dt = dt;
            this.t = t;
            this.steps = (int) (duration / dt);
            this.integrator = integrator;
        }

        public void AddEntity(SimulatedEntity entity)
        {
            entity.Initialize(t, dt);
            entities.Add(entity);
        }

        public void Solve(int steps)
        {
            int initial_step = current_step;

            while(current_step++ < (initial_step + steps)) { 
                foreach (SimulatedEntity entity in entities)
                {
                    entity.state = integrator.Integrate(entity.state, t, dt);
                    entity.PushBackState();
                }

                t = current_step * dt;
            }
        }

        public void Solve()
        {
            Solve(this.steps);
        }
    }
}