using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class Cable : MonoBehaviour {
        private Port device1;
        private Port device2;
        private LineRenderer lineRenderer;

        const int NumberOfCurveSteps = 25;

        private void Start() {
        }

        private void Awake() {
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void Initalize(Port port1, Port port2) {
            device1 = port1;
            device2 = port2;
            UpdateCurve();
        }

        public void UpdateCurve() {
            lineRenderer.positionCount = NumberOfCurveSteps;
            lineRenderer.SetPositions(GetBezierCurve(NumberOfCurveSteps));
        }

        private Vector3[] GetBezierCurve(int steps) {
            var curve = new Vector3[steps];
            var stepSize = 1f / (steps - 1);
            for (int i = 0; i < steps; i++) {
                curve[i] = Bezier(i * stepSize, device1.Position, device1.BezierPoint, device2.BezierPoint, device2.Position);
            }
            return curve;
        }

        private Vector3 Bezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            var add1 = Mathf.Pow(1 - t, 3) * p0;
            var add2 = 3 * Mathf.Pow(1 - t, 2) * t * p1;
            var add3 = 3 * (1 - t) * Mathf.Pow(t, 2) * p2;
            var add4 = Mathf.Pow(t, 3) * p3;
            return add1 + add2 + add3 + add4;
        }
    }
}
