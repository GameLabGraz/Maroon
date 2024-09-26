using Maroon.Physics.Optics.TableObject;
using Maroon.ReusableScripts.ExperimentParameters;
using System.Collections.Generic;
using static Maroon.Physics.Optics.Camera.CameraControls;

namespace Maroon.Physics.Optics.Manager
{
    [System.Serializable]
    public class OpticsParameters : ExperimentParameters
    {
        public float rayThickness;
        public CameraSetting cameraSettingBaseView;
        public CameraSetting cameraSettingTopView;
        public List<TableObjectParameters> tableObjectParameters;
    }
}