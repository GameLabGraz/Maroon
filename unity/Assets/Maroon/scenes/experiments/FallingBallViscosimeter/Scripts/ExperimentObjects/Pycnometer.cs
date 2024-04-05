using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.Physics
{
  public class Pycnometer : MonoBehaviour, IResetObject
  {
    decimal volume = 50.28m / 1000000.0m; //m^3
    WeighableObject weighableObject;
    decimal starting_weight = 0.029167m;
    private MeshRenderer renderer_;
    public bool filled;

    // Start is called before the first frame update
    void Start()
    {
      weighableObject = GetComponent<WeighableObject>();
      starting_weight = ViscosimeterManager.addInaccuracy(starting_weight);
      weighableObject.starting_weight = starting_weight;
      weighableObject.resetWeight();
      renderer_ = GetComponent<MeshRenderer>();
      filled = false;
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void fillPycnometer() //kg/m^3
    {
      decimal density = ViscosimeterManager.Instance.fluid_density_;
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