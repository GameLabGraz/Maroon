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
             - Some scenes may not contain instances (gameobjects) of this singleton.
                 Although it is possible to design singletons so that they always exist 
                 (by spawning a new gameObject if not, or by using static member functions that return default values if no instance exists),
                 I decided to just return null as the instance, which will cause a nullpointerexception.
                 I guess this depends on the use-case of the singleton, but for all singletons in CoulombsLaw 
                 (SimulationBox, CameraController, ElectricField, SelectionSystem),
                 there aren't any good ways to set default values if the singleton does not exist.
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
