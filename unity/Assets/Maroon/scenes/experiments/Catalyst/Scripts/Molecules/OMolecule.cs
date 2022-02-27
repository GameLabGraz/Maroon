using System;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts.Molecules
{
    public class OMolecule : Molecule
    {
        public List<Molecule> potentialDrawMolecules = new List<Molecule>();

        public Action<Molecule, Molecule> CreateO2;
        public DateTime CreationTimeStamp = DateTime.Now;

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
                // handle general movement and drawing to CO molecule or surface drawing spot
                base.HandleFixedUpdate();
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
        
        /**
         * Create O2 if two O atoms collide.
         * To now create two O2 atoms with the action call we use DateTime.Ticks as it is very accurate
         * (single tick represents one hundred nanoseconds). It can still happen that two atoms have the
         * same value and do not create O2.
         */
        private void OnCollisionEnter(Collision other)
        {
            if (State == MoleculeState.Moving)
            {
                OMolecule otherMolecule = other.gameObject.GetComponent<Molecule>() as OMolecule;
                if (otherMolecule != null)
                {
                    // make sure only one O2 is created, just take timestamps and the molecule that is earlier spawns it
                    if (CreationTimeStamp.Ticks < otherMolecule.CreationTimeStamp.Ticks)
                        CreateO2?.Invoke(this, otherMolecule);
                }
            }
        }
    }
}