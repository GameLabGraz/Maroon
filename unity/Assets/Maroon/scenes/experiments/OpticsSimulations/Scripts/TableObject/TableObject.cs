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
        
        public ComponentType ComponentType => componentType;

        private void Awake()
        {
            
        }


    }
    
    public enum ComponentType
    {
        LightSource = 0,
        OpticalComponent = 1,
        Wall = 2,
    }
}
