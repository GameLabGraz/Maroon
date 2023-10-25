using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Serialization;

namespace Maroon.Physics
{
  [RequireComponent(typeof(WeighableObject))]
  public class Ball : PausableObject, IResetObject
  {
    //ball variables
    private const decimal G = 9.81m;
    private decimal _velocity = 0;

    public decimal Velocity
    {
      get { return _velocity; }
      private set { _velocity = value; }
    }
    private decimal _volume = 0;
    
    private Vector3 start_position_;
    public decimal start_weight_ = 0.033m / 1000m;
    public decimal start_diameter_ = 1.97m;
    private bool dropped_ = true;
    private bool touching_oil = false;
    private decimal viscosity_force_ = 0.0m;
    private decimal buoyancy_force_ = 0.0m;

    private decimal ball_density_ = 0.0m;
    private WeighableObject _weighableObject;


    public QuantityDecimal diameter_millimeter_;
    public decimal DiameterMillimeter
    {
      get => diameter_millimeter_.Value;
      set
      {
        diameter_millimeter_.Value = value;
      }
    }
    
    private decimal radius_;
    //diameter in meter
    public QuantityDecimal diameter_;
    public decimal Diameter
    {
      get => diameter_.Value;
      set
      {
        diameter_.Value = value;
        radius_ = diameter_.Value/2;
      }
    }

    //Weight in kg
    [FormerlySerializedAs("weight_")] public QuantityDecimal _mass;

    public decimal Mass
    {
      get => _mass.Value;
      set
      {
        _mass.Value = value;
      }
    }

    private Rigidbody _rigidbody;

    private void Awake()
    {
      _rigidbody = GetComponent<Rigidbody>();
      _weighableObject = GetComponent<WeighableObject>();
      _mass = start_weight_;
      _weighableObject.starting_weight = start_weight_;
      DiameterMillimeter = start_diameter_;
      diameter_millimeter_.minValue = 1.0m;
      diameter_millimeter_.maxValue = 30.0m;
      radius_ = start_diameter_ / 2000m;
      ball_density_ = Mass / calculateVolume();
      start_position_ = transform.position;
      updateBall();
    }

    protected override void Start()
    {
      base.Start();
      
    }

    protected override void HandleUpdate()
    {
      
    }

    protected override void HandleFixedUpdate()
    {
      updateBall();
      ApplyFallPhysics();
    }



    decimal calculateVolume()
    {
      return (4.0m / 3.0m) * (decimal)Mathf.PI * radius_ * radius_ * radius_;
    }


    void calculateBuoyancy()
    {
      //to make this more accurate volume should only be the displaced volume
      buoyancy_force_ = (_volume * ViscosimeterManager.Instance.fluid_density_ * 9.81m); //kg/m^3
      Debug.Log("Buoyancy: " + buoyancy_force_);
    }

    
    void calculateViscosityForce()
    {
      //Debug.Log("Radius: " + radius_);
      decimal velocity = (decimal)_rigidbody.velocity.y;
      decimal viscosity = (2.0m / 9.0m) * ((ball_density_ - ViscosimeterManager.Instance.fluid_density_) / velocity) * 9.81m * radius_ * radius_;
      //Debug.Log("Velocity: " + velocity);
      Debug.Log("Viscosity: " + viscosity);
      viscosity_force_ = 6.0m * (decimal)Mathf.PI * viscosity * radius_ * velocity;
    }



    public void updateBall()
    {
      _volume = calculateVolume();
      Diameter = DiameterMillimeter / 1000m;
      radius_ = Diameter / 2.0m;
      transform.localScale = new Vector3((float)Diameter, (float)Diameter, (float)Diameter);
      _mass = ball_density_ * _volume;
      _weighableObject.setWeight(_mass);
      //calculateBuoyancy();
    }


    public void ResetObject()
    {
      dropped_ = false;
      touching_oil = false;
      _velocity = 0;

      transform.position = start_position_;
      //rigidbody_.velocity = Vector3.zero;
    }

    public void dropBall()
    {
      dropped_ = true;
    }

    private void OnTriggerStay(Collider other)
    {
      if (other.gameObject.layer == 4) //Water
      {
        touching_oil = true;
      }
    }

    private void OnTriggerExit(Collider other)
    {
      touching_oil = false;
    }

    void ApplyFallPhysics()
    {
      //calculate gravity force:
      decimal f_grav = _mass * G;

      if (!touching_oil)
      {
        ApplyForce(f_grav);
        return;
      }

      decimal fluid_density = ViscosimeterManager.Instance.fluid_density_;
      //calculate buoyancy force:
      decimal f_buoy = fluid_density * _volume * G;
      
      //calculate dynamic viscosity
      decimal dynamic_viscosity = (2.0m / 9.0m) *
                                  ((ball_density_ - fluid_density) / _velocity) * G *
                                  radius_ * radius_;
      
      Debug.Log("Dynamic Viscosity: " + dynamic_viscosity);
      
      //calculate viscosity friction force (stoke's law)
      decimal f_fric = 6.0m * (decimal)Math.PI * dynamic_viscosity * radius_ * _velocity;
      
      //calculate net force applied to ball
      decimal f_net = f_grav - (f_fric + f_buoy);
      
      ApplyForce(f_net);
    }

    void ApplyForce(decimal force)
    {
      decimal acceleration = force / _mass;
      Vector3 current_position = transform.position;
      _velocity += acceleration * (decimal)Time.deltaTime;
      bool noCollision = CheckCollision(current_position, _velocity);

      if (noCollision)
      {
        transform.position = new Vector3(current_position.x,
        current_position.y - (float)(_velocity * (decimal)Time.deltaTime),
        current_position.z);
      }
    }

    bool CheckCollision(Vector3 position, decimal velocity)
    {
      float range = (float)(velocity * (decimal)Time.deltaTime);
      Ray ray = new Ray(position, Vector3.down * range);
      Debug.DrawRay(position, Vector3.down * range);

      if (UnityEngine.Physics.Raycast(ray, out RaycastHit hit, range))
      {
        if (hit.collider.CompareTag("Floor"))
        {
          float possibleMovement = GetPossibleMovement(ray, range);
          transform.position = new Vector3(position.x, position.y - possibleMovement + (float)radius_, position.z);
          _velocity = 0;
          return false;
        }
      }

      return true;
    }

    float GetPossibleMovement(Ray ray, float range)
    {
      float new_range = range;

      while (UnityEngine.Physics.Raycast(ray, out RaycastHit hit, new_range))
      {
        new_range -= 0.001f;
      }
      return new_range;
    }
  }
}
