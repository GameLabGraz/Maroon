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
        
        public RaySegment(Vector3 startpoint, Vector3 endpoint, float intensity, float wavelength)
        {
            // Transform in local coords (startpoint, endpoint) into global coords
            Vector3 globalStart = startpoint + Constants.TableBaseOffset + new Vector3(0, Constants.TableObjectHeight, 0);
            Vector3 globalEnd = endpoint + Constants.TableBaseOffset + new Vector3(0, Constants.TableObjectHeight, 0);
            
            this.r0 = globalStart;
            this.n = Vector3.Normalize(globalEnd - globalStart);
            this.intensity = intensity;
            this.wavelength = wavelength;
            this.endpoint = globalEnd;
            raylength = Vector3.Distance(globalEnd ,globalStart);
            
            GameObject ls = new GameObject("LineSegment");
            _lineSegment = ls.AddComponent<LineRenderer>();
            _lineSegment.material = new Material(Shader.Find("Sprites/Default"));
            _lineSegment.startColor = Util.Math.WavelengthToColor(wavelength);
            _lineSegment.endColor = Util.Math.WavelengthToColor(wavelength);
            _lineSegment.positionCount = 2;
            _lineSegment.SetPosition(0, this.r0);
            _lineSegment.SetPosition(1, this.endpoint);
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

        public void DestroyRaySegment()
        {
            GameObject.Destroy(_lineSegment.gameObject);
        }

    }
}