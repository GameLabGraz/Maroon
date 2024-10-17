using System.Collections.Generic;
using System.Linq;
using Maroon.Physics.Optics.Light;
using Maroon.Physics.Optics.Manager;
using Maroon.Physics.Optics.Util;
using UnityEngine;

namespace Maroon.Physics.Optics.TableObject.OpticalComponent
{
    public class Aperture : OpticalComponent
    {
        [Header("Aperture Properties")] 
        public Vector3 r;
        public Vector3 n;
        public float Rin;
        public float Rout;
        
        private readonly float _thickness = 0.0003f;

        private void Start()
        {
            UpdateProperties();
            LightComponentManager.Instance.RecalculateAllLightRoutes();
            RecalculateMesh();
        }

        public override void UpdateProperties()
        {
            r = transform.localPosition;
            n = transform.right;
        }
        
        public void SetParameters(ApertureParameters parameters)
        {
            this.Rin = parameters.Rin;
            this.Rout = parameters.Rout;
        }

        public override (float inRayLength, RaySegment reflection, RaySegment refraction) CalculateDistanceReflectionRefraction(RaySegment inRay)
        {
            float d = GetRelevantDistance(inRay.r0Local, inRay.n);
            return (d, null, null);
        }
        
        public override float GetRelevantDistance(Vector3 rayOrigin, Vector3 rayDirection)
        {
            float d = Util.Math.IntersectLinePlane(rayOrigin, rayDirection, r, n);
            float dmin = Mathf.Infinity;
            
            // skip if d is negative, very small or NaN
            if (d > Constants.Epsilon && !float.IsNaN(d)) 
            {
                var _r = rayOrigin + d * rayDirection;
                var r1 = _r - this.r;
                    
                if (this.Rin < r1.magnitude && r1.magnitude < this.Rout && d < dmin)
                    dmin = d;
            }

            return dmin;
        }
        
        // ----------------------------------- Mesh Calculation -----------------------------------
        public override void RecalculateMesh()
        {
            Mesh leftRing = new Mesh();
            Mesh rightRing = new Mesh();
            Mesh outerMantle = new Mesh();
            Mesh innerMantle = new Mesh();
            var mcm = MeshCalculationManager.Instance;
            
            List<Vector3> verticesOuterLeft = mcm.CalculateDiskVertices(-n * (_thickness / 2), Rout, Mathf.Infinity, nrOfSegments);
            List<Vector3> verticesInnerLeft = mcm.CalculateDiskVertices(-n * (_thickness / 2), Rin, Mathf.Infinity, nrOfSegments);
            List<Vector3> verticesOuterRight = mcm.CalculateDiskVertices(n * (_thickness / 2), Rout, Mathf.Infinity, nrOfSegments);
            List<Vector3> verticesInnerRight = mcm.CalculateDiskVertices(n * (_thickness / 2), Rin, Mathf.Infinity, nrOfSegments);
            
            List<int> triangleIndices = mcm.CalculateRingFaces(verticesInnerLeft.Count, nrOfSegments);
            List<int> trianglesLeft =  new List<int>(triangleIndices);
            List<int> trianglesRight = new List<int>(triangleIndices);
            List<int> trianglesOuter = new List<int>(triangleIndices);
            List<int> trianglesInner = new List<int>(triangleIndices);

            mcm.FlipTriangleVertexOrder(ref trianglesLeft);
            mcm.FlipTriangleVertexOrder(ref trianglesInner);
            
            List<Vector3> verticesLeftRing = verticesOuterLeft.Concat(verticesInnerLeft).ToList();
            List<Vector3> verticesRightRing = verticesOuterRight.Concat(verticesInnerRight).ToList();
            List<Vector3> verticesOuterMantle = verticesOuterLeft.Concat(verticesOuterRight).ToList();
            List<Vector3> verticesInnerMantle = verticesInnerLeft.Concat(verticesInnerRight).ToList();
            
            List<Vector3> normalsLeftRing = mcm.CalculateMeshNormals(verticesLeftRing, trianglesLeft);
            List<Vector3> normalsRightRing = mcm.CalculateMeshNormals(verticesRightRing, trianglesRight);
            List<Vector3> normalsOuterMantle = mcm.CalculateMeshNormals(verticesOuterMantle, trianglesOuter);
            List<Vector3> normalsInnerMantle = mcm.CalculateMeshNormals(verticesInnerMantle, trianglesInner);
            
            leftRing.SetVertices(verticesLeftRing);
            rightRing.SetVertices(verticesRightRing);
            outerMantle.SetVertices(verticesOuterMantle);
            innerMantle.SetVertices(verticesInnerMantle);
            
            leftRing.SetTriangles(trianglesLeft, 0);
            rightRing.SetTriangles(trianglesRight, 0);
            outerMantle.SetTriangles(trianglesOuter, 0);
            innerMantle.SetTriangles(trianglesInner, 0);
            
            leftRing.SetNormals(normalsLeftRing);
            rightRing.SetNormals(normalsRightRing);
            outerMantle.SetNormals(normalsOuterMantle);
            innerMantle.SetNormals(normalsInnerMantle);
            
            CombineInstance[] c = new CombineInstance[4];
            c[0].mesh = leftRing;
            c[0].transform = Matrix4x4.identity;
            c[1].mesh = rightRing;
            c[1].transform = Matrix4x4.identity;
            c[2].mesh = outerMantle;
            c[2].transform = Matrix4x4.identity;
            c[3].mesh = innerMantle;
            c[3].transform = Matrix4x4.identity;
            Mesh apertureMesh = new Mesh();
            apertureMesh.CombineMeshes(c);
            apertureMesh.RecalculateNormals();
            
            Component.GetComponent<MeshFilter>().mesh = apertureMesh;
            Component.GetComponent<MeshCollider>().sharedMesh = apertureMesh;
        }
        
    }
}
