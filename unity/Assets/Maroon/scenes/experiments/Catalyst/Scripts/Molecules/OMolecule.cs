using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts.Molecules
{
    public class OMolecule : Molecule
    {
        private SphereCollider _collider;
        private float _timeToIncreaseCollider = 2.0f;
        private float _currentTimeColliderIncrease = 0.0f;

        private bool _canBeDrawn = false;

        public void SetCanBeDrawn(bool canBeDrawn)
        {
            _canBeDrawn = canBeDrawn;
        }

        protected override void Start()
        {
            base.Start();
            _collider = GetComponent<SphereCollider>();
        }

        protected override void HandleFixedUpdate()
        {
            if (State != MoleculeState.Fixed)
            {
                // handle drawing to CO molecule
                base.HandleFixedUpdate();
            }
            else if (State == MoleculeState.Fixed && 
                     CatalystController.ExperimentVariation == ExperimentVariation.LangmuirHinshelwood &&
                     ( !CatalystController.DoStepWiseSimulation ||
                       CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.OReactCO ))
            {
                // gradually increase drawing collider if spawned on plat to find a fixed CO molecule
                _currentTimeColliderIncrease += Time.deltaTime;
                if (_currentTimeColliderIncrease >= _timeToIncreaseCollider && _collider.radius <= 4)
                {
                    _collider.radius *= 2;
                    _currentTimeColliderIncrease = 0.0f;
                }
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (State == MoleculeState.DrawnBySurfaceMolecule || State == MoleculeState.DrawnByCO) return;
            if (ConnectedMolecule == null) // draw O atoms to nearby CO molecules
            {
                Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
                if (otherMolecule != null && otherMolecule.Type == MoleculeType.CO &&
                    otherMolecule.State == MoleculeState.Fixed &&
                    otherMolecule.ConnectedMolecule != null && otherMolecule.ConnectedMolecule.Type == MoleculeType.Pt &&
                    ( !CatalystController.DoStepWiseSimulation ||
                      CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.OReactCO )
                    )
                {
                    SetMoleculeDrawn(otherMolecule, MoleculeState.DrawnByCO);
                    otherMolecule.ConnectedMolecule = this;
                    ActivateDrawingCollider(false);
                    otherMolecule.ActivateDrawingCollider(false);
                }
            }
        }
    }
}