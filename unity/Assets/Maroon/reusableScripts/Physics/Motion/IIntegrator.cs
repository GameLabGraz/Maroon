using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.Physics.Motion;

public interface IIntegrator
{
    public MotionState Integrate(MotionState initial, double t, double dt);
}