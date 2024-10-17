using Maroon.Physics.Optics.Light;
using Maroon.Physics.Optics.Manager;
using Maroon.Physics.Optics.Util;
using UnityEngine;

namespace Maroon.Physics.Optics.TableObject.OpticalComponent
{
    public class Eye : OpticalComponent
    {
        [Header("Eye Properties")]
        public Vector3 r0;
        public Vector3 n;
        public float f = 0.024f;
        private readonly float R = 0.012f;

        // normal to surface R at p
        private Vector3 NormR(Vector3 p)
        {
            return 1 / this.R * (p - this.r0);
        }
        
        private void Start()
        {
            UpdateProperties();
            LightComponentManager.Instance.RecalculateAllLightRoutes();
        }

        public override void UpdateProperties()
        {
            r0 = transform.localPosition;
            n = -transform.right;
        }
        
        public void SetParameters(EyeParameters parameters)
        {
            this.f = parameters.f;
        }

        public override (float inRayLength, RaySegment reflection, RaySegment refraction) CalculateDistanceReflectionRefraction(RaySegment inRay)
        {
            float d = GetRelevantDistance(inRay.r0Local, inRay.n);
            Vector3 hitPoint = inRay.r0Local + inRay.n * d;
            RaySegment reflection = null;
            RaySegment refraction = null;

            var normal = this.NormR(hitPoint);
            var rndotn = Vector3.Dot(inRay.n, normal);
            
            // check that the rays strike the pupil
            if (Vector3.Angle(hitPoint - this.r0, this.n) < Mathf.Rad2Deg * 0.25f)
            { 
                var rnpara = rndotn * normal;
                var rnperp = inRay.n - rnpara;
                var n_reflected = rnperp - rnpara;
                if (Mathf.Abs(rnperp.magnitude) < Constants.Epsilon)
                {
                    var n_refracted = inRay.n;
                    if ((1f - Constants.ReflectIntensity) * inRay.intensity > 0.01f) // got a refraction
                        refraction = new RaySegment(hitPoint, (1f - Constants.ReflectIntensity) * inRay.intensity, inRay.wavelength, n_refracted);

                    if (Constants.ReflectIntensity * inRay.intensity > 0.01f) // got a reflection
                        reflection = new RaySegment(hitPoint, Constants.ReflectIntensity * inRay.intensity, inRay.wavelength, inRay.n * -1f);
                }
                else
                {
                    // env to lens
                    var n1 = Util.Math.Nenv(inRay.wavelength, Constants.Aenv, Constants.Benv);
                    //adjust n2 so the focus is at f for n1 = 1
                    var n2 = (1f / this.R) / (1f / this.R - 1f / this.f);

                    var theta1 = Mathf.Acos(Mathf.Abs(rndotn));
                    if (n1 * Mathf.Sin(theta1) / n2 < 1f)
                    {
                        var theta2 = Mathf.Asin(n1 * Mathf.Sin(theta1) / n2);
                        var lperp = Mathf.Tan(theta2);
                        var npara = Vector3.Normalize(rnpara);
                        var nperp = Vector3.Normalize(rnperp);
                        var r_refracted = npara + lperp * nperp;
                        var n_refracted = Vector3.Normalize(r_refracted);
                        
                        if ((1f - Constants.ReflectIntensity) * inRay.intensity > 0.01f) // got a refraction
                            refraction = new RaySegment(hitPoint, (1f - Constants.ReflectIntensity) * inRay.intensity, inRay.wavelength, n_refracted);

                        if (Constants.ReflectIntensity * inRay.intensity > 0.01f) // got a reflection
                            reflection = new RaySegment(hitPoint, Constants.ReflectIntensity * inRay.intensity, inRay.wavelength, n_reflected);
                    }
                    else // total internal reflection 
                        reflection = new RaySegment(hitPoint, inRay.intensity, inRay.wavelength, n_reflected);
                }
            }

            return (d, reflection, refraction);
        }

        public override float GetRelevantDistance(Vector3 rayOrigin, Vector3 rayDirection)
        {
            (float d1, float d2) = Math.IntersectLineSphere(rayOrigin, rayDirection, R, r0);
            float dmin = Mathf.Infinity;

            // skip if (d1 or d2) is negative, very small or NaN
            if (d1 > Constants.Epsilon && !float.IsNaN(d1) && d1 < dmin)
                dmin = d1;
            if (d2 > Constants.Epsilon && !float.IsNaN(d2) && d2 < dmin)
                dmin = d2;
            return dmin;
        }
        
        public override void RecalculateMesh(){}
    }
}