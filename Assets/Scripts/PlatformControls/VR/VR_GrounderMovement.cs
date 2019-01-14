using PlatformControls.BaseControls;
using UnityEngine;
using VRTK;

namespace PlatformControls.VR
{
    public class VR_GrounderMovement : GrounderMovement
    {
        [SerializeField]
        private float _hapticPulseStrength = 0.5f;

        private GameObject _leftController;
        private GameObject _rightController;

        protected override void Start()
        {
            base.Start();

            _leftController = VRTK_DeviceFinder.GetControllerLeftHand();
            _rightController = VRTK_DeviceFinder.GetControllerRightHand();
        }

        private void Update()
        {       
            if(!_leftController)
                _leftController = VRTK_DeviceFinder.GetControllerLeftHand();
            if(!_rightController)
                _rightController = VRTK_DeviceFinder.GetControllerRightHand();

            if (_leftController)
            {
                var controllerEvent = _leftController.GetComponent<VRTK_ControllerEvents>();
                if(controllerEvent.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.GripPress))
                {
                    Debug.Log("Move grounder to LEFT");
                    Move(Vector3.left, MaxMovementLeft);

                    VRTK_ControllerHaptics.TriggerHapticPulse(
                        VRTK_ControllerReference.GetControllerReference(_leftController),
                        _hapticPulseStrength);
                }
            }
            if(_rightController)
            {
                var controllerEvent = _rightController.GetComponent<VRTK_ControllerEvents>();
                if (controllerEvent.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.GripPress))
                {
                    Debug.Log("Move grounder to RIGHT");
                    Move(Vector3.right, MaxMovementLeft);

                    VRTK_ControllerHaptics.TriggerHapticPulse(
                        VRTK_ControllerReference.GetControllerReference(_rightController),
                        _hapticPulseStrength);
                }
            }
        }
    }
}
