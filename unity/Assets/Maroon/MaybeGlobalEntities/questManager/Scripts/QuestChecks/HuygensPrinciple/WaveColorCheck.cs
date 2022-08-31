using System;
using UnityEngine;

namespace QuestManager
{
    [RequireComponent(typeof(IQuest))]
    public class WaveColorCheck : IQuestCheck
    {
        private WaterPlane _waterPlane;
        private MeshRenderer _meshRenderer;

        private Color _initColorMin;
        private Color _initColorMax;


        protected override void InitCheck()
        {
            _waterPlane = FindObjectOfType<WaterPlane>();

            if (_waterPlane == null) throw new NullReferenceException("There is no water plane in the scene.");

            _meshRenderer = _waterPlane.GetComponent<MeshRenderer>();
            
            _initColorMin = _meshRenderer.sharedMaterial.GetColor("_ColorMin");
            _initColorMax = _meshRenderer.sharedMaterial.GetColor("_ColorMax");
        }

        protected override bool CheckCompliance()
        {
            return _initColorMin != _meshRenderer.sharedMaterial.GetColor("_ColorMin") ||
                   _initColorMax != _meshRenderer.sharedMaterial.GetColor("_ColorMax");
        }
    }
}