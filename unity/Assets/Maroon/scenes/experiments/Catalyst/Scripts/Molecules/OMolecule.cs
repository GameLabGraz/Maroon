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

        /**
         * Set whether this atom can be drawn (only used in van Krevelen variant for
         * O atoms sit on the surface.
         */
        public void SetCanBeDrawn(bool canBeDrawn)
        {
            _canBeDrawn = canBeDrawn;
        }

        protected override void Start()
        {
            base.Start();
            _collider = GetComponent<SphereCollider>();
        }

        /**
         * Override base method to handle increasing of collider in Langmuir method if state is fixed.
         * Also handles potential drawing to other molecules if potentialDrawMolecules list contains
         * suitable molecules.
         * Calls the base method to keep molecule moving if state not fixed.
         */
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
                       CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.OReactCO_CO2Desorb ))
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
                      CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.OReactCO_CO2Desorb )
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
        
        /**
         * Use the collider trigger to possibly get drawn to a CO molecule that is fixed and connected to
         * a surface molecule (either platinum or cobalt).
         * We put molecules that enter the collider into a list since we can not be sure that these
         * molecules are in a fixed state and connected to a surface atom when they enter. We use that list
         * to periodically check if we find a molecule this O atom can be drawn to.
         * <param name="other"> Collider of object entering this collider. </param>
         */
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
        
        /**
         * Handle removal from potential CO molecules that this O atom could have been drawn to from the
         * potential drawing list.
         * * <param name="other"> Collider of object leaving this collider. </param>
         */
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