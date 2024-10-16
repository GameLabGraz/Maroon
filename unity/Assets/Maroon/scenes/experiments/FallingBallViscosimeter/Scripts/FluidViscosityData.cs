using UnityEngine;
using UnityEngine.Serialization;

namespace Maroon.scenes.experiments.FallingBallViscosimeter.Scripts
{
    [CreateAssetMenu(fileName = "Fluid", menuName = "Scriptable Objects/FluidViscosityData", order = 1)]
    public class FluidViscosityData : ScriptableObject
    {
        public string fluidName;
        [Header("Viscosity Parameters (Curve-fit to f(x) = a * b^x + c)")]
        public double viscosity_a;
        public double viscosity_b;
        public double viscosity_c;
        
        [Header("Viscosity Parameters (Curve-fit to f(x) = a*x + b)")]
        public double density_a;
        public double density_b;

        public Color fluid_color;
    }
}
