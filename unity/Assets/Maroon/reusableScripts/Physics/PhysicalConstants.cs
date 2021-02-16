using UnityEngine;

namespace Maroon.Physics
{
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
    }
}