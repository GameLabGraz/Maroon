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
    public GameObject PendulumWeight;
    public GameObject StandRopeJoint;
    public GameObject StopWatch;
    private  bool mouseDown;
    private Vector3 mouseStart;
    private Vector3 lastForce;
    private GameObject lastLine;

    public float ropeLength = 0.2f;


    /// <summary>
    /// Initialization
    /// </summary>
    public void Start()
    {
        
    }

    public void Update()
    {

        HingeJoint joint = PendulumWeight.GetComponent<HingeJoint>();

        if (Input.GetMouseButtonUp(0))
        {
            StopWatch.SendMessage("SWStart");
            mouseDown = false;
            joint.useLimits = false;
        }
         else if (Input.GetMouseButtonDown(0) || mouseDown)
        {
            if (!mouseDown)
            {
                StopWatch.SendMessage("SWStop");
                mouseDown = true;
                mouseStart = Input.mousePosition;

            }

            limitHinge(joint, (mouseStart.x - Input.mousePosition.x) / 2);
        } 

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Debug.Log("Wheel Up");
            setRopeLengthRelative(0.01f);

        } else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Debug.Log("Wheel Down");
            setRopeLengthRelative(-0.01f);
        }

        var startPos = GameObject.Find(name + "/weight_obj").transform.position;
        startPos.Set(startPos.x, startPos.y, startPos.z + 0.0257f);
        DrawLine(startPos, StandRopeJoint.transform.position, new Color(0, 0, 0));
    }


     void setRopeLengthRelative(float value)
    {
        limitHinge(PendulumWeight.GetComponent<HingeJoint>(), 0);
        Vector3 currPos = PendulumWeight.transform.position;
        var obj = GameObject.Find(name + "/weight_obj");
        var pos = obj.transform.position;
        float newVal = Math.Max(-0.5f, ropeLength + value);
        newVal = Math.Min(newVal, -0.1f);
        ropeLength = newVal;

        pos.Set(transform.position.x, transform.position.y + ropeLength, transform.position.z);

        Debug.Log("new pos: " + pos.ToString());
        obj.transform.position = pos;
    }



    void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = lr.endColor = color;
        lr.startWidth = lr.endWidth = 0.001f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(lastLine);
        lastLine = myLine;
    }

    void limitHinge(HingeJoint joint, float angle)
    {
        JointLimits jl = new JointLimits {
            min = angle,
            max = angle + 0.0001f // because it bugs out otherwise...
        };
        joint.useLimits = true;
        joint.limits = jl;
    }



}
