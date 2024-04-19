using System.Collections.Generic;
using UnityEngine;
using System;

namespace Maroon.Physics.Motion
{
    public class Solver
    {
        private List<State> objects;
        private float dt;
        private int steps;
        private float t;
        private IIntegrator integrator;

        private String log;

        public Solver(float dt, int steps)
        {
            this.dt = dt;
            this.steps = steps;
            this.t = 0.0F;
            this.integrator = new RungeKutta4();
            this.objects = new List<State>();
        }

        public Solver(float dt, int steps, IIntegrator integrator)
        {
            this.dt = dt;
            this.steps = steps;
            this.t = 0.0F;
            this.integrator = integrator;
            this.objects = new List<State>();
        }

        public void AddObject(State o)
        {
            objects.Add(new State(o));
        }

        public void Solve()
        {
            for (int i = 0; i <= steps; i++)
            {
                Step(i);
            }
        }

        public void Step(int step)
        {
            foreach (State obj in objects)
            { 
                log += String.Format("{0:f5} {1:f5} {2:f5} {3:f5} {4:f5} {5:f5} {6:f5} \n", step*dt, obj.position.x, obj.position.y, obj.position.z, obj.velocity.x, obj.velocity.y, obj.velocity.z);
                integrator.Integrate(obj, t, dt);
            }
        }

        public void PrintLog()
        {
            Debug.Log(log);
        }
    }
}