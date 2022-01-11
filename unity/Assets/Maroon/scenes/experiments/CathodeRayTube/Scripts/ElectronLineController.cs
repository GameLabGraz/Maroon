using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            var points = new Vector3[_crtController.lineResolution];
            Vector3 startPoint = _crtController.GetCRTStart();
            points[0] = startPoint;

            float currentVelX = 0;
            float currentVelY = 0;
            float currentVelZ = 0;
            
            for (int i = 1; i < _crtController.lineResolution; i++)
            {
                currentVelX += _crtController.RK4(0, points[i - 1].x);
                currentVelY += _crtController.RK4(1, points[i - 1].x);
                currentVelZ += _crtController.RK4(2, points[i - 1].x);

                Vector3 newPoint = points[i - 1];
                newPoint.x += currentVelX * _timeStep;
                newPoint.y += currentVelY * _timeStep;
                newPoint.z += currentVelZ * _timeStep;

                points[i] = newPoint;
            }
            lineRenderer.SetPositions(points);
        }

        public void ResetObject()
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
        }
    }
}
