using UnityEngine;

namespace Maroon.Physics.HuygensPrinciple
{
    public class InterferencePlate : MonoBehaviour
    {
        [SerializeField]
        private SlitPlate slitPlate;

        private MeshRenderer _meshRenderer;

        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            UpdateParameters();
        }


        private void Update()
        {
            //ToDo: Add OnMovement Event to the SlitPlate script
            if (slitPlate.transform.hasChanged)
            {
                slitPlate.transform.hasChanged = false;
                UpdateParameters();
            }
        }

        public void UpdateParameters()
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
    }
}
