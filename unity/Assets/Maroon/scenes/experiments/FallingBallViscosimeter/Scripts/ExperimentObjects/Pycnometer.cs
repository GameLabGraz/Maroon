using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.Physics.Viscosimeter
{
    public class Pycnometer : MonoBehaviour, IResetObject
    {
        decimal volume = 50.28m / 1000000.0m; //m^3
        WeighableObject weighableObject;
        decimal starting_weight = 0.029167m;
        private MeshRenderer renderer_;
        public bool filled;
        private Vector3 startingPosition;

        private void Start()
        {
            startingPosition = gameObject.transform.position;
            weighableObject = GetComponent<WeighableObject>();
            starting_weight = ViscosimeterManager.AddInaccuracy(starting_weight);
            weighableObject.starting_weight = starting_weight;
            weighableObject.ResetWeight();
            renderer_ = GetComponent<MeshRenderer>();
            filled = false;
        }

        public void FillPycnometer() //kg/m^3
        {
            decimal density = ViscosimeterManager.Instance.fluid_density_;
            weighableObject.SetWeight(weighableObject.starting_weight + (density * volume));
            renderer_.material.color = new Color(0.65f,0.16f,0.16f,0.3f);
            filled = true;
        }

        public void EmptyPycnometer()
        {
            weighableObject.ResetWeight();
            renderer_.material.color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
            filled = false;
        }

        public void ResetObject()
        {
            EmptyPycnometer();
            gameObject.transform.position = startingPosition;
        }

    }
}