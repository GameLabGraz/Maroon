using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Maroon.Chemistry.Catalyst.VR
{
    [RequireComponent(typeof(Molecule))]
    public class MoleculeVRInteractionHandler : MonoBehaviour
    {
        private Molecule _molecule;

        private Interactable _interactable;
        private Throwable _throwable;

        private Transform _prePickupParent;

        private void Start()
        {
            _prePickupParent = gameObject.transform.parent;

            _interactable = GetComponent<Interactable>();
            _throwable = GetComponent<Throwable>();
            _interactable.enabled = true;

            _molecule = GetComponent<Molecule>();
            _molecule.OnReactionStarted.AddListener(DisableVRInteraction);
        }

        private void DisableVRInteraction()
        {
            if (_throwable)
            {
                _throwable.onDetachFromHand.Invoke();
                Destroy(_throwable);
            }
            if(_interactable)
                Destroy(_interactable);
        }

        public void OnVRPickup()
        {
            if (_molecule.State != MoleculeState.Fixed || !SimulationController.Instance.SimulationRunning) return;
            _molecule.OnMoleculeFreed?.Invoke();
            _interactable.enabled = false;
        }

        public void OnVRDrop()
        {
            gameObject.transform.SetParent(_prePickupParent);
        }
    }
}