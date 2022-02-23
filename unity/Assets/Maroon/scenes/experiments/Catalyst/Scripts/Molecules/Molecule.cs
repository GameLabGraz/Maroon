using System;
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
        Co,
        DrawingSpot
    }

    public enum MoleculeState
    {
        Fixed,
        Moving,
        Desorb,
        InDrawingCollider,
        DrawnBySurfaceMolecule, // either platinum or cobalt
        DrawnByCO,
        DrawnByDrawingSpot,
        Disappear,
        WaitingToDissociate,
        InSurfaceDrawingSpot
    }

    public class Molecule : PausableObject
    {
        [Header("Molecule Specifics")]
        [SerializeField] MoleculeType type;
        [SerializeField] Collider collider;

        [Header("Molecule Movement")]
        [SerializeField] float movementSpeed = 1;

        [SerializeField] QuantityFloat temperature = new QuantityFloat();
        [SerializeField] QuantityFloat partialPressure = new QuantityFloat();

        [SerializeField] MoleculeState state; // do not set this directly, use the property setter below

        private Molecule _possibleDrawingMolecule; // should always be a platinum or cobalt molecule
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

        public Action<Molecule> OnDissociate;
        public Action<Molecule, Molecule> OnCO2Created;
        public Action OnMoleculeFreed;

        public void SetMoleculeDrawn(Molecule drawingMolecule, MoleculeState drawnState)
        {
            State = drawnState;
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
            // activate drawing colliders for platinum, cobalt and oxygen
            if (type != MoleculeType.Pt && type != MoleculeType.Co && type != MoleculeType.O) return;

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
            if (state == MoleculeState.Fixed && _connectedMolecule == null &&
                state != MoleculeState.DrawnByCO && state != MoleculeState.DrawnByDrawingSpot) return;

            if (state != MoleculeState.Fixed && state != MoleculeState.InSurfaceDrawingSpot)
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
                else if (state == MoleculeState.DrawnBySurfaceMolecule)
                    HandleMoleculeTouchingSurface();
                else if (state == MoleculeState.DrawnByCO)
                    HandleOTouchingCO();
                else if (state == MoleculeState.DrawnByDrawingSpot)
                    HandleOFillDrawingSpot();

                if (state == MoleculeState.Moving || state == MoleculeState.InDrawingCollider)
                {
                    CurrentTimeMove = 0.0f;
                    GetRandomPositionAndRotation();
                }
            }
        }

        private void HandleMoleculeTouchingSurface()
        {
            if (type != MoleculeType.CO && type != MoleculeType.O2) return;
            State = MoleculeState.Fixed;
            transform.position = new Vector3(NewMoleculePosition.x, NewMoleculePosition.y += CatalystController.FixedMoleculeYDist, NewMoleculePosition.z);
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            if (type == MoleculeType.O2)
                State = MoleculeState.WaitingToDissociate;
        }

        private void HandleOTouchingCO()
        {
            State = MoleculeState.Fixed;
            OnCO2Created?.Invoke(this, _connectedMolecule);
        }

        private void HandleOFillDrawingSpot()
        {
            if (type != MoleculeType.O) return;
            State = MoleculeState.Fixed;
            transform.position = NewMoleculePosition;
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