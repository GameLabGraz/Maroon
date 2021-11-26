using System.Collections;
using UnityEngine;
using VRTK;

public class MagnetController : VRTK_InteractableObject
{
    private bool _isMoving = false;

    private GameObject _grabbingObject;

    public override void Grabbed(VRTK_InteractGrab currentGrabbingObject = null)
    {
        base.Grabbed(currentGrabbingObject);

        if (currentGrabbingObject != null)
            _grabbingObject = currentGrabbingObject.gameObject;

        _isMoving = true;

        SimulationController.Instance.StartSimulation();

        StartCoroutine(TriggerHapticPulse());
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject = null)
    {
        base.Ungrabbed(previousGrabbingObject);

        _isMoving = false;

        SimulationController.Instance.StopSimulation();
    }


    private IEnumerator TriggerHapticPulse()
    {
        while (_isMoving)
        {
            var hapticPulseStrength = GetComponent<Magnet>().GetExternalForce().magnitude / 10;

            VRTK_ControllerHaptics.TriggerHapticPulse(
                    VRTK_ControllerReference.GetControllerReference(_grabbingObject),
                    hapticPulseStrength);

            yield return new WaitForFixedUpdate();
        }
    }

}
