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
            if (State == MoleculeState.Fixed && State != MoleculeState.Desorb &&
                ConnectedMolecule != null && ConnectedMolecule.Type == MoleculeType.Pt)
            {
                if (Type == MoleculeType.CO && ConnectedMolecule.Type == MoleculeType.Pt && ReactionStarted)
                {
                    _currentTimeDesorb += Time.deltaTime;
                    if (timeUntilNextDesorb <= _currentTimeDesorb)
                    {
                        if (Random.Range(0, 100) > 99 - CurrentTurnOverRate)
                        {
                            DesorbCO();
                        }
                        _currentTimeDesorb = 0.0f;
                    }
                }
                return;
            }
            
            base.HandleFixedUpdate();
            
            // can only happen to O2 or CO
            if (State == MoleculeState.InDrawingCollider && PossibleDrawingMolecule != null)
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
                PossibleDrawingMolecule.ConnectedMolecule = this; // connect this (CO) to plat molecule
                SetMoleculeDrawn(PossibleDrawingMolecule, MoleculeState.DrawnByPlat); // drawn by plat
                ConnectedMolecule.ActivateDrawingCollider(false); // deactivate plat drawing collider
            }
        }

        private void DesorbCO()
        {
            StartMoleculePosition = transform.position;
            StartMoleculeRotation = transform.rotation;
            NewMoleculePosition = new Vector3(StartMoleculePosition.x, StartMoleculePosition.y + 0.8f, StartMoleculePosition.z);
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