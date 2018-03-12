//-----------------------------------------------------------------------------
// FieldLineManager.cs
//
// Controller class to manage the field lines
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller class to manage the field lines
/// </summary>
public class PendulumManager : MonoBehaviour
{
    private GameObject rope;
    private GameObject weight;
    private  bool mouseDown;
    private Vector3 mouseStart;
    private Vector3 lastForce;


    /// <summary>
    /// Initialization
    /// </summary>
    public void Start()
    {
        GameObject[] sensedObjects = GameObject.FindGameObjectsWithTag("Pendulum_Rope");
        if (sensedObjects.Length != 1)
            throw new Exception(String.Format("Found {0} ropes. only 1 is supported!", sensedObjects.Length));
        rope = sensedObjects[0];

        sensedObjects = GameObject.FindGameObjectsWithTag("Pendulum_Weight");
        if (sensedObjects.Length != 1)
            throw new Exception(String.Format("Found {0} weights. only 1 is supported!", sensedObjects.Length));
        weight = sensedObjects[0];

    }

    public void Update()
    {

        HingeJoint joint = rope.GetComponent<HingeJoint>();

        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            joint.useLimits = false;
        }
         else if (Input.GetMouseButtonDown(0) || mouseDown)
        {
            if (!mouseDown)
            {
                mouseDown = true;
                mouseStart = Input.mousePosition;
            }

            float angle = (mouseStart.x - Input.mousePosition.x) / 2 ;
            JointLimits jl = new JointLimits();
            jl.min = angle;
            jl.max = angle + 0.0001f; // because it bugs out otherwise...
            joint.useLimits = true;
            joint.limits = jl;

        } 

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Debug.Log("Wheel Up");
            var obj = GameObject.Find(name + "/default");
            
            Debug.Log(obj.name);
            var pos = obj.transform.position;
            pos.Set(transform.position.x, Math.Min(1, pos.y + 0.5f), transform.position.z);
            obj.transform.position = pos;
        } else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Debug.Log("Wheel Down");
            var obj = GameObject.Find(name + "/default");

            Debug.Log(name + "/default: " + obj.name);
            var pos = obj.transform.position;
            pos.Set(transform.position.x, Math.Max(-8, pos.y - 0.5f), transform.position.z);
            obj.transform.position = pos;
        }
    }


     void setRopeLength(int value)
    {
        Vector3 currPos = weight.transform.position;

        weight.transform.position.Set(currPos.x, (float)(value * 0.02695), currPos.z); //Yeah seems random, but calculates from the origin distance of 200 mm in the scaling of unity
    }
    

   
    
}
