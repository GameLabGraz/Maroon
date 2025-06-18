using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class SimulationBoxRenderer : MonoBehaviour
    {
        // Note(MartinR): It seems like it's not possible to draw multiple lines with a single line-renderer,
        //      so we have a list of them here...
        private LineRenderer[] lineRenderers = new LineRenderer[12];
        [SerializeField] private Material lineMaterial;
        [SerializeField] private float lineWidth;

        private void Initialize()
        {
            // Search for line-renderers in children
            List<GameObject> children = new List<GameObject>();
            int i = 0;
            foreach (Transform child in transform)
            {
                var lineRendererChild = child.GetComponent<LineRenderer>();
                if (lineRendererChild == null) continue;
                lineRenderers[i] = lineRendererChild;
                i += 1;
                if (i == lineRenderers.Length) break;
            }

            // Add line renderers as children if we don't have enough
            for (; i < lineRenderers.Length; i++)
            {
                var lineObject = new GameObject("LineRenderer#" + i);
                lineObject.transform.SetParent(transform);
                lineRenderers[i] = lineObject.AddComponent<LineRenderer>();
            }

            // Update line-renderer parameters
            foreach (var lineRenderer in lineRenderers)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.material = lineMaterial;
                lineRenderer.generateLightingData = false;
                lineRenderer.endWidth = lineWidth;
                lineRenderer.startWidth = lineWidth;
                lineRenderer.numCapVertices = 8;
            }
        }

        // Start is called before the first frame update
        private void Awake()
        {
            Initialize();
            UpdateLines(SimulationBox.Instance.Bounds);
            SimulationBox.Instance.OnBoundsChanged.AddListener(UpdateLines);
        }

        private void OnValidate()
        {
            Initialize();
            UpdateLines(new Bounds(transform.position, Vector3.one * 2));
        }

        private static Vector3 elementwiseMultiply(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        private void UpdateLines(Bounds bounds)
        {
            var box = SimulationBox.Instance.Bounds;

            int i = 0;
            // Lines parallel to x-axis
            lineRenderers[i  ].SetPosition(0, box.min + elementwiseMultiply(box.size, new Vector3(0, 0, 0)));
            lineRenderers[i++].SetPosition(1, box.min + elementwiseMultiply(box.size, new Vector3(1, 0, 0)));
            lineRenderers[i  ].SetPosition(0, box.min + elementwiseMultiply(box.size, new Vector3(0, 0, 1)));
            lineRenderers[i++].SetPosition(1, box.min + elementwiseMultiply(box.size, new Vector3(1, 0, 1)));
            lineRenderers[i  ].SetPosition(0, box.min + elementwiseMultiply(box.size, new Vector3(0, 1, 0)));
            lineRenderers[i++].SetPosition(1, box.min + elementwiseMultiply(box.size, new Vector3(1, 1, 0)));
            lineRenderers[i  ].SetPosition(0, box.min + elementwiseMultiply(box.size, new Vector3(0, 1, 1)));
            lineRenderers[i++].SetPosition(1, box.min + elementwiseMultiply(box.size, new Vector3(1, 1, 1)));

            // Lines parallel to y-axis
            lineRenderers[i  ].SetPosition(0, box.min + elementwiseMultiply(box.size, new Vector3(0, 0, 0)));
            lineRenderers[i++].SetPosition(1, box.min + elementwiseMultiply(box.size, new Vector3(0, 1, 0)));
            lineRenderers[i  ].SetPosition(0, box.min + elementwiseMultiply(box.size, new Vector3(0, 0, 1)));
            lineRenderers[i++].SetPosition(1, box.min + elementwiseMultiply(box.size, new Vector3(0, 1, 1)));
            lineRenderers[i  ].SetPosition(0, box.min + elementwiseMultiply(box.size, new Vector3(1, 0, 0)));
            lineRenderers[i++].SetPosition(1, box.min + elementwiseMultiply(box.size, new Vector3(1, 1, 0)));
            lineRenderers[i  ].SetPosition(0, box.min + elementwiseMultiply(box.size, new Vector3(1, 0, 1)));
            lineRenderers[i++].SetPosition(1, box.min + elementwiseMultiply(box.size, new Vector3(1, 1, 1)));

            // Lines parallel to z-axis
            lineRenderers[i  ].SetPosition(0, box.min + elementwiseMultiply(box.size, new Vector3(0, 0, 0)));
            lineRenderers[i++].SetPosition(1, box.min + elementwiseMultiply(box.size, new Vector3(0, 0, 1)));
            lineRenderers[i  ].SetPosition(0, box.min + elementwiseMultiply(box.size, new Vector3(0, 1, 0)));
            lineRenderers[i++].SetPosition(1, box.min + elementwiseMultiply(box.size, new Vector3(0, 1, 1)));
            lineRenderers[i  ].SetPosition(0, box.min + elementwiseMultiply(box.size, new Vector3(1, 0, 0)));
            lineRenderers[i++].SetPosition(1, box.min + elementwiseMultiply(box.size, new Vector3(1, 0, 1)));
            lineRenderers[i  ].SetPosition(0, box.min + elementwiseMultiply(box.size, new Vector3(1, 1, 0)));
            lineRenderers[i++].SetPosition(1, box.min + elementwiseMultiply(box.size, new Vector3(1, 1, 1)));
        }
    }
}
