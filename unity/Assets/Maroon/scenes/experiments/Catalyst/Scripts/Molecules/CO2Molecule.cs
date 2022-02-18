using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts.Molecules
{
    public class CO2Molecule : Molecule
    {
        public void MoveOutCO2()
        {
            GetComponent<CapsuleCollider>().enabled = false;
            StartMoleculePosition = gameObject.transform.position;
            StartMoleculeRotation = gameObject.transform.rotation;
            NewMoleculePosition = new Vector3(StartMoleculePosition.x, StartMoleculePosition.y + 2.0f, StartMoleculePosition.z);
            CurrentTimeMove = 0.0f;
            State = MoleculeState.Disappear;
            ConnectedMolecule = null;
        }
    }
}