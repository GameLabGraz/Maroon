using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Physics.CathodeRayTube
{
    public class ElectronLineController : PausableObject, IResetObject
    {
        private CRTController _crtController;
        
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
            Vector3 startPoint = _crtController.GetCRTStart();
            points.Add(startPoint);

            float currentVelX = 0;
            float currentVelY = 0;
            float currentVelZ = 0;
            
            for (int i = 1; i < _crtController.lineResolution; i++)
            {
                Vector3 oldPoint = points[i - 1];
                
                currentVelX += _crtController.RK4(0, oldPoint);
                currentVelY += _crtController.RK4(1, oldPoint);
                currentVelZ += _crtController.RK4(2, oldPoint);

                Vector3 newPoint = oldPoint;
                newPoint.x += currentVelX * _timeStep;
                newPoint.y += currentVelY * _timeStep;
                newPoint.z += currentVelZ * _timeStep;

                if (UnityEngine.Physics.Linecast(oldPoint, newPoint))
                    points.Add(oldPoint);
                else
                    points.Add(newPoint);
            }
            _crtController.checkScreenHit(points.Last());
            lineRenderer.SetPositions(points.ToArray());
        }

        public void ResetObject()
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
        }
    }
}
