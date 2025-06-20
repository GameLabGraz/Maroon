using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Maroon.Experiments.CoulombsLawNew
{
    public class SimulationBox : MonoBehaviour
    {
        // Note(MartinR): Instead of having a Bounds member to store the extends, this
        //      class uses transform.postion for the center and _size member for box size.
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
        /* Note(MartinR): 
            See https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
            for considerations when making a singleton in unity.

            Here are some thoughts/design for this singleton implementation:
             - GameObjects using the singleton should be allowed to assume that the Singleton-Instance always exists in the scene,
                 so they don't need extra code-paths if the singleton does not exist. To facilitate this,
                 a new Instance is created in the scene with reasonable default values if a gameObject accesses the 
                 singleton for the first time and no instance is present in the scene.
             - It should be possible to set the initial values of the SimulationBox (position/size) in the Unity Editor.
                 To do this, a scene needs to contain one GameObject with this Singelton-Component. Then, the inspector
                 can be used to tweak the properties of the singleton. The problem that arises with this is that
                 multiple GameObjects with this component could be created in the scene. To solve this, if an
                 instance of this singleton is created when another instance already exists, the newly created singleton
                 updates the values of the currently existing one, and then destroys itself.
             - Scene changes should be handled correctly 
                 If two scenes each contain one instance of the SimulationBox (with possibly different values), then 
                 when the scenes are changed, the Instance member should point to the correct object and the correct objects should be used.
                 Note that editing prefabs also change the scene, and this use case should also work.
                 Most of this is currently handled by setting the instance to null in OnDestroy().
             - GameObjects should be able to access the singleton-values in Awake().
                 Note that the order of Awake() calls in Unity cannot be relied upon, so what is currently done
                 is that all gameObjects that use the singleton values should have logic to handle changes to
                 the singleton (If SimulationBox is resized/moved) by listening to the correct UnityEvents (OnBoundsChanged for SimulationBox)

            Note that Maroon also has GlobalEntities and maybe making this class a global entity is the preferred way of doing this...
        */
        private static SimulationBox _instance;

        public static SimulationBox Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Check if scene already contains a simulationBox
                    _instance = GameObject.FindObjectOfType<SimulationBox>();
                    // Create a simulation box object if none exists in the scene
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
                _instance.Bounds = Bounds;
                _instance.OnBoundsChanged.Invoke(_instance.Bounds);
                Destroy(this.gameObject);
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
