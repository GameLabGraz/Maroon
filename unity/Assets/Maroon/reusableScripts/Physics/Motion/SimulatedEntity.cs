using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.Physics;
using NCalc;
using System;
using System.Linq;
using System.Text.RegularExpressions;

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
            return new Vector3d(
                Convert.ToDouble(_exprs["fx"].Evaluate()),
                Convert.ToDouble(_exprs["fy"].Evaluate()),
                Convert.ToDouble(_exprs["fz"].Evaluate())
            );
        }

        public double EvaluateMassAt(double t)
        {
            return Convert.ToDouble(_exprs["m"].Evaluate());
        }

        public void AddExpression(String signature, String expression)
        {
            var expr = new Expression(expression, EvaluateOptions.IgnoreCase);
            expr.EvaluateParameter += Expression_EvaluateParameter;
            expr.EvaluateFunction += Expression_EvaluateFunction;


            (String name, List<String> parameters) = SplitSignature(signature);

            if (parameters != null)
            {
                foreach (String parameter in parameters)
                {
                    expr.Parameters[parameter] = null;
                }
            }

            _exprs[name] = expr;
        }

        private (String, List<String>) SplitSignature(String signature)
        {
            String pattern = @"(\w+)(?:\(([^)]*)\))?";
            // This regex pattern matches expression signatures
            //
            // It works by using 2 capture groups:
            //  (\w+) - matches the name eg. h(t) -> h
            //  (?:\(([^)]*)\)) - matches the parameters eg. f(x,y,_) -> x,y,_
            // 
            // The question mark at the end ensures that there is a
            // match for the name of a expression if it is not a
            // function which takes parameters eg. mass -> mass

            String name = null;
            List<String> parameters = null;

            Match match = Regex.Match(signature, pattern);

            if (match.Success)
            {
                name = match.Groups[1].Value;

                if (match.Groups[2].Success)
                {
                    parameters = new List<String>();
                    String parameterStr = match.Groups[2].Value;
                    String[] parameterArray = parameterStr.Split(',');

                    foreach (String parameter in parameterArray)
                    {
                        parameters.Add(parameter.Trim());
                    }
                }
            }

            return (name, parameters);
        }

        private void Expression_EvaluateFunction(string name, FunctionArgs args)
        {
            if(_exprs.ContainsKey(name))
            {
                var expr = _exprs[name];

                for(int i = 0; i < expr.Parameters.Count; i++)
                {
                    var key = expr.Parameters.Keys.ToList()[i];
                    expr.Parameters[key] = args.Parameters[i].Evaluate();
                }

                args.Result = expr.Evaluate();
            }
        }

        private void Expression_EvaluateParameter(string param, ParameterArgs args)
        {
            switch (param)
            {

                case "t": args.Result = state.t; break;
                case "x": args.Result = state.position.x; break;
                case "y": args.Result = state.position.y; break;
                case "z": args.Result = state.position.z; break;
                case "vx": args.Result = state.velocity.x; break;
                case "vy": args.Result = state.velocity.y; break;
                case "vz": args.Result = state.velocity.z; break;
                default:
                    if (_exprs.ContainsKey(param))
                    {
                        args.Result = _exprs[param].Evaluate();
                        break;
                    }

                    throw new ArgumentException("Unknown Parameter", param);
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

        public void PrintData()
        {
            String log = "";
            foreach (var item in states)
            {
                log += String.Format("{0:f5} {1:f5} {2:f5} {3:f5} {4:f5} {5:f5} {6:f5} \n", item.t, item.position.x, item.position.y, item.position.z, item.velocity.x, item.velocity.y, item.velocity.z);
            }
            Debug.Log(log);
        }

        public void PrintDataCsv(String header_prefix)
        {
            String log = String.Format("t;{0}_x;{0}_y;{0}_z;{0}_vx;{0}_vy;{0}_vz\n", header_prefix);
            foreach (var item in states)
            {
                log += String.Format("{0:f5};{1:f5};{2:f5};{3:f5};{4:f5};{5:f5};{6:f5} \n", item.t, item.position.x, item.position.y, item.position.z, item.velocity.x, item.velocity.y, item.velocity.z);
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
            AddExpression("fx", Fx);
            AddExpression("fy", Fy);
            AddExpression("fz", Fz);
        }
        public SimulatedEntity(Vector3d initialPosition, Vector3d initialVelocity, String Fx, String Fy, String Fz, String m) :
            this(initialPosition, initialVelocity, Fx, Fy, Fz)
        {
            AddExpression("m", m);
        }
    }
}