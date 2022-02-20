﻿using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts.Molecules
{
    public class PlatinMolecule : Molecule
    {
        protected override void ReactionStart_Impl()
        {
            if (IsTopLayerSurfaceMolecule)
                ActivateDrawingCollider(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ConnectedMolecule == null) // let O2 or CO molecules know they can be drawn
            {
                Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
                if (otherMolecule != null && 
                    (otherMolecule.Type == MoleculeType.O2 || otherMolecule.Type == MoleculeType.CO) &&
                    otherMolecule.ConnectedMolecule == null)
                {
                    otherMolecule.State = MoleculeState.InDrawingCollider;
                    otherMolecule.PossibleDrawingMolecule = this;
                }
            }
        }

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