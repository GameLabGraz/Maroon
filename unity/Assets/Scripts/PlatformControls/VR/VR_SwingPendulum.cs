using Maroon.Physics;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(VRTK_InteractableObject), typeof(Pendulum))]
public class VR_SwingPendulum : MonoBehaviour
{
    private Pendulum _pendulum;

    private void Start()
    {
        _pendulum = GetComponent<Pendulum>();

        GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += (sender, args) =>
        {
            Debug.Log("Swing Pendulum");
            if (!SimulationController.Instance.SimulationRunning)
                SimulationController.Instance.StartSimulation();
        };

        GetComponent<VRTK_InteractableObject>().InteractableObjectUngrabbed += (sender, args) =>
        {
            _pendulum.GetComponent<Rigidbody>().isKinematic = false;
        };
    }
}
