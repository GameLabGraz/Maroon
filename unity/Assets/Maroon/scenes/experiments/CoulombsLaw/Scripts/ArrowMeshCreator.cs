using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class ArrowMeshCreator
    {
        private static int PushVertexRing(Vector2 point2D, List<Vector3> vertices, int radialResolution)
        {
            int ringStartIndex = vertices.Count;

            // Point2D is in xz Plane
            for (int i = 0; i < radialResolution; i++)
            {
                float angle = 2 * Mathf.PI * i / (float)radialResolution;
                Vector3 point = new Vector3(0, 0, point2D.y);
                point.x = Mathf.Sin(angle) * point2D.x;
                point.y = Mathf.Cos(angle) * point2D.x;
                vertices.Add(point);
            }

            return ringStartIndex;
        }

        private static void PushRingConnectionTriangles(
            List<int> indices, int firstRingStartIndex, int secondRingStartIndex, int radialResolution, bool addSingleTriangleOnly)
        {
            for (int i = 0; i < radialResolution; i++)
            {
                // Indices of Quad in CCW-order
                int i0 = firstRingStartIndex + i;
                int i1 = secondRingStartIndex + i;
                int i2 = secondRingStartIndex + (i + 1) % radialResolution;
                int i3 = firstRingStartIndex + (i + 1) % radialResolution;

                if (addSingleTriangleOnly)
                {
                    indices.Add(i0);
                    indices.Add(i1);
                    indices.Add(i3);
                }
                else
                {
                    indices.Add(i0);
                    indices.Add(i1);
                    indices.Add(i2);
                    indices.Add(i0);
                    indices.Add(i2);
                    indices.Add(i3);
                }
            }
        }

        private static void PushRingConnectionPoint(List<int> indices, int ringStartIndex, int radialResolution, int pointIndex, bool swap = false)
        {
            for (int i = 0; i < radialResolution; i++)
            {
                // Indices of the quad
                int i0 = ringStartIndex + i;
                int i1 = ringStartIndex + (i + 1) % radialResolution;

                if (swap)
                {
                    int s = i0;
                    i0 = i1;
                    i1 = s;
                }

                indices.Add(i0);
                indices.Add(pointIndex);
                indices.Add(i1);
            }
        }

        private static void PushRingFace(List<int> indices, int ringStartIndex, int radialResolution, bool invert = false)
        {
            for (int i = 0; i < radialResolution - 2; i++)
            {
                // Indices of the quad
                int i0 = ringStartIndex;
                int i1 = ringStartIndex + i + 1;
                int i2 = ringStartIndex + i + 2;

                if (invert)
                {
                    int swap = i1;
                    i1 = i2;
                    i2 = swap;
                }

                indices.Add(i0);
                indices.Add(i1);
                indices.Add(i2);
            }
        }

        private static Mesh GenerateMeshFromVertices(List<Vector3> vertices, List<int> indices)
        {
            // Generate normals (Average connected triangle normals...)
            Vector3[] normals = new Vector3[vertices.Count];
            Vector3[] vertexArray = vertices.ToArray();
            int[] indexArray = indices.ToArray();
            {
                // Initialize normals (Not sure if necessary)
                for (int i = 0; i < normals.Length; i++)
                {
                    normals[i] = new Vector3(0, 0, 0);
                }

                // Add normals to vertices for each connected triangle
                for (int i = 0; i + 2 < indexArray.Length; i += 3)
                {
                    Vector3 a = vertices[indexArray[i]];
                    Vector3 b = vertices[indexArray[i + 1]];
                    Vector3 c = vertices[indexArray[i + 2]];
                    Vector3 normal = -Vector3.Cross(a - c, a - b).normalized;

                    // Add up normal vectors
                    normals[indexArray[i]] += normal;
                    normals[indexArray[i + 1]] += normal;
                    normals[indexArray[i + 2]] += normal;
                }

                // Normalize normals (Averages normals of adjacent triangles for each vertex)
                for (int i = 0; i < normals.Length; i++)
                {
                    normals[i] = normals[i].normalized;
                }
            }

            Mesh arrowMesh = new Mesh();
            arrowMesh.SetVertices(vertexArray);
            arrowMesh.SetIndices(indexArray, MeshTopology.Triangles, 0);
            arrowMesh.SetNormals(normals);
            return arrowMesh;
        }

        // Cylinder points in z-direction, and starts at (0, 0, 0)
        public static Mesh CreateCylinderMesh(float radiusCylinder, float length, int radialResolution)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            // Stem backface
            int stemStartRingIndex = PushVertexRing(new Vector2(radiusCylinder, 0), vertices, radialResolution);
            PushRingFace(indices, stemStartRingIndex, radialResolution);
            stemStartRingIndex = PushVertexRing(new Vector2(radiusCylinder, 0), vertices, radialResolution); // New ring for normals
            int stemEndRingIndex = PushVertexRing(new Vector2(radiusCylinder, length), vertices, radialResolution);
            PushRingConnectionTriangles(indices, stemStartRingIndex, stemEndRingIndex, radialResolution, false);
            stemEndRingIndex = PushVertexRing(new Vector2(radiusCylinder, length), vertices, radialResolution); // New ring for normals
            PushRingFace(indices, stemEndRingIndex, radialResolution, true);

            return GenerateMeshFromVertices(vertices, indices);
        }

        public static Mesh CreateConeMesh(float coneRadius, float length, int radialResolution, int coneSubdivisions)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            // Stem backface
            int ringStartIndex = PushVertexRing(new Vector2(coneRadius, 0), vertices, radialResolution);
            PushRingFace(indices, ringStartIndex, radialResolution);
            ringStartIndex = PushVertexRing(new Vector2(coneRadius, 0), vertices, radialResolution); // New ring for normals

            // Cone to tip connection
            Vector2 tipPos = new Vector2(0, length);
            Vector2 lastPos = new Vector2(coneRadius, 0);
            int lastRingIndex = ringStartIndex;
            for (int i = 0; i < coneSubdivisions; i++)
            {
                Vector2 nextPos = (lastPos + tipPos) / 2.0f;
                lastPos = nextPos;
                int ringIndex = PushVertexRing(nextPos, vertices, radialResolution);
                PushRingConnectionTriangles(indices, lastRingIndex, ringIndex, radialResolution, false);
                lastRingIndex = ringIndex;
            }

            int tipVertexIndex = vertices.Count;
            vertices.Add(new Vector3(0, 0, length));
            PushRingConnectionPoint(indices, lastRingIndex, radialResolution, tipVertexIndex);

            return GenerateMeshFromVertices(vertices, indices);
        }

        public static Mesh CreateArrowMesh(float radiusCylinder, float lengthCylinder, float radiusCone, int radialResolution, int coneSubdivisions)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            // Generate vertices and indices for Arrow-Mesh
            {
                // Stem backface
                int stemStartRingIndex = PushVertexRing(new Vector2(radiusCylinder, -1), vertices, radialResolution);
                PushRingFace(indices, stemStartRingIndex, radialResolution);

                // Stem Cylinder-Face triangles
                stemStartRingIndex = PushVertexRing(new Vector2(radiusCylinder, -1), vertices, radialResolution);
                int stemEndRingIndex = PushVertexRing(new Vector2(radiusCylinder, -1 + lengthCylinder), vertices, radialResolution);
                PushRingConnectionTriangles(indices, stemStartRingIndex, stemEndRingIndex, radialResolution, false);

                // Cylinder to Cone connection
                Vector2 outerCone = new Vector2(radiusCone, -1 + lengthCylinder);
                stemEndRingIndex = PushVertexRing(new Vector2(radiusCylinder, -1 + lengthCylinder), vertices, radialResolution);
                int headRingIndex = PushVertexRing(outerCone, vertices, radialResolution);
                PushRingConnectionTriangles(indices, stemEndRingIndex, headRingIndex, radialResolution, false);

                // Cone to tip connection
                Vector2 tipPos = new Vector2(0, 1);
                int lastRingIndex = PushVertexRing(outerCone, vertices, radialResolution);
                Vector2 lastPos = outerCone;
                for (int i = 0; i < coneSubdivisions; i++)
                {
                    Vector2 nextPos = (lastPos + tipPos) / 2.0f;
                    lastPos = nextPos;
                    int ringIndex = PushVertexRing(nextPos, vertices, radialResolution);
                    PushRingConnectionTriangles(indices, lastRingIndex, ringIndex, radialResolution, false);
                    lastRingIndex = ringIndex;
                }

                int tipRingIndex = vertices.Count;
                vertices.Add(new Vector3(0, 0, 1));
                PushRingConnectionPoint(indices, lastRingIndex, radialResolution, tipRingIndex);
            }

            return GenerateMeshFromVertices(vertices, indices);
        }
    }
}
