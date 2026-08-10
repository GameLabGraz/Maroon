using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.Chemistry.Catalyst
{
    public class COMolecule : Molecule
    {
        [SerializeField] float timeUntilNextDesorb = 3.0f;
        
        public List<Molecule> potentialDrawMolecules = new List<Molecule>();
        
        private int _moleculeClickCounter = 0;
        private float _wobbleStrength = 0.1f;
        private bool _isWobbling = false;
        
        private float _currentTimeDesorb = 0.0f;

        private bool _desorbActivatesConnectedMoleculeCollider = false;

        protected override void Start()
        {
            base.Start();
            ActivateDrawingCollider(CatalystController.ExperimentVariation == CatalystVariation.EleyRideal);
        }
        
        protected override void ReactionStart_Impl()
        {
            _desorbActivatesConnectedMoleculeCollider = true;
        }
        
        /**
         * Only needed for the Langmuir variant.
         * If CO molecule is fixed and stuck in surface make it wobble a bit.
         * Fourth time clicking it will free it and call the OnMoleculeFreed action which
         * increments a counter in the CatalystController which in turn starts the reaction
         * if 4 CO molecules have been removed manually.
         */
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

        /**
         * Override base method to handle desorb and drawing possibility.
         * Also calls the base method to keep molecule moving.
         */
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
            
            bool clearList = false;
            foreach (var potentialDrawMolecule in potentialDrawMolecules)
            {
                if (potentialDrawMolecule != null && potentialDrawMolecule.Type == MoleculeType.O &&
                    potentialDrawMolecule.State == MoleculeState.Moving &&
                    potentialDrawMolecule.ConnectedMolecule == null &&
                    ( !CatalystController.DoStepWiseSimulation ||
                      CatalystController.DoStepWiseSimulation && CatalystController.CurrentExperimentStage == ExperimentStages.OReactCO_CO2Desorb ))
                {
                    potentialDrawMolecule.SetMoleculeDrawn(this, MoleculeState.DrawnByCO);

                    potentialDrawMolecule.ConnectedMolecule = this;
                    ActivateDrawingCollider(false);
                    potentialDrawMolecule.ActivateDrawingCollider(false);
                    clearList = true;
                    break;
                }
            }
            if (clearList)
                potentialDrawMolecules.Clear();
            
            
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

        /**
         * Handle possibly being drawn to a surface molecule.
         * Possibility is based on current turn over rate / frequency.
         * If drawn sets connected molecule, draws molecule, and deactivates drawing collider.
         */
        protected override void HandleDrawingPossibility()
        {
            if (Random.Range(0, 100) > 100 - CurrentTurnOverRate  && PossibleDrawingMolecule.ConnectedMolecule == null)
            {
                PossibleDrawingMolecule.ConnectedMolecule = this; // connect this (CO) to plat or cobalt molecule
                SetMoleculeDrawn(PossibleDrawingMolecule, MoleculeState.DrawnBySurfaceMolecule); // drawn by plat or cobalt
                ConnectedMolecule.ActivateDrawingCollider(false); // deactivate plat or cobalt drawing collider
            }
        }

        /**
         * Handle desorb movement of molecule by setting new position and rotation.
         * Resets connected molecules.
         */
        public void DesorbCO()
        {
            StartMoleculePosition = transform.position;
            StartMoleculeRotation = transform.rotation;
            NewMoleculePosition = new Vector3(StartMoleculePosition.x, StartMoleculePosition.y + 1.5f, StartMoleculePosition.z);
            CurrentTimeMove = 0.0f;
            State = MoleculeState.Desorb;
            ConnectedMolecule.ConnectedMolecule = null;
            if (_desorbActivatesConnectedMoleculeCollider)
                ConnectedMolecule.ActivateDrawingCollider(true);
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
        
        /**
         * Use the collider trigger to possibly drawn in an O molecule that is fixed and connected to
         * a surface molecule (platinum).
         * Is only used in the Eley-Rideal variant (trigger collider not active otherwise).
         * <param name="other"> Collider of object entering this collider. </param>
         */
        private void OnTriggerEnter(Collider other)
        {
            if (State != MoleculeState.Moving) return;
            if (ConnectedMolecule == null) // draw O atoms to nearby CO molecules
            {
                Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
                if (otherMolecule != null && otherMolecule.Type == MoleculeType.O && otherMolecule.State == MoleculeState.Moving &&
                    otherMolecule.ConnectedMolecule == null)
                {
                    if (!potentialDrawMolecules.Contains(otherMolecule))
                        potentialDrawMolecules.Add(otherMolecule);
                }
            }
        }
        
        /**
         * Handle removal from potential O molecules that this C atom could have drawn in from the
         * potential drawing list.
         * Is only used in the Eley-Rideal variant (trigger collider not active otherwise).
         * * <param name="other"> Collider of object leaving this collider. </param>
         */
        private void OnTriggerExit(Collider other)
        {
            if (State != MoleculeState.Moving) return;
            if (ConnectedMolecule == null)
            {
                Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
                if (otherMolecule != null && otherMolecule.Type == MoleculeType.O)
                {
                    // if O2 exits collider remove from list
                    if (potentialDrawMolecules.Contains(otherMolecule))
                        potentialDrawMolecules.Remove(otherMolecule);
                }
            }
        }
    }
}