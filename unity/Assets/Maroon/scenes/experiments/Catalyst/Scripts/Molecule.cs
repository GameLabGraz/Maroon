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
        DrawnByPlat,
        DrawnByCO
    }

    public class Molecule : PausableObject
    {
        [Header("Molecule Specifics")]
        [SerializeField] MoleculeType type;
        [SerializeField] List<MoleculeType> canConnectToList = new List<MoleculeType>();
        [SerializeField] Collider collider;

        [Header("Molecule Movement")]
        [SerializeField] float movementSpeed;
        [SerializeField] float timeToMove = 3.0f;
        [SerializeField] float timeUntilNextDesorb = 3.0f;

        private QuantityFloat _temperature;
        private QuantityFloat _partialPressure;

        private MoleculeState _state;
        
        private float _currentTimeMove = 0.0f;
        private float _currentTimeDesorb = 0.0f;
        private Vector3 _newRandomPosition;
        private Quaternion _newRandomRotation;
        public Molecule _connectedMolecule;
        private bool _desorbMoveActive;
        
        private float _currenTimeDrawn = 0.0f;
        private Vector3 _drawingMoleculePosition;

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


        public void SetMoleculeDrawn(Molecule drawingMolecule, MoleculeState state)
        {
            State = state;
            _drawingMoleculePosition = drawingMolecule.transform.position;
            _connectedMolecule = drawingMolecule;
        }

        public void TemperatureChanged(float temp)
        {
            _temperature = temp;
            movementSpeed = _temperature / 20.0f;
        }

        public void PressureChanged(float pressure)
        {
            _partialPressure = pressure;
        }
        
        protected override void Start()
        {
            base.Start();
            GetRandomPositionAndRotation();
        }

        protected override void HandleUpdate()
        {
            
        }

        protected override void HandleFixedUpdate()
        {
            if (State == MoleculeState.Fixed && _connectedMolecule == null && State != MoleculeState.DrawnByCO) return;
            if (State == MoleculeState.Fixed  && _connectedMolecule != null && _connectedMolecule.Type == MoleculeType.Pt && !_desorbMoveActive)
            {
                if (Type == MoleculeType.CO && _connectedMolecule.Type == MoleculeType.Pt)
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
                else if (Type == MoleculeType.O2 && _connectedMolecule.Type == MoleculeType.Pt)
                {
                    // todo handle O2 split? currently done once movement is complete
                }

                return;
            }

            if (_desorbMoveActive)
            {
                HandleCODesorbMovement();
            }
            else if (State == MoleculeState.DrawnByPlat || State == MoleculeState.DrawnByCO)
            {
                HandleDrawnToMoleculeMovement();
            }
            else if (State != MoleculeState.Fixed)
            {
                HandleRandomMovement();
            }
        }

        private void HandleRandomMovement()
        {
            _currentTimeMove += Time.deltaTime;
            if (timeToMove >= _currentTimeMove)
            {
                Vector3 currentPosition = transform.position;
                Quaternion currentRotation = transform.rotation;
                transform.position = Vector3.Lerp(currentPosition, _newRandomPosition, Time.deltaTime * movementSpeed);
                transform.rotation = Quaternion.Lerp(currentRotation, _newRandomRotation, Time.deltaTime * movementSpeed);
            }
            else
            {
                GetRandomPositionAndRotation();
                _currentTimeMove = 0.0f;
            }
        }

        private void HandleCODesorbMovement()
        {
            _currentTimeMove += Time.deltaTime;
            if (timeToMove >= _currentTimeMove)
            {
                Vector3 currentPosition = transform.position;
                transform.position = Vector3.Lerp(currentPosition, _newRandomPosition, Time.deltaTime * movementSpeed);
            }
            else
            {
                _currentTimeMove = 0.0f;
                _currentTimeDesorb = 0.0f;
                _desorbMoveActive = false;
                GetRandomPositionAndRotation();
            }
        }

        private void HandleDrawnToMoleculeMovement()
        {
            _currenTimeDrawn += Time.deltaTime;
            if (timeToMove >= _currenTimeDrawn)
            {
                Vector3 currentPosition = transform.position;
                Quaternion currentRotation = transform.localRotation;
                transform.position = Vector3.Lerp(currentPosition, _drawingMoleculePosition, Time.deltaTime * movementSpeed);
                transform.localRotation = Quaternion.Lerp(currentRotation, Quaternion.Euler(0.0f, 0.0f, 90.0f), Time.deltaTime * movementSpeed);
            }
            else
            {
                if (State == MoleculeState.DrawnByPlat)
                    HandleO2TouchingPlat();
                else if (State == MoleculeState.DrawnByCO)
                    HandleOTouchingCO();
            }
        }
        
        private void DesorbCO()
        {
            _newRandomPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            _currentTimeMove = 0.0f;
            State = MoleculeState.Moving;
            _desorbMoveActive = true;
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
            transform.position = new Vector3(_drawingMoleculePosition.x, _drawingMoleculePosition.y += CatalystController.FixedMoleculeYDist, _drawingMoleculePosition.z);
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            StartCoroutine(DissociateO2());
        }

        private void HandleOTouchingCO()
        {
            State = MoleculeState.Fixed;
            OnCO2Created?.Invoke(this, _connectedMolecule);
        }

        private void GetRandomPositionAndRotation()
        {
            _newRandomPosition = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0.05f, -0.2f), Random.Range(-0.2f, 0.2f));
            _newRandomRotation = Quaternion.Euler(Random.Range(-180.0f, 180.0f),Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
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
    }
}