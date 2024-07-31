#if UNITY_WEBGL && !UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Maroon.Config
{
    public class ConfigLoader3DMotionSimulation : Maroon.Config.ConfigLoader
    {
        public override void SetParameters()
        {
            var parameters = JsonConvert.DeserializeObject<ParameterLoader.Parameters>(_parametersString);
            ParameterUI.Instance.LoadParameters(parameters);
        }
    }
}
#endif
