using UnityEngine;

namespace Maroon.Experiments.PlanetarySystem
{
    public class PlanetRotation : MonoBehaviour
    {
        public PlanetInfo planetInfo;

        private const float ROTATION_PERIOD_SCALE_FACTOR = 23.9f;
        float planetRotationPerSecond = 0;

        /// <summary>
        /// RotatePlanets every 0.02 sec = 50x per sec
        /// </summary>
        private void FixedUpdate()
        {
            RotatePlanets();
        }


        /// <summary>
        /// rotate planets obliquityToOrbit = rotation angle
        /// called after resetting Simulation
        /// z axis
        /// </summary>
        public void SetObliquityToOrbit()
        {
            transform.localRotation = Quaternion.identity;
            transform.Rotate(new Vector3(0, 0, -planetInfo.obliquityToOrbit));
            //Debug.Log("PlanetRotation: SetObliquityToOrbit(): z" + transform.rotation);
        }


        /// <summary>
        /// rotate planet in its rotation period
        /// y axis
        /// 360° rotation for each earth day (24h) in 1 FixedUpdate(0.02) * 50 for 1 sec
        /// </summary>
        public void RotatePlanets()
        {
            planetRotationPerSecond = (360 / -planetInfo.rotationPeriod) * (ROTATION_PERIOD_SCALE_FACTOR * Time.fixedDeltaTime);
            transform.Rotate(new Vector3(0, planetRotationPerSecond, 0));
        }
    }
}