using System.Collections;
using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;

public class OilHeatingElement : MonoBehaviour
{
    public static OilHeatingElement Instance;
    private ViscosimeterManager _viscosimeterManager;
    [SerializeField] private bool cooling = false;
    [SerializeField]private bool heating = false;
    [SerializeField]private float heatingSpeed = 0.1f;
    
    private void Awake()
    { 
      if(Instance == null)
      {
        Instance = this;
      }
    }
    // Start is called before the first frame update
    void Start()
    {
        _viscosimeterManager = ViscosimeterManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (heating)
        {
            _viscosimeterManager.FluidTemperature += (decimal)(heatingSpeed * Time.deltaTime);
        }
        else if (cooling)
        {
            _viscosimeterManager.FluidTemperature -= (decimal)(heatingSpeed * Time.deltaTime);
        }
    }

    public bool switchCooling()
    {
        cooling = !cooling;
        if (cooling)
        {
            heating = false;
        }

        return cooling;
    }

    public bool switchHeating()
    {
        heating = !heating;
        if (heating)
        {
            cooling = false;
        }

        return heating;
    }
}