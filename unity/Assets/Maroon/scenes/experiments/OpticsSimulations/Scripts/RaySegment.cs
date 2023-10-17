using System;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts
{
    public class RaySegment
    {
        [SerializeField] private Vector3 p1;
        [SerializeField] private Vector3 p2;
        [SerializeField] private float intensity;
        [SerializeField] private Color color;

        private LineRenderer _lineSegment;

        public float Length => Vector3.Distance(p1, p2);
        
        public Vector3 P1
        {
            get => p1;
            set => p1 = value;
        }

        public Vector3 P2
        {
            get => p2;
            set => p2 = value;
        }

        public float Intensity
        {
            get => intensity;
            set => intensity = value;
        }

        public Color Color
        {
            get => color;
            set => color = value;
        }

        public RaySegment(Vector3 p1, Vector3 p2, float intensity, Color color)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.intensity = intensity;
            this.color = color;

            GameObject lo = new GameObject("LineSegment");
            
            _lineSegment = lo.AddComponent<LineRenderer>();
            _lineSegment.material = new Material(Shader.Find("Sprites/Default"));
            _lineSegment.startColor = color;
            _lineSegment.endColor = color;
            _lineSegment.positionCount = 2;
            _lineSegment.SetPosition(0, p1);
            _lineSegment.SetPosition(1, p2);
            _lineSegment.startWidth = Constants.LaserWidth;
            _lineSegment.endWidth = Constants.LaserWidth;
            _lineSegment.numCapVertices = 5;
            _lineSegment.useWorldSpace = false;
        }
    }
}