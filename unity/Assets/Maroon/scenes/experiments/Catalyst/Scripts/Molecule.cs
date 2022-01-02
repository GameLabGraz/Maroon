using System;
using System.Collections;
using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maroon.scenes.experiments.Catalyst.Scripts
{
    public enum MoleculeType
    {
        CO,
        CO2,
        O,
        O2,
        Pt
    }

    public enum MoleculeState
    {
        Fixed,
        Moving,
        Desorb,
        DrawnByPlat,
        DrawnByCO,
        Disappear
    }

    public class Molecule : PausableObject
    {
        [Header("Molecule Specifics")]
        [SerializeField] MoleculeType type;
        [SerializeField] List<MoleculeType> canConnectToList = new List<MoleculeType>();
        [SerializeField] Collider collider;

        [Header("Molecule Movement")]
        [SerializeField] float movementSpeed = 1;
        [SerializeField] float timeToMove = 3.0f;
        [SerializeField] float timeUntilNextDesorb = 3.0f;

        [SerializeField] QuantityFloat temperature = new QuantityFloat();
        private QuantityFloat _partialPressure = new QuantityFloat();

        private MoleculeState _state;
        
        private float _currentTimeMove = 0.0f;
        private float _currentTimeDesorb = 0.0f;
        private Vector3 _newMoleculePosition;
        private Quaternion _newMoleculeRotation;
        public Molecule _connectedMolecule;
        
        private float _currenTimeDrawn = 0.0f;

        private int _moleculeClickCounter = 0;
        private float _wobbleStrength = 0.1f;
        private bool _isWobbling = false;
        private bool _reactionStarted = false;

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
        
        public bool IsDrawnByPlat { get; set; }
        public bool IsDrawnByCO { get; set; }

        public Action<Molecule> OnDissociate;
        public Action<Molecule, Molecule> OnCO2Created;
        public Action OnMoleculeFreed;

        public void OnMouseDown()
        {
            if (type != MoleculeType.CO || State != MoleculeState.Fixed || !SimulationController.Instance.SimulationRunning) return;
            if (_moleculeClickCounter == 3 && SimulationController.Instance.SimulationRunning)
            {
                DesorbCO();
                OnMoleculeFreed?.Invoke();
                return;
            }

            StartCoroutine(Wobble());
            _moleculeClickCounter++;
        }

        public void SetMoleculeDrawn(Molecule drawingMolecule, MoleculeState state)
        {
            State = state;
            _newMoleculePosition = drawingMolecule.transform.position;
            _newMoleculeRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            _connectedMolecule = drawingMolecule;
        }

        public void MoveOutCO2()
        {
            if (type != MoleculeType.CO2) return; // in case this is called on non co2 molecules somehow
            
            _newMoleculePosition = new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z);
            _currentTimeMove = 0.0f;
            State = MoleculeState.Disappear;
            _connectedMolecule = null;
            timeToMove = 4.0f;
        }
        
        public void TemperatureChanged(float temp)
        {
            // normal temp goes from -23.15f to 76.85 degree celsius, since we divide here we define
            // the temp of molecules to go from 0 - 100 hence we add 23.15 here
            temperature.Value = temp + 23.15f;
            movementSpeed = temperature.Value / temperature.maxValue;
        }

        public void PressureChanged(float pressure)
        {
            _partialPressure = pressure;
        }

        public void ReactionStart()
        {
            _reactionStarted = true;
            if (type == MoleculeType.Pt)
            {
                GetComponent<CapsuleCollider>().enabled = true;
            }
        }
        
        protected override void Start()
        {
            base.Start();
            GetRandomPositionAndRotation();
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

        protected override void HandleFixedUpdate()
        {
            if (State == MoleculeState.Fixed && _connectedMolecule == null && State != MoleculeState.DrawnByCO) return;
            if (State == MoleculeState.Fixed && State != MoleculeState.Desorb &&
               _connectedMolecule != null && _connectedMolecule.Type == MoleculeType.Pt)
            {
                if (Type == MoleculeType.CO && _connectedMolecule.Type == MoleculeType.Pt && _reactionStarted)
                {
                    _currentTimeDesorb += Time.deltaTime;
                    if (timeUntilNextDesorb <= _currentTimeDesorb)
                    {
                        if (Random.Range(0, 100) > 95)
                        {
                            DesorbCO();
                        }
                        _currentTimeDesorb = 0.0f;
                    }
                }
                return;
            }
            
            if (_state != MoleculeState.Fixed &&
                (_state == MoleculeState.Moving ||
                 _state == MoleculeState.Desorb ||
                 _state == MoleculeState.Disappear ||
                 _state == MoleculeState.DrawnByPlat ||
                 _state == MoleculeState.DrawnByCO) )
            {
                HandleMoleculeMovement();
            }
        }

        private void HandleMoleculeMovement()
        {
            _currentTimeMove += Time.deltaTime;
            if (timeToMove >= _currentTimeMove)
            {
                Vector3 currentPosition = transform.position;
                Quaternion currentRotation = transform.rotation;
                transform.position = Vector3.Lerp(currentPosition, _newMoleculePosition, Time.deltaTime * movementSpeed);
                transform.rotation = Quaternion.Lerp(currentRotation, _newMoleculeRotation, Time.deltaTime * movementSpeed);
            }
            else
            {
                if (_state == MoleculeState.Desorb)
                {
                    _currentTimeDesorb = 0.0f;
                    State = MoleculeState.Moving;
                }
                else if (_state == MoleculeState.Disappear)
                    Destroy(this.gameObject);
                else if (State == MoleculeState.DrawnByPlat)
                    HandleO2TouchingPlat();
                else if (State == MoleculeState.DrawnByCO)
                    HandleOTouchingCO();

                if (_state == MoleculeState.Moving)
                {
                    _currentTimeMove = 0.0f;
                    GetRandomPositionAndRotation();
                }
            }
        }

        private void DesorbCO()
        {
            Vector3 pos = transform.position;
            _newMoleculePosition = new Vector3(pos.x, pos.y + 0.8f, pos.z);
            _currentTimeMove = 0.0f;
            State = MoleculeState.Desorb;
            _connectedMolecule.ConnectedMolecule = null;
            _connectedMolecule = null;
        }

        private IEnumerator DissociateO2()
        {
            yield return new WaitForSeconds(2.0f);
            _connectedMolecule.ActivateDrawingCollider(true);
            OnDissociate?.Invoke(this);
        }

        private void HandleO2TouchingPlat()
        {
            State = MoleculeState.Fixed;
            transform.position = new Vector3(_newMoleculePosition.x, _newMoleculePosition.y += CatalystController.FixedMoleculeYDist, _newMoleculePosition.z);
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            // todo perform radial check here and only dissociate of 2 co molecules are nearby
            StartCoroutine(DissociateO2());
        }

        private void HandleOTouchingCO()
        {
            State = MoleculeState.Fixed;
            OnCO2Created?.Invoke(this, _connectedMolecule);
        }

        private void GetRandomPositionAndRotation()
        {
            _newMoleculePosition = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0.1f, -0.2f), Random.Range(-0.2f, 0.2f));
            _newMoleculeRotation = Quaternion.Euler(Random.Range(-180.0f, 180.0f),Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
        }

        public void ActivateDrawingCollider(bool activate)
        {
            if (type != MoleculeType.Pt && type != MoleculeType.O) return;

            collider.enabled = activate;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (State == MoleculeState.DrawnByPlat || State == MoleculeState.DrawnByCO) return;
            if (type == MoleculeType.Pt && _connectedMolecule == null)
            {
                Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
                if (otherMolecule != null && otherMolecule.Type == MoleculeType.O2 && otherMolecule.ConnectedMolecule == null)
                {
                    _connectedMolecule = otherMolecule;
                    otherMolecule.SetMoleculeDrawn(this, MoleculeState.DrawnByPlat);
                    ActivateDrawingCollider(false);
                }
            }
            else if (type == MoleculeType.O && _connectedMolecule == null)
            {
                Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
                if (otherMolecule != null && otherMolecule.Type == MoleculeType.CO && // todo find cause of possible nullref exception
                    otherMolecule.State == MoleculeState.Fixed && otherMolecule.ConnectedMolecule.Type == MoleculeType.Pt)
                {
                    SetMoleculeDrawn(otherMolecule, MoleculeState.DrawnByCO);
                    otherMolecule.ConnectedMolecule = this;
                    ActivateDrawingCollider(false);
                    otherMolecule.ActivateDrawingCollider(false);
                }
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
            if (otherMolecule == null) return;
            if (type == MoleculeType.O2 
                && _connectedMolecule != null
                && other.gameObject.GetComponent<Molecule>().Type == MoleculeType.Pt
                && _connectedMolecule.Type == MoleculeType.Pt)
            {
                HandleO2TouchingPlat();
            }
            if (type == MoleculeType.O 
                && _connectedMolecule != null
                && other.gameObject.GetComponent<Molecule>().Type == MoleculeType.CO
                && _connectedMolecule.Type == MoleculeType.Pt)
            {
                HandleOTouchingCO();
            }
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