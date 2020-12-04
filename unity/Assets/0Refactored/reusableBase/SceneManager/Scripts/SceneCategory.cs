using System;
using UnityEngine;

namespace Maroon
{
    [Serializable] class SceneCategory
    {
        [SerializeField] private string Name;
        [SerializeField] private Utilities.SceneField[] SceneFields;
    }
}
