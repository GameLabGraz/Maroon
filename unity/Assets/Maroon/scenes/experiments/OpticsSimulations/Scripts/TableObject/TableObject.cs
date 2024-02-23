using System;
using UnityEngine;

namespace Maroon.scenes.experiments.OpticsSimulations.Scripts.TableObject
{
    public class TableObject : MonoBehaviour
    {
        [Header("Mesh Properties")] 
        public int nrOfSegments = 16;
        
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

        public virtual void RemoveFromTable()
        {
            throw new Exception("Should not call base RemoveFromTable Method!");
        }

        public void SetArrowsActive(bool value)
        {
            rotationArrowY.SetActive(value);
            rotationArrowZ.SetActive(value);
            translationArrowY.SetActive(value);
        }

        public virtual void RecalculateMesh()
        {
            throw new Exception("Should not call base RecalculateMesh Method!");
        }

    }
    
    public enum ComponentType
    {
        LightSource = 0,
        OpticalComponent = 1,
        Wall = 2,
    }
}
