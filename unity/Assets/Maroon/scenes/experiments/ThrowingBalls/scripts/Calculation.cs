using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCalc;
using Maroon.UI;
using ObjectsInUse;

public class Calculation : PausableObject, IResetObject
{
    private static Calculation _instance;
    private ParticleObject _particleInUse;
    private Vector3 _point;
    
    private bool _stopSimulation = false;
    private bool _startCalcPlot = false;
    private int _currentSteps = 0;
    private int _currentUpdateRate = 0;
    private bool _drawTrajectory = true;
    private bool _firstIteration = true;
    private bool _initialized = false;

    private float _xMax = System.Int64.MinValue;
    private float _yMax = System.Int64.MinValue;
    private float _zMax = System.Int64.MinValue;

    private float _xMin = System.Int64.MaxValue;
    private float _yMin = System.Int64.MaxValue;
    private float _zMin = System.Int64.MaxValue;

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

    /// <summary>
    /// Initialization
    /// </summary>
    protected override void Start()
    {
        
    }

    /// <summary>
    /// Update is called every frame
    /// </summary>
    protected override void HandleUpdate()
    {

    }

    /// <summary>
    /// This function is called every fixed framerate frame 
    /// </summary>
    protected override void HandleFixedUpdate()
    {
        if (!_startCalcPlot && !_initialized)
            StartCalcPlot();

        try
        {
            if (_startCalcPlot && _currentSteps < _steps && _currentUpdateRate == 1)
            {
                _point = new Vector3(_dataX[_currentSteps].y, _dataZ[_currentSteps].y, _dataY[_currentSteps].y);
                Vector3 mappedPoint = initCoordSystem.Instance.MapValues(_point);
                initCoordSystem.Instance.DrawPoint(mappedPoint, _drawTrajectory, _particleInUse);
                
                _currentSteps++;
                _currentUpdateRate = 0;
            }
            else if (_startCalcPlot && _currentSteps >= _steps)
            {
                _currentSteps = 0;
                _drawTrajectory = false;
            }
            else
            {
                if (_startCalcPlot)
                    _currentUpdateRate++;
            }
        } catch
        {
            ShowError();
            SimulationController.Instance.ResetSimulation();
            _startCalcPlot = false;
        }
    }

    // GUI button
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

            if (_particleInUse == ParticleObject.Satellite)
                initCoordSystem.Instance.SetColor(Color.white);
            initCoordSystem.Instance.SetRealCoordBorders(border_min, border_max);

            _initialized = true;
            _startCalcPlot = true;

            initCoordSystem.Instance.SetParticleActive(_particleInUse);
        }
    }

    // get formula expression with dynamic parameters
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

    // calculate the forces Fx, Fy, Fz
    private void CalcForces()
    {
        Expression e;

        try
        {
            // calc x-force
            if (!System.String.IsNullOrEmpty(_formulaFx))
            {
                e = GetExpression(_formulaFx);
                _currentFx = System.Convert.ToDouble(e.Evaluate());
            }

            // calc y-force
            if (!System.String.IsNullOrEmpty(_formulaFx))
            {
                e = GetExpression(_formulaFy);
                _currentFy = System.Convert.ToDouble(e.Evaluate());
            }

            // calc z-force
            if (!System.String.IsNullOrEmpty(_formulaFx))
            {
                e = GetExpression(_formulaFz);
                _currentFz = System.Convert.ToDouble(e.Evaluate());
            }
        } 
        catch
        {
            ShowError();
            SimulationController.Instance.ResetSimulation();
            _stopSimulation = true;
        }
        
    }

    // calculates the values with the 4th order Runge-Kutta method
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
            _firstIteration = false;
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

    // init the parameters from GUI
    private void InitParameter()
    {
        _formulaFx = ParameterUI.Instance.GetFunctionFx();
        _formulaFy = ParameterUI.Instance.GetFunctionFy();
        _formulaFz = ParameterUI.Instance.GetFunctionFz();
        
        _mass = (double) ParameterUI.Instance.GetMass();
        _currentTime = (double) ParameterUI.Instance.GetTimes().x;
        _deltaT = (double) ParameterUI.Instance.GetTimes().y;
        _steps = (double) ParameterUI.Instance.GetTimes().z;

        _currentX = (double) ParameterUI.Instance.GetXYZ().x;
        _currentY = (double) ParameterUI.Instance.GetXYZ().y;
        _currentZ = (double) ParameterUI.Instance.GetXYZ().z;

        _currentVx = (double) ParameterUI.Instance.GetVxVyVz().x;
        _currentVy = (double) ParameterUI.Instance.GetVxVyVz().y;
        _currentVz = (double) ParameterUI.Instance.GetVxVyVz().z;

        _currentP = 0;
        _currentEkin = 0;
        _currentW = 0;

        ClearData();
    }

    // calculate min, max to set the borders of the coord-system
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

        _xMin = _xMin - GetMinMaxScaleFactor(_xMin);
        _yMin = _yMin - GetMinMaxScaleFactor(_yMin);
        _zMin = _zMin - GetMinMaxScaleFactor(_zMin);

        _xMax = _xMax + GetMinMaxScaleFactor(_xMax);
        _yMax = _yMax + GetMinMaxScaleFactor(_yMax);
        _zMax = _zMax + GetMinMaxScaleFactor(_zMax);

    }

    private float GetMinMaxScaleFactor(float value)
    {
        value = System.Math.Abs(value);

        if (value == 0)
            return 1f;
        else
            return 0f;
    }

    // debugging GUI parameters
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

    // debugging current values
    private void DebugCurrentValues()
    {
        Debug.Log("Time: " + _currentTime.ToString() + " X: " + _currentX.ToString() + " Y: " + _currentY.ToString() + " Z: " + _currentZ.ToString() +
                "\nVx: " + _currentVx.ToString() + " Vy: " + _currentVy.ToString() + " Vz: " + _currentVz.ToString() +
                "\nFx: " + _currentFx.ToString() + " Fy: " + _currentFy.ToString() + " Fz: " + _currentFz.ToString());
    }
    
    public static Calculation Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<Calculation>();
            return _instance;
        }
    }

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

    public List<Vector2> GetDataX() { return _dataX; }
    public List<Vector2> GetDataY() { return _dataY; }
    public List<Vector2> GetDataZ() { return _dataZ; }

    public List<Vector2> GetDataVX() { return _dataVx; }
    public List<Vector2> GetDataVY() { return _dataVy; }
    public List<Vector2> GetDataVZ() { return _dataVz; }

    public List<Vector2> GetDataFX() { return _dataFx; }
    public List<Vector2> GetDataFY() { return _dataFy; }
    public List<Vector2> GetDataFZ() { return _dataFz; }

    public List<Vector2> GetDataP() { return _dataP; }
    public List<Vector2> GetDataEkin() { return _dataEkin; }
    public List<Vector2> GetDataW() { return _data_W; }


    /// <summary>
    /// Resets the object
    /// </summary>
    public void ResetObject()
    {
        //Debug.Log("Reset Calculation\n");

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

        _xMax = System.Int64.MinValue;
        _yMax = System.Int64.MinValue;
        _zMax = System.Int64.MinValue;

        _xMin = System.Int64.MaxValue;
        _yMin = System.Int64.MaxValue;
        _zMin = System.Int64.MaxValue;

        ClearData();
    }

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

    private void CalcEnergies()
    {
        _currentEkin = (System.Math.Pow(_currentVx, 2) + System.Math.Pow(_currentVy, 2) + System.Math.Pow(_currentVz, 2)) /
                        (2 * _mass);
        
        _currentP = _currentVx * _currentFx + _currentVy * _currentFy + _currentVz * _currentFz;
        _currentW += 0.5 * (_currentP + _oldPower) * _deltaT;
        _oldPower = _currentP;

    }
    private void ShowError()
    {
        ParameterUI.Instance.DisplayMessage("Something went wrong with the calculation. Please check the formula.");
    }

}
