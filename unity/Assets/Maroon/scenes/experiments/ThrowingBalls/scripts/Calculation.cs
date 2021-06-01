using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCalc;
using Maroon.UI;

public class Calculation : PausableObject, IResetObject
{
    private static Calculation _instance;

    private Vector3 point_;
    
    private bool stop_simulation_ = false;
    private bool start_calc_plot_ = false;
    private int current_steps_ = 0;
    private int current_update_rate_ = 0;
    private bool draw_trajectory_ = true;
    private bool first_it = true;

    private float x_max_ = System.Int64.MinValue;
    private float y_max_ = System.Int64.MinValue;
    private float z_max_ = System.Int64.MinValue;

    private float x_min_ = System.Int64.MaxValue;
    private float y_min_ = System.Int64.MaxValue;
    private float z_min_ = System.Int64.MaxValue;

    private string formula_fx_; 
    private string formula_fy_; 
    private string formula_fz_; 

    private double current_Fx_ = 0;
    private double current_Fy_ = 0;
    private double current_Fz_ = 0;

    private double current_x_ = 0;
    private double current_y_ = 0;
    private double current_z_ = 0;

    private double current_vx_ = 0;
    private double current_vy_ = 0;
    private double current_vz_ = 0;

    private double current_p_ = 0;
    private double current_ekin_ = 0;
    private double current_w_ = 0;
    private double old_power_ = 0;

    private double delta_t_ = 0;
    private double steps_ = 0;
    private double current_time_ = 0;
    private double mass_ = 1;
   
    private List<Vector2> data_x_ = new List<Vector2>();
    private List<Vector2> data_y_ = new List<Vector2>();
    private List<Vector2> data_z_ = new List<Vector2>();

    private List<Vector2> data_vx_ = new List<Vector2>();
    private List<Vector2> data_vy_ = new List<Vector2>();
    private List<Vector2> data_vz_ = new List<Vector2>();

    private List<Vector2> data_fx_ = new List<Vector2>();
    private List<Vector2> data_fy_ = new List<Vector2>();
    private List<Vector2> data_fz_ = new List<Vector2>();

    private List<Vector2> data_p_ = new List<Vector2>();
    private List<Vector2> data_ekin_ = new List<Vector2>();
    private List<Vector2> data_w_ = new List<Vector2>();

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
        try
        {
            if (start_calc_plot_ && current_steps_ < steps_ && current_update_rate_ == 1)
            {
                point_ = new Vector3(data_x_[current_steps_].y, data_z_[current_steps_].y, data_y_[current_steps_].y);
                Vector3 mappedPoint = initCoordSystem.Instance.mapValues(point_);
                initCoordSystem.Instance.drawPoint(mappedPoint, draw_trajectory_);
                
                current_steps_++;
                current_update_rate_ = 0;
            }
            else if (start_calc_plot_ && current_steps_ >= steps_)
            {
                current_steps_ = 0;
                draw_trajectory_ = false;
            }
            else
            {
                if (start_calc_plot_)
                    current_update_rate_++;
            }
        } catch
        {
            Debug.Log("HandleFixedUpdate() exception\n");
            showError();
            SimulationController.Instance.ResetSimulation();
            start_calc_plot_ = false;
        }
    }

    // GUI button
    public void startCalcPlot()
    {
        Debug.Log("startCalcPlot()\n");
        stop_simulation_ = false;
        
        // load parameters from GUI
        initParameter();

        // calc min max for x,y,z coords
        calcMinMax();
        if (!stop_simulation_)
        {
            Vector3 border_min = new Vector3(x_min_, z_min_, y_min_);
            Vector3 border_max = new Vector3(x_max_, z_max_, y_max_);
            initCoordSystem.Instance.setRealCoordBorders(border_min, border_max);

            start_calc_plot_ = true;
            initCoordSystem.Instance.setParticleActive();
        }
    }

    // get formula expression with dynamic parameters
    private Expression getExpression(string formula)
    {
        Expression e = new Expression(formula);
        e.Parameters["x"] = current_x_;
        e.Parameters["y"] = current_y_;
        e.Parameters["z"] = current_z_;

        e.Parameters["vx"] = current_vx_;
        e.Parameters["vy"] = current_vy_;
        e.Parameters["vz"] = current_vz_;

        e.Parameters["m"] = mass_;
        e.Parameters["t"] = current_time_;

        return e;
    }

    // calculate the forces Fx, Fy, Fz
    private void calcForces()
    {
        Expression e;

        try
        {
            // calc x-force
            if (!System.String.IsNullOrEmpty(formula_fx_))
            {
                e = getExpression(formula_fx_);
                current_Fx_ = System.Convert.ToDouble(e.Evaluate());
            }

            // calc y-force
            if (!System.String.IsNullOrEmpty(formula_fx_))
            {
                e = getExpression(formula_fy_);
                current_Fy_ = System.Convert.ToDouble(e.Evaluate());
            }

            // calc z-force
            if (!System.String.IsNullOrEmpty(formula_fx_))
            {
                e = getExpression(formula_fz_);
                current_Fz_ = System.Convert.ToDouble(e.Evaluate());
            }
        } 
        catch
        {
            Debug.Log("calcForces() exception\n");
            showError();
            SimulationController.Instance.ResetSimulation();
            stop_simulation_ = true;
        }
        
    }

    // calculates the values with the 4th order Runge-Kutta method
    private void calcValues()
    {
        double temp_x = current_x_;
        double temp_y = current_y_;
        double temp_z = current_z_;

        double temp_vx = current_vx_;
        double temp_vy = current_vy_;
        double temp_vz = current_vz_;

        if (first_it)
        {
            calcForces();
            first_it = false;
        }
        if (stop_simulation_)
            return;
            
        double k1x = current_vx_ * delta_t_;
        double k1y = current_vy_ * delta_t_;
        double k1z = current_vz_ * delta_t_;

        double k1vx = current_Fx_ / mass_ * delta_t_;
        double k1vy = current_Fy_ / mass_ * delta_t_;
        double k1vz = current_Fz_ / mass_ * delta_t_;

        current_x_ = temp_x + 0.5 * k1x;
        current_y_ = temp_y + 0.5 * k1y;
        current_z_ = temp_z + 0.5 * k1z;

        current_vx_ = temp_vx + 0.5 * k1vx;
        current_vy_ = temp_vy + 0.5 * k1vy;
        current_vz_ = temp_vz + 0.5 * k1vz;

        calcForces();
    
        double k2x = current_vx_ * delta_t_;
        double k2y = current_vy_ * delta_t_;
        double k2z = current_vz_ * delta_t_;

        double k2vx = current_Fx_ / mass_ * delta_t_;
        double k2vy = current_Fy_ / mass_ * delta_t_;
        double k2vz = current_Fz_ / mass_ * delta_t_;

        current_x_ = temp_x + 0.5 * k2x;
        current_y_ = temp_y + 0.5 * k2y;
        current_z_ = temp_z + 0.5 * k2z;

        current_vx_ = temp_vx + 0.5 * k2vx;
        current_vy_ = temp_vy + 0.5 * k2vy;
        current_vz_ = temp_vz + 0.5 * k2vz;

        calcForces();

        double k3x = current_vx_ * delta_t_;
        double k3y = current_vy_ * delta_t_;
        double k3z = current_vz_ * delta_t_;

        double k3vx = current_Fx_ / mass_ * delta_t_;
        double k3vy = current_Fy_ / mass_ * delta_t_;
        double k3vz = current_Fz_ / mass_ * delta_t_;

        current_x_ = temp_x + k3x;
        current_y_ = temp_y + k3y;
        current_z_ = temp_z + k3z;

        current_vx_ = temp_vx + k3vx;
        current_vy_ = temp_vy + k3vy;
        current_vz_ = temp_vz + k3vz;

        calcForces();

        double k4x = current_vx_ * delta_t_;
        double k4y = current_vy_ * delta_t_;
        double k4z = current_vz_ * delta_t_;

        double k4vx = current_Fx_ / mass_ * delta_t_;
        double k4vy = current_Fy_ / mass_ * delta_t_;
        double k4vz = current_Fz_ / mass_ * delta_t_;

        current_x_ = temp_x + (1.0 / 6.0) * (k1x + 2 * k2x + 2 * k3x + k4x);
        current_y_ = temp_y + (1.0 / 6.0) * (k1y + 2 * k2y + 2 * k3y + k4y);
        current_z_ = temp_z + (1.0 / 6.0) * (k1z + 2 * k2z + 2 * k3z + k4z);

        current_vx_ = temp_vx + (1.0 / 6.0) * (k1vx + 2 * k2vx + 2 * k3vx + k4vx);
        current_vy_ = temp_vy + (1.0 / 6.0) * (k1vy + 2 * k2vy + 2 * k3vy + k4vy);
        current_vz_ = temp_vz + (1.0 / 6.0) * (k1vz + 2 * k2vz + 2 * k3vz + k4vz);

        calcForces();
        calcEnergies();

        addData();

        debugCurrentValues();

        current_time_ += delta_t_;
    }

    // init the parameters from GUI
    private void initParameter()
    {
        formula_fx_ = ParameterUI.Instance.getFunctionFx();
        formula_fy_ = ParameterUI.Instance.getFunctionFy();
        formula_fz_ = ParameterUI.Instance.getFunctionFz();
        /*
        mass_ = System.Convert.ToDouble(ParameterUI.Instance.getMass(), System.Globalization.CultureInfo.InvariantCulture);
        current_time_ = System.Convert.ToDouble(ParameterUI.Instance.getTimes().x);
        delta_t_ = System.Convert.ToDouble(ParameterUI.Instance.getTimes().y, System.Globalization.CultureInfo.InvariantCulture);
        steps_ = System.Convert.ToDouble(ParameterUI.Instance.getTimes().z);
        */
        mass_ = (double) ParameterUI.Instance.getMass();
        current_time_ = (double) ParameterUI.Instance.getTimes().x;
        delta_t_ = (double) ParameterUI.Instance.getTimes().y;
        steps_ = (double) ParameterUI.Instance.getTimes().z;

        current_x_ = (double) ParameterUI.Instance.getXYZ().x;
        current_y_ = (double) ParameterUI.Instance.getXYZ().y;
        current_z_ = (double) ParameterUI.Instance.getXYZ().z;

        current_vx_ = (double) ParameterUI.Instance.getVxVyVz().x;
        current_vy_ = (double) ParameterUI.Instance.getVxVyVz().y;
        current_vz_ = (double) ParameterUI.Instance.getVxVyVz().z;

        current_p_ = 0;
        current_ekin_ = 0;
        current_w_ = 0;

        clearData();
        debugGUIParameters();
    }

    // calculate min, max to set the borders of the coord-system
    private void calcMinMax()
    {
        Debug.Log("Current x start: " + current_x_.ToString());

        for (int i = 0; i < steps_; i++)
        {
            calcValues();
            if (stop_simulation_)
                return;
            // get min
            if (x_min_ > current_x_)
            {
                x_min_ = (float)current_x_;
            }
            if (y_min_ > current_y_)
            {
                y_min_ = (float)current_y_;
            }
            if (z_min_ > current_z_)
            {
                z_min_ = (float)current_z_;
            }

            // get max
            if (x_max_ < current_x_)
            {
                x_max_ = (float)current_x_;
            }
            if (y_max_ < current_y_)
            {
                y_max_ = (float)current_y_;
            }
            if (z_max_ < current_z_)
            {
                z_max_ = (float)current_z_;
            }
        }

        Debug.Log("xmin: " + x_min_.ToString() + " xmax: " + x_max_.ToString());

        x_min_ = (float)System.Math.Floor(x_min_) - 1;
        y_min_ = (float)System.Math.Floor(y_min_) - 1;
        z_min_ = (float)System.Math.Floor(z_min_) - 1;

        x_max_ = (float)System.Math.Ceiling(x_max_) + 1;
        y_max_ = (float)System.Math.Ceiling(y_max_) + 1;
        z_max_ = (float)System.Math.Ceiling(z_max_) + 1;
    }

    // debugging GUI parameters
    private void debugGUIParameters()
    {
        Debug.Log("PARAMETERS\n");
        Debug.Log("Fx: " + formula_fx_ + "\nFy: " + formula_fy_ +
            "\nFz: " + formula_fz_);
        Debug.Log("Mass: " + mass_.ToString());
        Debug.Log("To: " + current_time_.ToString() +
            "\nDeltaT: " + delta_t_.ToString() +
            "\nSteps: " + steps_.ToString());
        Debug.Log("X: " + current_x_.ToString() +
            "\nY: " + current_y_.ToString() +
            "\nZ: " + current_z_.ToString());
        Debug.Log("VX: " + current_vx_.ToString() +
            "\nVY: " + current_vy_.ToString() +
            "\nVZ: " + current_vz_.ToString());
    }

    // debugging current values
    private void debugCurrentValues()
    {
        Debug.Log("Time: " + current_time_.ToString() + " X: " + current_x_.ToString() + " Y: " + current_y_.ToString() + " Z: " + current_z_.ToString() +
                "\nVx: " + current_vx_.ToString() + " Vy: " + current_vy_.ToString() + " Vz: " + current_vz_.ToString() +
                "\nFx: " + current_Fx_.ToString() + " Fy: " + current_Fy_.ToString() + " Fz: " + current_Fz_.ToString());
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

    private void addData()
    {
        Vector2 tmp;

        tmp = new Vector2((float)current_time_, (float)current_x_);
        data_x_.Add(tmp);
        tmp = new Vector2((float)current_time_, (float)current_y_);
        data_y_.Add(tmp);
        tmp = new Vector2((float)current_time_, (float)current_z_);
        data_z_.Add(tmp);

        tmp = new Vector2((float)current_time_, (float)current_vx_);
        data_vx_.Add(tmp);
        tmp = new Vector2((float)current_time_, (float)current_vy_);
        data_vy_.Add(tmp);
        tmp = new Vector2((float)current_time_, (float)current_vz_);
        data_vz_.Add(tmp);
        
        tmp = new Vector2((float)current_time_, (float)current_Fx_);
        data_fx_.Add(tmp);
        tmp = new Vector2((float)current_time_, (float)current_Fy_);
        data_fy_.Add(tmp);
        tmp = new Vector2((float)current_time_, (float)current_Fz_);
        data_fz_.Add(tmp);

        tmp = new Vector2((float)current_time_, (float)current_ekin_);
        data_ekin_.Add(tmp);
        tmp = new Vector2((float)current_time_, (float)current_p_);
        data_p_.Add(tmp);
        tmp = new Vector2((float)current_time_, (float)current_w_);
        data_w_.Add(tmp);
    }

    public List<Vector2> getDataX() { return data_x_; }
    public List<Vector2> getDataY() { return data_y_; }
    public List<Vector2> getDataZ() { return data_z_; }

    public List<Vector2> getDataVX() { return data_vx_; }
    public List<Vector2> getDataVY() { return data_vy_; }
    public List<Vector2> getDataVZ() { return data_vz_; }

    public List<Vector2> getDataFX() { return data_fx_; }
    public List<Vector2> getDataFY() { return data_fy_; }
    public List<Vector2> getDataFZ() { return data_fz_; }

    public List<Vector2> getDataP() { return data_p_; }
    public List<Vector2> getDataEkin() { return data_ekin_; }
    public List<Vector2> getDataW() { return data_w_; }


    /// <summary>
    /// Resets the object
    /// </summary>
    public void ResetObject()
    {
        Debug.Log("Reset Calculation\n");

        stop_simulation_ = true;
        start_calc_plot_ = false;
        first_it = true;
        draw_trajectory_ = true;
        current_steps_ = 0;
        current_update_rate_ = 0;
        current_time_ = 0;

        current_Fx_ = 0;
        current_Fy_ = 0;
        current_Fz_ = 0;

        current_p_ = 0;
        current_ekin_ = 0;
        current_w_ = 0;
        old_power_ = 0;

        x_max_ = System.Int64.MinValue;
        y_max_ = System.Int64.MinValue;
        z_max_ = System.Int64.MinValue;

        x_min_ = System.Int64.MaxValue;
        y_min_ = System.Int64.MaxValue;
        z_min_ = System.Int64.MaxValue;

        clearData();
    }

    private void clearData()
    {
        data_x_.Clear();
        data_y_.Clear();
        data_z_.Clear();

        data_vx_.Clear();
        data_vy_.Clear();
        data_vz_.Clear();

        data_fx_.Clear();
        data_fy_.Clear();
        data_fz_.Clear();

        data_ekin_.Clear();
        data_p_.Clear();
        data_w_.Clear();
    }

    private void calcEnergies()
    {
        current_ekin_ = (System.Math.Pow(current_vx_, 2) + System.Math.Pow(current_vy_, 2) + System.Math.Pow(current_vz_, 2)) /
                        (2 * mass_);
        
        current_p_ = current_vx_ * current_Fx_ + current_vy_ * current_Fy_ + current_vz_ * current_Fz_;
        current_w_ += 0.5 * (current_p_ + old_power_) * delta_t_;
        old_power_ = current_p_;

    }
    private void showError()
    {
        ParameterUI.Instance.displayMessage("Something went wrong with the calculation. Please check the formula.");
    }

}
