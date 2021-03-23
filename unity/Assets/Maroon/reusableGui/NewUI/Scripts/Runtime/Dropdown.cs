using System;
using TMPro;
using UnityEngine;

namespace Maroon.UI
{
    [Serializable]
    public class Dropdown : TMP_Dropdown, IResetObject
    {
        [SerializeField] private bool allowReset = true;

        private int _startValue;

        public bool AllowReset
        {
            get => allowReset;
            set => allowReset = value;
        }

        protected override void Start()
        {
            base.Start();
            _startValue = value;
        }

        public void ResetObject()
        {
            if (allowReset)
                value = _startValue;
        }
    }
}