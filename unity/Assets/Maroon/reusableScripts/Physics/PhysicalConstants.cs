using UnityEngine;
using System.Collections.Generic;
namespace Maroon.Physics
{


    public enum CauchyMaterial
    {
        PrismGlass,
        WindowGlass,
        Acrylic,
        Water,
        Diamond,
        Sapphire,
        HardCrownGlass,
        DenseFlintGlass,
        FusedSilica,
        BorosilicateGlass
    }

    public struct Cauchy
    {
        public Cauchy(float a, float b)
        {
            A = a;
            B = b;
        }

        public float A;
        public float B;
    }


    public static class PhysicalConstants
    {
        /// <summary>
        /// The permittivity of free space in [C^2/Nm^2]
        /// </summary>
        public const float e0 = 8.8541878176e-12f;

        /// <summary>
        /// The relative permittivity of vacuum
        /// </summary>
        public static readonly float erVacuum = 1;

        /// <summary>
        /// The relative permittivity of vacuum over 4pi
        /// </summary>
        public static readonly float erVacuumOver4Pi = erVacuum / (4.0f * Mathf.PI);

        /// <summary>
        /// The field line strength factor
        /// </summary>
        public static readonly int FieldStrengthFactor = 100;


        public static readonly Dictionary<CauchyMaterial, Cauchy> CauchyValues = new Dictionary<CauchyMaterial, Cauchy>
        {
            {CauchyMaterial.PrismGlass,         new Cauchy(1.7387f, 0.01590f)},
            {CauchyMaterial.WindowGlass,        new Cauchy(1.5111f, 0.00425f)},
            {CauchyMaterial.Acrylic,            new Cauchy(1.4767f, 0.00480f)},
            {CauchyMaterial.Water,              new Cauchy(1.3244f, 0.00310f)},
            {CauchyMaterial.Diamond,            new Cauchy(2.3818f, 0.01210f)},
            {CauchyMaterial.Sapphire,           new Cauchy(1.7522f, 0.00550f)},
            {CauchyMaterial.HardCrownGlass,     new Cauchy(1.5220f, 0.00459f)},
            {CauchyMaterial.DenseFlintGlass,    new Cauchy(1.7280f, 0.01342f)},
            {CauchyMaterial.FusedSilica,        new Cauchy(1.4580f, 0.00354f)},
            {CauchyMaterial.BorosilicateGlass,  new Cauchy(1.5046f, 0.00420f)},
        };


    }
}