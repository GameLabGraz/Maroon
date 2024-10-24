﻿using UnityEngine;

namespace Maroon.Chemistry.Catalyst
{
    public class CobaltMolecule : Molecule
    {
        protected override void ReactionStart_Impl()
        {
            ActivateDrawingCollider(true);
        }

        /**
         * Use the collider trigger to possibly draw in O2 or CO molecules.
         * <param name="other"> Collider of object entering this collider. </param>
         */
        private void OnTriggerEnter(Collider other)
        {
            if (ConnectedMolecule == null) // draw in O2 or CO molecules
            {
                Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
                if (otherMolecule != null && 
                    (otherMolecule.Type == MoleculeType.O2 || otherMolecule.Type == MoleculeType.CO) &&
                    otherMolecule.ConnectedMolecule == null && otherMolecule.State == MoleculeState.Moving)
                {
                    otherMolecule.State = MoleculeState.InDrawingCollider;
                    otherMolecule.PossibleDrawingMolecule = this;
                }
            }
        }

        /**
         * Reset possibility to draw in O2 or CO molecule
         * <param name="other"> Collider of object leaving this collider. </param>
         */
        private void OnTriggerExit(Collider other)
        {
            if (ConnectedMolecule == null) // reset drawing state and possible drawn molecule
            {
                Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
                if (otherMolecule != null && 
                    (otherMolecule.Type == MoleculeType.O2 || otherMolecule.Type == MoleculeType.CO) &&
                    otherMolecule.ConnectedMolecule == null)
                {
                    otherMolecule.State = MoleculeState.Moving;
                    otherMolecule.PossibleDrawingMolecule = null;
                }
            }
        }
    }
}