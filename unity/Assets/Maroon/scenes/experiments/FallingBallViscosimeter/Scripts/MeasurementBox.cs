using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics
{
  [RequireComponent(typeof(MeasurableObject))]
  public class MeasurementBox : MonoBehaviour
  {

    public float length_;
    private MeshRenderer renderer_;

    private void OnMouseEnter()
    {
      renderer_.material.color = Color.red;
    }

    private void OnMouseExit()
    {
      renderer_.material.color = Color.white;
    }

    private void Awake()
    {
      renderer_ = GetComponent<MeshRenderer>();
      renderer_.enabled = false;
    }

    void toggleMeasurement(bool mode)
    {
      renderer_.enabled = mode;
    }

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
    
    }
  }
}