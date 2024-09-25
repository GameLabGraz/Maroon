using Maroon.ReusableScripts.ExperimentParameters;
using static Maroon.Physics.Optics.Camera.CameraControls;

namespace Maroon.Physics.Optics.Manager
{
    [System.Serializable]
    public class OpticsParameters : ExperimentParameters
    {
        public float rayThickness;
        public CameraSetting cameraSettingBaseView;
        public CameraSetting cameraSettingTopView;
    }
}