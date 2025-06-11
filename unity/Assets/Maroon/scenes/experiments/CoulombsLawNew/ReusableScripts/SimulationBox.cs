using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class SimulationBox : MonoBehaviour
    {
        [SerializeField] private Vector3 _size = Vector3.one * 2; // Sidelengths of the bounding box (Not extends, to use Unity vocabulary of Bounds)
        public UnityEvent<Bounds> OnBoundsChanged;

        public Bounds Bounds { 
            get { return new Bounds(transform.position, _size); } 
            set 
            {
                transform.position = value.center;
                _size = value.size;
                OnBoundsChanged.Invoke(Bounds);
            }
        }

        // Singleton pattern
        // Note(MartinR):
        //      See https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
        //      for rationals behind this singleton design.
        private static SimulationBox _instance;

        public static SimulationBox Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Check if scene already contains a simulationBox
                    // Note(MartinR):
                    //      This is necessary in addition to the "_instance = this;" in Awake,
                    //      because during Scene initialization, other GameObjects may use SimulationBox in their Awake,
                    //      and as the order in which Awake is called among GameObjects is undefined,
                    //      the _instance may not be set at that point, so a scene-search is required.
                    _instance = GameObject.FindObjectOfType<SimulationBox>();

                    // Create a simulation box object if none was found
                    if (_instance == null) _instance = new GameObject("SimulationBox").AddComponent<SimulationBox>();
                }

                return _instance;
            }
        }

        private void Awake()
        {
            // To avoid duplication, this script delets the attached gameObject if an instance of the Singleton already exists
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                Debug.LogWarning("SimulationBox instance was destroyed due to duplication");
                return;
            }
            _instance = this;
        }

        private void OnDestroy()
        {
            if (this == _instance) { _instance = null; }
        }
    }
}
