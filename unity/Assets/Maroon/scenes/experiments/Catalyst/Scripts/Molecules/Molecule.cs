using System;
using System.Collections;
using Maroon.Physics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maroon.scenes.experiments.Catalyst.Scripts.Molecules
{
    public enum MoleculeType
    {
        CO,
        CO2,
        O,
        O2,
        Pt,
        Co
    }

    public enum MoleculeState
    {
        Fixed,
        Moving,
        Desorb,
        InDrawingCollider,
        DrawnByPlat,
        DrawnByCO,
        Disappear
    }

    public class Molecule : PausableObject//, IMoleculeInterface
    {
        [Header("Molecule Specifics")]
        [SerializeField] MoleculeType type;
        [SerializeField] Collider collider;

        [Header("Molecule Movement")]
        [SerializeField] float movementSpeed = 1;
        [SerializeField] float timeToMove = 3.0f;

        [SerializeField] QuantityFloat temperature = new QuantityFloat();
        [SerializeField] QuantityFloat partialPressure = new QuantityFloat();

        [SerializeField] private MoleculeState _state;
        
        protected float _currentTimeMove = 0.0f;
        protected Vector3 _startMoleculePosition;
        protected Vector3 _newMoleculePosition;
        protected Quaternion _startMoleculeRotation;
        protected Quaternion _newMoleculeRotation;
        private Molecule _possibleDrawingMolecule; // should always be a platinum molecule
        private Molecule _connectedMolecule;
        
        private float _currenTimeDrawn = 0.0f;
        
        protected bool ReactionStarted = false;

        protected float CurrentTurnOverRate = 0.0f;

        public MoleculeType Type { get => type; }

        public MoleculeState State
        {
            get => _state;
            set {
                if (value == MoleculeState.Fixed)
                {
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                } 
                else if (_state == MoleculeState.Fixed)
                {
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                }
                _state = value;
            }
        }

        public Molecule ConnectedMolecule { get => _connectedMolecule; set => _connectedMolecule = value; }
        public Molecule PossibleDrawingMolecule { get => _possibleDrawingMolecule; set => _possibleDrawingMolecule = value; }
        
        public bool IsDrawnByPlat { get; set; }
        public bool IsDrawnByCO { get; set; }

        public Action<Molecule> OnDissociate;
        public Action<Molecule, Molecule> OnCO2Created;
        public Action OnMoleculeFreed;

        public void SetMoleculeDrawn(Molecule drawingMolecule, MoleculeState state)
        {
            State = state;
            _startMoleculePosition = transform.position;
            _startMoleculeRotation = transform.rotation;
            _newMoleculePosition = drawingMolecule.transform.position;
            _newMoleculeRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            _connectedMolecule = drawingMolecule;
        }

        public void MoveOutCO2()
        {
            if (type != MoleculeType.CO2) return; // in case this is called on non co2 molecules somehow
            _startMoleculePosition = transform.position;
            _newMoleculePosition = new Vector3(_startMoleculePosition.x, _startMoleculePosition.y + 2.0f, _startMoleculePosition.z);
            _currentTimeMove = 0.0f;
            State = MoleculeState.Disappear;
            _connectedMolecule = null;
            timeToMove = 4.0f;
        }
        
        public void TemperatureChanged(float newTemp)
        {
            // normal temp goes from -23.15f to 176.85 degree celsius
            // scale this between 0 - 1 for movement speed by adding 23.15 to current and max value before dividing
            temperature.Value = newTemp;
            movementSpeed = Mathf.Clamp((temperature.Value + 23.15f) / (temperature.maxValue + 23.15f), 0.1f, 1.0f); // only temperature influences movement speed
            CurrentTurnOverRate = CatalystController.TurnOverRates[CatalystController.GetTemperatureIndex(temperature.Value)][CatalystController.GetPartialPressureIndex(partialPressure.Value)];
        }

        public void PressureChanged(float pressure)
        {
            partialPressure.Value = pressure;
            CurrentTurnOverRate = CatalystController.TurnOverRates[CatalystController.GetTemperatureIndex(temperature.Value)][CatalystController.GetPartialPressureIndex(partialPressure.Value)];
        }

        public void ReactionStart()
        {
            ReactionStarted = true;
            ReactionStart_Impl();
        }

        public void ActivateDrawingCollider(bool activate)
        {
            if (type != MoleculeType.Pt && type != MoleculeType.O) return;

            collider.enabled = activate;
        }

        protected override void Start()
        {
            base.Start();
            GetRandomPositionAndRotation();
        }

        protected virtual void ReactionStart_Impl()
        {
            
        }

        protected override void HandleUpdate()
        {
            
        }

        protected override void HandleFixedUpdate()
        {
            if (State == MoleculeState.Fixed && _connectedMolecule == null && State != MoleculeState.DrawnByCO) return;

            if (_state != MoleculeState.Fixed)
            {
                HandleMoleculeMovement();
            }


        }

        private void HandleMoleculeMovement()
        {
            _currentTimeMove += Time.deltaTime * movementSpeed;
            if (Vector3.Distance(transform.position, _newMoleculePosition) > 0.05f)
            {
                transform.position = Vector3.Lerp(_startMoleculePosition, _newMoleculePosition, _currentTimeMove);
                transform.rotation = Quaternion.Lerp(_startMoleculeRotation, _newMoleculeRotation, _currentTimeMove);
            }
            else
            {
                if (_state == MoleculeState.Desorb)
                {
                    //_currentTimeDesorb = 0.0f;
                    State = MoleculeState.Moving;
                }
                else if (_state == MoleculeState.Disappear)
                    Destroy(this.gameObject);
                else if (State == MoleculeState.DrawnByPlat)
                    HandleMoleculeTouchingPlat();
                else if (State == MoleculeState.DrawnByCO)
                    HandleOTouchingCO();

                if (_state == MoleculeState.Moving)
                {
                    _currentTimeMove = 0.0f;
                    GetRandomPositionAndRotation();
                }
            }
        }

        private IEnumerator DissociateO2()
        {
            yield return new WaitForSeconds(2.0f);
            _connectedMolecule.ActivateDrawingCollider(true);
            OnDissociate?.Invoke(this);
        }

        private void HandleMoleculeTouchingPlat()
        {
            if (type != MoleculeType.CO && type != MoleculeType.O2) return;
            State = MoleculeState.Fixed;
            transform.position = new Vector3(_newMoleculePosition.x, _newMoleculePosition.y += CatalystController.FixedMoleculeYDist, _newMoleculePosition.z);
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            if (type == MoleculeType.O2)
                StartCoroutine(DissociateO2());
        }

        private void HandleOTouchingCO()
        {
            State = MoleculeState.Fixed;
            OnCO2Created?.Invoke(this, _connectedMolecule);
        }

        protected virtual void HandleDrawingPossibility()
        {
            // only used in CO and O2 molecules
            // note that the prob of O2 being drawn is slightly less than the prob of CO
        }

        private void GetRandomPositionAndRotation()
        {
            _startMoleculePosition = transform.position;
            _startMoleculeRotation = transform.rotation;
            _newMoleculePosition = _startMoleculePosition + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0.1f, -0.2f), Random.Range(-0.2f, 0.2f));
            _newMoleculeRotation = Quaternion.Euler(Random.Range(-180.0f, 180.0f),Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
        }

    }
}