using System.Collections.Generic;
using UnityEngine;
using System;

namespace Maroon.Physics.Motion
{
    public class Simulation
    {
        private List<SimultaedEntity> entities;
        private double dt;
        private int current_step = 0;
        private double t = 0.0;
        private IIntegrator integrator;

        private String log;

        public Simulation(double dt)
        {
            this.dt = dt;
            this.t = 0.0;
            this.integrator = new RungeKutta4();
            this.entities = new List<SimultaedEntity>();
        }

        public Simulation(double dt, IIntegrator integrator)
        {
            this.dt = dt;
            this.t = 0.0;
            this.integrator = integrator;
            this.entities = new List<SimultaedEntity>();
        }

        public void AddEntity(SimultaedEntity o)
        {
            entities.Add(o);
        }

        public void Solve(int steps)
        {
            current_step = 0;

            for (; current_step <= steps; current_step++)
            {
                Step();
            }
        }

        public void Step()
        {
            foreach (SimultaedEntity entity in entities)
            { 
                integrator.Integrate(entity.state, t, dt);
            }
        }

        public void PrintLog()
        {
            Debug.Log(log);
        }
    }
}