using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Physics
{
  public class Ball : PausableObject, IResetObject
  {
    //ball variables
    private Vector3 start_position_;
    private float start_weight_;
    private float start_radius_;
    private bool dropped_ = true;
    private bool touching_oil = false;
    private float viscosity_force_ = 0.0f;
    private float buoyancy_force_ = 0.0f;

    public QuantityFloat radius_ = 0.5f;
    public float Radius
    {
      get => radius_;
      set
      {
        radius_.Value = value;
        updateBall();
      }
    }


    public QuantityFloat weight_ = 1f;

    public float Weight
    {
      get => weight_;
      set
      {
        weight_.Value = value;
        updateBall();
      }
    }

    //fluid variables
    float fluid_density_ = 0.0f;

    public QuantityFloat fluid_temperature_ = 20.0f;
    public float FluidTemperature
    {
      get => fluid_temperature_;
      set
      {
        fluid_temperature_.Value = value;
        updateBall();
      }
    }

    //private Rigidbody rigidbody_ = this.gameObject.GetComponent<Rigidbody>();





    protected override void Start()
    {
      base.Start();
      start_weight_ = weight_;
      start_radius_ = radius_;
      start_position_ = transform.position;
      updateBall();

    }

    protected override void HandleUpdate()
    {
      if (!dropped_)
      {
        return;
      }
      if (touching_oil)
      {
        gameObject.GetComponent<Rigidbody>().AddForce(transform.up * 10, ForceMode.Acceleration);
        Debug.Log("Touching Oil!");
      }
    }

    protected override void HandleFixedUpdate()
    {

    }



    float calculateVolume()
    {
      return (4.0f / 3.0f) * Mathf.PI * radius_;
    }



    void calculateFluidDensity()
    {
      fluid_density_ = fluid_temperature_ * -0.37f + 891.83f;
    }



    void calculateBuoyancy()
    {
      //to make this more accurate volume should only be the displaced volume
      float volume = calculateVolume();
      buoyancy_force_ = volume * fluid_density_ * 9.81f;
      Debug.Log("Buoyancy: " + buoyancy_force_);
    }



    void updateBall()
    {
      float diameter = radius_ * 2;
      transform.localScale.Set(diameter, diameter, diameter);
      calculateFluidDensity();
      calculateBuoyancy();
    }




    public void ResetObject()
    {
      dropped_ = false;
      weight_ = start_weight_;
      radius_ = start_radius_;

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
