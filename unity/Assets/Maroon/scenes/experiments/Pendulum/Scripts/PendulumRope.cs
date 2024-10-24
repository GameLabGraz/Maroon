using UnityEngine;

namespace Maroon.Physics.Pendulum
{
    [RequireComponent(typeof(AdvancedLineRenderer))]
    public class PendulumRope : PausableObject, IResetObject
    {
        private AdvancedLineRenderer _lineRenderer;

        [SerializeField]
        private GameObject _ropeJoint;

        [SerializeField]
        private GameObject _weight;

        [SerializeField]
        private float _ropeWidth = 0.001f;

        protected override void Start()
        {
            base.Start();
            _lineRenderer = GetComponent<AdvancedLineRenderer>();
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.InitLineRenderer();
            _lineRenderer.SetWidth(_ropeWidth, _ropeWidth);

            SimulationController.Instance.OnStop.AddListener(() => { DrawRope();});

            DrawRope();
        }

        protected override void HandleUpdate()
        {
            DrawRope();
        }
        
        protected override void HandleFixedUpdate()
        {
        
        }

        public void DrawRope()
        {
            if (_lineRenderer == null)
                return;
            
            _lineRenderer.Clear();
            _lineRenderer.SetPosition(0, _ropeJoint.transform.position);
            _lineRenderer.SetPosition(1, _weight.transform.position);
            _lineRenderer.WritePositionsToLineRenderer();
        }

        public void ResetObject()
        {
            DrawRope();
        }
    }
}
