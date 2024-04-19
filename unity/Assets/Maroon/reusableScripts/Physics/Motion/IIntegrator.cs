using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.Physics.Motion;

public interface IIntegrator
{
    public State Integrate(State initial, float t, float dt);
}