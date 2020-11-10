using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Maroon.Lighting
{
    [ExecuteInEditMode]
    public class ExperimentRoomLighting : MonoBehaviour
    {
        [SerializeField] private List<GameObject> ignoreForLightBaking;

        private void Start()
        {
#if UNITY_EDITOR
            Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;

            Lightmapping.bakeStarted += () =>
            {
                Debug.Log("Disable Roof and FrontWall for Lightning");

                foreach (var obj in ignoreForLightBaking)
                {
                    obj.SetActive(false);
                }
            };

            Lightmapping.bakeCompleted += () =>
            {
                Debug.Log("Enable Roof and FrontWall");

                foreach (var obj in ignoreForLightBaking)
                {
                    obj.SetActive(true);
                }
            };
#endif
        }
    }
}
