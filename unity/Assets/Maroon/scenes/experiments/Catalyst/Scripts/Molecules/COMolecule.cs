using System.Collections;
using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts.Molecules
{
    public class COMolecule : Molecule
    {
        [SerializeField] float timeUntilNextDesorb = 3.0f;
        
        private int _moleculeClickCounter = 0;
        private float _wobbleStrength = 0.1f;
        private bool _isWobbling = false;
        
        private float _currentTimeDesorb = 0.0f;

        public void OnMouseDown()
        {
            if (State != MoleculeState.Fixed || !SimulationController.Instance.SimulationRunning) return;
            if (_isWobbling) return;
            if (_moleculeClickCounter == 3)
            {
                DesorbCO();
                OnMoleculeFreed?.Invoke();
                return;
            }

            StartCoroutine(Wobble());
            _moleculeClickCounter++;
        }

        protected override void HandleFixedUpdate()
        {
            if (ReactionStarted && State == MoleculeState.Fixed && State != MoleculeState.Desorb &&
                ConnectedMolecule != null && ConnectedMolecule.Type == MoleculeType.Pt &&
                ( !CatalystController.DoStepWiseSimulation ||
                 CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.CODesorb )
                )
            {
                _currentTimeDesorb += Time.deltaTime;
                if (timeUntilNextDesorb <= _currentTimeDesorb)
                {
                    if (Random.Range(0, 100) > 100 - CurrentTurnOverRate)
                    {
                        DesorbCO();
                    }
                    _currentTimeDesorb = 0.0f;
                }
                return;
            }
            
            base.HandleFixedUpdate();
            
            // can only happen to O2, CO, or O
            if (State == MoleculeState.InDrawingCollider && PossibleDrawingMolecule != null &&
                ( !CatalystController.DoStepWiseSimulation ||
                  CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.COAdsorb ))
            {
                HandleDrawingPossibility();
            }
        }

        protected override void HandleUpdate()
        {
            if (_isWobbling)
            {
                Vector3 newPos = transform.position + Random.insideUnitSphere * (Time.deltaTime * _wobbleStrength);
                newPos.z = transform.position.z;
                transform.position = newPos;
            }
        }

        protected override void HandleDrawingPossibility()
        {
            if (Random.Range(0, 100) > 100 - CurrentTurnOverRate)
            {
                PossibleDrawingMolecule.ConnectedMolecule = this; // connect this (CO) to plat or cobalt molecule
                SetMoleculeDrawn(PossibleDrawingMolecule, MoleculeState.DrawnBySurfaceMolecule); // drawn by plat or cobalt
                ConnectedMolecule.ActivateDrawingCollider(false); // deactivate plat or cobalt drawing collider
            }
        }

        private void DesorbCO()
        {
            StartMoleculePosition = transform.position;
            StartMoleculeRotation = transform.rotation;
            NewMoleculePosition = new Vector3(StartMoleculePosition.x, StartMoleculePosition.y + 1.5f, StartMoleculePosition.z);
            CurrentTimeMove = 0.0f;
            State = MoleculeState.Desorb;
            ConnectedMolecule.ConnectedMolecule = null;
            ConnectedMolecule = null;
        }

        private IEnumerator Wobble()
        {
            Vector3 currentPosition = transform.position;
            if (!_isWobbling)
            {
                _isWobbling = true;
            }
            yield return new WaitForSeconds(0.3f);
            _isWobbling = false;
            transform.position = currentPosition;
        }
    }
}