using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics
{
  public class Pycnometer : MonoBehaviour, IResetObject, IWeighableObject
  {
    float volume = 50.28f / 1000000.0f; //m^3

    float empty_weight = 29.167f / 1000.0f; //kg
    float weight;
    private MeshRenderer renderer_;


    // Start is called before the first frame update
    void Start()
    {

      weight = empty_weight;
      renderer_ = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void fill(float density) //kg/m^3
    {
      weight = empty_weight + (density * volume);
    }

    public void empty()
    {
      weight = empty_weight;
    }

    public void ResetObject()
    {
      empty();
    }

    public float getWeight()
    {
      return weight;
    }

    public void setWeight(float weight)
    {
      return;
    }

  }
}