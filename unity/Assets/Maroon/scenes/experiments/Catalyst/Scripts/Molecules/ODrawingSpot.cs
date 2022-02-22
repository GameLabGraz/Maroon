using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts.Molecules
{
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