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
        DrawnBySurfaceMolecule, // either platinum or cobalt
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

        [SerializeField] QuantityFloat temperature = new QuantityFloat();
        [SerializeField] QuantityFloat partialPressure = new QuantityFloat();

        [SerializeField] MoleculeState state;

        private Molecule _possibleDrawingMolecule; // should always be a platinum molecule
        private Molecule _connectedMolecule;
        
        protected float CurrentTimeMove = 0.0f;
        protected Vector3 StartMoleculePosition;
        protected Vector3 NewMoleculePosition;
        protected Quaternion StartMoleculeRotation;
        protected Quaternion NewMoleculeRotation;

        protected bool ReactionStarted = false;
        protected bool IsTopLayerSurfaceMolecule = false;

        protected float CurrentTurnOverRate = 0.0f;

        public MoleculeType Type { get => type; }

        public MoleculeState State
        {
            get => state;
            set {
                if (value == MoleculeState.Fixed)
                {
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                } 
                else if (state == MoleculeState.Fixed)
                {
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                }
                state = value;
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
            StartMoleculePosition = transform.position;
            StartMoleculeRotation = transform.rotation;
            NewMoleculePosition = drawingMolecule.transform.position;
            NewMoleculeRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            _connectedMolecule = drawingMolecule;
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

        public void SetIsTopLayerSurfaceMolecule(bool isTopLayerMolecule)
        {
            IsTopLayerSurfaceMolecule = isTopLayerMolecule;
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

            if (state != MoleculeState.Fixed)
            {
                HandleMoleculeMovement();
            }
        }

        private void HandleMoleculeMovement()
        {
            CurrentTimeMove += Time.deltaTime * movementSpeed;
            if (Vector3.Distance(transform.position, NewMoleculePosition) > 0.05f)
            {
                transform.position = Vector3.Lerp(StartMoleculePosition, NewMoleculePosition, CurrentTimeMove);
                transform.rotation = Quaternion.Lerp(StartMoleculeRotation, NewMoleculeRotation, CurrentTimeMove);
            }
            else
            {
                if (state == MoleculeState.Desorb)
                {
                    State = MoleculeState.Moving;
                }
                else if (state == MoleculeState.Disappear)
                    Destroy(this.gameObject);
                else if (State == MoleculeState.DrawnBySurfaceMolecule)
                    HandleMoleculeTouchingPlat();
                else if (State == MoleculeState.DrawnByCO)
                    HandleOTouchingCO();

                if (state == MoleculeState.Moving)
                {
                    CurrentTimeMove = 0.0f;
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
            transform.position = new Vector3(NewMoleculePosition.x, NewMoleculePosition.y += CatalystController.FixedMoleculeYDist, NewMoleculePosition.z);
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
        }

        private void GetRandomPositionAndRotation()
        {
            StartMoleculePosition = transform.position;
            StartMoleculeRotation = transform.rotation;
            NewMoleculePosition = StartMoleculePosition + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0.1f, -0.2f), Random.Range(-0.2f, 0.2f));
            NewMoleculeRotation = Quaternion.Euler(Random.Range(-180.0f, 180.0f),Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
        }

    }
}