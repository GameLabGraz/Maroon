using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts
{
    public class RaySegment : MonoBehaviour
    {
        [SerializeField] private Vector2 p1;
        [SerializeField] private Vector2 p2;
        [SerializeField] private float intensity;
        [SerializeField] private Color color;

        public Vector2 P1
        {
            get => p1;
            set => p1 = value;
        }

        public Vector2 P2
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
    }
}