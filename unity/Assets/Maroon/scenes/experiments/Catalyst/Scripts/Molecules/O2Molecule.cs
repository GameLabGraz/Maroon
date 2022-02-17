using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts.Molecules
{
    public class O2Molecule : Molecule
    {
        protected override void HandleFixedUpdate()
        {
            base.HandleFixedUpdate();
            
            // can only happen to O2 or CO
            if (State == MoleculeState.InDrawingCollider && PossibleDrawingMolecule != null)
            {
                HandleDrawingPossibility();
            }
        }
        
        protected override void HandleDrawingPossibility()
        {
            if (Random.Range(0, 100) > 100 - CurrentTurnOverRate)
            {
                PossibleDrawingMolecule.ConnectedMolecule = this; // connect this (O2) to plat molecule
                SetMoleculeDrawn(PossibleDrawingMolecule, MoleculeState.DrawnByPlat); // drawn by plat
                ConnectedMolecule.ActivateDrawingCollider(false); // deactivate plat drawing collider
            }
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