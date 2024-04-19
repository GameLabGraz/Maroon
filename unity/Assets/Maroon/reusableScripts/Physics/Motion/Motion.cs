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
}