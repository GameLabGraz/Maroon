using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Util
{
    public static class Math
    {
        // find the intersection of a line and a sphere
        // r0 is the intitial point on the line, n is the unit vector in the direction of the line
        // R is the radius of the sphere, C is the center point of the sphere
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
            return (Mathf.Infinity, Mathf.Infinity);
        }
        // find the intersection of a line and a plane. 
        // r0 is the initial point on the line, n is the unit vector in the direction of the line
        // p0 is a point on the plane and np is the unit normal to the plane
        // Returns the distance from r0 to plane if hit, infinity otherwise
        public static float IntersectLinePlane(Vector3 r0, Vector3 n, Vector3 p0, Vector3 np)
        { 
            float ndotnp = Vector3.Dot(n,np);
            if (ndotnp == 0)
                return Mathf.Infinity;  // the line and plane are parallel
            
            float d = Vector3.Dot(np,p0-r0)/ndotnp;
            return d;
        }
        
        // function intersect_line_cylinder(r0,n,R,C,nc) { //find the intersection of a line and a cylinder
        //     // r0 is the intitial point on the line, n is the unit vector in the direction of the line
        //     // R is the radius of the cylinder, n is the unit vector along the central axis, C is a point on the central axis
        //     let r1 = vsub(r0,C); 
        //     let r1dotnc = dot(r1,nc);
        //     let rpara = dot(r1dotnc,nc);
        //     let rperp = vsub(r1,rpara);
        //     let ndotnc = dot(n,nc);
        //     let npara = dot(ndotnc,nc);
        //     let nperp = vsub(n,npara);
        //     let a = dot(nperp,nperp); 
        //     let b = 2*dot(nperp,rperp);
        //     let c = dot(rperp,rperp) - R*R;
        //     if ((b*b - 4*a*c) > 0) {
        //         return [(-b + Math.sqrt(b*b - 4*a*c))/(2*a), (-b - Math.sqrt(b*b - 4*a*c))/(2*a)];
        //     }
        //     else {
        //         return null;
        //     }
        // }
        
        
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