﻿using System.Collections;
using UnityEngine;

namespace Maroon.Chemistry.Catalyst
{
    public class O2Molecule : Molecule
    {
        [SerializeField] Material darkRedMaterial;
        
        protected override void HandleFixedUpdate()
        {
            if (State == MoleculeState.WaitingToDissociate)
            {
                if (!CatalystController.DoStepWiseSimulation ||
                    CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.O2Adsorb_O2Dissociate)
                {
                    StartCoroutine(DissociateO2());
                }
            }
            else if (State == MoleculeState.Fixed && CatalystController.ExperimentVariation == CatalystVariation.EleyRideal)
            {
                // in eley-rideal dissociate based on turn over frequency
                if (Random.Range(0, 100) > 100 - CurrentTurnOverRate)
                {
                    State = MoleculeState.WaitingToDissociate;
                }
            }

            else
                base.HandleFixedUpdate();

            // can only happen to O2, CO, or O
            if (State == MoleculeState.InDrawingCollider && PossibleDrawingMolecule != null &&
                ( !CatalystController.DoStepWiseSimulation ||
                  CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.O2Adsorb_O2Dissociate)
                )
            {
                HandleDrawingPossibility();
            }
        }
        
        protected override void HandleDrawingPossibility()
        {
            if (Random.Range(0, 100) > 100 - CurrentTurnOverRate && PossibleDrawingMolecule.ConnectedMolecule == null)
            {
                PossibleDrawingMolecule.ConnectedMolecule = this; // connect this (O2) to plat molecule
                SetMoleculeDrawn(PossibleDrawingMolecule, MoleculeState.DrawnBySurfaceMolecule); // drawn by plat or cobalt
                ConnectedMolecule.ActivateDrawingCollider(false); // deactivate plat or cobalt drawing collider
            }
        }

        public void SetDarkMaterial()
        {
            gameObject.transform.GetChild(0).transform.GetComponent<MeshRenderer>().material = darkRedMaterial;
            gameObject.transform.GetChild(1).transform.GetComponent<MeshRenderer>().material = darkRedMaterial;
            gameObject.transform.GetChild(2).transform.GetComponent<MeshRenderer>().material = darkRedMaterial;
        }
        
        private IEnumerator DissociateO2()
        {
            yield return new WaitForSeconds(2.0f);
            ConnectedMolecule.ActivateDrawingCollider(true);
            OnDissociate?.Invoke(this);
        }

        // never called since collider of plat is deactivated - also huge performance loss if
        // plat sphere collider is activated
        /*private void OnCollisionEnter(Collision other)
        {
            Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
            if (otherMolecule == null) return;
            if (ConnectedMolecule != null
                && other.gameObject.GetComponent<Molecule>().Type == MoleculeType.Pt
                && ConnectedMolecule.Type == MoleculeType.Pt)
            {
                HandleMoleculeTouchingPlat();
            }
        }*/
    }
}