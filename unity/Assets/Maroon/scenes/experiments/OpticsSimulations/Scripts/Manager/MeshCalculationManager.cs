using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager
{
    public class MeshCalculationManager : MonoBehaviour
    {
        public static MeshCalculationManager Instance;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("SHOULD NOT OCCUR - Destroyed MeshCalculationManager");
                Destroy(gameObject);
            }
        }

        public List<T> ConcatenateLists<T>(params List<T>[] lists)
        {
            return lists.Aggregate(new List<T>(), (result, list) => result.Concat(list).ToList());
        }

        public void FlipTriangleVertexOrder(ref List<int> triangles)
        {
            for (int i = 0; i < triangles.Count; i += 3)
            {
                int temp = triangles[i];
                triangles[i] = triangles[i + 2];
                triangles[i + 2] = temp;
            }
        }

        public void FlipNormals(ref List<Vector3> normals)
        {
            for (int i = 0; i < normals.Count; i++)
                normals[i] *= -1;
        }
        
        public List<int> CalculateRingFaces(int nrOfVertices, int nrOfSegments)
        {
            if (nrOfVertices != nrOfSegments)
            {
                Debug.LogError("nrOfVertices has to be the same as nrOfSegments! (" + nrOfVertices + ", " + nrOfSegments + ")\n");
                return null;
            }
            
            List<int> triangles = new List<int>();
            // 2 Triangles per face (= quad)
            for (int i = 0; i < nrOfSegments - 1; i++)
            {
                int v0 = i;                         // left disk 0
                int v1 = i + 1;                     // left disk 1
                int v2 = i + nrOfSegments;          // right disk 0
                int v3 = i + 1 + nrOfSegments;      // right disk 1
                
                triangles.Add(v0);
                triangles.Add(v2);
                triangles.Add(v1);
                triangles.Add(v1);
                triangles.Add(v2);
                triangles.Add(v3);
            }
            
            // Last face (2 triangles) of the cylinder -> connection of first and last vertex
            triangles.Add(nrOfSegments - 1);
            triangles.Add(2 * nrOfSegments - 1);
            triangles.Add(0);
            triangles.Add(0);
            triangles.Add(2 * nrOfSegments - 1);
            triangles.Add(nrOfSegments);
            
            return triangles;
        }
        
        public List<Vector3> CalculateMeshNormals(List<Vector3> vertices, List<int> triangles)
        {
            int numberOfTriangles = triangles.Count / 3;
            List<Vector3> normals = new List<Vector3>();

            for (int i = 0; i < numberOfTriangles; i++)
            {
                int v0 = triangles[i * 3];
                int v1 = triangles[i * 3 + 1];
                int v2 = triangles[i * 3 + 2];

                Vector3 edge0 = vertices[v1] - vertices[v0];
                Vector3 edge1 = vertices[v2] - vertices[v0];
                Vector3 triangleNormal = Vector3.Cross(edge0, edge1).normalized;

                normals.Add(triangleNormal);
            }
            return normals;
        }

        public List<Vector3> CalculateDiskVertices(Vector3 localCenter, float radius, float Rc, int nrOfSegments)
        {
            List<Vector3> vertexPositions = new List<Vector3>();

            for (int i = 0; i < nrOfSegments; i++)
            {
                float angle = i * 360f / nrOfSegments;
                float radians = angle * Mathf.Deg2Rad;

                vertexPositions.Add(new Vector3(
                    localCenter.x, 
                    localCenter.y + radius * Mathf.Sin(radians), 
                    localCenter.z + radius * Mathf.Cos(radians)
                ));
            }
            return vertexPositions;
        }

        public (List<Vector3>, List<Vector3>) CalculateHalfSphereVerticesNormals(float radius, float Rc, Vector3 n,
            int ringSegments, int nrOfLatitudeSegments, float baseYRotation, float offsetAlongOpticalAxis = 0)
        {
            Vector3[] vertices = new Vector3[(nrOfLatitudeSegments + 1) * (ringSegments + 1)];
            Vector3[] normals = new Vector3[vertices.Length];
            radius = Mathf.Abs(radius);

            float thetaMax = Rc < radius ? Mathf.Asin(Rc / radius) : Mathf.PI / 2.0f;
            for (int lat = 0; lat <= nrOfLatitudeSegments; lat++)
            {
                float normalizedLatitude = lat / (float)nrOfLatitudeSegments;
                float theta = normalizedLatitude * thetaMax;

                for (int lon = 0; lon <= ringSegments; lon++)
                {
                    float normalizedLongitude = lon / (float)ringSegments;
                    float phi = normalizedLongitude * Mathf.PI * 2.0f;
                    Quaternion quaternionRot = Quaternion.Euler(new Vector3(0, baseYRotation, 0));

                    float x = Mathf.Sin(theta) * Mathf.Cos(phi);
                    float y = Mathf.Sin(theta) * Mathf.Sin(phi);
                    float z = Mathf.Cos(theta);
                    Vector3 rotatedCoordinates = quaternionRot * new Vector3(x, y, z);
                    
                    int index = lat * (ringSegments + 1) + lon;
                    vertices[index] = rotatedCoordinates * radius + n * offsetAlongOpticalAxis;
                    normals[index] = rotatedCoordinates.normalized;
                }
            }
            
            return (vertices.ToList(), normals.ToList());
        }

        public List<int> CalculateHalfSphereFaces(int ringSegments, int nrOfLatitudeSegments)
        {
            int[] triangles = new int[nrOfLatitudeSegments * ringSegments * 6];

            int triangleIndex = 0;
            for (int lat = 0; lat < nrOfLatitudeSegments; lat++)
            {
                for (int lon = 0; lon < ringSegments; lon++)
                {
                    int current = lat * (ringSegments + 1) + lon;
                    int next = current + ringSegments + 1;

                    triangles[triangleIndex++] = current;
                    triangles[triangleIndex++] = next + 1;
                    triangles[triangleIndex++] = current + 1;

                    triangles[triangleIndex++] = current;
                    triangles[triangleIndex++] = next;
                    triangles[triangleIndex++] = next + 1;
                }
            }
            return triangles.ToList();
        }

    }
}
