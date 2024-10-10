using System.Collections.Generic;
using Maroon.Physics.Optics.Light;
using Maroon.Physics.Optics.Manager;
using Maroon.Physics.Optics.Util;
using UnityEngine;

namespace Maroon.Physics.Optics.TableObject.OpticalComponent
{
    public class Mirror : OpticalComponent
    {
        [Header("Mirror Properties")] 
        public Vector3 r;
        public Vector3 n;
        public float R;
        public float Rc;
        
        private int _nrOfLatitudeSegments = 16;     // Number of rings along the Y-axis
        
        private float _prevR;
        private bool _flipped;

        public bool flipped => _flipped;

        private void Start()
        {
            UpdateProperties();
            LightComponentManager.Instance.RecalculateAllLightRoutes();
        }
             
        public override void UpdateProperties()
        {
            r = transform.localPosition;
            n = transform.right;
            RecalculateMesh();
        }

        public void SetParameters(MirrorParameters parameters)
        {
            this.R = parameters.R;
            this.Rc = parameters.Rc;
            _prevR = R;
        }

        // ---- Mirror helper methods ----
        // center of Mirror
        public Vector3 Center()
        {
            return r + R * n;
        }
        
        // normal to surface at p
        public Vector3 NormR(Vector3 p)
        {
            return (p - (this.r + this.R * this.n)).normalized;
        }
        
        // distance along cylinder from central plane to intersection with surface R
        public float Dc()
        { 
            var absR = Mathf.Abs(this.R);
            return absR - Mathf.Sqrt(absR*absR - AdjustRc() * AdjustRc());
        }
        
        public float AdjustRc()
        {
            return Mathf.Min(this.Rc,Mathf.Abs(this.R)); 
        } 
        
        // Calculations from Peter
        public override (float inRayLength, RaySegment reflection, RaySegment refraction) CalculateDistanceReflectionRefraction(RaySegment inRay)
        {
            float d = GetRelevantDistance(inRay.r0Local, inRay.n);

            Vector3 hitPoint = inRay.r0Local + inRay.n * d;
            return (d, CalcReflectedRay(hitPoint, inRay), null);
        }

        public override float GetRelevantDistance(Vector3 rayOrigin, Vector3 rayDirection)
        {
            (float d1, float d2) = Math.IntersectLineSphere(rayOrigin, rayDirection, R, Center());
            float dmin = Mathf.Infinity;
            
            // skip if d1 is negative, very small or NaN
            if (d1 > Constants.Epsilon && !float.IsNaN(d1))
            {
                var _r = rayOrigin + d1 * rayDirection;
                var r1 = _r - this.r;
                var r1dotnc = Vector3.Dot(r1, this.n);
                var rperp = r1 - r1dotnc * this.n;
                
                // is r within the cylinder?
                if (rperp.magnitude < this.AdjustRc())
                {
                    // close solution
                    if (Mathf.Abs(r1dotnc) < this.Dc())
                        dmin = d1;
                }
            }
            // skip if d2 is negative or vey small
            if (d2 > Constants.Epsilon && !float.IsNaN(d2))
            {
                var _r = rayOrigin + d2 * rayDirection;  
                var r1 = _r - this.r;
                var r1dotnc = Vector3.Dot(r1, this.n);
                var rperp = r1 - r1dotnc * this.n;
                
                // is r within the cylinder?
                if (rperp.magnitude < this.AdjustRc())
                { 
                    // close solution
                    if (Mathf.Abs(r1dotnc) < this.Dc())
                        if (d2 < dmin) 
                            dmin = d2;
                }
            }
            return dmin;
        }

        private RaySegment CalcReflectedRay(Vector3 hitPoint, RaySegment inRay)
        {
            var normal = this.NormR(hitPoint);
            var reflectedRayDirection = Vector3.Reflect(inRay.n, normal);
            return new RaySegment(hitPoint, inRay.intensity, inRay.wavelength, reflectedRayDirection);
        }
        
        // ----------------------------------- Mesh Calculation -----------------------------------
        public override void RecalculateMesh()
        {
            TranslateComponent();
            Mesh inner = new Mesh();
            Mesh outer = new Mesh();
            var mcm = MeshCalculationManager.Instance;

            if (R == 0) return;
            float baseYRotation = (R < 0) ? 90f : -90f;
            
            var (verticesInner, normalsInner) = 
                mcm.CalculateHalfSphereVerticesNormals(
                    R, 
                    AdjustRc(), 
                    n, 
                    nrOfSegments, 
                    _nrOfLatitudeSegments, 
                    baseYRotation
                );
            List<Vector3> verticesOuter = new List<Vector3>(verticesInner);
            List<Vector3> normalOuter = new List<Vector3>(normalsInner);
            mcm.FlipNormals(ref normalOuter);

            var trianglesInner = mcm.CalculateHalfSphereFaces(nrOfSegments, _nrOfLatitudeSegments);
            List<int> trianglesOuter = new List<int>(trianglesInner);
            mcm.FlipTriangleVertexOrder(ref trianglesOuter);

            inner.SetVertices(verticesInner);
            inner.SetTriangles(trianglesInner, 0);
            inner.SetNormals(normalsInner);
            
            outer.SetVertices(verticesOuter);
            outer.SetTriangles(trianglesOuter, 0);
            outer.SetNormals(normalOuter);
            
            CombineInstance[] c = new CombineInstance[2];
            c[0].mesh = inner;
            c[0].transform = Matrix4x4.identity;
            c[1].mesh = outer;
            c[1].transform = Matrix4x4.identity;

            Mesh sphericalMirrorMesh = new Mesh();
            sphericalMirrorMesh.CombineMeshes(c);
            sphericalMirrorMesh.RecalculateNormals();
            
            // Assign the mesh to the MeshFilter component
            Component.GetComponent<MeshFilter>().mesh = sphericalMirrorMesh;
            Component.GetComponent<MeshCollider>().sharedMesh = sphericalMirrorMesh;
        }
        
        public void TranslateComponent()
        {
            Component.transform.localPosition = new Vector3(R, 0, 0);
        }
        
        public void TranslateArrows()
        {
            if ((R < 0 && _prevR > 0) || (R > 0 && _prevR < 0))
            {
                float factor = _flipped ? -1 : 1;
                Vector3 tmpScale = RotationArrowY.transform.localScale;
                tmpScale.x *= -1;
                RotationArrowY.transform.localScale = tmpScale;
                RotationArrowY.transform.localPosition += Constants.MirrorArrowShift * factor;
                
                tmpScale = RotationArrowZ.transform.localScale;
                tmpScale.x *= -1;
                RotationArrowZ.transform.localScale = tmpScale;
                RotationArrowZ.transform.localPosition += Constants.MirrorArrowShift * factor;

                TranslationArrowY.transform.localPosition += (Constants.MirrorTransformArrowPos + Constants.MirrorArrowShift) * factor;

                _flipped = !_flipped;
            }

            _prevR = R;
        }

    }
}
