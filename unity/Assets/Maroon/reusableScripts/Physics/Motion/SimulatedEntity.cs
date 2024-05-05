using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.Physics;
using NCalc;
using System;
using System.Linq;

namespace Maroon.Physics.Motion
{
    public class SimulatedEntity
    {
        public State state;

        private Dictionary<String, Expression> _exprs = new Dictionary<String, Expression>();

        private List<State> states;
        private State _initial_state;

        private double _dt;
        private bool _isInitialized = false;
        private Bounds _bounds = new Bounds();

        public Bounds bounds { get { return _bounds; } }
        public bool isInitialized { get { return _isInitialized; } }


        public Vector3d EvaluateForceAt(double t)
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

        public double EvaluateMassAt(double t)
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

        public void SetInitialState(State s)
        {
            _initial_state = new State(s, this);
        }

        public void Initialize(double initial_t, double dt)
        {
            states = new List<State>();

            state = _initial_state;
            state.Acceleration(initial_t);
            state.CalculateEnergy();
            state.CalculatePower();

            states.Add(new State(state));

            _dt = dt;
            _isInitialized = true;
        }

        public void SaveState()
        {
            _bounds.Encapsulate((Vector3)state.position);
            
            double prev_power = states.Last().power;

            state.CalculateEnergyPowerWork(prev_power, _dt);
            states.Add(new State(state));
        }

        public void PrintDataPoints()
        {
            String log = "";
            foreach (var item in states)
            {
                log += String.Format("{0:f5} {1:f5} {2:f5} {3:f5} {4:f5} {5:f5} {6:f5} \n", item.t, item.position.x, item.position.y, item.position.z, item.velocity.x, item.velocity.y, item.velocity.z);
            }
            Debug.Log(log);
        }

        public SimulatedEntity()
        {
            _exprs["m"] = new Expression("1", EvaluateOptions.IgnoreCase);
            _exprs["fx"] = new Expression("0", EvaluateOptions.IgnoreCase);
            _exprs["fy"] = new Expression("0", EvaluateOptions.IgnoreCase);
            _exprs["fz"] = new Expression("0", EvaluateOptions.IgnoreCase);
        }

        public SimulatedEntity(Vector3d initialPosition, Vector3d initialVelocity) : this()
        {
            SetInitialState(new State(initialPosition, initialVelocity));
        }

        public SimulatedEntity(Vector3d initialPosition, Vector3d initialVelocity, String Fx, String Fy, String Fz) :
            this(initialPosition, initialVelocity)
        {
            _exprs["fx"] = new Expression(Fx, EvaluateOptions.IgnoreCase);
            _exprs["fx"].EvaluateParameter += MotionObject_EvaluateParameter;
            _exprs["fy"] = new Expression(Fy, EvaluateOptions.IgnoreCase);
            _exprs["fy"].EvaluateParameter += MotionObject_EvaluateParameter;
            _exprs["fz"] = new Expression(Fz, EvaluateOptions.IgnoreCase);
            _exprs["fz"].EvaluateParameter += MotionObject_EvaluateParameter;
        }
        public SimulatedEntity(Vector3d initialPosition, Vector3d initialVelocity, String Fx, String Fy, String Fz, String m) :
            this(initialPosition, initialVelocity, Fx, Fy, Fz)
        {
            _exprs["m"] = new Expression(m, EvaluateOptions.IgnoreCase);
            _exprs["m"].EvaluateParameter += MotionObject_EvaluateParameter;
        }
    }
}