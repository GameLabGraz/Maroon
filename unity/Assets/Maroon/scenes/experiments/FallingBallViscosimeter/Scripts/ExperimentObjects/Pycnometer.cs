using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.Physics
{
  public class Pycnometer : MonoBehaviour, IResetObject
  {
    float volume = 50.28f / 1000000.0f; //m^3
    WeighableObject weighableObject;
    float weight;
    private MeshRenderer renderer_;
    public bool filled;


    // Start is called before the first frame update
    void Start()
    {
      weighableObject = GetComponent<WeighableObject>();
      renderer_ = GetComponent<MeshRenderer>();
      Debug.Log(renderer_);
      filled = false;
    }

    // Update is called once per frame
    void Update()
    {
      
    }


    public void fillPycnometer() //kg/m^3
    {
      float density = ViscosimeterManager.Instance.fluid_density_;
      weighableObject = GetComponent<WeighableObject>();
      weighableObject.setWeight(weighableObject.starting_weight + (density * volume));
      renderer_.material.color = new Color(0.65f,0.16f,0.16f,0.3f);
      filled = true;
    }

    public void emptyPycnometer()
    {
      weighableObject.resetWeight();
      renderer_.material.color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
      filled = false;
    }

    public void ResetObject()
    {
      emptyPycnometer();
    }

  }
}