using UnityEngine;

namespace Maroon.Physics.HuygensPrinciple
{
    public class InterferencePlate : PausableObject
    {
        [SerializeField]
        private SlitPlate slitPlate;

        private MeshRenderer _meshRenderer;
        private bool _parameterHasChanged;
        
        protected override void Start()
        {
            base.Start();

            _meshRenderer = GetComponent<MeshRenderer>();
            UpdateParameters();
        }

        protected override void HandleUpdate()
        {
            if(!slitPlate.transform.hasChanged && !_parameterHasChanged)
            {
                return;
            }
           
            _parameterHasChanged = false;
            slitPlate.transform.hasChanged = false;
            UpdateParameters();   
        }

        protected override void HandleFixedUpdate()
        {   
        }

        private void UpdateParameters()
        {
            if (slitPlate.NumberOfSlits > 1)
            {
                _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_DistanceBetweenSlits"), 
                    slitPlate.GetDistanceBetweenSlitCenters());
            }

            _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_WaveLength"), 
                WaveGeneratorPoolHandler.Instance.WaveLength);

            _meshRenderer.sharedMaterial.SetInt(Shader.PropertyToID("_NumberOfSlits"), 
                slitPlate.NumberOfSlits);

            _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_SlitWidth"), 
                slitPlate.SlitWidth);

            _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_DistanceBetweenPlates"), 
                Vector4.Distance(slitPlate.transform.position, transform.position));
        }

        public void SetParameterChangeTrue()
        {
            _parameterHasChanged = true;
        }
    }
}
