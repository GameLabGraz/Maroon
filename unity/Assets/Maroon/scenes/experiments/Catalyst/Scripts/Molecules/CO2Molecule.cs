using UnityEngine;

namespace Maroon.Chemistry.Catalyst
{
    public class CO2Molecule : Molecule
    {
        /**
         * Override base method to handle moving out of CO2.
         * Calls base method to move the molecule.
         */
        protected override void HandleFixedUpdate()
        {
            if (State == MoleculeState.Fixed && 
                ( !CatalystController.DoStepWiseSimulation ||
                  CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.OReactCO_CO2Desorb )
                ) MoveOutCO2();
            
            base.HandleFixedUpdate();
        }

        /**
         * Set the new molecule position to go straight up.
         * Molecule will disappear after it reaches its destination.
         */
        public void MoveOutCO2()
        {
            GetComponent<CapsuleCollider>().enabled = false;
            StartMoleculePosition = gameObject.transform.position;
            StartMoleculeRotation = gameObject.transform.rotation;
            NewMoleculePosition = new Vector3(StartMoleculePosition.x, StartMoleculePosition.y + 2.5f, StartMoleculePosition.z);
            CurrentTimeMove = 0.0f;
            State = MoleculeState.Disappear;
            ConnectedMolecule = null;
        }
    }
}