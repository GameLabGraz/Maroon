using System.Collections;
using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts.Molecules
{
    public class O2Molecule : Molecule
    {
        protected override void HandleFixedUpdate()
        {
            if (State == MoleculeState.WaitingToDissociate)
            {
                if (!CatalystController.DoStepWiseSimulation ||
                    CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.O2Dissociate)
                {
                    StartCoroutine(DissociateO2());
                }
            }
            else
                base.HandleFixedUpdate();

            // can only happen to O2, CO, or O
            if (State == MoleculeState.InDrawingCollider && PossibleDrawingMolecule != null &&
                ( !CatalystController.DoStepWiseSimulation ||
                  CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.O2Adsorb)
                )
            {
                HandleDrawingPossibility();
            }
        }
        
        protected override void HandleDrawingPossibility()
        {
            if (Random.Range(0, 100) > 100 - CurrentTurnOverRate)
            {
                PossibleDrawingMolecule.ConnectedMolecule = this; // connect this (O2) to plat molecule
                SetMoleculeDrawn(PossibleDrawingMolecule, MoleculeState.DrawnBySurfaceMolecule); // drawn by plat
                ConnectedMolecule.ActivateDrawingCollider(false); // deactivate plat drawing collider
            }
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