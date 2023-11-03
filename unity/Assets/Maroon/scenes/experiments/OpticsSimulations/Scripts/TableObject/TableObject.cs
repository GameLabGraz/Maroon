using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.scenes.experiments.OpticsSimulations.Scripts.Manager;
using UnityEngine;


namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject
{
    public class TableObject : MonoBehaviour
    {
        [SerializeField] private ComponentType componentType;
        [SerializeField] private GameObject component;
        [SerializeField] private GameObject rotationArrowY;
        [SerializeField] private GameObject rotationArrowZ;
        [SerializeField] private GameObject translationArrowY;
        
        public ComponentType ComponentType => componentType;

        public GameObject Component => component;

        public GameObject TranslationArrowY => translationArrowY;

        public GameObject RotationArrowY => rotationArrowY;

        public GameObject RotationArrowZ => rotationArrowZ;

        private void Awake()
        {
            
        }

        public void SetArrowsActive(bool value)
        {
            rotationArrowY.SetActive(value);
            rotationArrowZ.SetActive(value);
            translationArrowY.SetActive(value);
        }
        

    }
    
    public enum ComponentType
    {
        LightSource = 0,
        OpticalComponent = 1,
        Wall = 2,
    }
}
