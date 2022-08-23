using UnityEngine;

namespace Maroon.Physics.HuygensPrinciple
{
    public class InterferencePlate : PausableObject
    {
        [SerializeField]
        private SlitPlate slitPlate;

        private MeshRenderer _meshRenderer;

        protected void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        protected override void Start()
        {
            base.Start();
            UpdateParameters();
        }

        protected override void HandleUpdate()
        {
        }

        protected override void HandleFixedUpdate()
        {   
        }

        public void UpdateParameters()
        {
            UpdateNumberOfSlits();
            UpdateSlitWidth();
            UpdatePlatePosition();
            UpdateWaveLength();
        }

        public void UpdateSlitWidth()
        {
            _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_SlitWidth"), 
                slitPlate.SlitWidth);         
        }

        public void UpdateNumberOfSlits()
        {
            if (slitPlate.NumberOfSlits > 1)
            {
                _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_DistanceBetweenSlits"),
                    slitPlate.GetDistanceBetweenSlitCenters());
            }

            _meshRenderer.sharedMaterial.SetInt(Shader.PropertyToID("_NumberOfSlits"),
                slitPlate.NumberOfSlits);
        }

        public void UpdatePlatePosition()
        {
            _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_DistanceBetweenPlates"),
                 Vector4.Distance(slitPlate.transform.position, transform.position));
        }

        public void UpdateWaveLength()
        {
            _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_WaveLength"),
                (WaveGeneratorPoolHandler.Instance.WaveLength * 0.3f));
        }

    }
}
