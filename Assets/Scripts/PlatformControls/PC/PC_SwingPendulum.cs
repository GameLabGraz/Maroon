using System;
using Maroon.Physics;
using UnityEngine;
using UnityEngine.Events;

namespace PlatformControls.PC
{

    [RequireComponent(typeof(Pendulum))]
    public class PC_SwingPendulum : MonoBehaviour
    {
        private SimulationController _simulationController;
        private Pendulum _pendulum;

        private Vector3 _mouseStart;

        public UnityEvent OnRelease;

    	private void Start()
        {
            _simulationController = FindObjectOfType<SimulationController>();
            if(!_simulationController)
                throw new System.NullReferenceException("Simulation Controller is null");

            _pendulum = GetComponent<Pendulum>();
        }

        private void OnMouseDown()
        {
            if (!_simulationController.SimulationRunning)
                _simulationController.StartSimulation();

            _mouseStart = Input.mousePosition;
            _pendulum.GetComponent<Rigidbody>().WakeUp();
        }

        private void OnMouseUp()
        {
            _pendulum.Joint.useLimits = false;
            OnRelease?.Invoke();
        }

        private void OnMouseDrag()
        {
            // relative mouse movement / (scaling factor for easy use) 
            var angle = ((_mouseStart.x - Input.mousePosition.x) / (_pendulum.RopeLength * 10));

            // Everything above 140 degrees seems to freak out unity enormously, just don't allow it
            angle = Math.Min(Math.Max(-140f, angle), 140f);

            _pendulum.Joint.limits = new JointLimits
            {
                min = angle,
                max = angle + 0.0001f
            };
            _pendulum.Joint.useLimits = true;
        }
    }
}
