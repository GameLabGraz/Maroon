using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightComponent
{
    public class LaserPointer : LightComponent
    {
        private LightRoute _sr;

        private void Start()
        {
            _sr = new LightRoute(transform.position, 1, 700);
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                _sr.UpdateOrigin(transform.position);
                transform.hasChanged = false;
            }
        }
    }
}