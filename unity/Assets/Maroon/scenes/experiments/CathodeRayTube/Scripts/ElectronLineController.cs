using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics.CathodeRayTube
{
    public class ElectronLineController : PausableObject, IResetObject
    {
        private CRTController _crtController;
        [SerializeField] private GameObject screen;
        [SerializeField] private GameObject anode;
        [SerializeField] private GameObject spiral;
        [SerializeField] private Material metal;
        [SerializeField] private Material hotMetal;
        [SerializeField] private GameObject electronCloud;
        [SerializeField] private Material electronLineMaterial;
        private float _timeStep;

        private new void Start()
        {
            _crtController = transform.parent.gameObject.GetComponent<CRTController>();
            _timeStep = _crtController.GetTimeStep();
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = electronLineMaterial;
            lineRenderer.widthMultiplier = 0.005f;
            lineRenderer.positionCount = _crtController.lineResolution;
            lineRenderer.enabled = false;
            spiral.GetComponent<MeshRenderer>().material = metal;
            electronCloud.SetActive(false);
        }

        protected override void HandleUpdate()
        {
        }

        protected override void HandleFixedUpdate()
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = _crtController.lineResolution;
            lineRenderer.enabled = true;
            spiral.GetComponent<MeshRenderer>().material = hotMetal;
            electronCloud.SetActive(true);
            Vector3 currentVel = new Vector3();
            List<Vector3> points = new List<Vector3>();
            List<Vector3> lineRendererPoints = new List<Vector3>();
            List<Vector3> velocities = new List<Vector3>();
            List<Vector3> forces = new List<Vector3>();

            points.Add(_crtController.GetCRTStart());
            lineRendererPoints.Add(anode.transform.position);
            velocities.Add(Vector3.zero);
            forces.Add(_crtController.ApplyForce(points[0]));
            Vector3 oldPoint = points[0];
            Vector3 newPoint = points[0];

            for (int i = 1; i < _crtController.lineResolution; i++)
            {
                oldPoint = points[i - 1];

                currentVel += _crtController.RK4(oldPoint);

                newPoint = oldPoint;
                newPoint += currentVel * _timeStep;

                points.Add(UnityEngine.Physics.Linecast(newPoint, oldPoint) ? oldPoint : newPoint);

                if (points[i].x > anode.transform.position.x)
                    lineRendererPoints.Add(points[i]);
                else
                    lineRendererPoints.Add(anode.transform.position);

                velocities.Add(currentVel);
                forces.Add(_crtController.ApplyForce(points[i]));
            }

            UnityEngine.Physics.Linecast(newPoint, oldPoint, out var hitInfo);
            if (hitInfo.collider != null && hitInfo.collider.name.Equals("Screen"))
                screen.GetComponent<ScreenSetup>().ActivatePixel(hitInfo.point);
            
            lineRenderer.SetPositions(lineRendererPoints.ToArray());
            _crtController.UpdateData(points, velocities, forces);
        }

        public void ResetObject()
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            
            lineRenderer.enabled = false;
            spiral.GetComponent<MeshRenderer>().material = metal;
            electronCloud.SetActive(false);

            List<Vector3> points = new List<Vector3>();
            List<Vector3> velocities = new List<Vector3>();
            List<Vector3> forces = new List<Vector3>();

            for (int i = 0; i < _crtController.lineResolution; i++)
            {
                points.Add(Vector3.zero);
                velocities.Add(Vector3.zero);
                forces.Add(Vector3.zero);
            }

            _crtController.UpdateData(points, velocities, forces);
        }
    }
}