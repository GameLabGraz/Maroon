using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Physics
{
  public class ViscosimeterManager : MonoBehaviour
  {
    public static ViscosimeterManager Instance;
    
    public Ball ball;


    public float fluid_density_ = 0.0f;
    public QuantityFloat fluid_temperature_ = 25.0f;
    public float FluidTemperature
    {
      get => fluid_temperature_;
      set
      {
        fluid_temperature_.Value = value;
        calculateFluidDensity();
        ball.updateBall();
      }
    }

    public QuantityBool measurement_mode_ = false;
    public bool MeasurementMode
    {
      get => measurement_mode_;
      set
      {
        measurement_mode_.Value = value;
        toggleMeasurementBoxes(value);
      }
    }


    private void Awake()
    {
      if(Instance == null)
      {
        Instance = this;
      }
      calculateFluidDensity();
    }

    void calculateFluidDensity()
    {
      fluid_density_ = (fluid_temperature_ * -0.37f + 891.83f); //kg/m^3
      Debug.Log("Fluid Density: " + fluid_density_);
    }


    private void toggleMeasurementBoxes(bool mode)
    {
      BroadcastMessage("toggleMeasurement", mode);
    }

    public void toggleMeasurementMode()
    {
      MeasurementMode = !MeasurementMode;
    }


    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
      
    }
  }
}