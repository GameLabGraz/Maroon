using System.Collections.Generic;
using Maroon.ReusableScripts.ExperimentParameters;

namespace Maroon.Physics.ThreeDimensionalMotion
{
    [System.Serializable]
    public class ThreeDimensionalMotionParameters : ExperimentParameters
    {
        public string Background;
        public string Particle;
        public float T0;
        public float DeltaT;
        public float Steps;
        public float X;
        public float Y;
        public float Z;
        public float Vx;
        public float Vy;
        public float Vz;

        public string m;
        public string fx;
        public string fy;
        public string fz;

        public Dictionary<string, string> expressions = new Dictionary<string, string>();
    }
}