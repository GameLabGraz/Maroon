using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics
{
  public class Ball : PausableObject, IResetObject
  {
    //ball variables
    private Vector3 start_position_;
    private decimal start_weight_;
    private decimal start_radius_;
    private bool dropped_ = true;
    private bool touching_oil = false;
    private decimal viscosity_force_ = 0.0m;
    private decimal buoyancy_force_ = 0.0m;

    private decimal ball_density_ = 0.0m;
    
    private decimal radius_;
    //diameter in meter
    public Quantity<decimal> diameter_;
    public decimal Diameter
    {
      get => diameter_.Value;
      set
      {
        diameter_.Value = value;
        radius_ = diameter_.Value/2;
        updateBall();
      }
    }

    //Weight in kg
    public Quantity<decimal> weight_;

    public decimal Weight
    {
      get => weight_.Value;
      set
      {
        weight_.Value = value;
        updateBall();
      }
    }

    private Rigidbody _rigidbody;

    private void Awake()
    {
      _rigidbody = GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
      base.Start();
      start_weight_ = 1.97m / 1000m;
      start_radius_ = 0.033m / 1000m;
      Weight = start_weight_;
      Diameter = start_radius_;
      radius_ = Diameter/2;
      Debug.Log("Diameter: " + Diameter);
      start_position_ = transform.position;
      updateBall();
    }

    protected override void HandleUpdate()
    {
      
    }

    protected override void HandleFixedUpdate()
    {
      Debug.Log("Oil " + touching_oil);
      if (touching_oil)
      {
        //apply viscosity friction force
        calculateViscosityForce();
        _rigidbody.AddForce(transform.up * ((float)viscosity_force_ + (float)buoyancy_force_), ForceMode.Force);
        Debug.Log("Applied Viscosity Force: " + viscosity_force_);
      }
    }



    decimal calculateVolume()
    {
      return (4.0m / 3.0m) * (decimal)Mathf.PI * radius_ * radius_ * radius_;
    }


    void calculateBuoyancy()
    {
      //to make this more accurate volume should only be the displaced volume
      decimal volume = calculateVolume();
      //Debug.Log("Volume: " + volume);
      buoyancy_force_ = (volume * ViscosimeterManager.Instance.fluid_density_ * 9.81m); //kg/m^3
      //Debug.Log("Buoyancy: " + buoyancy_force_);
    }


    void calculateViscosityForce()
    {
      //Debug.Log("Radius: " + radius_);
      decimal velocity = (decimal)_rigidbody.velocity.magnitude;
      decimal viscosity = (2.0m / 9.0m) * ((ball_density_ - ViscosimeterManager.Instance.fluid_density_) / velocity) * 9.81m * radius_ * radius_;
      //Debug.Log("Velocity: " + velocity);
      //Debug.Log("Viscosity: " + viscosity);
      viscosity_force_ = 6.0m * (decimal)Mathf.PI * -viscosity * radius_ * velocity;
    }



    public void updateBall()
    {
      transform.localScale.Set((float)Diameter, (float)Diameter, (float)Diameter);
      ball_density_ = calculateVolume() / Weight;
      _rigidbody.mass = (float)Weight;
      calculateBuoyancy();
    }


    public void ResetObject()
    {
      dropped_ = false;
      Weight = start_weight_;
      diameter_.Value = start_radius_;
      radius_ = diameter_.Value / 2;
      touching_oil = false;

      transform.position = start_position_;
      //rigidbody_.velocity = Vector3.zero;
    }

    public void dropBall()
    {
      dropped_ = true;
    }

    private void OnTriggerEnter(Collider other)
    {
      touching_oil = true;
    }

    private void OnTriggerExit(Collider other)
    {
      touching_oil = false;
    }
  }
}
