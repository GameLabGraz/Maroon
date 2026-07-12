using System.Collections.Generic;
using UnityEngine;

namespace Maroon.ComputerScience.ConvexHull3D
{
    public static class HullUtils
    {
        public const float Epsilon = 0.001f;

        public class Face
        {
            private List<int> _verts;
            public List<int> Verts
            {
                get { return _verts; }
            }

            public int a => Verts[0];
            public int b => Verts[1];
            public int c => Verts[2];

            public List<int> AssignedPoints = new List<int>();

            private Vector3 _normal;


            public Face(int a, int b, int c, List<Vector3> pts)
            {
                _verts = new List<int> { a, b, c };
                _normal = Vector3.Cross(pts[b] - pts[a], pts[c] - pts[a]).normalized;
            }

            public float Distance(Vector3 p, List<Vector3> pts)
            {
                return Vector3.Dot(p - pts[a], _normal);
            }

            public bool CanSee(Vector3 p, List<Vector3> pts)
            {
                return Distance(p, pts) > Epsilon;
            }

            public Vector3 Center(List<Vector3> pts)
            {
                return (pts[a] + pts[b] + pts[c]) / 3f;
            }
        }

        public struct Edge
        {
            public int a, b;
            public Edge(int a, int b) { this.a = a; this.b = b; }
        }

        //-------------------------------------------------------------------------

        
        public static int[] FindInitialTetrahedron(List<Vector3> pts)
        {
            if(pts.Count < 4) return null;

            int p0 = 0;
            int p1 = 1;
            float best = 0;

            for(int i = 0; i < pts.Count; i++)
            {
                for(int j = i + 1; j < pts.Count; j++)
                {
                    float d = (pts[i] - pts[j]).sqrMagnitude;
                    if(d > best)
                    {
                        best = d;
                        p0 = i;
                        p1 = j;
                    }
                }
            }

            int p2 = -1;
            float bestLine = 0;

            Vector3 dir = (pts[p1] - pts[p0]).normalized;

            for(int i = 0; i < pts.Count; i++)
            {
                if(i == p0 || i == p1) continue;

                Vector3 v = pts[i] - pts[p0];
                float d = (v - Vector3.Dot(v, dir) * dir).magnitude;
                
                if(d > bestLine)
                { 
                    bestLine = d;
                    p2 = i;
                }
            }
            if(p2 == -1) return null;

            int p3 = -1;
            float bestPlane = 0;
            Vector3 n = Vector3.Cross(pts[p1] - pts[p0], pts[p2] - pts[p0]).normalized;

            for(int i = 0; i < pts.Count; i++)
            {
                if(i == p0 || i == p1 || i == p2) continue;
                
                float d = Mathf.Abs(Vector3.Dot(pts[i] - pts[p0], n));
                if(d > bestPlane)
                { 
                    bestPlane = d;
                    p3 = i;
                }
            }
            if(p3 == -1 || bestPlane < Epsilon) return null;

            return new int[] {p0, p1, p2, p3};
        }

        public static List<Face> BuildTetrahedron(int p0, int p1, int p2, int p3, List<Vector3> pts)
        {
            var faces = new List<Face>();

            var combos = new (int a, int b, int c, int opposite)[]
            {
                (p0, p1, p2, p3),
                (p0, p1, p3, p2),
                (p0, p2, p3, p1),
                (p1, p2, p3, p0),
            };

            foreach(var (a, b, c, opposite) in combos)
            {
                var f = new Face(a, b, c, pts);

                if(f.CanSee(pts[opposite], pts))
                {
                    f = new Face(a, c, b, pts);
                }
                faces.Add(f);
            }

            return faces;
        }

        public static List<Face> FindVisibleFaces(List<Face> faces, Vector3 point, List<Vector3> pts)
        {
            var visible = new List<Face>();

            foreach(var f in faces)
            {
                if(f.CanSee(point, pts))
                {
                    visible.Add(f);
                }
            }

            return visible;
        }

        public static List<Edge> FindHorizonEdges(List<Face> faces, List<Face> visibleFaces)
        {
            var visibleSet = new HashSet<Face>(visibleFaces);
            var faceByDirectedEdge = new Dictionary<(int, int), Face>();
            
            foreach(var f in faces)
            {
                faceByDirectedEdge[(f.a, f.b)] = f;
                faceByDirectedEdge[(f.b, f.c)] = f;
                faceByDirectedEdge[(f.c, f.a)] = f;
            }

            var horizon = new List<Edge>();

            foreach(var vf in visibleFaces)
            {
                Edge[] edges = { 
                    new Edge(vf.a, vf.b),
                    new Edge(vf.b, vf.c),
                    new Edge(vf.c, vf.a)};

                foreach(var e in edges)
                {
                    bool neighborVisible = faceByDirectedEdge.TryGetValue((e.b, e.a), out Face neighbor) && visibleSet.Contains(neighbor);
                    
                    if(!neighborVisible)
                    {
                        horizon.Add(e);
                    }
                }
            }

            return horizon;
        }


        public static List<Face> CreateFacesFromHorizon(List<Edge> horizon, int pointIdx, List<Face> existingFaces, List<Vector3> pts)
        {
            Vector3 centroid = ComputeCentroid(existingFaces, pts);
            var newFaces = new List<Face>();

            foreach(var e in horizon)
            {
                var face = new Face(e.a, e.b, pointIdx, pts);
                if(face.CanSee(centroid, pts))
                {
                    face = new Face(e.b, e.a, pointIdx, pts);
                }

                newFaces.Add(face);
            }

            return newFaces;
        }

        public static Vector3 ComputeCentroid(List<Face> faces, List<Vector3> pts)
        {
            var indices = new HashSet<int>();

            foreach(var f in faces) 
            { 
                indices.Add(f.a);
                indices.Add(f.b);
                indices.Add(f.c);
            }

            Vector3 sum = Vector3.zero;

            foreach(int i in indices)
            {
                sum += pts[i];
            }
            
            if(indices.Count > 0)
            {
                return sum / indices.Count;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
}