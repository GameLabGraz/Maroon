using System;
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

        private float _currentTime = 0.0f;
        private Vector3 _newRandomPosition;
        private Quaternion _newRandomRotation;
        
        public MoleculeType Type { get => type; }
        public bool IsFixedMolecule { get => IsFixedMolecule; set => isFixedMolecule = value; }
        public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }


        private void Start()
        {
            GetRandomPositionAndRotation();
        }

        private void FixedUpdate()
        {
            if (isFixedMolecule) return;
            
            _currentTime += Time.deltaTime;
            if (timeToMove >= _currentTime)
            {
                Vector3 currentPosition = transform.position;
                Quaternion currentRotation = transform.rotation;
                transform.position = Vector3.Lerp(currentPosition, _newRandomPosition, Time.deltaTime * movementSpeed);
                transform.rotation = Quaternion.Lerp(currentRotation, _newRandomRotation, Time.deltaTime * movementSpeed);

                float dist = Vector3.Distance(transform.position, _newRandomPosition);
                if (dist <= 0.04f)
                {
                    GetRandomPositionAndRotation();
                    _currentTime = 0.0f;
                }
            }
        }

        private void GetRandomPositionAndRotation()
        {
            _newRandomPosition = transform.position + new Vector3(Random.Range(-0.05f, 0.2f), Random.Range(0.1f, -0.2f), Random.Range(-0.1f, 0.1f));
            _newRandomRotation = Quaternion.Euler(Random.Range(-180.0f, 180.0f),Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
        }
    }
}