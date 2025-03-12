using Maroon.Physics.Optics.Camera;
using Maroon.Physics.Optics.TableObject;
using Maroon.ReusableScripts.ExperimentParameters;
using System.Collections.Generic;

namespace Maroon.Physics.Optics.Manager
{
    [System.Serializable]
    public class OpticsParameters : ExperimentParameters
    {
        public string presetNameTranslationKey;
        public float rayThickness;
        public CameraSetting cameraSettingBaseView;
        public CameraSetting cameraSettingTopView;
        public List<TableObjectParameters> tableObjectParameters;

        public override void OnLoaded()
        {
            base.OnLoaded();

            cameraSettingBaseView.CheckRotations();
            cameraSettingTopView.CheckRotations();

            foreach (TableObjectParameters top in tableObjectParameters)
            {
                top.CheckRotations();
            }
        }
    }
}