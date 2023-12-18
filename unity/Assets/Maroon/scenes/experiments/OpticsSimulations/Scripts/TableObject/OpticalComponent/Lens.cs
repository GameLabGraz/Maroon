using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Light;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Util;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject.OpticalComponent
{
    public class Lens : OpticalComponent
    {
        [Header("Lens Properties")] public Vector3 r;
        public Vector3 n;
        public float R1;
        public float R2;
        public float d1;
        public float d2;
        public float Rc0;
        public float Rc;
        public float A;
        public float B;
        public float lambda;

        private int _nrOfLatitudeSegments = 16; // Number of rings along the Y-axis

        private void Start()
        {
            R1 = 0.1f;
            R2 = -0.1f;
            d1 = 0.2f;
            d2 = 0.2f;
            Rc = 0.1f;
            A = 1.728f;
            B = 13420f;
            UpdateProperties();
            LightComponentManager.Instance.RecalculateAllLightRoutes();
        }

        public void AdjustCylinderRadiusIfUnphysical()
        {
            float Rc1;
            float Rc2;
            if (this.R1 < 0)
                Rc1 = Mathf.Abs(this.R1);
            else
            {
                if (0.5 * this.d1 > this.R1)
                    Rc1 = Mathf.Abs(this.R1);
                else
                    Rc1 = Mathf.Sqrt(this.R1 * this.R1 - (this.R1 - this.d1) * (this.R1 - this.d1));
            }

            if (this.R2 < 0)
            {
                if (this.d2 > Mathf.Abs(this.R2))
                    Rc2 = Mathf.Abs(this.R2);
                else
                    Rc2 = Mathf.Sqrt(this.R2 * this.R2 - (this.R2 + this.d2) * (this.R2 + this.d2));
            }
            else
                Rc2 = Mathf.Abs(this.R2);

            this.Rc = Mathf.Min(this.Rc, Rc1, Rc2);
            var dn = this.n.magnitude;
            if (Mathf.Abs(1 - dn) > 1E-6)
                throw new Exception("Lens Error: " + this.n.ToString("f3") + "is not a normalized unit vector.");
        }

        public override void UpdateProperties()
        {
            r = transform.localPosition;
            n = transform.right;
            RecalculateMesh();
            // AdjustCylinderRadiusIfUnphysical();     // TODO check if necessary
        }

        // ---- Lens helper methods ----

        // center of R1 surface
        private Vector3 Center1()
        {
            return this.r + (this.R1 - this.d1) * this.n;
        }

        // center of R2 surface
        private Vector3 Center2()
        {
            return this.r + (this.R2 + this.d2) * this.n;
        }

        //distance along cylinder from central plane to intersection with surface R1
        private float Dc1()
        {
            if (this.R1 < 0)
                return this.d1 + Mathf.Abs(this.R1) - Mathf.Sqrt(this.R1 * this.R1 - this.Rc * this.Rc);
            // R1 > 0
            return this.d1 - (this.R1 - Mathf.Sqrt(this.R1 * this.R1 - this.Rc * this.Rc));
        }

        //distance along cylinder from central plane to intersection with surface R2
        private float Dc2()
        {
            if (this.R2 < 0)
                return this.d2 - (Mathf.Abs(this.R2) - Mathf.Sqrt(this.R2 * this.R2 - this.Rc * this.Rc));
            // R2 > 0
            return this.d2 + (this.R2 - Mathf.Sqrt(this.R2 * this.R2 - this.Rc * this.Rc));
        }

        // maximum distance of lens from central plane to surface R1
        private float D1()
        {
            if (this.R1 < 0)
                return this.d1 + Mathf.Abs(this.R1) - Mathf.Sqrt(this.R1 * this.R1 - this.Rc * this.Rc);
            // R1 > 0
            return this.d1;
        }

        // maximum distance of lens from central plane to surface R2
        private float D2()
        {
            if (this.R2 < 0)
                return this.d2;
            // R2 > 0
            return this.d2 + (this.R2 - Mathf.Sqrt(this.R2 * this.R2 - this.Rc * this.Rc));
        }

        // index of refraction
        private float Ior(float lambda1)
        {
            return this.A + this.B / (lambda1 * lambda1);
        }

        // normal to surface R1 at p
        private Vector3 NormR1(Vector3 p)
        {
            return (1 / this.R1) * (p - this.Center1());
        }

        // normal to surface R2 at p
        private Vector3 NormR2(Vector3 p)
        {
            return -(1 / this.R2) * (p - this.Center2());
        }

        // normal to cylinder at p
        private Vector3 Normcyl(Vector3 p)
        {
            var rdotn = Vector3.Dot(p - this.r, this.n);
            var r_para = rdotn * this.n;
            return ((p - this.r) - r_para).normalized;
        }

        // adjust Rc for new R1,R2
        private void AdjustRc()
        {
            float Rc1;
            float Rc2;
            if (this.R1 < 0)
                Rc1 = Mathf.Abs(this.R1);
            else
            {
                if (this.d1 > this.R1)
                    Rc1 = Mathf.Abs(this.R1);
                else
                    Rc1 = Mathf.Sqrt(this.R1 * this.R1 - (this.R1 - this.d1) * (this.R1 - this.d1));
            }

            if (this.R2 < 0)
            {
                if (this.d2 > Mathf.Abs(this.R2))
                    Rc2 = Mathf.Abs(this.R2);
                else
                    Rc2 = Mathf.Sqrt(this.R2 * this.R2 - (this.R2 + this.d2) * (this.R2 + this.d2));
            }
            else
                Rc2 = Mathf.Abs(this.R2);

            this.Rc = Mathf.Min(this.Rc0, Rc1, Rc2);
        }

        // use small angle approximation to estimate the focal length in air
        private float FocalLength(float lambda1)
        {
            this.lambda = lambda1; // wavelength in nm that the focal length is estimated for
            return 1 / ((1 / this.R1 - 1 / this.R2) * (this.Ior(this.lambda) - 1));
        }

        // ---- Lens helper reflection and refraction methods ----

        public override (float inRayLength, RaySegment reflection, RaySegment refraction)
            CalculateDistanceReflectionRefraction(RaySegment inRay)
        {
            // todo immer inRay.r0Local und inRay.endpointLocal verwenden
            float dCylinder = GetDistanceCylinder(inRay.r0Local, inRay.n);
            float dR1 = GetDistanceR1(inRay.r0Local, inRay.n);
            float dR2 = GetDistanceR2(inRay.r0Local, inRay.n);

            float dmin = Mathf.Min(dCylinder, Mathf.Min(dR1, dR2));

            Vector3 hitPoint = inRay.r0Local + inRay.n * dmin;
            RaySegment reflection = null;
            RaySegment refraction = null;

            Vector3 normal = this.NormR2(hitPoint);

            // Nearest surface is cylinder
            if (dCylinder < dR1 && dCylinder < dR2)
                normal = this.Normcyl(hitPoint);
            // Nearest surface is R1
            else if (dR1 < dCylinder && dR1 < dR2)
                normal = this.NormR1(hitPoint);
            // when none of the above is true -> Nearest surface is R2

            var rndotn = Vector3.Dot(inRay.n, normal);
            var rnpara = rndotn * normal;
            var rnperp = inRay.n - rnpara;
            var n_reflected = rnperp - rnpara;
            if (Mathf.Abs(rnperp.magnitude) < Constants.Epsilon)
            {
                var n_refracted = inRay.n;
                if ((1f - Constants.ReflectIntensity) * inRay.intensity > 0.01f) // got a refraction
                    refraction = new RaySegment(hitPoint, (1f - Constants.ReflectIntensity) * inRay.intensity,
                        inRay.wavelength, n_refracted);

                if (Constants.ReflectIntensity * inRay.intensity > 0.01f) // got a reflection
                    reflection = new RaySegment(hitPoint, Constants.ReflectIntensity * inRay.intensity,
                        inRay.wavelength, -1f * inRay.n);
            }
            else
            {
                float n1;
                float n2;
                if (rndotn < 0) // air to lens
                {
                    n1 = Util.Math.Nenv(inRay.wavelength, Constants.Aenv, Constants.Benv);
                    n2 = this.Ior(inRay.wavelength);
                }
                else // lens to air
                {
                    n2 = Util.Math.Nenv(inRay.wavelength, Constants.Aenv, Constants.Benv);
                    n1 = this.Ior(inRay.wavelength);
                }

                var theta1 = Mathf.Acos(Mathf.Abs(rndotn));
                if (n1 * Mathf.Sin(theta1) / n2 < 1f)
                {
                    var theta2 = Mathf.Asin(n1 * Mathf.Sin(theta1) / n2);
                    var lperp = Mathf.Tan(theta2);
                    var npara = rnpara.normalized;
                    var nperp = rnperp.normalized;
                    var r_refracted = npara + lperp * nperp;
                    var n_refracted = r_refracted.normalized;
                    if ((1f - Constants.ReflectIntensity) * inRay.intensity > 0.01f) // got a refraction
                        refraction = new RaySegment(hitPoint, (1f - Constants.ReflectIntensity) * inRay.intensity,
                            inRay.wavelength, n_refracted);

                    if (Constants.ReflectIntensity * inRay.intensity > 0.01f) // got a reflection
                        reflection = new RaySegment(hitPoint, Constants.ReflectIntensity * inRay.intensity,
                            inRay.wavelength, n_reflected);
                }
                else // total internal reflection  
                    reflection = new RaySegment(hitPoint, inRay.intensity, inRay.wavelength, n_reflected);
            }

            return (dmin, reflection, refraction);
        }


        public override float GetRelevantDistance(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float dCylinder = GetDistanceCylinder(rayOrigin, rayDirection);
            float dR1 = GetDistanceR1(rayOrigin, rayDirection);
            float dR2 = GetDistanceR2(rayOrigin, rayDirection);

            return Mathf.Min(dCylinder, Mathf.Min(dR1, dR2));
        }

        private float GetDistanceCylinder(Vector3 rayOrigin, Vector3 rayDirection)
        {
            (float d1, float d2) = Util.Math.IntersectLineCylinder(rayOrigin, rayDirection, this.Rc, this.r, this.n);
            float dmin = Mathf.Infinity;

            // skip if d1 is negative, very small or NaN
            if (d1 > Constants.Epsilon && !float.IsNaN(d1))
            {
                var _r = rayOrigin + (d1 * rayDirection);
                var rdotnc = Vector3.Dot((_r - this.r), this.n);

                // negative side of the optical axis
                if (rdotnc <= 0 && Mathf.Abs(rdotnc) < this.Dc1() && d1 < dmin)
                    dmin = d1;

                // positive side of the optical axis
                else if (Mathf.Abs(rdotnc) < this.Dc2() && d1 < dmin)
                    dmin = d1;
            }

            // skip if d2 is negative, very small or NaN
            if (d2 > Constants.Epsilon && !float.IsNaN(d2))
            {
                //skip if d is negative  or very small  
                var _r = rayOrigin + (d2 * rayDirection);
                var rdotnc = Vector3.Dot((_r - this.r), this.n);


                // negative side of the optical axis
                if (rdotnc <= 0 && Mathf.Abs(rdotnc) < this.Dc1() && d2 < dmin)
                    dmin = d2;

                // positive side of the optical axis
                else if (Mathf.Abs(rdotnc) < this.Dc2() && d2 < dmin)
                    dmin = d2;
            }

            return dmin;
        }

        private float GetDistanceR1(Vector3 rayOrigin, Vector3 rayDirection)
        {
            (float d1, float d2) = Util.Math.IntersectLineSphere(rayOrigin, rayDirection, this.R1, this.Center1());
            float dmin = Mathf.Infinity;

            //skip if d1 is negative  or very small
            if (d1 > Constants.Epsilon && !float.IsNaN(d1))
            {
                var _r = rayOrigin + d1 * rayDirection;
                var r1 = _r - this.r;
                var r1dotnc = Vector3.Dot(r1, this.n);
                var rperp = r1 - r1dotnc * this.n;

                // is r within the cylinder? And correct curvature
                if (rperp.magnitude < this.Rc && Vector3.Dot(this.NormR1(_r), this.n) < 0 && d1 < dmin)
                    dmin = d1;
            }

            //skip if d2 is negative or vey small
            if (d2 > Constants.Epsilon && !float.IsNaN(d2))
            {
                var _r = rayOrigin + d2 * rayDirection;
                var r1 = _r - this.r;
                var r1dotnc = Vector3.Dot(r1, this.n);
                var rperp = r1 - r1dotnc * this.n;

                // is r within the cylinder? And correct curvature
                if (rperp.magnitude < this.Rc && Vector3.Dot(this.NormR1(_r), this.n) < 0 && d2 < dmin)
                    dmin = d2;
            }

            return dmin;
        }

        private float GetDistanceR2(Vector3 rayOrigin, Vector3 rayDirection)
        {
            (float _d1, float _d2) = Util.Math.IntersectLineSphere(rayOrigin, rayDirection, this.R2, this.Center2());
            float dmin = Mathf.Infinity;

            //skip if d1 is negative  or very small
            if (_d1 > Constants.Epsilon && !float.IsNaN(_d1))
            {
                var _r = rayOrigin + _d1 * rayDirection;
                var r1 = _r - this.r;
                var r1dotnc = Vector3.Dot(r1, this.n);
                var rperp = r1 - r1dotnc * this.n;

                // is r within the cylinder? And correct curvature
                if (rperp.magnitude < this.Rc && Vector3.Dot(this.NormR2(_r), this.n) > 0 && _d1 < dmin)
                    dmin = _d1;
            }

            //skip if d2 is negative or vey small
            if (_d2 > Constants.Epsilon && !float.IsNaN(_d2))
            {
                var _r = rayOrigin + _d2 * rayDirection;
                var r1 = _r - this.r;
                var r1dotnc = Vector3.Dot(r1, this.n);
                var rperp = r1 - r1dotnc * this.n;

                // is r within the cylinder? And correct curvature
                if (rperp.magnitude < this.Rc && Vector3.Dot(this.NormR2(_r), this.n) > 0 && _d2 < dmin)
                    dmin = _d2;
            }

            return dmin;
        }

        // ----------------------------------- Mesh Calculation -----------------------------------
        public override void RecalculateMesh()
        {
            if (d1 == 0 || d2 == 0 || R1 == 0 || R2 == 0 || Rc == 0)
                return;
            
            CombineInstance[] c = new CombineInstance[4];
            c[0].mesh = CalculateLeftSphere();
            c[0].transform = Matrix4x4.identity;
            c[1].mesh = CalculateCylinder(d1, R1, true);
            c[1].transform = Matrix4x4.identity;
            c[2].mesh = CalculateCylinder(d2, R2, false);
            c[2].transform = Matrix4x4.identity;
            c[3].mesh = CalculateRightSphere();
            c[3].transform = Matrix4x4.identity;
            
            // CombineInstance[] c = new CombineInstance[2];
            // c[0].mesh = CalculateCylinder(-d1, R1);
            // c[0].transform = Matrix4x4.identity;
            // c[1].mesh = CalculateCylinder(d2, R2);
            // c[1].transform = Matrix4x4.identity;

            Mesh lensMesh = new Mesh();
            lensMesh.CombineMeshes(c);
            lensMesh.RecalculateNormals();

            Component.GetComponent<MeshFilter>().mesh = lensMesh;
            Component.GetComponent<MeshCollider>().sharedMesh = lensMesh;
        }

        private Mesh CalculateLeftSphere()
        {
            Mesh leftSphere = new Mesh();
            var mcm = MeshCalculationManager.Instance;

            float yRotation = (R1 < 0) ? 90f : -90f;

            var (vertices, normals) =
                mcm.CalculateHalfSphereVerticesNormals(R1, Rc, n, nrOfSegments, _nrOfLatitudeSegments, yRotation, -d1 + R1);

            var triangles = mcm.CalculateHalfSphereFaces(nrOfSegments, _nrOfLatitudeSegments);

            leftSphere.SetVertices(vertices);
            leftSphere.SetTriangles(triangles, 0);
            leftSphere.SetNormals(normals);

            return leftSphere;
        }

        private Mesh CalculateRightSphere()
        {
            Mesh rightSphere = new Mesh();
            var mcm = MeshCalculationManager.Instance;

            float yRotation = (R2 < 0) ? 90f : -90f;

            var (vertices, normals) =
                mcm.CalculateHalfSphereVerticesNormals(R2, Rc, n, nrOfSegments, _nrOfLatitudeSegments, yRotation, d2 + R2);

            var triangles = mcm.CalculateHalfSphereFaces(nrOfSegments, _nrOfLatitudeSegments);

            rightSphere.SetVertices(vertices);
            rightSphere.SetTriangles(triangles, 0);
            rightSphere.SetNormals(normals);

            return rightSphere;
        }

        private Mesh CalculateCylinder(float thickness, float radius, bool isLeft)
        {
            Mesh cylinder = new Mesh();
            var mcm = MeshCalculationManager.Instance;

            radius = Mathf.Abs(radius);
            float cylinderThickness = isLeft ? -(thickness - radius) : (thickness - radius);

            List<Vector3> verticesLeftDisk =
                mcm.CalculateDiskVertices(n * cylinderThickness, radius, Mathf.Infinity, nrOfSegments);
            List<Vector3> verticesRightDisk = mcm.CalculateDiskVertices(Vector3.zero, radius, Mathf.Infinity, nrOfSegments);

            List<int> triangleIndices = mcm.CalculateRingFaces(verticesLeftDisk.Count, nrOfSegments);
            List<Vector3> verticesMantle = verticesLeftDisk.Concat(verticesRightDisk).ToList();
            List<Vector3> normalsMantle = mcm.CalculateMeshNormals(verticesMantle, triangleIndices);

            // if (thickness > 0)
            if (!isLeft)
                mcm.FlipTriangleVertexOrder(ref triangleIndices);
            
            cylinder.SetVertices(verticesMantle);
            cylinder.SetTriangles(triangleIndices, 0);
            cylinder.SetNormals(normalsMantle);

            return cylinder;
        }
    }
}