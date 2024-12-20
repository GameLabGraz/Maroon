using System;
using UnityEngine;
using GameLabGraz.QuestManager;

namespace Quests
{
    [RequireComponent(typeof(Quest))]
    public class VectorFieldResolutionCheck : QuestCheck
    {
        private VectorField _vectorField;
        private int _initResolution;

        protected override void InitCheck()
        {
            _vectorField = FindObjectOfType<VectorField>();
            if (_vectorField == null) throw new NullReferenceException("There is no VectorField in the scene.");

            _initResolution = _vectorField.Resolution;
        }

        protected override bool CheckCompliance()
        {
            return _initResolution != _vectorField.Resolution;
        }
    }
}