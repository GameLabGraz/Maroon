using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts.Molecules
{
    public class OMolecule : Molecule
    {
        // was never called
        /*private void OnCollisionEnter(Collision other)
        {
            Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
            if (otherMolecule == null) return;

            if (ConnectedMolecule != null
                && other.gameObject.GetComponent<Molecule>().Type == MoleculeType.CO
                && ConnectedMolecule.Type == MoleculeType.Pt)
            {
                HandleOTouchingCO();
            }
        }*/
        
        private void OnTriggerEnter(Collider other)
        {
            if (State == MoleculeState.DrawnByPlat || State == MoleculeState.DrawnByCO) return;
            if (ConnectedMolecule == null) // draw O atoms to nearby CO molecules
            {
                Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
                if (otherMolecule != null && otherMolecule.Type == MoleculeType.CO &&
                    otherMolecule.State == MoleculeState.Fixed && otherMolecule.ConnectedMolecule.Type == MoleculeType.Pt)
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