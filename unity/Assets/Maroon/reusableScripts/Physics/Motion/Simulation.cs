using System.Collections.Generic;
using UnityEngine;
using System;

namespace Maroon.Physics.Motion
{
    public class Simulation
    {
        private List<SimulationEntity> objects;
        private float dt;
        private int current_step = 0;
        private float t = 0.0f;
        private IIntegrator integrator;

        private String log;

        public Simulation(float dt)
        {
            this.dt = dt;
            this.t = 0.0F;
            this.integrator = new RungeKutta4();
            this.objects = new List<SimulationEntity>();
        }

        public Simulation(float dt, IIntegrator integrator)
        {
            this.dt = dt;
            this.t = 0.0F;
            this.integrator = integrator;
            this.objects = new List<SimulationEntity>();
        }

        public void AddObject(SimulationEntity o)
        {
            objects.Add(o);
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
            foreach (SimulationEntity obj in objects)
            { 

                log += String.Format("{0:f5} {1:f5} {2:f5} {3:f5} {4:f5} {5:f5} {6:f5} \n", current_step*dt, obj.state.position.x, obj.state.position.y, obj.state.position.z, obj.state.velocity.x, obj.state.velocity.y, obj.state.velocity.z);
                integrator.Integrate(obj.state, t, dt);
            }
        }

        public void PrintLog()
        {
            Debug.Log(log);
        }
    }
}