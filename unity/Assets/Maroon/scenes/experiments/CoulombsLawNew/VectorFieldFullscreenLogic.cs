using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class VectorFieldFullscreenLogic : MonoBehaviour
    {
        [SerializeField] private Material vectorFieldMaterial;
        [SerializeField] private GuiBoolInputHandler uiEnabledToggle;
        [SerializeField] private GuiIntInputHandler  uiResolutionSlider;

        private void Start()
        {
            Camera cam = GetComponent<Camera>();
            cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth; // Request depth texture for rendering
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // Early exit if not enabled
            if (!uiEnabledToggle.GetValue())
            {
                Graphics.Blit(source, destination);
                return;
            }

            var box = SimulationBox.Instance.Bounds;
            int resolution = uiResolutionSlider.GetValue();

            float maxDimSize = Mathf.Max(box.size.z, Mathf.Max(box.size.x, box.size.y));
            // if (in3DMode) {
            //     maxDimSize = Mathf.Max(maxDimSize, box.size.z);
            // }
            float cellSize = maxDimSize / uiResolutionSlider.GetValue();
            var domainOrigin = box.min +
                (box.size - Vector3.one * resolution * cellSize) / 2;

            Matrix4x4 inverseView = Camera.main.worldToCameraMatrix.inverse;
            vectorFieldMaterial.SetMatrix("_InverseView", inverseView);
            vectorFieldMaterial.SetVector("_BoxMin", domainOrigin);
            vectorFieldMaterial.SetInt("_FieldResolution", resolution);
            vectorFieldMaterial.SetFloat("_CellSize", cellSize);
            // vectorFieldMaterial.SetBuffer("_MyBuffer", computeBuffer);
            Graphics.Blit(source, destination, vectorFieldMaterial);
        }
    }
}
