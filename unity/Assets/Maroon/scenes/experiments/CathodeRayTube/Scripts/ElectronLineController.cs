using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Physics.CathodeRayTube
{
    public class ElectronLineController : PausableObject, IResetObject
    {
        private CRTController _crtController;
        [SerializeField] private GameObject screen;
        
        [SerializeField] private Material material;
        private float _timeStep;

        private new void Start()
        {
            _crtController = transform.parent.gameObject.GetComponent<CRTController>();
            _timeStep = _crtController.getTimeStep();
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = material;
            lineRenderer.widthMultiplier = 0.005f;
            lineRenderer.positionCount = _crtController.lineResolution;
        }
        
        protected override void HandleUpdate()
        {
            
        }

        protected override void HandleFixedUpdate()
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = _crtController.lineResolution;
            Vector3 currentVel = new Vector3();
            List<Vector3> points = new List<Vector3>();
            List<Vector3> velocities = new List<Vector3>();
            List<Vector3> forces = new List<Vector3>();

            points.Add(_crtController.GetCRTStart());
            velocities.Add(new Vector3(0, 0, 0));
            forces.Add(_crtController.ApplyForce(points[0]));
            Vector3 oldPoint = points[0];
            Vector3 newPoint = points[0];
            
            for (int i = 1; i < _crtController.lineResolution; i++)
            {
                oldPoint = points[i - 1];
                
                currentVel += _crtController.RK4(oldPoint);

                newPoint = oldPoint;
                newPoint += currentVel * _timeStep;

                if (UnityEngine.Physics.Linecast(oldPoint, newPoint))
                    points.Add(oldPoint);
                else
                    points.Add(newPoint);
                
                velocities.Add(currentVel);
                forces.Add(_crtController.ApplyForce(points[i]));
            }

            UnityEngine.Physics.Linecast(oldPoint, newPoint, out var hitInfo);
            if (hitInfo.collider != null && hitInfo.collider.name.Equals("Screen"))
                screen.GetComponent<ScreenSetup>().ActivatePixel(hitInfo.point);
            
            lineRenderer.SetPositions(points.ToArray());
            _crtController.updateData(points, velocities, forces);
        }

        public void ResetObject()
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            
            List<Vector3> points = new List<Vector3>();
            List<Vector3> velocities = new List<Vector3>();
            List<Vector3> forces = new List<Vector3>();
            
            for (int i = 0; i < _crtController.lineResolution; i++)
            {
                points.Add(new Vector3(0, 0, 0));
                velocities.Add(new Vector3(0, 0, 0));
                forces.Add(new Vector3(0, 0, 0));
            }
            
            _crtController.updateData(points, velocities, forces);
        }
    }
}
