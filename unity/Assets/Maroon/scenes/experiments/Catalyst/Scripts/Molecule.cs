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

    public class Molecule : MonoBehaviour
    {
        [SerializeField] MoleculeType type;
        [SerializeField] List<MoleculeType> canConnectToList = new List<MoleculeType>();

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

        public MoleculeType Type { get => type; }
        public bool IsFixedMolecule { get => IsFixedMolecule; set => isFixedMolecule = value; }
        public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
        public Molecule ConnectedMolecule { get => _connectedMolecule; set => _connectedMolecule = value; }


        public void Desorb()
        {
            _newRandomPosition = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
            _currentTimeMove = 0.0f;
            _connectedMolecule = null;
            isFixedMolecule = false;
            _desorbMoveActive = true;
        }

        private void Start()
        {
            GetRandomPositionAndRotation();
        }

        private void FixedUpdate()
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
                            Desorb();
                        }
                        _currentTimeDesorb = 0.0f;
                    }
                }
                else if (Type == MoleculeType.O2 && _connectedMolecule.Type == MoleculeType.Pt)
                {
                    // todo handle O2 split
                }

                return;
            }

            if (_desorbMoveActive)
            {
                HandleDesorbMovement();
                return;
            }
            
            HandleRandomMovement();
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

                float dist = Vector3.Distance(transform.position, _newRandomPosition);
                if (dist <= 0.04f)
                {
                    GetRandomPositionAndRotation();
                    _currentTimeMove = 0.0f;
                }
            }
        }

        private void HandleDesorbMovement()
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

        private void GetRandomPositionAndRotation()
        {
            _newRandomPosition = transform.position + new Vector3(Random.Range(-0.05f, 0.2f), Random.Range(0.1f, -0.2f), Random.Range(-0.1f, 0.1f));
            _newRandomRotation = Quaternion.Euler(Random.Range(-180.0f, 180.0f),Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
        }
    }
}