using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.Physics;
using NCalc;
using System;

namespace Maroon.Physics.Motion {
    public class State
    {
        public Vector3 position { get; set; }
        public Vector3 velocity { get; set; }

        public Vector3 acceleration(float t)
        {
            return model.acceleration(this, t);
        }

        private Model model;

        public State(Vector3 position, Vector3 velocity, Model m)
        {
            this.position = position;
            this.velocity = velocity;
            this.model = m;
        }
    }

    public class Model
    {
        private Expression[] forces;
        private Expression mass;

        public Vector3 acceleration(State s, float t)
        {
            mass = AddParametersToExpression(mass, s, t);
            float m = (float) Convert.ToDouble(mass.Evaluate());

            for (int i = 0; i < forces.Length; i++)
            {
                forces[i] = AddParametersToExpression(forces[i], s, t);
                forces[i].Parameters["m"] = m;
            }
            
            return new Vector3(
                (float) Convert.ToDouble(forces[0].Evaluate()),
                (float) Convert.ToDouble(forces[1].Evaluate()),
                (float) Convert.ToDouble(forces[2].Evaluate())
            );
        }

        public Model(string Fx, string Fy, string Fz, string mass)
        {
            this.forces = new[] { 
                new Expression(Fx, EvaluateOptions.IgnoreCase), 
                new Expression(Fy, EvaluateOptions.IgnoreCase),
                new Expression(Fz, EvaluateOptions.IgnoreCase) 
            };

            this.mass = new Expression(mass, EvaluateOptions.IgnoreCase);
        }

        private Expression AddParametersToExpression(Expression expr, State s, float t)
        {
            expr.Parameters["x"] = s.position.x;
            expr.Parameters["y"] = s.position.y;
            expr.Parameters["z"] = s.position.z;
            expr.Parameters["vx"] = s.velocity.x;
            expr.Parameters["vy"] = s.velocity.y;
            expr.Parameters["vz"] = s.velocity.z;

            expr.Parameters["t"] = t;

            return expr;
        }
    }

    public interface IIntegrator
    {
        public State Integrate(State s, float t, float dt);
    }

    public class ExplicitEuler : IIntegrator
    {
        public State Integrate(State s, float t, float dt)
        {
            s.position += s.velocity * dt;
            s.velocity += s.acceleration(t + dt) * dt;

            return s;
        }
    }

    public class SemiImplicitEuler : IIntegrator
    {
        public State Integrate(State s, float t, float dt)
        {
            s.velocity += s.acceleration(t + dt) * dt;
            s.position += s.velocity * dt;

            return s;
        }
    }

    public class RungeKutta4 : IIntegrator
    {
        public State Integrate(State s, float t, float dt)
        {
            State initial = s;

            var k1 = Evaluate(initial, t, 0.0f, new Derivative());
            var k2 = Evaluate(initial, t, dt * 0.5f, k1);
            var k3 = Evaluate(initial, t, dt * 0.5f, k2);
            var k4 = Evaluate(initial, t, dt, k3);

            var dxdt = 1.0f / 6.0f * (k1.dx + 2.0f * k2.dx + 2.0f * k3.dx + k4.dx);
            var dvdt = 1.0f / 6.0f * (k1.dv + 2.0f * k2.dv + 2.0f * k3.dv + k4.dv);

            s.position += dxdt * dt;
            s.velocity += dvdt * dt;

            return s;
        }

        private Derivative Evaluate(State s, float t, float dt, Derivative d)
        {
            return new Derivative(s.velocity + d.dv * dt, s.acceleration(t + dt));
        }

        private class Derivative
        {
            public Vector3 dx; // velocity
            public Vector3 dv; // acceleration

            public Derivative(Vector3 dx, Vector3 dv)
            {
                this.dx = dx;
                this.dv = dv;
            }

            public Derivative()
            {

            }
        }
    }

    public class MotionSimulation
    {
        private List<State> objects;
        private float dt;
        private int steps;
        private float t;
        private IIntegrator integrator;

        private String log;

        public MotionSimulation(float dt, int steps)
        {
            this.dt = dt;
            this.steps = steps;
            this.t = 0.0F;
            this.integrator = new RungeKutta4();
            this.objects = new List<State>();
        }

        public void AddObject(State o)
        {
            objects.Add(o);
        }

        public void Solve()
        {
            for (int i = 0; i < steps; i++)
            {
                Step(i);
            }
        }

        public void Step(int step)
        {
            foreach (State obj in objects)
            { 
                //Debug.Log(step);
                log += String.Format("{0:f5} {1:f5} {2:f5} {3:f5} {4:f5} {5:f5} {6:f5} \n", step*dt, obj.position.x, obj.position.y, obj.position.z, obj.velocity.x, obj.velocity.y, obj.velocity.z);
                integrator.Integrate(obj, t, dt);
            }
        }

        public void PrintLog()
        {
            Debug.Log(log);
        }
    }

    public class Tests
    {
        public static void TestSimulation()
        {
            Debug.Log("HOI IM TESTING");
            var model = new Model("-4*vx", "-4*vy", "-4*vz - m*9.807", "1");
            var state = new State(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 10.0f), model);

            var sim = new MotionSimulation(0.01f, 200);
            sim.AddObject(state);
            sim.Solve();

            sim.PrintLog();
        }
    }
}