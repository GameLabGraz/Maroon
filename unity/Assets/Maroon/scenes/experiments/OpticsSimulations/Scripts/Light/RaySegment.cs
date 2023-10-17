using System;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Light
{
    public class RaySegment
    {
        public Vector3 r0;
        public Vector3 endpoint;
        public Vector3 n;
        public float intensity;
        public float wavelength;
        public float raylength;

        private LineRenderer _lineSegment;

        public LineRenderer LineSegment
        {
            get => _lineSegment;
            set => _lineSegment = value;
        }

        public RaySegment(Vector3 r0, Vector3 n, float intensity, float wavelength)
        {
            this.r0 = r0;
            this.n = n;
            this.intensity = intensity;
            this.wavelength = wavelength;
            endpoint = r0;
            raylength = 0;
            
            GameObject ls = new GameObject("LineSegment");
            _lineSegment = ls.AddComponent<LineRenderer>();
            _lineSegment.material = new Material(Shader.Find("Sprites/Default"));
            _lineSegment.startColor = Math.Util.WavelengthToColor(wavelength);
            _lineSegment.endColor = Math.Util.WavelengthToColor(wavelength);
            _lineSegment.positionCount = 2;
            _lineSegment.SetPosition(0, r0);
            _lineSegment.SetPosition(1, r0 + n * raylength); // = r0 because we start with length 0
            _lineSegment.startWidth = Constants.LaserWidth;
            _lineSegment.endWidth = Constants.LaserWidth;
            _lineSegment.numCapVertices = 5;
            _lineSegment.useWorldSpace = false;
        }

        public void UpdateLength(float len)
        {
            raylength = len;
            endpoint = r0 + n * raylength;
            _lineSegment.SetPosition(1, endpoint);
        }

        public void UpdateStartingPoint(Vector3 startingPoint)
        {
            r0 = startingPoint;
            _lineSegment.SetPosition(0, r0);
        }
    }
}