using System;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Util
{
    public static class Math
    {
        
        public static bool IsValidPoint(Vector3 p)
        {
            return !float.IsNaN(p.x) && !float.IsNaN(p.y) && !float.IsNaN(p.z)
                   && !float.IsInfinity(p.x) && !float.IsInfinity(p.y) && !float.IsInfinity(p.z);
        }

        public static bool IsValidDistance(float d)
        {
            return !float.IsInfinity(d) && !float.IsNaN(d) && d > Constants.Epsilon;
        }
        
        /// <summary>
        /// <para>Find the intersection of a line and a plane. </para>
        /// </summary>
        /// <param name="r0">Initial point on the line</param>
        /// <param name="n">Unit vector in the direction of the line</param>
        /// <param name="p0">Point on the plane</param>
        /// <param name="np">Unit normal to the plane</param>
        /// <returns>Distance from r0 to plane if hit, NaN otherwise (plane is parallel to line)</returns>
        public static float IntersectLinePlane(Vector3 r0, Vector3 n, Vector3 p0, Vector3 np)
        { 
            float ndotnp = Vector3.Dot(n,np);
            if (ndotnp == 0)
                return Single.NaN;  // the line and plane are parallel
            
            float d = Vector3.Dot(np,p0-r0)/ndotnp;
            return d;
        }
        
        /// <summary>
        /// <para>Find the intersection of a line and a sphere</para>
        /// </summary>
        /// <param name="r0">Initial point on the line</param>
        /// <param name="n">Unit vector in the direction of the line</param>
        /// <param name="R">Radius of the sphere</param>
        /// <param name="C">Center point of the sphere</param>
        /// <returns>Distances from r0 to the sphere (first and second intersection) if hit, NaN otherwise</returns>
        public static (float, float) IntersectLineSphere(Vector3 r0, Vector3 n, float R, Vector3 C)
        { 
            float b = Vector3.Dot(n,r0 - C);
            float q  = Vector3.Dot(r0 - C,r0 - C) - R * R;
            float delta = b*b - q;
            if (delta > 0) {
                float d1 = -b + Mathf.Sqrt(delta);
                float d2 = -b - Mathf.Sqrt(delta); 
                return (d1, d2);
            }
            return (Single.NaN, Single.NaN);
        }
        
        /// <summary>
        /// <para>Find the intersection of a line and a cylinder</para>
        /// </summary>
        /// <param name="r0">Initial point on the line</param>
        /// <param name="n">Unit vector in the direction of the line</param>
        /// <param name="R">Radius of the cylinder</param>
        /// <param name="C">Point on the central axis</param>
        /// <param name="nc">Unit vector along the central axis</param>
        /// <returns>Distances from r0 to the cylinder (first and second intersection) if hit, NaN otherwise</returns>
        public static (float, float) IntersectLineCylinder(Vector3 r0, Vector3 n, float R, Vector3 C, Vector3 nc)
        {
            var r1 = r0 - C;
            var r1dotnc = Vector3.Dot(r1, nc);
            var rpara = r1dotnc * nc;
            var rperp = r1 - rpara;
            var ndotnc = Vector3.Dot(n, nc);
            var npara = ndotnc * nc;
            var nperp = n - npara;
            var a = Vector3.Dot(nperp, nperp);
            var b = 2 * Vector3.Dot(nperp, rperp);
            var c = Vector3.Dot(rperp, rperp) - R * R;
            if (b * b - 4 * a * c > 0)
                return (
                    (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a),
                    (-b - Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a)
                );

            return (Single.NaN, Single.NaN);
        }
        
        
        //http://www.physics.sfasu.edu/astro/color/spectra.html
        public static Color WavelengthToColor(float wavelength)
        {
            if (wavelength < 381)
                return new Color(1, 1, 1);
            if ((wavelength > 380) & (wavelength < 440)) 
                return new Color((440-wavelength)/(440-380), 0, 1);
            if ((wavelength>439)&(wavelength<490))
                return new Color(0, (wavelength-440)/(490-440), 1);
            if ((wavelength>489)&(wavelength<510))
                return new Color(0, 1, (510-wavelength)/(510-490));
            if ((wavelength>509)&(wavelength<580))
                return new Color((wavelength-510)/(580-510), 1, 0);
            if ((wavelength>579)&(wavelength<645))
                return new Color(1, (645-wavelength)/(645-580), 0);
            if ((wavelength > 644) & (wavelength < 780))
                return new Color(1, 0, 0);
            
            return new Color(1, 1, 1);
        }
    }
}