using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics
{
  public class Ball : PausableObject, IResetObject, IWeighableObject
  {
    //ball variables
    private Vector3 start_position_;
    private float start_weight_;
    private float start_radius_;
    private bool dropped_ = true;
    private bool touching_oil = false;
    private float viscosity_force_ = 0.0f;
    private float buoyancy_force_ = 0.0f;

    private float ball_density_ = 0.0f;

    private float radius_;
    //diameter in meter
    public QuantityFloat diameter_ = 1.97f / 1000f;
    public float Diameter
    {
      get => diameter_;
      set
      {
        diameter_.Value = value;
        radius_ = diameter_.Value/2;
        updateBall();
      }
    }

    //Weight in kg
    public QuantityFloat weight_ = 0.033f / 1000f;

    public float Weight
    {
      get => weight_;
      set
      {
        weight_.Value = value;
        updateBall();
      }
    }



    protected override void Start()
    {
      base.Start();
      start_weight_ = weight_.Value;
      start_radius_ = diameter_.Value;
      radius_ = diameter_.Value/2;
      Debug.Log("Diameter: " + diameter_.Value);
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
        gameObject.GetComponent<Rigidbody>().AddForce(transform.up * (viscosity_force_ + buoyancy_force_), ForceMode.Force);
        Debug.Log("Applied Viscosity Force: " + viscosity_force_);
      }
    }



    float calculateVolume()
    {
      return (4.0f / 3.0f) * Mathf.PI * radius_ * radius_ * radius_;
    }


    void calculateBuoyancy()
    {
      //to make this more accurate volume should only be the displaced volume
      float volume = calculateVolume();
      Debug.Log("Volume: " + volume);
      buoyancy_force_ = (volume * ViscosimeterManager.Instance.fluid_density_ * 9.81f); //kg/m^3
      Debug.Log("Buoyancy: " + buoyancy_force_);
    }


    void calculateViscosityForce()
    {
      Debug.Log("Radius: " + radius_);
      float velocity = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
      float viscosity = (2.0f / 9.0f) * ((ball_density_ - ViscosimeterManager.Instance.fluid_density_) / velocity) * 9.81f * radius_ * radius_;
      Debug.Log("Velocity: " + velocity);
      Debug.Log("Viscosity: " + viscosity);
      viscosity_force_ = 6.0f * Mathf.PI * -viscosity * radius_ * velocity;
    }



    public void updateBall()
    {
      transform.localScale.Set(diameter_, diameter_, diameter_);
      ball_density_ = calculateVolume() / weight_;
      gameObject.GetComponent<Rigidbody>().mass = weight_;
      calculateBuoyancy();
    }


    public void ResetObject()
    {
      dropped_ = false;
      weight_ = start_weight_;
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
    public float getWeight()
    {
      return weight_.Value;
    }
  
    public void setWeight(float value)
    {
      weight_.Value = value;
    }
  }





}
