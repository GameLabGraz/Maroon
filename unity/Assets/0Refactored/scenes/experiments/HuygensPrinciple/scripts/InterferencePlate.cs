using UnityEngine;

namespace Maroon.Physics.HuygensPrinciple
{
    public class InterferencePlate : MonoBehaviour
    {
        [SerializeField]
        private SlitPlate slitPlate;

        private float _waveLength;
        private float _distanceBetweenSlits;
        private float _distanceBetweenPlates;
        private MeshRenderer _meshRenderer;

        void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            UpdateParameters();
        }

        // Update is called once per frame
        void Update()
        {
            //Check if there is a other way to check if distance betwen plates changed
            if (slitPlate.transform.hasChanged)
            {
                slitPlate.transform.hasChanged = false;
                UpdateParameters();
            }
        }

        public void UpdateParameters()
        {
            int numberOfSlits = slitPlate.NumberOfSlits;
            _waveLength = WaveGeneratorPoolHandler.Instance.WaveLength;
            _distanceBetweenPlates = Vector4.Distance(slitPlate.transform.position, transform.position);

            if (slitPlate.NumberOfSlits > 1)
            {
                _distanceBetweenSlits = slitPlate.GetDistanceBetweenSlitCenters();
                _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_DistanceBetweenSlits"), _distanceBetweenSlits);
            }

            _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_WaveLength"), _waveLength);
            _meshRenderer.sharedMaterial.SetInt(Shader.PropertyToID("_NumberOfSlits"), numberOfSlits);
            _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_SlitWidth"), slitPlate.SlitWidth);
            _meshRenderer.sharedMaterial.SetFloat(Shader.PropertyToID("_DistanceBetweenPlates"), _distanceBetweenPlates);
        }
    }
}
