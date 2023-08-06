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


    // Start is called before the first frame update
    void Start()
    {
      weighableObject = GetComponent<WeighableObject>();
      renderer_ = GetComponent<MeshRenderer>();
      Debug.Log(renderer_);
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void toggleFill(bool fill)
    {
      Debug.Log(fill);
      if(fill)
      {
        fillPycnometer();
      }
      else
      {
        emptyPycnometer();
      }


    }
    private void fillPycnometer() //kg/m^3
    {
      float density = ViscosimeterManager.Instance.fluid_density_;
      weighableObject = GetComponent<WeighableObject>();
      Debug.Log("IID in Pycno: " + weighableObject.GetInstanceID());
      weighableObject.setWeight(weighableObject.starting_weight + (density * volume));
      renderer_.material.color = new Color(0.65f,0.16f,0.16f,1.0f);
    }

    private void emptyPycnometer()
    {
      weighableObject.resetWeight();
      renderer_.material.color = Color.white;
    }

    public void ResetObject()
    {
      emptyPycnometer();
    }

  }
}