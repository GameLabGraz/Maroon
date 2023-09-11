using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Maroon.Physics
{
  public class ViscosimeterManager : MonoBehaviour
  {
    public static ViscosimeterManager Instance;
    
    public TMP_Text debug_text;

    public Ball ball;
    public Pycnometer pycnometer;


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
    
    MeasurableObject currentMeasurableObject;
    public List<MeasurableObject> measurableObjects;



    private void Awake()
    {
      if(Instance == null)
      {
        Instance = this;
      }
      calculateFluidDensity();
      getAllMeasurableObjects();
    }

    void calculateFluidDensity()
    {
      fluid_density_ = (fluid_temperature_ * -0.37f + 891.83f); //kg/m^3
      Debug.Log("Fluid Density: " + fluid_density_);
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
    void Update()
    {
      update_debug_text();
    }

    void update_debug_text()
    {
      float ball_velocity = ball.GetComponent<Rigidbody>().velocity.y;

      debug_text.text = "Ball Velocity:\n" + ball_velocity;
      
    }
  }
}
