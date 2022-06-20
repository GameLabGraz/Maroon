using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics.CathodeRayTube
{
    public class ElectronLineController : MonoBehaviour, IResetObject
    {
        [SerializeField] private CRTController crtController;
        [SerializeField] private GameObject screen;
        [SerializeField] private GameObject anode;
        [SerializeField] private GameObject spiral;
        [SerializeField] private Material metal;
        [SerializeField] private Material hotMetal;
        [SerializeField] private GameObject electronCloud;
        [SerializeField] private Material electronLineMaterial;
        private LineRenderer _lineRenderer;
        private float _timeStep;

        private void Start()
        {
            _timeStep = crtController.GetTimeStep();
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.material = electronLineMaterial;
            _lineRenderer.widthMultiplier = 0.005f;
            _lineRenderer.positionCount = crtController.lineResolution;
            _lineRenderer.enabled = false;
            spiral.GetComponent<MeshRenderer>().material = metal;
            electronCloud.SetActive(false);
        }

        public void UpdateElectronLine()
        {
            if (!SimulationController.Instance.SimulationRunning)
                return;
            StartCoroutine(UpdateElectronLineCoRoutine());
        }

        private IEnumerator UpdateElectronLineCoRoutine()
        {
            _lineRenderer.positionCount = crtController.lineResolution;
            _lineRenderer.enabled = true;
            spiral.GetComponent<MeshRenderer>().material = hotMetal;
            electronCloud.SetActive(true);
            var currentVel = new Vector3();
            List<Vector3> points = new List<Vector3>();
            List<Vector3> lineRendererPoints = new List<Vector3>();
            List<Vector3> velocities = new List<Vector3>();
            List<Vector3> forces = new List<Vector3>();

            points.Add(crtController.GetCRTStart());
            lineRendererPoints.Add(anode.transform.position);
            velocities.Add(Vector3.zero);
            forces.Add(crtController.ApplyForce(points[0]));
            var oldPoint = points[0];
            var newPoint = points[0];

            for (int i = 1; i < crtController.lineResolution; i++)
            {
                oldPoint = points[i - 1];

                currentVel += crtController.RK4(oldPoint);

                newPoint = oldPoint;
                newPoint += currentVel * _timeStep;

                points.Add(UnityEngine.Physics.Linecast(newPoint, oldPoint) ? oldPoint : newPoint);

                if (points[i].x > anode.transform.position.x)
                    lineRendererPoints.Add(points[i]);
                else
                    lineRendererPoints.Add(anode.transform.position);

                velocities.Add(currentVel);
                forces.Add(crtController.ApplyForce(points[i]));
            }

            UnityEngine.Physics.Linecast(newPoint, oldPoint, out var hitInfo);
            if (hitInfo.collider != null && hitInfo.collider.name.Equals("Screen"))
                screen.GetComponent<ScreenSetup>().ActivatePixel(hitInfo.point);
            
            _lineRenderer.SetPositions(lineRendererPoints.ToArray());
            crtController.UpdateData(points, velocities, forces);
            yield return null;
        }

        public void ResetObject()
        {
            _lineRenderer.positionCount = 0;
            
            _lineRenderer.enabled = false;
            spiral.GetComponent<MeshRenderer>().material = metal;
            electronCloud.SetActive(false);

            List<Vector3> points = new List<Vector3>();
            List<Vector3> velocities = new List<Vector3>();
            List<Vector3> forces = new List<Vector3>();

            for (int i = 0; i < crtController.lineResolution; i++)
            {
                points.Add(Vector3.zero);
                velocities.Add(Vector3.zero);
                forces.Add(Vector3.zero);
            }

            crtController.UpdateData(points, velocities, forces);
        }
    }
}