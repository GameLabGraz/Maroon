using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PointChargeExperiment
{
    public class PointCharge : MonoBehaviour
    {
        // Charge in nano Coulombs
        public float charge = 0.0f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material.color = ExperimentController.ChargeValueToColor(charge);
            }
        }
    }
}
