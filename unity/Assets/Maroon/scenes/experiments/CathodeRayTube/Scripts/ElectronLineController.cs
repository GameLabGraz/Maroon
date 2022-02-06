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
            List<Vector3> points = new List<Vector3>();
            Vector3 currentVel = new Vector3();

            points.Add(_crtController.GetCRTStart());
            Vector3 oldPoint = points[0];
            Vector3 newPoint = points[0];
            
            for (int i = 1; i < _crtController.lineResolution; i++)
            {
                oldPoint = points[i - 1];
                
                currentVel += _crtController.RK4(oldPoint);

                newPoint = oldPoint;
                newPoint.x += currentVel.x * _timeStep;
                newPoint.y += currentVel.y * _timeStep;
                newPoint.z += currentVel.z * _timeStep;

                if (UnityEngine.Physics.Linecast(oldPoint, newPoint))
                    points.Add(oldPoint);
                else
                    points.Add(newPoint);
            }

            UnityEngine.Physics.Linecast(oldPoint, newPoint, out var hitInfo);
            if (hitInfo.collider != null && hitInfo.collider.name.Equals("Screen"))
                screen.GetComponent<ScreenSetup>().ActivatePixel(hitInfo.point);
            
            lineRenderer.SetPositions(points.ToArray());
        }

        public void ResetObject()
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
        }
    }
}
