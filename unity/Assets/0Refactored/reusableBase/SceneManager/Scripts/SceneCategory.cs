using System;
using UnityEngine;

namespace Maroon
{
    [Serializable] class SceneCategory
    {
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Fields

        [SerializeField] private string name;
        [SerializeField] private Maroon.CustomSceneAsset[] scenes;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // Properties

        public string Name
        {
            get { return this.name; }
        }

        public Maroon.CustomSceneAsset[] Scenes
        {
            get { return this.scenes; }
        }
    }
}
