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
    //public decimal start_diameter_ = 1.97m;
    public decimal start_diameter_ = 30.00m; //millimeter
    private bool dropped_ = true;
    private bool touching_oil = false;
    private decimal viscosity_force_ = 0.0m;
    private decimal buoyancy_force_ = 0.0m;

    private decimal ball_density_ = 8243.5949328270666898586m;
    private WeighableObject _weighableObject;


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


    private void Awake()
    {
      //randomize start diameter a bit here
      start_diameter_ = ViscosimeterManager.addInaccuracy(start_diameter_);
      
      Mass = ViscosimeterManager.addInaccuracy(Mass);
      _weighableObject = GetComponent<WeighableObject>();
      radius_ = start_diameter_ / 2000m;
      _volume = calculateVolume();
      Mass = ball_density_ * _volume;
      _weighableObject.starting_weight = Mass;
      start_position_ = transform.position;
      transform.localScale = new Vector3((float)start_diameter_/1000, (float)start_diameter_/1000, (float)start_diameter_/1000);
      _weighableObject.setWeight(_mass);
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
      //updateBall();
      ApplyFallPhysics();
    }



    decimal calculateVolume()
    {
      return (4.0m / 3.0m) * (decimal)Mathf.PI * radius_ * radius_ * radius_;
    }



    public void ResetObject()
    {
      dropped_ = false;
      touching_oil = false;
      Velocity = 0;

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
      decimal f_grav = Mass * G;

      if (!touching_oil)
      {
        ApplyForce(f_grav);
        return;
      }

      decimal fluid_density = ViscosimeterManager.Instance.fluid_density_;
      decimal temperature = ViscosimeterManager.Instance.fluid_temperature_;
      //calculate buoyancy force:
      decimal f_buoy = 0;
      if (_velocity != 0)
      {
        f_buoy = fluid_density * _volume * G;
      }
      
      //calculate dynamic viscosity
      decimal dynamic_viscosity = ViscosimeterManager.Instance.calculateDynamicViscosity(temperature);
      
      Debug.Log("dynamic visc " + dynamic_viscosity);
       
      //calculate viscosity friction force (stoke's law)
      decimal f_fric = 6.0m * (decimal)Math.PI * dynamic_viscosity * radius_ * Velocity;
      
      //calculate net force applied to ball
      decimal f_net = f_grav - (f_fric + f_buoy);
      
      ApplyForce(f_net);
    }

    void ApplyForce(decimal force)
    {
      decimal acceleration = force / Mass;
      Vector3 current_position = transform.position;
      Velocity += acceleration * (decimal)Time.deltaTime;
      bool noCollision = CheckCollision(current_position, Velocity);

      if (noCollision)
      {
        transform.position = new Vector3(current_position.x,
        current_position.y - (float)(Velocity * (decimal)Time.deltaTime),
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
          Velocity = 0;
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