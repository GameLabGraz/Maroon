using System;
using Maroon.Physics.Pendulum;
using UnityEngine;
using UnityEngine.Events;

namespace PlatformControls.PC
{

    [RequireComponent(typeof(Pendulum))]
    public class PC_SwingPendulum : MonoBehaviour
    {
        private Pendulum _pendulum;

        private Vector3 _mouseStart;

        public UnityEvent OnRelease;

        public UnityEvent OnGrab;

    	private void Start()
        {
            _pendulum = GetComponent<Pendulum>();
        }

        private void OnMouseDown()
        {
            if (!enabled)
                return;
            
            if (!SimulationController.Instance.SimulationRunning)
                SimulationController.Instance.StartSimulation();

            _mouseStart = Input.mousePosition;
            _pendulum.GetComponent<Rigidbody>().WakeUp();
            
            OnGrab?.Invoke();
        }

        private void OnMouseUp()
        {
            if (!enabled)
                return;
            
            _pendulum.Joint.useLimits = false;
            OnRelease?.Invoke();
        }

        private void OnMouseDrag()
        {
            if (!enabled)
                return;
            
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
