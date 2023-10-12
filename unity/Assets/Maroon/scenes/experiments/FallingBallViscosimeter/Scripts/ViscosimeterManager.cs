using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Maroon.Physics
{
  public class ViscosimeterManager : MonoBehaviour, IResetObject
  {
    public static ViscosimeterManager Instance;
    
    public TMP_Text debug_text;

    public Ball ball;
    public Pycnometer pycnometer;

    public decimal fluid_density_ = 0.0m;
    public QuantityDecimal fluid_temperature_ = 25.0m;
    public decimal FluidTemperature
    {
      get => fluid_temperature_.Value;
      set
      {
        fluid_temperature_.Value = value;
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
    
    MeasurableObject currentMeasurableObject;
    public List<MeasurableObject> measurableObjects;

    private decimal ballMaxSpeed = -1.0m;
    private Rigidbody _rigidbody;
    private void Awake()
    {
      if(Instance == null)
      {
        Instance = this;
      }
      calculateFluidDensity();
      getAllMeasurableObjects();
      _rigidbody = ball.gameObject.GetComponent<Rigidbody>();
      fluid_temperature_.minValue = 15.0m;
      fluid_temperature_.maxValue = 100.0m;
    }

    void calculateFluidDensity()
    {
      fluid_density_ = ((decimal)FluidTemperature * -0.37m + 891.83m); //kg/m^3
    }

    public void togglePycnometerFill(bool fill)
    {
      if(fill)
      {
        pycnometer.fillPycnometer();
      }
      else
      {
        pycnometer.emptyPycnometer();
      }
    }


    private void toggleMeasurementBoxes(bool mode)
    {
      BroadcastMessage("toggleMeasurement", mode);
    }

    public void toggleMeasurementMode()
    {
      MeasurementMode = !MeasurementMode;
    }

    void getAllMeasurableObjects()
    {
      Object[] foundObjects = Object.FindObjectsOfType<MeasurableObject>();

      measurableObjects = new List<MeasurableObject>();

      foreach (Object obj in foundObjects)
      {
        measurableObjects.Add(obj as MeasurableObject);
      }
    }


    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      calculateFluidDensity();
      ball.updateBall();
      update_debug_text();
    }

    void update_debug_text()
    {
      decimal ball_velocity = -1.0m * (decimal)_rigidbody.velocity.y;
      if (ball_velocity > ballMaxSpeed)
      {
        ballMaxSpeed = ball_velocity;
      }
      debug_text.text = "Ball Velocity:\n" + ball_velocity + "\nMax Vel:\n" + ballMaxSpeed + "\nFluid density\n" + fluid_density_;
      
    }

    public void ResetObject()
    {
      ballMaxSpeed = -1;
    }
  }
}
