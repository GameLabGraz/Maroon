using System;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts.Molecules
{
    public class OMolecule : Molecule
    {
        private SphereCollider _collider;
        private float _timeToIncreaseCollider = 2.0f;
        private float _currentTimeColliderIncrease = 0.0f;

        private bool _canBeDrawn = false;

        public List<Molecule> potentialDrawMolecules = new List<Molecule>();

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
                // O can move across whole surface to react with CO in the Langmuir Hinshelwood variant
                // so gradually increase drawing collider if spawned on plat to find a fixed CO molecule
                _currentTimeColliderIncrease += Time.deltaTime;
                if (_currentTimeColliderIncrease >= _timeToIncreaseCollider && _collider.radius <= 4)
                {
                    _collider.radius *= 2;
                    _currentTimeColliderIncrease = 0.0f;
                }
            }

            bool clearList = false;
            foreach (var potentialDrawMolecule in potentialDrawMolecules)
            {
                if (potentialDrawMolecule != null && potentialDrawMolecule.Type == MoleculeType.CO &&
                    potentialDrawMolecule.State == MoleculeState.Fixed &&
                    potentialDrawMolecule.ConnectedMolecule != null &&
                    (potentialDrawMolecule.ConnectedMolecule.Type == MoleculeType.Pt ||
                     potentialDrawMolecule.ConnectedMolecule.Type == MoleculeType.Co ) &&
                    ( !CatalystController.DoStepWiseSimulation ||
                      CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.OReactCO )
                )
                {
                    SetMoleculeDrawn(potentialDrawMolecule, MoleculeState.DrawnByCO);
                    potentialDrawMolecule.ConnectedMolecule = this;
                    ActivateDrawingCollider(false);
                    potentialDrawMolecule.ActivateDrawingCollider(false);
                    clearList = true;
                    break;
                }
            }
            if (clearList)
                potentialDrawMolecules.Clear();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (State == MoleculeState.DrawnBySurfaceMolecule || State == MoleculeState.DrawnByCO) return;
            if (ConnectedMolecule == null || State == MoleculeState.InSurfaceDrawingSpot) // draw O atoms to nearby CO molecules
            {
                Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
                if (otherMolecule != null && otherMolecule.Type == MoleculeType.CO)
                {
                    // O can only react with CO in the near vicinity in the Mars van Krevelen variant
                    // since CO will not be fixed yet when the surface O is spawned, OnTriggerEnter will most likely
                    // not work reliably  as is does for the Langmuir variant, therefore put all potential CO candidates
                    // in a list and check each FixedUpdate if the state of the CO state is fixed
                    if (!potentialDrawMolecules.Contains(otherMolecule))
                        potentialDrawMolecules.Add(otherMolecule);
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (State == MoleculeState.DrawnBySurfaceMolecule || State == MoleculeState.DrawnByCO) return;
            if (ConnectedMolecule == null || State == MoleculeState.InSurfaceDrawingSpot)
            {
                Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
                if (otherMolecule != null && otherMolecule.Type == MoleculeType.CO)
                {
                    // if CO exits collider remove from list
                    if (potentialDrawMolecules.Contains(otherMolecule))
                        potentialDrawMolecules.Remove(otherMolecule);
                }
            }
        }
    }
}