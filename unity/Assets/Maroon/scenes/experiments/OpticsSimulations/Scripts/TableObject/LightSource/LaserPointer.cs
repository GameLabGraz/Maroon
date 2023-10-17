using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.LightSource
{
    public class LaserPointer : LightSource
    {
        private List<RaySegment> _raySegments = new List<RaySegment>();

        public List<RaySegment> RaySegments
        {
            get => _raySegments;
            set => _raySegments = value;
        }

        void Start()
        {
            _raySegments.Add(
                new RaySegment(
                    transform.position, 
                    transform.position + Vector3.right * 0.5f, 
                    1f, 
                    Color.red));
        }

    }
}
