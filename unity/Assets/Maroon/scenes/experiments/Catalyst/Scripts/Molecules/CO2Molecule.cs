using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts.Molecules
{
    public class CO2Molecule : Molecule
    {
        protected override void HandleFixedUpdate()
        {
            if (State == MoleculeState.Fixed && 
                ( !CatalystController.DoStepWiseSimulation ||
                  CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.CO2Desorb )
                ) MoveOutCO2();
            
            base.HandleFixedUpdate();
        }

        public void MoveOutCO2()
        {
            GetComponent<CapsuleCollider>().enabled = false;
            StartMoleculePosition = gameObject.transform.position;
            StartMoleculeRotation = gameObject.transform.rotation;
            NewMoleculePosition = new Vector3(StartMoleculePosition.x, StartMoleculePosition.y + 1.5f, StartMoleculePosition.z);
            CurrentTimeMove = 0.0f;
            State = MoleculeState.Disappear;
            ConnectedMolecule = null;
        }
    }
}