using System.Collections.Generic;
using UnityEngine;
using NCalc;
using ObjectsInUse;
using System;
using System.Linq;
using GEAR.Localization;

namespace Maroon.Physics.ThreeDimensionalMotion
{
    public class MotionCalculation : PausableObject, IResetObject
    {
        private ParticleObject _particleInUse;
        private Vector3 _point;

        private bool _stopSimulation = false;
        private bool _startCalcPlot = false;
        private int _currentSteps = 0;
        private int _currentUpdateRate = 0;
        private bool _drawTrajectory = true;
        private bool _firstIteration = true;
        private bool _initialized = false;
        private const float Threshold = 0.001f;

        private float _xMax = long.MinValue;
        private float _yMax = long.MinValue;
        private float _zMax = long.MinValue;

        private float _xMin = long.MaxValue;
        private float _yMin = long.MaxValue;
        private float _zMin = long.MaxValue;

        private string _formulaFx;
        private string _formulaFy;
        private string _formulaFz;

        private double _currentFx = 0;
        private double _currentFy = 0;
        private double _currentFz = 0;

        private double _currentX = 0;
        private double _currentY = 0;
        private double _currentZ = 0;

        private double _currentVx = 0;
        private double _currentVy = 0;
        private double _currentVz = 0;

        private double _currentP = 0;
        private double _currentEkin = 0;
        private double _currentW = 0;
        private double _oldPower = 0;

        private double _deltaT = 0;
        private double _steps = 0;
        private double _currentTime = 0;
        private double _mass = 1;

        private List<Vector2> _dataX = new List<Vector2>();
        private List<Vector2> _dataY = new List<Vector2>();
        private List<Vector2> _dataZ = new List<Vector2>();

        private List<Vector2> _dataVx = new List<Vector2>();
        private List<Vector2> _dataVy = new List<Vector2>();
        private List<Vector2> _dataVz = new List<Vector2>();

        private List<Vector2> _dataFx = new List<Vector2>();
        private List<Vector2> _dataFy = new List<Vector2>();
        private List<Vector2> _dataFz = new List<Vector2>();

        private List<Vector2> _dataP = new List<Vector2>();
        private List<Vector2> _dataEkin = new List<Vector2>();
        private List<Vector2> _data_W = new List<Vector2>();

        /// new members
        private Motion.Simulation simulation = null;
        private Motion.SimulatedEntity entity = null;

        private int render_step = 0;

        /// <summary>
        /// Update is called every frame
        /// </summary>
        protected override void HandleUpdate()
        {

        }

        /// <summary>
        /// Handles the Play/Pause/Step functionality
        /// Play button triggers the start of the calculation and displaying results
        /// On error -> shows error message and resets the experiment
        /// </summary>
        protected override void HandleFixedUpdate()
        {
            // Create new simulation from UI Paramters
            if (simulation == null && entity == null)
            {
                render_step = 0;

                Vector3 initialPosition = ParameterUI.Instance.GetXYZ();
                Vector3 initialVelocity = ParameterUI.Instance.GetVxVyVz();

                entity = new Motion.SimulatedEntity();
                entity.SetInitialState(new Motion.State(initialPosition, initialVelocity));
                entity.AddExpression("fx", ParameterUI.Instance.GetFunctionFx());
                entity.AddExpression("fy", ParameterUI.Instance.GetFunctionFy());
                entity.AddExpression("fz", ParameterUI.Instance.GetFunctionFz());
                entity.AddExpression("m", ParameterUI.Instance.GetMass());

                simulation = new Motion.Simulation();
                simulation.t0 = ParameterUI.Instance.GetTimes().x;
                simulation.dt = ParameterUI.Instance.GetTimes().y;
                simulation.steps = (int) ParameterUI.Instance.GetTimes().z;

                simulation.AddEntity(entity);
                simulation.Run();

                Vector3 min_hack = TransformZUp(simulation.bounds.min);
                Vector3 max_hack = TransformZUp(simulation.bounds.max);

                // oh god oh god why do i have to abuse this thing like this??? 
                // i would really like to clean up this experiment but i don't know
                // if this would be out of scope....

                // this is from the previous implementation
                min_hack.x -= GetMinMaxScaleFactor(min_hack.x);
                min_hack.y -= GetMinMaxScaleFactor(min_hack.y);
                min_hack.z -= GetMinMaxScaleFactor(min_hack.z);

                if (ScalingNeeded(min_hack.x))
                    max_hack.x += GetMinMaxScaleFactor(max_hack.x);
                if (ScalingNeeded(min_hack.y))
                    max_hack.y += GetMinMaxScaleFactor(max_hack.y);
                if (ScalingNeeded(min_hack.z))
                    max_hack.z += GetMinMaxScaleFactor(max_hack.z);


                CoordSystem.Instance.SetRealCoordBorders(min_hack, max_hack);

                CoordSystem.Instance.SetParticleActive(_particleInUse);
            }

            var pos = TransformZUp((Vector3)entity.States[render_step++ % simulation.steps].position);
            var mapped_pos = CoordSystem.Instance.MapValues(pos);
            CoordSystem.Instance.DrawPoint(mapped_pos, render_step < simulation.steps);
        }

        static Vector3 TransformZUp(Vector3 v)
        {
            return new Vector3(v.x, v.z, v.y);
        }
        /// <summary>
        /// Inits the calculation process
        /// </summary>
        public void StartCalcPlot()
        {
            _stopSimulation = false;

            // load parameters from GUI
            InitParameter();

            _particleInUse = ParameterUI.Instance.GetObjectInUse();

            // calc min max for x,y,z coords
            CalcMinMax();

            if (!_stopSimulation)
            {
                Vector3 border_min = new Vector3(_xMin, _zMin, _yMin);
                Vector3 border_max = new Vector3(_xMax, _zMax, _yMax);

                // when satellite/space background is used -> white color for coord lines
                if (_particleInUse == ParticleObject.Satellite)
                    CoordSystem.Instance.SetColor(Color.white);

                CoordSystem.Instance.SetRealCoordBorders(border_min, border_max);

                _initialized = true;
                _startCalcPlot = true;

                CoordSystem.Instance.SetParticleActive(_particleInUse);
            }
        }

        /// <summary>
        /// Gets and evaluates the given formula with dynamic variables
        /// </summary>
        /// <param name="formula">Formula to evaluate</param>
        /// <returns>Expression for further evaluation</returns>
        private Expression GetExpression(string formula)
        {
            Expression e = new Expression(formula);
            e.Parameters["x"] = _currentX;
            e.Parameters["y"] = _currentY;
            e.Parameters["z"] = _currentZ;

            e.Parameters["vx"] = _currentVx;
            e.Parameters["vy"] = _currentVy;
            e.Parameters["vz"] = _currentVz;

            e.Parameters["m"] = _mass;
            e.Parameters["t"] = _currentTime;

            return e;
        }

        /// <summary>
        /// Calculates the forces for Fx, Fy and Fz
        /// Evaluates the formula and handles errors while evaluating
        /// </summary>
        private void CalcForces()
        {
            Expression e;

            try
            {
                // calc x-force
                if (!string.IsNullOrEmpty(_formulaFx))
                {
                    e = GetExpression(_formulaFx);
                    _currentFx = Convert.ToDouble(e.Evaluate());
                }

                // calc y-force
                if (!string.IsNullOrEmpty(_formulaFx))
                {
                    e = GetExpression(_formulaFy);
                    _currentFy = Convert.ToDouble(e.Evaluate());
                }

                // calc z-force
                if (!string.IsNullOrEmpty(_formulaFx))
                {
                    e = GetExpression(_formulaFz);
                    _currentFz = Convert.ToDouble(e.Evaluate());
                }
            }
            catch
            {
                ShowError();
                SimulationController.Instance.ResetSimulation();
                _stopSimulation = true;
            }

        }

        /// <summary>
        /// Calculates all needed values for the experiment with the 4th order Runge-Kutta algorithm
        /// </summary>
        private void CalcValues()
        {
            double tempX = _currentX;
            double tempY = _currentY;
            double tempZ = _currentZ;

            double tempVx = _currentVx;
            double tempVy = _currentVy;
            double tempVz = _currentVz;

            if (_firstIteration)
            {
                CalcForces();
                CalcEnergies();
                AddData();
                _firstIteration = false;
                return;
            }
            if (_stopSimulation)
                return;

            double k1x = _currentVx * _deltaT;
            double k1y = _currentVy * _deltaT;
            double k1z = _currentVz * _deltaT;

            double k1vx = _currentFx / _mass * _deltaT;
            double k1vy = _currentFy / _mass * _deltaT;
            double k1vz = _currentFz / _mass * _deltaT;

            _currentX = tempX + 0.5 * k1x;
            _currentY = tempY + 0.5 * k1y;
            _currentZ = tempZ + 0.5 * k1z;

            _currentVx = tempVx + 0.5 * k1vx;
            _currentVy = tempVy + 0.5 * k1vy;
            _currentVz = tempVz + 0.5 * k1vz;

            CalcForces();

            double k2x = _currentVx * _deltaT;
            double k2y = _currentVy * _deltaT;
            double k2z = _currentVz * _deltaT;

            double k2vx = _currentFx / _mass * _deltaT;
            double k2vy = _currentFy / _mass * _deltaT;
            double k2vz = _currentFz / _mass * _deltaT;

            _currentX = tempX + 0.5 * k2x;
            _currentY = tempY + 0.5 * k2y;
            _currentZ = tempZ + 0.5 * k2z;

            _currentVx = tempVx + 0.5 * k2vx;
            _currentVy = tempVy + 0.5 * k2vy;
            _currentVz = tempVz + 0.5 * k2vz;

            CalcForces();

            double k3x = _currentVx * _deltaT;
            double k3y = _currentVy * _deltaT;
            double k3z = _currentVz * _deltaT;

            double k3vx = _currentFx / _mass * _deltaT;
            double k3vy = _currentFy / _mass * _deltaT;
            double k3vz = _currentFz / _mass * _deltaT;

            _currentX = tempX + k3x;
            _currentY = tempY + k3y;
            _currentZ = tempZ + k3z;

            _currentVx = tempVx + k3vx;
            _currentVy = tempVy + k3vy;
            _currentVz = tempVz + k3vz;

            CalcForces();

            double k4x = _currentVx * _deltaT;
            double k4y = _currentVy * _deltaT;
            double k4z = _currentVz * _deltaT;

            double k4vx = _currentFx / _mass * _deltaT;
            double k4vy = _currentFy / _mass * _deltaT;
            double k4vz = _currentFz / _mass * _deltaT;

            _currentX = tempX + (1.0 / 6.0) * (k1x + 2 * k2x + 2 * k3x + k4x);
            _currentY = tempY + (1.0 / 6.0) * (k1y + 2 * k2y + 2 * k3y + k4y);
            _currentZ = tempZ + (1.0 / 6.0) * (k1z + 2 * k2z + 2 * k3z + k4z);

            _currentVx = tempVx + (1.0 / 6.0) * (k1vx + 2 * k2vx + 2 * k3vx + k4vx);
            _currentVy = tempVy + (1.0 / 6.0) * (k1vy + 2 * k2vy + 2 * k3vy + k4vy);
            _currentVz = tempVz + (1.0 / 6.0) * (k1vz + 2 * k2vz + 2 * k3vz + k4vz);

            CalcForces();
            CalcEnergies();
            AddData();

            _currentTime += _deltaT;
        }

        /// <summary>
        /// Inits the parameters from the GUI 
        /// </summary>
        private void InitParameter()
        {
            _formulaFx = ParameterUI.Instance.GetFunctionFx();
            _formulaFy = ParameterUI.Instance.GetFunctionFy();
            _formulaFz = ParameterUI.Instance.GetFunctionFz();

            _mass = ParameterUI.Instance.GetMass();
            _currentTime = ParameterUI.Instance.GetTimes().x;
            _deltaT = ParameterUI.Instance.GetTimes().y;
            _steps = ParameterUI.Instance.GetTimes().z;

            _currentX = ParameterUI.Instance.GetXYZ().x;
            _currentY = ParameterUI.Instance.GetXYZ().y;
            _currentZ = ParameterUI.Instance.GetXYZ().z;

            _currentVx = ParameterUI.Instance.GetVxVyVz().x;
            _currentVy = ParameterUI.Instance.GetVxVyVz().y;
            _currentVz = ParameterUI.Instance.GetVxVyVz().z;

            _currentP = 0;
            _currentEkin = 0;
            _currentW = 0;

            ClearData();
        }

        /// <summary>
        /// Calculates min/max values to set the borders of the coord-system
        /// </summary>
        private void CalcMinMax()
        {
            for (int i = 0; i < _steps; i++)
            {
                CalcValues();
                if (_stopSimulation)
                    return;
                // get min
                if (_xMin > _currentX)
                {
                    _xMin = (float)_currentX;
                }
                if (_yMin > _currentY)
                {
                    _yMin = (float)_currentY;
                }
                if (_zMin > _currentZ)
                {
                    _zMin = (float)_currentZ;
                }

                // get max
                if (_xMax < _currentX)
                {
                    _xMax = (float)_currentX;
                }
                if (_yMax < _currentY)
                {
                    _yMax = (float)_currentY;
                }
                if (_zMax < _currentZ)
                {
                    _zMax = (float)_currentZ;
                }
            }

            // add scaling for nice visualization
            _xMin -= GetMinMaxScaleFactor(_xMin);
            _yMin -= GetMinMaxScaleFactor(_yMin);
            _zMin -= GetMinMaxScaleFactor(_zMin);

            if (ScalingNeeded(_xMin))
                _xMax += GetMinMaxScaleFactor(_xMax);
            if (ScalingNeeded(_yMin))
                _yMax += GetMinMaxScaleFactor(_yMax);
            if (ScalingNeeded(_zMin))
                _zMax += GetMinMaxScaleFactor(_zMax);

        }

        /// <summary>
        /// This method checks if scaling is needed
        /// If a certain value is smaller than the threshold -> scaling leads to 
        /// incorrect visualization
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>Bool if scaling is needed</returns>
        private bool ScalingNeeded(float value)
        {
            return !(Math.Abs(value) < Threshold);
        }

        /// <summary>
        /// Method to scale min/max values for visualization
        /// </summary>
        /// <param name="value">Value to scale</param>
        /// <returns>Adapted value</returns>
        private float GetMinMaxScaleFactor(float value)
        {
            value = Math.Abs(value);
            return value == 0 ? 0.5f : 0f;
        }

        /// <summary>
        /// THIS FUNCTION IS NOT ACTIVE
        /// Can be used for debugging GUI parameters
        /// </summary>
        private void DebugGUIParameters()
        {
            Debug.Log("PARAMETERS\n");
            Debug.Log("Fx: " + _formulaFx + "\nFy: " + _formulaFy +
                "\nFz: " + _formulaFz);
            Debug.Log("Mass: " + _mass.ToString());
            Debug.Log("To: " + _currentTime.ToString() +
                "\nDeltaT: " + _deltaT.ToString() +
                "\nSteps: " + _steps.ToString());
            Debug.Log("X: " + _currentX.ToString() +
                "\nY: " + _currentY.ToString() +
                "\nZ: " + _currentZ.ToString());
            Debug.Log("VX: " + _currentVx.ToString() +
                "\nVY: " + _currentVy.ToString() +
                "\nVZ: " + _currentVz.ToString());
        }

        /// <summary>
        /// THIS FUNCTION IS NOT ACTIVE
        /// Can be used for debugging calculated values
        /// </summary>
        private void DebugCurrentValues()
        {
            Debug.Log("Time: " + _currentTime.ToString() + " X: " + _currentX.ToString() + " Y: " + _currentY.ToString() + " Z: " + _currentZ.ToString() +
                    "\nVx: " + _currentVx.ToString() + " Vy: " + _currentVy.ToString() + " Vz: " + _currentVz.ToString() +
                    "\nFx: " + _currentFx.ToString() + " Fy: " + _currentFy.ToString() + " Fz: " + _currentFz.ToString());
        }

        /// <summary>
        /// Since the values are calculated only once at the beginning -> store them for later visualization
        /// </summary>
        private void AddData()
        {
            Vector2 tmp;

            tmp = new Vector2((float)_currentTime, (float)_currentX);
            _dataX.Add(tmp);
            tmp = new Vector2((float)_currentTime, (float)_currentY);
            _dataY.Add(tmp);
            tmp = new Vector2((float)_currentTime, (float)_currentZ);
            _dataZ.Add(tmp);

            tmp = new Vector2((float)_currentTime, (float)_currentVx);
            _dataVx.Add(tmp);
            tmp = new Vector2((float)_currentTime, (float)_currentVy);
            _dataVy.Add(tmp);
            tmp = new Vector2((float)_currentTime, (float)_currentVz);
            _dataVz.Add(tmp);

            tmp = new Vector2((float)_currentTime, (float)_currentFx);
            _dataFx.Add(tmp);
            tmp = new Vector2((float)_currentTime, (float)_currentFy);
            _dataFy.Add(tmp);
            tmp = new Vector2((float)_currentTime, (float)_currentFz);
            _dataFz.Add(tmp);

            tmp = new Vector2((float)_currentTime, (float)_currentEkin);
            _dataEkin.Add(tmp);
            tmp = new Vector2((float)_currentTime, (float)_currentP);
            _dataP.Add(tmp);
            tmp = new Vector2((float)_currentTime, (float)_currentW);
            _data_W.Add(tmp);
        }

        // Getter for all values
        // --------------------------------------------------------------
        public List<Vector2> GetDataX() { return entity.States.Select(s => new Vector2((float) s.t, (float) s.position.x)).ToList(); }
        public List<Vector2> GetDataY() { return entity.States.Select(s => new Vector2((float)s.t, (float)s.position.y)).ToList(); }
        public List<Vector2> GetDataZ() { return entity.States.Select(s => new Vector2((float)s.t, (float)s.position.z)).ToList(); }

        public List<Vector2> GetDataVX() { return entity.States.Select(s => new Vector2((float)s.t, (float)s.velocity.x)).ToList(); }
        public List<Vector2> GetDataVY() { return entity.States.Select(s => new Vector2((float)s.t, (float)s.velocity.y)).ToList(); }
        public List<Vector2> GetDataVZ() { return entity.States.Select(s => new Vector2((float)s.t, (float)s.velocity.z)).ToList(); }

        public List<Vector2> GetDataFX() { return entity.States.Select(s => new Vector2((float)s.t, (float)s.force.x)).ToList(); }
        public List<Vector2> GetDataFY() { return entity.States.Select(s => new Vector2((float)s.t, (float)s.force.y)).ToList(); }
        public List<Vector2> GetDataFZ() { return entity.States.Select(s => new Vector2((float)s.t, (float)s.force.z)).ToList(); }

        public List<Vector2> GetDataP() { return entity.States.Select(s => new Vector2((float)s.t, (float)s.power)).ToList(); }
        public List<Vector2> GetDataEkin() { return entity.States.Select(s => new Vector2((float)s.t, (float)s.kinetic_energy)).ToList(); }
        public List<Vector2> GetDataW() { return entity.States.Select(s => new Vector2((float)s.t, (float)s.work)).ToList(); }
        // --------------------------------------------------------------

        /// <summary>
        /// Resets the object
        /// </summary>
        public void ResetObject()
        {
            _stopSimulation = true;
            _startCalcPlot = false;
            _initialized = false;
            _firstIteration = true;
            _drawTrajectory = true;
            _currentSteps = 0;
            _currentUpdateRate = 0;
            _currentTime = 0;

            _currentFx = 0;
            _currentFy = 0;
            _currentFz = 0;

            _currentP = 0;
            _currentEkin = 0;
            _currentW = 0;
            _oldPower = 0;

            _xMax = long.MinValue;
            _yMax = long.MinValue;
            _zMax = long.MinValue;

            _xMin = long.MaxValue;
            _yMin = long.MaxValue;
            _zMin = long.MaxValue;

            ClearData();

            // clear new members
            entity = null;
            simulation = null;
            render_step = 0;
        }

        /// <summary>
        /// Deletes all stored data
        /// </summary>
        private void ClearData()
        {
            _dataX.Clear();
            _dataY.Clear();
            _dataZ.Clear();

            _dataVx.Clear();
            _dataVy.Clear();
            _dataVz.Clear();

            _dataFx.Clear();
            _dataFy.Clear();
            _dataFz.Clear();

            _dataEkin.Clear();
            _dataP.Clear();
            _data_W.Clear();
        }

        /// <summary>
        /// Method for calculating KineticEnergy, Work and Power
        /// </summary>
        private void CalcEnergies()
        {
            _currentEkin = (Math.Pow(_currentVx, 2) + Math.Pow(_currentVy, 2) + Math.Pow(_currentVz, 2)) /
                            (2 * _mass);

            _currentP = _currentVx * _currentFx + _currentVy * _currentFy + _currentVz * _currentFz;
            _currentW += 0.5 * (_currentP + _oldPower) * _deltaT;
            _oldPower = _currentP;

        }

        /// <summary>
        /// Method for showing error messages
        /// </summary>
        private void ShowError()
        {
            string message = LanguageManager.Instance.GetString("CalculationError");
            ParameterUI.Instance.DisplayMessage(message);
        }

    }
}