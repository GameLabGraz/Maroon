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

        // Start is called before the first frame update
        void Awake()
        {
            // Initialize line renderers
            for (int i = 0; i < 12; i++)
            {
                var lineObject = new GameObject("");
                lineObject.transform.SetParent(transform);
                
                lineRenderers[i] = lineObject.AddComponent<LineRenderer>();
                lineRenderers[i].positionCount = 2;
                lineRenderers[i].material = lineMaterial;
                lineRenderers[i].generateLightingData = false;
                lineRenderers[i].endWidth = lineWidth;
                lineRenderers[i].startWidth = lineWidth;
                lineRenderers[i].numCapVertices = 8;
            }

            var simBox = SimulationBox.Instance;
            UpdateLines(simBox.Bounds);
            simBox.OnBoundsChanged.AddListener(UpdateLines);
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
