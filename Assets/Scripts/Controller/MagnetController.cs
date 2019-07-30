using System.Collections;
using UnityEngine;
using VRTK;

public class MagnetController : VRTK_InteractableObject
{
    private bool _isMoving = false;

    private GameObject _grabbingObject;

    private SimulationController _simController;

    private void Start()
    {
        _simController = FindObjectOfType<SimulationController>();
    }

    public override void Grabbed(VRTK_InteractGrab currentGrabbingObject = null)
    {
        base.Grabbed(currentGrabbingObject);

        if (currentGrabbingObject != null)
            _grabbingObject = currentGrabbingObject.gameObject;

        _isMoving = true;

        _simController.StartSimulation();

        StartCoroutine(TriggerHapticPulse());
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject = null)
    {
        base.Ungrabbed(previousGrabbingObject);

        _isMoving = false;

        _simController.StopSimulation();
    }


    private IEnumerator TriggerHapticPulse()
    {
        while (_isMoving)
        {
            var hapticPulseStrength = GetComponent<Magnet>().getExternalForce().magnitude / 10;

            VRTK_ControllerHaptics.TriggerHapticPulse(
                    VRTK_ControllerReference.GetControllerReference(_grabbingObject),
                    hapticPulseStrength);

            yield return new WaitForFixedUpdate();
        }
    }

}
