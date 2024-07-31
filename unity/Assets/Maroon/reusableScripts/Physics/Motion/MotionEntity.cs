using System.Collections.Generic;
using UnityEngine;
using NCalc;
using NCalc.Handlers;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Maroon.Physics.Motion
{

    /// <summary>
    /// An entity which moves through space as described by differential equations of first order
    /// </summary>
    public class MotionEntity
    {
        internal MotionState current_state;

        private Dictionary<String, Expression> _expressions = new();
        private Dictionary<String, object> _parameters = new();

        private List<MotionState> _state;
        private MotionState _initial_state;

        private double _dt;
        private Bounds _bounds = new();

        public List<Vector3> Position { get => _state.Select(s => (Vector3)s.position).ToList(); }
        public List<Vector3> Velocity { get => _state.Select(s => (Vector3)s.Velocity).ToList(); }
        public List<Vector3> Acceleration { get => _state.Select(s => (Vector3)s.Acceleration).ToList(); }
        public List<Vector3> Force { get => _state.Select(s => (Vector3)s.Force).ToList(); }
        public List<Double> Mass { get => _state.Select(s => s.Mass).ToList(); }
        public List<MotionState> State { get => _state; }
        public Bounds Bounds { get => _bounds; }

        internal Vector3d EvaluateForceAt(double t)
        {
            return new Vector3d(
                Convert.ToDouble(_expressions["fx"].Evaluate()),
                Convert.ToDouble(_expressions["fy"].Evaluate()),
                Convert.ToDouble(_expressions["fz"].Evaluate())
            );
        }

        internal double EvaluateMassAt(double t)
        {
            if (_parameters.ContainsKey("m"))
                return Convert.ToDouble(_parameters["m"]);

            return Convert.ToDouble(_expressions["m"].Evaluate());
        }

        public void AddExpression(String signature, String expression)
        {
            var expr = new Expression(expression, ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
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

            _expressions[name] = expr;
        }

        public void AddParameter(String name, object value)
        {
            _parameters[name] = value;
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
            if (_expressions.ContainsKey(name) && _expressions[name].Parameters.Count == args.Parameters.Length)
            {
                var expr = _expressions[name];

                for (int i = 0; i < expr.Parameters.Count; i++)
                {
                    var key = expr.Parameters.Keys.ToList()[i];
                    expr.Parameters[key] = args.Parameters[i].Evaluate();
                }

                args.Result = expr.Evaluate();
            }
        }

        private void Expression_EvaluateParameter(string name, ParameterArgs args)
        {
            args.Result = name switch
            {
                "t" => current_state.t,
                "x" => current_state.position.x,
                "y" => current_state.position.y,
                "z" => current_state.position.z,
                "vx" => current_state.velocity.x,
                "vy" => current_state.velocity.y,
                "vz" => current_state.velocity.z,
                _ => null
            };

            if (args.Result != null) return;

            if (_parameters.ContainsKey(name))
            {
                args.Result = _parameters[name];
            }
            else if (_expressions.ContainsKey(name))
            {
                args.Result = _expressions[name].Evaluate();
            }
            else
            {
                throw new ArgumentException(String.Format("Unknown Parameter '{0}'", name));
            }
        }

        public void SetInitialState(Vector3 position, Vector3 velocity)
        {
            _initial_state = new MotionState(position, velocity, this);
        }

        public void Initialize(double initial_t, double dt)
        {
            _state = new List<MotionState>();

            current_state = _initial_state;
            current_state.EvaluateAccelerationAt(initial_t);
            current_state.CalculateEnergy();
            current_state.CalculatePower();

            _state.Add(new MotionState(current_state));

            _dt = dt;
        }

        public void SaveState()
        {
            _bounds.Encapsulate((Vector3)current_state.position);
            
            double prev_power = _state.Last().power;

            current_state.CalculateEnergyPowerWork(prev_power, _dt);
            _state.Add(new MotionState(current_state));
        }

        public void PrintData()
        {
            String log = "";
            foreach (var item in _state)
            {
                log += String.Format("{0:f5} {1:f5} {2:f5} {3:f5} {4:f5} {5:f5} {6:f5} \n", item.t, item.position.x, item.position.y, item.position.z, item.velocity.x, item.velocity.y, item.velocity.z);
            }
            Debug.Log(log);
        }

        public void PrintDataCsv(String header_prefix)
        {
            String log = String.Format("t;{0}_x;{0}_y;{0}_z;{0}_vx;{0}_vy;{0}_vz\n", header_prefix);
            foreach (var item in _state)
            {
                log += String.Format("{0:f5};{1:f5};{2:f5};{3:f5};{4:f5};{5:f5};{6:f5} \n", item.t, item.position.x, item.position.y, item.position.z, item.velocity.x, item.velocity.y, item.velocity.z);
            }
            Debug.Log(log);
        }

        public MotionEntity()
        {
            _parameters["pi"] = Math.PI;

            _expressions["m"] = new Expression("1", ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
            _expressions["fx"] = new Expression("0", ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
            _expressions["fy"] = new Expression("0", ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
            _expressions["fz"] = new Expression("0", ExpressionOptions.IgnoreCaseAtBuiltInFunctions);
        }
    }
}