using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCalc;
using Maroon.UI;

public class Calculation : PausableObject, IResetObject
{
    private static Calculation _instance;

    private float time_ = -30.0f;
    private bool start_ = false;
    [SerializeField] private bool debug_parameters_ = true;
    [SerializeField] private bool start_test_ = true;
    private Vector3 point_;
    private DialogueManager _dialogueManager;
    private bool stop_simulation_ = false;
    private bool start_calc_plot_ = false;
    private double current_steps_ = 0;
    private int current_update_rate_ = 0;

    // min/max values for init coord borders
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

    private double delta_t_ = 0;
    private double steps_ = 0;
    private double current_time_ = 0;
    private double mass_ = 1;

    private bool first_it = true;
    private double v_max_;
    private double r_max_ = 0.13;

    /// <summary>
    /// Initialization
    /// </summary>
    protected override void Start()
    {
        displayMessage("Lorem ipsum dolor sit amet, consetetur sadipscing elitr, " +
            "sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, " +
            "sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, " +
            "no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, " +
            "sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam era");
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
        if (start_calc_plot_ && current_steps_ < steps_ && current_update_rate_ == 1)
        {
            point_ = new Vector3((float)current_x_, (float)current_z_, (float)current_y_);
            Vector3 mappedPoint = initCoordSystem.Instance.mapValues(point_);
            initCoordSystem.Instance.drawPoint(mappedPoint, true);

            // calculate new values
            calcValues();
            current_steps_++;
            current_update_rate_ = 0;
        }
        else
        {
            if (start_calc_plot_)
                current_update_rate_++;
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
        Vector3 border_min = new Vector3(x_min_, z_min_, y_min_);
        Vector3 border_max = new Vector3(x_max_, z_max_, y_max_);
        initCoordSystem.Instance.setRealCoordBorders(border_min, border_max);
        resetParameters();

        start_calc_plot_ = true;
        initCoordSystem.Instance.setParticleActive();

        //Debug.Log("xyz min: " + x_min_.ToString() + " " + y_min_.ToString() + " " + z_min_.ToString());
        //Debug.Log("xyz max: " + x_max_.ToString() + " " + y_max_.ToString() + " " + z_max_.ToString());

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

    // calculates the delta t
    private void calcDeltaT()
    {
        v_max_ = System.Math.Sqrt(System.Math.Pow(current_vx_, 2) + System.Math.Pow(current_vy_, 2) + System.Math.Pow(current_vz_, 2));
        delta_t_ = r_max_ / v_max_;
        Debug.Log("Delta T: " + delta_t_.ToString());
    }

    // calculate the forces Fx, Fy, Fz
    private void calcForces()
    {
        Expression e;

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

        current_time_ += delta_t_;
    }

    // calculates the function and visualize it
    IEnumerator _wait(float time)
    {
        for (int i = 0; i < steps_; i++)
        {
            if (stop_simulation_)
                break;
            // debugCurrentValues();
           
            // calculate coord-position and draw point with trajectory
            // point_ = new Vector3((float)current_x_, (float)current_y_, (float)current_z_);
            point_ = new Vector3((float)current_x_, (float)current_z_, (float)current_y_);
            Vector3 mappedPoint = initCoordSystem.Instance.mapValues(point_);
            initCoordSystem.Instance.drawPoint(mappedPoint, true);

            // calculate new values
            calcValues();

            yield return new WaitForSeconds(time);
        }
        
    }

    // init the parameters from GUI
    private void initParameter()
    {
        formula_fx_ = ParameterUI.Instance.getFunctionFx();
        formula_fy_ = ParameterUI.Instance.getFunctionFy();
        formula_fz_ = ParameterUI.Instance.getFunctionFz();

        mass_ = System.Convert.ToDouble(ParameterUI.Instance.getMass(), System.Globalization.CultureInfo.InvariantCulture);
        current_time_ = System.Convert.ToDouble(ParameterUI.Instance.getTimes().x);
        delta_t_ = System.Convert.ToDouble(ParameterUI.Instance.getTimes().y, System.Globalization.CultureInfo.InvariantCulture);
        steps_ = System.Convert.ToDouble(ParameterUI.Instance.getTimes().z);

        current_x_ = System.Convert.ToDouble(ParameterUI.Instance.getXYZ().x);
        current_y_ = System.Convert.ToDouble(ParameterUI.Instance.getXYZ().y);
        current_z_ = System.Convert.ToDouble(ParameterUI.Instance.getXYZ().z);

        current_vx_ = System.Convert.ToDouble(ParameterUI.Instance.getVxVyVz().x);
        current_vy_ = System.Convert.ToDouble(ParameterUI.Instance.getVxVyVz().y);
        current_vz_ = System.Convert.ToDouble(ParameterUI.Instance.getVxVyVz().z);

        // debugGUIParameters();
    }

    // calculate min, max to set the borders of the coord-system
    private void calcMinMax()
    {
        for (int i = 0; i < steps_; i++)
        {
            calcValues();

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

        x_min_ = (float)System.Math.Floor(x_min_) - 1;
        y_min_ = (float)System.Math.Floor(y_min_) - 1;
        z_min_ = (float)System.Math.Floor(z_min_) - 1;

        x_max_ = (float)System.Math.Ceiling(x_max_) + 1;
        y_max_ = (float)System.Math.Ceiling(y_max_) + 1;
        z_max_ = (float)System.Math.Ceiling(z_max_) + 1;
    }

    // resets the parameters to the values from the GUI
    public void resetParameters()
    {
        initParameter();

        first_it = true;
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

    private void displayMessage(string message)
    {
        if (_dialogueManager == null)
            _dialogueManager = FindObjectOfType<DialogueManager>();

        if (_dialogueManager == null)
            return;

        _dialogueManager.ShowMessage(message);
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

    /// <summary>
    /// Resets the object
    /// </summary>
    public void ResetObject()
    {
        Debug.Log("Reset Calculation\n");
        stop_simulation_ = true;
        start_calc_plot_ = false;
        current_steps_ = 0;
        current_update_rate_ = 0;

        initParameter();
        current_Fx_ = 0;
        current_Fy_ = 0;
        current_Fz_ = 0;

        x_max_ = System.Int64.MinValue;
        y_max_ = System.Int64.MinValue;
        z_max_ = System.Int64.MinValue;

        x_min_ = System.Int64.MaxValue;
        y_min_ = System.Int64.MaxValue;
        z_min_ = System.Int64.MaxValue;
    }

}
