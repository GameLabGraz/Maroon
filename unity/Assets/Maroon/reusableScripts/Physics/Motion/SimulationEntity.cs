using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.Physics;
using NCalc;
using System;

namespace Maroon.Physics.Motion
{
    public class SimulationEntity
    {
        public State state;
        private Dictionary<String, Expression> _exprs = new Dictionary<String, Expression>();

        public Vector3d EvaluateForceAt(float t)
        {
            _exprs["fx"].Parameters["t"] = t;
            _exprs["fy"].Parameters["t"] = t;
            _exprs["fz"].Parameters["t"] = t;

            return new Vector3d(
                Convert.ToDouble(_exprs["fx"].Evaluate()),
                Convert.ToDouble(_exprs["fy"].Evaluate()),
                Convert.ToDouble(_exprs["fz"].Evaluate())
            );
        }

        public double EvaluateMassAt(float t)
        {
            _exprs["m"].Parameters["t"] = t;
            return Convert.ToDouble(_exprs["m"].Evaluate());
        }

        public void AddExpressions(String name, String expression)
        {
            _exprs[name] = new Expression(expression, EvaluateOptions.IgnoreCase);
            _exprs[name].EvaluateParameter += MotionObject_EvaluateParameter;
        }

        private void MotionObject_EvaluateParameter(string param, ParameterArgs args)
        {
            var name = param.ToLower();

            switch (name)
            {
                case "x": args.Result = state.position.x; break;
                case "y": args.Result = state.position.y; break;
                case "z": args.Result = state.position.z; break;
                case "vx": args.Result = state.velocity.x; break;
                case "vy": args.Result = state.velocity.y; break;
                case "vz": args.Result = state.velocity.z; break;
                default:
                    if (_exprs.ContainsKey(name))
                    {
                        args.Result = _exprs[name].Evaluate(); 
                        break;
                    }

                    throw new ArgumentException("Unknown Parameter", name);
            }
        }

        public SimulationEntity() {
            _exprs["m"] = new Expression("1", EvaluateOptions.IgnoreCase);
            _exprs["fx"] = new Expression("0", EvaluateOptions.IgnoreCase);
            _exprs["fy"] = new Expression("0", EvaluateOptions.IgnoreCase);
            _exprs["fz"] = new Expression("0", EvaluateOptions.IgnoreCase);
        }

        public SimulationEntity(SimulationEntity mo)
        {
            this.state = new State(mo.state, this);
            this._exprs = mo._exprs;
        }

        public SimulationEntity(State initialState) : 
            this()
        {
            this.state = new State(initialState, this);
        }

        public SimulationEntity(Vector3 initialPosition, Vector3 initialVelocity) :
            this(new State(initialPosition, initialVelocity))
        {
        }

        public SimulationEntity(Vector3 initialPosition, Vector3 initialVelocity, String Fx, String Fy, String Fz) :
            this(new State(initialPosition, initialVelocity))
        {
            _exprs["fx"] = new Expression(Fx, EvaluateOptions.IgnoreCase);
            _exprs["fx"].EvaluateParameter += MotionObject_EvaluateParameter;
            _exprs["fy"] = new Expression(Fy, EvaluateOptions.IgnoreCase);
            _exprs["fy"].EvaluateParameter += MotionObject_EvaluateParameter;
            _exprs["fz"] = new Expression(Fz, EvaluateOptions.IgnoreCase);
            _exprs["fz"].EvaluateParameter += MotionObject_EvaluateParameter;
        }
        public SimulationEntity(Vector3 initialPosition, Vector3 initialVelocity, String Fx, String Fy, String Fz, String m) :
            this(initialPosition, initialVelocity, Fx, Fy, Fz)
        {
            _exprs["m"] = new Expression(m, EvaluateOptions.IgnoreCase);
            _exprs["m"].EvaluateParameter += MotionObject_EvaluateParameter;
        }
    }
}