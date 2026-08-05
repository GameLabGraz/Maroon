using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maroon.Physics.Motion;
using System.Linq;

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
            _lineRenderer.positionCount = 0;
            _lineRenderer.enabled = true;
            spiral.GetComponent<MeshRenderer>().material = hotMetal;
            electronCloud.SetActive(true);

            var solver = new Solver();
            solver.t0 = 0;
            solver.dt = _timeStep;

            var electronBeam = new MotionEntity();
            electronBeam.SetInitialState(Vector3.zero, Vector3.zero);
            electronBeam.AddExpression("fx", "-q * ex * h(electronGunLength - x)");
            electronBeam.AddExpression("fy", "if(verticalPlateEnabled, -q * ey * h(verticalPlateLength - abs(verticalPlatePosition - x)) * h(verticalPlateWidth - y), 0)");
            electronBeam.AddExpression("fz", "if(horizontalPlateEnabled, -q * ez * h(horizontalPlateLength - abs(horizontalPlatePosition - x)) * h(horizontalPlateWidth - z), 0)");
            electronBeam.AddExpression("h(x)", "if(x == 0, 0.5, (1 + x/abs(x))/2)");

            electronBeam.AddParameter("m", CRTController.ElectronMass);
            electronBeam.AddParameter("q", CRTController.ElectronCharge);
            electronBeam.AddParameter("ex", crtController.EX);
            electronBeam.AddParameter("ey", crtController.EY);
            electronBeam.AddParameter("ez", crtController.EZ);
            electronBeam.AddParameter("electronGunLength", crtController.ElectronGunLength);
            electronBeam.AddParameter("verticalPlatePosition", crtController.VerticalPlatePosition);
            electronBeam.AddParameter("verticalPlateLength", crtController.VerticalPlateLength);
            electronBeam.AddParameter("verticalPlateWidth", crtController.VerticalPlateWidth);
            electronBeam.AddParameter("horizontalPlatePosition", crtController.HorizontalPlatePosition);
            electronBeam.AddParameter("horizontalPlateLength", crtController.HorizontalPlateLength);
            electronBeam.AddParameter("horizontalPlateWidth", crtController.HorizontalPlateWidth);
            electronBeam.AddParameter("horizontalPlateEnabled", crtController.HorizontalPlateEnabled);
            electronBeam.AddParameter("verticalPlateEnabled", crtController.VerticalPlateEnabled);

            solver.AddEntity(electronBeam);

            RaycastHit hitInfo = new();
            for (int i = 1; i <= crtController.lineResolution; i++)
            {
                solver.Step();

                if (UnityEngine.Physics.Linecast(ToWorldSpace(electronBeam.Position[i - 1]), ToWorldSpace(electronBeam.Position[i]), out hitInfo))
                {
                    break;
                }
            }

            if (hitInfo.collider != null && hitInfo.collider.name.Equals("Screen"))
                screen.GetComponent<ScreenSetup>().ActivatePixel(hitInfo.point);

            _lineRenderer.positionCount = solver.steps;
            _lineRenderer.SetPositions(electronBeam.Position.ConvertAll(x => ToWorldSpace(x)).ToArray());
            crtController.UpdateData(electronBeam.Position, electronBeam.Velocity, electronBeam.Force);
            yield return null;
        }

        private Vector3 ToWorldSpace(Vector3 input)
        {
            return input + crtController.WorldSpaceOrigin;
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