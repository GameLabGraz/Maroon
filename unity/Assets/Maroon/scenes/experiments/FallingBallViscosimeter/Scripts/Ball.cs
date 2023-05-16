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

    //TODO: this is the diameter, not the radius. need to refactor this.
    //Radius in meter
    public QuantityFloat radius_ = 1.97f / 1000f;
    public float Radius
    {
      get => radius_;
      set
      {
        radius_.Value = value;
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
      start_radius_ = radius_.Value;
      Debug.Log("Radius: " + radius_.Value);
      start_position_ = transform.position;
      updateBall();
    }

    protected override void HandleUpdate()
    {
      
    }

    protected override void HandleFixedUpdate()
    {
      if (!dropped_)
      {
        return;
      }
      if (touching_oil)
      {
        //apply buoyancy
        gameObject.GetComponent<Rigidbody>().AddForce(transform.up * buoyancy_force_, ForceMode.Force);
        Debug.Log("Applied Bouyancy Force: " + buoyancy_force_);
        //apply viscosity friction force
        calculateViscosityForce();
        gameObject.GetComponent<Rigidbody>().AddForce(transform.up * viscosity_force_, ForceMode.Force);
        Debug.Log("Applied Viscosity Force: " + -viscosity_force_);
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
      float velocity = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
      Debug.Log("Velocity: " + velocity);
      float viscosity = (2.0f / 9.0f) * ((ball_density_ - ViscosimeterManager.Instance.fluid_density_) / velocity) * 9.81f * radius_ * radius_;
      viscosity_force_ = 6.0f * Mathf.PI * viscosity * radius_ * velocity;
    }



    public void updateBall()
    {
      float diameter = radius_;
      transform.localScale.Set(diameter, diameter, diameter);
      ball_density_ = calculateVolume() / weight_;
      calculateBuoyancy();
    }


    public void ResetObject()
    {
      dropped_ = false;
      weight_ = start_weight_;
      radius_ = start_radius_;
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
