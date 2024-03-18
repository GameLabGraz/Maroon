using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;

public class Faucet : MonoBehaviour
{

    public SnapPoint snapPoint;
    private Pycnometer _pycnometer;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        _pycnometer = snapPoint.currentObject.gameObject.GetComponent<Pycnometer>();
        if (_pycnometer != null)
        {
            if (_pycnometer.filled)
            {
                _pycnometer.emptyPycnometer();
            }
            else
            {
                _pycnometer.fillPycnometer();
            }
        }
    }
}
