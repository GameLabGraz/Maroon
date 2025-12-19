using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class FieldLineGenerator : MonoBehaviour
    {
        [SerializeField] private Material lineMaterial;
        [SerializeField] private float lineWidth = 0.03f;
        [SerializeField] private float STEP_SIZE = 0.05f;
        [SerializeField] private float MIN_MAGNITUDE = 0.001f; // Magnitude in N/C
        [SerializeField] private int   MAX_STEPS_PER_LINE = 100;
        [SerializeField] private int   MAX_POSITION_COUNT = 10000;

        [SerializeField] private int ROD_RADIAL_DENSITY = 8;
        [SerializeField] private float ROD_LINEAR_STEPSIZE = 0.3f;
        [SerializeField] private float PLANE_STEPSIZE = 0.3f;

        private Unity.Collections.NativeArray<Vector3> vertexBuffer;
        private int nextVertexIndex = 0;
        private List<int> lineStartIndices = new List<int>();
        private Vector3[] prevPositionsRingBuffer = new Vector3[5];

        private List<LineRenderer> lineRenderers = new List<LineRenderer>();

        private void Awake()
        {
            vertexBuffer = new Unity.Collections.NativeArray<Vector3>(MAX_POSITION_COUNT, Unity.Collections.Allocator.Persistent);
        }

        private void OnDestroy()
        {
            vertexBuffer.Dispose();
        }

        private void AddVertex(Vector3 position)
        {
            if (nextVertexIndex >= MAX_POSITION_COUNT) return;
            vertexBuffer[nextVertexIndex] = position;
            nextVertexIndex += 1;
        }

        // Generates vertices and lineStartIndices
        private void GenerateLine(Vector3 pos, bool forwards)
        {
            var efield = ElectricField.Instance;
            var box = SimulationBox.Instance.Bounds;
            // box.extents = 2.0f * box.extents;
            if (!box.Contains(pos)) return;

            // Init ringbuffer values
            for (int i = 0; i < prevPositionsRingBuffer.Length; i++)
            {
                prevPositionsRingBuffer[i] = new Vector3(-10000, -1000, 0);
            }
            int RINGBUFFER_SIZE = prevPositionsRingBuffer.Length;
            int ringbufferIndex = 0;
            prevPositionsRingBuffer[(ringbufferIndex + RINGBUFFER_SIZE - 1) % RINGBUFFER_SIZE] = pos; // Initial previous pos

            // Add initial vertex
            lineStartIndices.Add(nextVertexIndex);
            AddVertex(pos);

            // Step through field line, generating new vertices
            float dirSign = forwards ? 1.0f : -1.0f;
            for (int i = 0; i < MAX_STEPS_PER_LINE; i++)
            {
                // Step
                Vector3 direction = efield.GetFieldValue(pos, true);
                float magnitude = direction.magnitude;
                if (magnitude < MIN_MAGNITUDE) break;
                direction = direction.normalized;

                Vector3 prevPos = pos;
                pos = pos + direction * dirSign * STEP_SIZE;
                prevPositionsRingBuffer[ringbufferIndex] = pos;

                // Check for stopping conditions
                // Check if we stepped out of simulation box
                if (!box.Contains(pos))
                {
                    AddVertex(pos);
                    break;
                }

                // Check if are too close to a charged object
                bool shouldBreak = false;
                foreach (var point in efield.chargedPoints)
                {
                    Vector3 toPoint = pos - point.transform.position;
                    bool movingTowards = Vector3.Dot(direction * dirSign, toPoint) >= 0.0f;
                    if (toPoint.sqrMagnitude < ChargedPoint.RADIUS * ChargedPoint.RADIUS && movingTowards)
                    {
                        AddVertex(pos);
                        shouldBreak = true;
                        break;
                    }
                }
                if (shouldBreak) break;
                foreach (var rod in efield.chargedRods)
                {
                    Vector3 rodDir = rod.GetDirection();
                    Vector3 closestPointOnRod = rod.ProjectPointOntoRod(pos);
                    Vector3 toPoint = pos - closestPointOnRod;
                    bool movingTowards = Vector3.Dot(direction * dirSign, toPoint) >= 0.0f;
                    if (toPoint.sqrMagnitude < ChargedRod.RADIUS * ChargedRod.RADIUS && movingTowards)
                    {
                        AddVertex(pos);
                        shouldBreak = true;
                        break;
                    }
                }
                if (shouldBreak) break;
                foreach (var plane in efield.chargedPlanes)
                {
                    // Break if we crossed plane
                    Vector4 planeEquation = plane.GetPlaneEquation();
                    float prevDistance = ChargedPlane.SignedDistanceToPlane(prevPos, planeEquation);
                    float currDistance = ChargedPlane.SignedDistanceToPlane(pos, planeEquation);
                    if (Mathf.Sign(prevDistance) != Mathf.Sign(currDistance)) // Check if we crossed the plane
                    {
                        AddVertex(ChargedPlane.GetLinePlaneIntersectionPoint(prevPos, pos, planeEquation));
                        shouldBreak = true;
                        break;
                    }
                }
                if (shouldBreak) break;

                // Check if we have moved far enough in the last few steps, otherwise terminate
                ringbufferIndex = (ringbufferIndex + 1) % RINGBUFFER_SIZE;
                Vector3 lastRecordedPos = prevPositionsRingBuffer[ringbufferIndex];
                if ((lastRecordedPos - pos).magnitude <= STEP_SIZE * 2.0f)
                {
                    break;
                }

                AddVertex(pos);
            }
        }

        private void LateUpdate()
        {
            // Generate FieldLines on all objects that have the option active
            nextVertexIndex = 0;
            lineStartIndices.Clear();
            Bounds box = SimulationBox.Instance.Bounds;
            var efield = ElectricField.Instance;

            Vector3[] pointOffsets = new Vector3[]
            {
                new Vector3(1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 1),
                new Vector3(-1, 0, 0),
                new Vector3(0, -1, 0),
                new Vector3(0, 0, -1),

                new Vector3( 1,  1,  1).normalized,
                new Vector3(-1, -1, -1).normalized,
                new Vector3(-1,  1,  1).normalized,
                new Vector3( 1, -1, -1).normalized,
                new Vector3(-1, -1,  1).normalized,
                new Vector3( 1,  1, -1).normalized,
                new Vector3(-1,  1, -1).normalized,
                new Vector3( 1, -1,  1).normalized,
            };

            foreach (var chargedPoint in efield.chargedPoints)
            {
                if (!chargedPoint.generateFieldLines) continue;
                foreach (var offset in pointOffsets)
                {
                    GenerateLine(chargedPoint.transform.position + offset * ChargedPoint.RADIUS, true);
                    GenerateLine(chargedPoint.transform.position + offset * ChargedPoint.RADIUS, false);
                }
            }

            foreach (var chargedRod in efield.chargedRods)
            {
                if (!chargedRod.generateFieldLines) continue;

                var pos = chargedRod.transform.position;
                var dir = chargedRod.GetDirection();
                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, dir);
                for (int i = 1; i < 100; i++)
                {
                    var rodPosA = pos + dir * (i - 1) * ROD_LINEAR_STEPSIZE;
                    var rodPosB = pos - dir * i * ROD_LINEAR_STEPSIZE;
                    bool posAInside = box.Contains(rodPosA);
                    bool posBInside = box.Contains(rodPosB);
                    if (!posAInside && !posBInside) break;

                    for (int j = 0; j < ROD_RADIAL_DENSITY; j++)
                    {
                        float angle = 2 * Mathf.PI / ROD_RADIAL_DENSITY * j;
                        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.0f) * ChargedRod.RADIUS;
                        offset = rotation * offset;

                        if (posAInside)
                        {
                            GenerateLine(rodPosA + offset, true);
                            GenerateLine(rodPosA + offset, false);
                        }
                        if (posBInside)
                        {
                            GenerateLine(rodPosB + offset, true);
                            GenerateLine(rodPosB + offset, false);
                        }
                    }
                }
            }

            foreach (var chargedPlane in efield.chargedPlanes)
            {
                if (!chargedPlane.generateFieldLines) continue;

                int linearSteps = 2 * (int) (2.0f / PLANE_STEPSIZE + 0.5f);
                Vector3 normal = chargedPlane.GetNormal();
                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, normal);
                for (int x = 0; x < linearSteps; x++)
                {
                    for (int y = 0; y < linearSteps; y++)
                    {
                        Vector3 offset = Vector3.zero;
                        offset.x = (x - (linearSteps / 2)) * PLANE_STEPSIZE;
                        offset.y = (y - (linearSteps / 2)) * PLANE_STEPSIZE;
                        offset = rotation * offset;
                        Vector3 point = chargedPlane.transform.position + offset;
                        if (!box.Contains(point)) continue;

                        // Start field line on both sides of the plane
                        GenerateLine(point + normal * ChargedPlane.THICKNESS * 0.4f, true);
                        GenerateLine(point + normal * ChargedPlane.THICKNESS * 0.4f, false);
                        GenerateLine(point - normal * ChargedPlane.THICKNESS * 0.4f, true);
                        GenerateLine(point - normal * ChargedPlane.THICKNESS * 0.4f, false);
                    }    
                }
            }


            // RENDER LINES WITH LINE-RENDERERS
            // Reset line renderers
            foreach (var line in lineRenderers)
            {
                line.enabled = false;
            }

            // Set line renderer data
            int lineRendererIndex = 0;
            lineStartIndices.Add(nextVertexIndex); // Add last index to make loop simpler
            for (int i = 0; i < lineStartIndices.Count - 1; i++)
            {
                int vertexStartIndex = lineStartIndices[i];
                int vertexEndIndex = lineStartIndices[i + 1];
                if (vertexEndIndex <= vertexStartIndex) continue;

                // Get line renderer (Either cached one or create new one)
                LineRenderer lineRenderer = null;
                if (lineRendererIndex < lineRenderers.Count)
                {
                    lineRenderer = lineRenderers[lineRendererIndex];
                }
                else
                {
                    var newObject = new GameObject("LineRenderer #" + lineRendererIndex);
                    newObject.transform.SetParent(this.transform);
                    lineRenderer = newObject.AddComponent<LineRenderer>();
                    lineRenderers.Add(lineRenderer);
                    lineRenderer.startWidth = lineWidth;
                    lineRenderer.endWidth = lineWidth;
                    lineRenderer.material = lineMaterial;
                    lineRenderer.generateLightingData = false;
                }
                lineRenderer.enabled = true;
                lineRendererIndex += 1;

                lineRenderer.positionCount = vertexEndIndex - vertexStartIndex;
                lineRenderer.SetPositions(
                    new Unity.Collections.NativeSlice<Vector3>(vertexBuffer, vertexStartIndex, vertexEndIndex - vertexStartIndex));
                lineRenderer.positionCount = vertexEndIndex - vertexStartIndex;
            }
        }
    }
}
