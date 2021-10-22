using System;
using System.Collections;
using System.Collections.Generic;
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

    public class Molecule : PausableObject
    {
        [Header("Molecule Specifics")]
        [SerializeField] MoleculeType type;
        [SerializeField] List<MoleculeType> canConnectToList = new List<MoleculeType>();
        [SerializeField] Collider collider;

        [Header("Molecule Movement")]
        [SerializeField] bool isFixedMolecule;
        [SerializeField] float movementSpeed;
        [SerializeField] float timeToMove = 3.0f;
        [SerializeField] float timeUntilNextDesorb = 3.0f;

        private float _currentTimeMove = 0.0f;
        private float _currentTimeDesorb = 0.0f;
        private Vector3 _newRandomPosition;
        private Quaternion _newRandomRotation;
        private Molecule _connectedMolecule;
        private bool _desorbMoveActive;

        private bool _isDrawnByMolecule;
        private float _currenTimeDrawn = 0.0f;
        private Vector3 _drawingMoleculePosition;

        public MoleculeType Type { get => type; }

        public bool IsFixedMolecule
        {
            get => isFixedMolecule;
            set {
                isFixedMolecule = value;
                if (value)
                {
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                }
                else
                {
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                }
            }
        }

        public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
        public Molecule ConnectedMolecule { get => _connectedMolecule; set => _connectedMolecule = value; }

        public Action<Molecule> OnDissociate;


        public void SetMoleculeDrawn(Molecule drawingMolecule)
        {
            _isDrawnByMolecule = true;
            _drawingMoleculePosition = drawingMolecule.transform.position;
            _connectedMolecule = drawingMolecule;
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
            if (isFixedMolecule && _connectedMolecule == null) return;
            if (isFixedMolecule && _connectedMolecule != null && !_desorbMoveActive)
            {
                // desorbtion or splitting of O2 molecule
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
            else if (_isDrawnByMolecule)
            {
                HandleDrawnToMoleculeMovement();
            }
            else
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
                transform.position = Vector3.Lerp(currentPosition, _drawingMoleculePosition, Time.deltaTime * movementSpeed);
            }
            else
            {
                //transform.position = new Vector3(_drawingMoleculePosition.x, _drawingMoleculePosition.y += CatalystController.FixedMoleculeYDist, _drawingMoleculePosition.z);
                transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                StartCoroutine(DissociateO2());
            }
        }
        
        private void DesorbCO()
        {
            _newRandomPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            _currentTimeMove = 0.0f;
            isFixedMolecule = false;
            _desorbMoveActive = true;
            _connectedMolecule.ConnectedMolecule = null;
            _connectedMolecule = null;
        }

        private IEnumerator DissociateO2()
        {
            yield return new WaitForSeconds(2.0f);
            OnDissociate?.Invoke(this);
        }

        private void GetRandomPositionAndRotation()
        {
            _newRandomPosition = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0.05f, -0.2f), Random.Range(-0.2f, 0.2f));
            _newRandomRotation = Quaternion.Euler(Random.Range(-180.0f, 180.0f),Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
        }

        private void OnCollisionEnter(Collision other)
        {
            if (Type == MoleculeType.Pt && _connectedMolecule == null)
            {
                Molecule molecule = other.gameObject.GetComponent<Molecule>();
                if (molecule != null && molecule.Type == MoleculeType.O2)
                {
                    _connectedMolecule = molecule;
                    molecule.SetMoleculeDrawn(this);
                }
            }
        }
    }
}