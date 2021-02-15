using System;
using UnityEngine;

namespace Maroon
{
    [Serializable] public class SceneCategory
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields

        [SerializeField] private string name = "CategoryName";

        [SerializeField] private bool hiddenCategory = false;

        [SerializeField] private Maroon.SceneType sceneTypeInThisCategory = Maroon.SceneType.Standard;

        [SerializeField] private Maroon.CustomSceneAsset[] scenes = null;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Getters and Properties

        public string Name
        {
            get { return this.name; }
        }

        public bool HiddenCategory
        {
            get { return this.hiddenCategory; }
        }

        public bool IsVRCategory
        {
            get
            {
                if(this.sceneTypeInThisCategory == Maroon.SceneType.VR)
                {
                    return true;
                }
                
                return false;
            }
        }

        public Maroon.SceneType SceneTypeInThisCategory
        {
            get { return this.sceneTypeInThisCategory; }
        }

        public Maroon.CustomSceneAsset[] Scenes
        {
            get { return this.scenes; }
        }
    }
}
