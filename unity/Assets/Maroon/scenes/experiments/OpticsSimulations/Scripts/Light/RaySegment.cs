using Maroon.Physics.Optics.Manager;
using Maroon.Physics.Optics.Util;
using UnityEngine;

namespace Maroon.Physics.Optics.Light
{
    public class RaySegment
    {
        public float wavelength;
        public float intensity;
        public Vector3 r0;
        public Vector3 r0Local;
        public Vector3 n;
        private Vector3 endpoint;
        private Vector3 endpointLocal;
        private float raylength;

        private LineRenderer _lineSegment;

        public LineRenderer LineSegment
        {
            get => _lineSegment;
            set => _lineSegment = value;
        }

        // Create RaySegment with unknown length
        public RaySegment(Vector3 startpoint, float intensity, float wavelength, Vector3 direction)
        {
            // Transform in local coords (startpoint, endpoint) into global coords
            Vector3 globalStart = ToGlobal(startpoint);
            
            this.r0 = globalStart;
            this.r0Local = startpoint;
            this.n = direction;
            this.intensity = intensity;
            this.wavelength = wavelength;
            this.endpoint = globalStart;
            this.endpointLocal = startpoint;
            this.raylength = 0;

            AddLineRenderer();
        }
        
        public RaySegment(Vector3 startpoint, Vector3 endpoint, float intensity, float wavelength)
        {
            // Transform in local coords (startpoint, endpoint) into global coords
            Vector3 globalStart = ToGlobal(startpoint);
            Vector3 globalEnd = ToGlobal(endpoint);
            
            this.r0 = globalStart;
            this.r0Local = startpoint;
            this.n = Vector3.Normalize(globalEnd - globalStart);
            this.intensity = intensity;
            this.wavelength = wavelength;
            this.endpoint = globalEnd;
            this.endpointLocal = endpoint;
            this.raylength = Vector3.Distance(globalEnd ,globalStart);

            AddLineRenderer();
        }

        private void AddLineRenderer()
        {
            GameObject ls = new GameObject("LineSegment");
            _lineSegment = ls.AddComponent<LineRenderer>();
            _lineSegment.material = new Material(Shader.Find("Sprites/Default"));
            _lineSegment.startColor = Util.Math.WavelengthToColor(this.wavelength, this.intensity);
            _lineSegment.endColor = Util.Math.WavelengthToColor(this.wavelength, this.intensity);
            _lineSegment.positionCount = 2;
            _lineSegment.SetPosition(0, this.r0);
            _lineSegment.SetPosition(1, this.endpoint);
            _lineSegment.startWidth = UIManager.Instance.rayThickness.Value / Constants.InMM;
            _lineSegment.endWidth = UIManager.Instance.rayThickness.Value / Constants.InMM;
            _lineSegment.numCapVertices = 5;
            _lineSegment.useWorldSpace = false;
            _lineSegment.sortingOrder = 1;

            ls.transform.parent = OpticalComponentManager.Instance.TableLowLeftCorner.transform;
        }

        private Vector3 ToGlobal(Vector3 localPoint)
        {
            return localPoint + Constants.TableBaseOffset + new Vector3(0, Constants.TableObjectHeight, 0);
        }

        public void UpdateLength(float len)
        {
            raylength = len;
            endpoint = r0 + n * raylength;
            endpointLocal = r0Local + n * raylength;
            _lineSegment.SetPosition(1, endpoint);
        }

        public void UpdateWavelength(float newWavelength)
        {
            this.wavelength = newWavelength;
            this._lineSegment.startColor = Util.Math.WavelengthToColor(wavelength, intensity);
            this._lineSegment.endColor = Util.Math.WavelengthToColor(wavelength, intensity);
        }
        
        public void UpdateIntensity(float newIntensity)
        {
            this.intensity = newIntensity;
            this._lineSegment.startColor = Util.Math.WavelengthToColor(wavelength, intensity);
            this._lineSegment.endColor = Util.Math.WavelengthToColor(wavelength, intensity);
        }

        public void UpdateStartingPoint(Vector3 startingPoint)
        {
            r0 = ToGlobal(startingPoint);
            r0Local = startingPoint;
            _lineSegment.SetPosition(0, r0);
        }

        public void DestroyRaySegment()
        {
            GameObject.Destroy(_lineSegment.gameObject);
        }

    }
}