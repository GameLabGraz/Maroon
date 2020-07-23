// SteamVR Controller|SDK_SteamVR|004
namespace VRTK
{
#if VRTK_DEFINE_SDK_STEAMVR
    using UnityEngine;
    using System.Collections.Generic;
    using System.Text;
    using Valve.VR;
    using System;
#if !VRTK_DEFINE_STEAMVR_PLUGIN_1_2_2_OR_NEWER
    using System;
    using System.Reflection;
#endif
#endif

    /// <summary>
    /// The SteamVR Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    [SDK_Description(typeof(SDK_SteamVRSystem))]
    public class SDK_SteamVRController
#if VRTK_DEFINE_SDK_STEAMVR
        : SDK_BaseController
#else
        : SDK_FallbackController
#endif
    {
        public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
            throw new NotImplementedException();
        }

        public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
            throw new NotImplementedException();
        }

        public override ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)
        {
            throw new NotImplementedException();
        }

        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            throw new NotImplementedException();
        }

        public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
        {
            throw new NotImplementedException();
        }

        public override uint GetControllerIndex(GameObject controller)
        {
            throw new NotImplementedException();
        }

        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            throw new NotImplementedException();
        }

        public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)
        {
            throw new NotImplementedException();
        }

        public override Transform GenerateControllerPointerOrigin(GameObject parent)
        {
            throw new NotImplementedException();
        }

        public override GameObject GetControllerLeftHand(bool actual = false)
        {
            throw new NotImplementedException();
        }

        public override GameObject GetControllerRightHand(bool actual = false)
        {
            throw new NotImplementedException();
        }

        public override bool IsControllerLeftHand(GameObject controller)
        {
            throw new NotImplementedException();
        }

        public override bool IsControllerRightHand(GameObject controller)
        {
            throw new NotImplementedException();
        }

        public override bool IsControllerLeftHand(GameObject controller, bool actual)
        {
            throw new NotImplementedException();
        }

        public override bool IsControllerRightHand(GameObject controller, bool actual)
        {
            throw new NotImplementedException();
        }

        public override bool WaitForControllerModel(ControllerHand hand)
        {
            throw new NotImplementedException();
        }

        public override GameObject GetControllerModel(GameObject controller)
        {
            throw new NotImplementedException();
        }

        public override GameObject GetControllerModel(ControllerHand hand)
        {
            throw new NotImplementedException();
        }

        public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)
        {
            throw new NotImplementedException();
        }

        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            throw new NotImplementedException();
        }

        public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength )
        {
            throw new NotImplementedException();
        }

        public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            throw new NotImplementedException();
        }

        public override SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            throw new NotImplementedException();
        }

        public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
        {
            throw new NotImplementedException();
        }

        public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            throw new NotImplementedException();
        }

        public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)
        {
            throw new NotImplementedException();
        }

        public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            throw new NotImplementedException();
        }

        public override float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            throw new NotImplementedException();
        }

        public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            throw new NotImplementedException();
        }

        public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType,
            VRTK_ControllerReference controllerReference)
        {
            throw new NotImplementedException();
        }
    }
}