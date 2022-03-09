using UnityEngine;

namespace Maroon.Chemistry.Catalyst
{
    /**
     * This class is used to draw in new O atom into the surface in the van Krevelen
     * variant. In addition to each original O surface atom a ODrawingSpot is spawned
     * and connected to that O atom.
     * When the O atom eventually leaves to react with a CO molecule this spot is empty
     * and uses it's trigger collider to draw in O atoms.
     */
    public class ODrawingSpot : Molecule
    {

        private Molecule _attachedMolecule;
        private float _timeToCheckForOMolecules = 2.0f;
        private float _currentTimeToCheckForOMolecules = 0.0f;

        public void SetAttachedMolecule(Molecule oMolecule)
        {
            _attachedMolecule = oMolecule;
            oMolecule.ConnectedMolecule = this;
        }

        protected override void HandleUpdate()
        {
            
        }

        /**
         * Override base method to handle finding a new O atom to attach to this drawing point.
         * Periodically check if there are any O atoms in a sphere around this drawing spot.
         */
        protected override void HandleFixedUpdate()
        {
            if (_attachedMolecule == null)
            {
                if (!CatalystController.DoStepWiseSimulation ||
                    (CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.OFillSurface))
                {
                    _currentTimeToCheckForOMolecules += Time.deltaTime;
                    if (_currentTimeToCheckForOMolecules >= _timeToCheckForOMolecules)
                    {
                        Collider[] colliders = UnityEngine.Physics.OverlapSphere(transform.position, 0.2f);
                        foreach (var otherCollider in colliders)
                        {
                            Molecule otherMolecule = otherCollider.gameObject.GetComponent<Molecule>();
                            if (otherMolecule != null && otherMolecule.Type == MoleculeType.O &&
                                otherMolecule.ConnectedMolecule == null && otherMolecule.State == MoleculeState.Fixed)
                            {
                                otherMolecule.SetMoleculeDrawn(this, MoleculeState.DrawnByDrawingSpot);
                                SetAttachedMolecule(otherMolecule);
                                break;
                            }
                        }
                    
                        _currentTimeToCheckForOMolecules = 0.0f;
                    }
                }
            }
        }

    }
}