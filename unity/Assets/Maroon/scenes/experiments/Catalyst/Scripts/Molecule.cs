using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        InDrawingCollider,
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
        [SerializeField] QuantityFloat partialPressure = new QuantityFloat();

        [SerializeField] private MoleculeState _state;
        
        private float _currentTimeMove = 0.0f;
        private float _currentTimeDesorb = 0.0f;
        private Vector3 _startMoleculePosition;
        private Vector3 _newMoleculePosition;
        private Quaternion _startMoleculeRotation;
        private Quaternion _newMoleculeRotation;
        private Molecule _possibleDrawingMolecule; // should always be a platinum molecule
        private Molecule _connectedMolecule;
        
        private float _currenTimeDrawn = 0.0f;

        private int _moleculeClickCounter = 0;
        private float _wobbleStrength = 0.1f;
        private bool _isWobbling = false;
        private bool _reactionStarted = false;

        private int[] _temperatureStageValues = new[] { 250, 275, 300, 325, 350, 375, 400, 425, 450 };
        private float[] _partialPressureValues = new[] { 0.01f, 0.02f, 0.04f, 0.2f };
        private float _currentTurnOverRate = 0.0f;

        private static readonly float[][] TurnOverRates = new float[][]
        {
            new float[] { 0f, 0f, 0.047619048f, 0.285714286f, 8.571428571f },
            new float[] { 0f, 0.047619048f, 0.142857143f, 0.666666667f, 8.571428571f },
            new float[] { 0f, 0.095238095f, 0.238095238f, 1.19047619f, 8.571428571f },
            new float[] { 0.047619048f, 0.19047619f, 0.380952381f, 1.952380952f, 8.571428571f },
            new float[] { 0.095238095f, 0.285714286f, 0.571428571f, 2.952380952f, 8.571428571f },
            new float[] { 0.19047619f, 0.380952381f, 0.80952381f, 4.19047619f, 8.571428571f },
            new float[] { 0.285714286f, 0.571428571f, 1.142857143f, 5.904761905f, 8.571428571f },
            new float[] { 0.333333333f, 0.714285714f, 1.428571429f, 7.428571429f, 8.571428571f },
            new float[] { 0.380952381f, 0.80952381f, 1.619047619f, 8.571428571f, 8.571428571f }
        };


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

        public void OnMouseDown()
        {
            if (type != MoleculeType.CO || State != MoleculeState.Fixed || !SimulationController.Instance.SimulationRunning) return;
            if (_isWobbling) return;
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
            // update turnover rates
            _currentTurnOverRate = TurnOverRates[GetTemperatureIndex()][GetPartialPressureIndex()];

        }

        public void PressureChanged(float pressure)
        {
            partialPressure.Value = pressure;
            // update turnover rates
            _currentTurnOverRate = TurnOverRates[GetTemperatureIndex()][GetPartialPressureIndex()];
        }

        public void ReactionStart()
        {
            _reactionStarted = true;
            if (type == MoleculeType.Pt)
            {
                GetComponent<CapsuleCollider>().enabled = true;
            }
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
            
            if (_state != MoleculeState.Fixed)
            {
                HandleMoleculeMovement();
            }

            if (_state == MoleculeState.InDrawingCollider && _possibleDrawingMolecule != null)
            {
                HandleDrawingPossibility();
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
            _startMoleculePosition = transform.position;
            _startMoleculeRotation = transform.rotation;
            _newMoleculePosition = new Vector3(_startMoleculePosition.x, _startMoleculePosition.y + 0.8f, _startMoleculePosition.z);
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

        private void HandleDrawingPossibility()
        {
            if (Random.Range(0, 100) > 95)
            {
                _possibleDrawingMolecule.ConnectedMolecule = this; // connect this (O2 or CO) to plat molecule
                SetMoleculeDrawn(_possibleDrawingMolecule, MoleculeState.DrawnByPlat); // drawn by plat
                _connectedMolecule.ActivateDrawingCollider(false); // deactivate plat drawing collider
            }

        }

        private void GetRandomPositionAndRotation()
        {
            _startMoleculePosition = transform.position;
            _startMoleculeRotation = transform.rotation;
            _newMoleculePosition = _startMoleculePosition + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0.1f, -0.2f), Random.Range(-0.2f, 0.2f));
            _newMoleculeRotation = Quaternion.Euler(Random.Range(-180.0f, 180.0f),Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_state == MoleculeState.DrawnByPlat || _state == MoleculeState.DrawnByCO) return;
            if (type == MoleculeType.Pt && _connectedMolecule == null) // draw in O2 or CO molecules
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
            else if (type == MoleculeType.O && _connectedMolecule == null) // draw O atoms to nearby CO molecules
            {
                Molecule otherMolecule = other.gameObject.GetComponent<Molecule>();
                if (otherMolecule != null && otherMolecule.Type == MoleculeType.CO && // todo find cause of possible nullref exception - maybe CO gets desorbed while O atom is drawn?
                    otherMolecule.State == MoleculeState.Fixed && otherMolecule.ConnectedMolecule.Type == MoleculeType.Pt)
                {
                    SetMoleculeDrawn(otherMolecule, MoleculeState.DrawnByCO);
                    otherMolecule.ConnectedMolecule = this;
                    ActivateDrawingCollider(false);
                    otherMolecule.ActivateDrawingCollider(false);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_state == MoleculeState.DrawnByPlat || _state == MoleculeState.DrawnByCO) return;
            if (type == MoleculeType.Pt && _connectedMolecule == null) // reset drawing state and possible drawing molecule
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

        private int GetTemperatureIndex()
        {
            // add 273.16 instead of 273.15 to always get at least the first element index
            return Array.IndexOf(_temperatureStageValues, _temperatureStageValues.TakeWhile(num => num <= temperature.Value + 273.16f).Last());
        }
        
        private int GetPartialPressureIndex()
        {
            return Array.IndexOf(_partialPressureValues, _partialPressureValues.TakeWhile(num => num <= partialPressure.Value).Last());
        }
    }
}