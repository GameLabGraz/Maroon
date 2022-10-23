using System;
using Maroon.Physics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maroon.Chemistry.Catalyst
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
        DrawingMolecule,
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
        [SerializeField] public float FixedMoleculeYDist; // = plat scale + endpoint atom scale

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

        protected float CurrentTurnOverRate = 0.0f;

        public MoleculeType Type { get => type; }

        /**
         * Property to set state as well as handle RigidbodyConstraints. Always use this to
         * set the state of a molecule!
         */
        public MoleculeState State
        {
            get => state;
            set {
                if (value == MoleculeState.Fixed || value == MoleculeState.InSurfaceDrawingSpot)
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

        /**
         * Sets the molecule state to a drawn state which is either DrawnBySurfaceMolecule, DrawnByDrawingSpot, or
         * DrawnByCO.
         * Sets the new position and rotation to the molecule this is drawn to.
         * Sets the connected molecule to the drawn molecule.
         * <param name="drawingMolecule"> The molecule that draws this molecule in. </param>
         * <param name="drawnState"> State specifying the kind of drawing state. </param>
         */
        public void SetMoleculeDrawn(Molecule drawingMolecule, MoleculeState drawnState)
        {
            State = drawnState;
            StartMoleculePosition = transform.position;

            StartMoleculeRotation = transform.rotation;
            NewMoleculePosition = drawingMolecule.transform.position;
            NewMoleculeRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            if (drawingMolecule.Type == MoleculeType.Pt || drawingMolecule.type == MoleculeType.Co)
                NewMoleculePosition.y += FixedMoleculeYDist;
            _connectedMolecule = drawingMolecule;
        }

        /**
         * Temperature changed callback.
         * Movement speed of molecules is adjusted based on temperature.
         * Influences turn over rates / frequencies.
         * Note: It is on purpose that the maximal temperature is not adjusted for the
         *  van Krevelen method to account for the lower temperature (about 1/2). Due to this
         *  molecules are moving slower in this variant which is also true in reality.
         * <param name="newTemp"> New Temperature. </param>
         */
        public void TemperatureChanged(float newTemp)
        {
            // normal temp goes from -23.15f to 176.85 (Langmuir) or from 47.85 to 89,85 (van Krevelen) degree celsius
            // scale this between 0 - 1 for movement speed (0,508 max for van Krevelen)
            temperature.Value = newTemp;
            float offsetToZero = 0;
            if (CatalystController.ExperimentVariation == ExperimentVariation.LangmuirHinshelwood ||
                CatalystController.ExperimentVariation == ExperimentVariation.EleyRideal)
            {
                offsetToZero = 23.15f;
            }
            movementSpeed = Mathf.Clamp((temperature.Value + offsetToZero) / (temperature.maxValue + offsetToZero), 0.1f, 1.0f);
            CurrentTurnOverRate = CatalystController.GetTurnOverFrequency(temperature.Value, partialPressure.Value);
        }

        /**
         * Partial pressure changed callback.
         * Influences turn over rates / frequencies.
         * <param name="pressure"> New pressure. </param>
         */
        public void PressureChanged(float pressure)
        {
            partialPressure.Value = pressure;
            CurrentTurnOverRate = CatalystController.GetTurnOverFrequency(temperature.Value, partialPressure.Value);
        }

        public void ReactionStart()
        {
            ReactionStarted = true;
            ReactionStart_Impl();
        }

        /**
         * Activate or deactivate drawing colliders of of platin, cobalt, CO, and O molecules.
         * The collider is used to detect other molecules and then draw them in.
         * <param name="activate"> Whether to activate or deactivate the collider. </param>
         */
        public void ActivateDrawingCollider(bool activate)
        {
            // activate drawing colliders for platinum, cobalt and oxygen
            if (type != MoleculeType.Pt &&
                type != MoleculeType.CO &&
                type != MoleculeType.Co &&
                type != MoleculeType.O) return;

            collider.enabled = activate;
        }

        protected override void Start()
        {
            base.Start();
            if (CatalystController.ExperimentVariation == ExperimentVariation.MarsVanKrevelen)
                FixedMoleculeYDist -= 0.005f; // cobalt is 0.005 smaller
            GetRandomPositionAndRotation();
        }

        protected virtual void ReactionStart_Impl()
        {
            
        }

        protected override void HandleUpdate()
        {
            
        }

        /**
         * Movement and other decisions are made each FixedUpdate.
         * Most molecules override this, add custom logic and call the base method if they are moving.
         */
        protected override void HandleFixedUpdate()
        {
            if (state == MoleculeState.Fixed && _connectedMolecule == null &&
                state != MoleculeState.DrawnByCO && state != MoleculeState.DrawnByDrawingSpot) return;

            if (state != MoleculeState.Fixed && state != MoleculeState.InSurfaceDrawingSpot)
            {
                HandleMoleculeMovement();
            }
        }

        /**
         * Handle movement of molecules. Called each fixed update if molecule is moving
         * or drawn to another molecule.
         * Uses time based movement with movement speed. If the distance of the current position
         * to the new desired position is smaller than some threshold different handling functions
         * are used to handle arrival at the desired position and possibly get a new random position.
         */
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

            // fallback since some CO2 did not get destroyed for some reason.
            if (Vector3.Distance(transform.position, NewMoleculePosition) < 0.05f && Type == MoleculeType.CO2)
            {
                if (state == MoleculeState.Disappear)
                    Destroy(this.gameObject);
            }
        }

        /**
         * Called after a atom / molecule that is drawn to a surface atom (platin or cobalt) has moved
         * near enough. Set final position such that is does not intersect with the surface atom
         * and adjust rotation.
         */
        private void HandleMoleculeTouchingSurface()
        {
            if (type != MoleculeType.CO && type != MoleculeType.O2) return;
            State = MoleculeState.Fixed;
            transform.position = new Vector3(NewMoleculePosition.x, NewMoleculePosition.y, NewMoleculePosition.z);
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            if (type == MoleculeType.O2 && CatalystController.ExperimentVariation != ExperimentVariation.EleyRideal)
                State = MoleculeState.WaitingToDissociate;
        }

        /**
         * Handle O atom being near enough to a CO molecule. Calls an action that creates CO2 in the
         * CatalystController.
         */
        private void HandleOTouchingCO()
        {
            State = MoleculeState.Fixed;
            OnCO2Created?.Invoke(this, _connectedMolecule);
        }

        /**
         * Handle an O atom being near enough to a surface drawing spot. Set final position and update
         * state.
         */
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

        /**
         * Gets a random position and rotation withing a certain random range.
         */
        private void GetRandomPositionAndRotation()
        {
            StartMoleculePosition = transform.position;
            StartMoleculeRotation = transform.rotation;

            float xDirVal = Random.Range(-0.2f, 0.2f);
            float yDirVal = Random.Range(0.15f, -0.2f);
            float zDirVal = Random.Range(-0.2f, 0.2f);

            if (StartMoleculePosition.x + xDirVal > CatalystController.MaxXCoord ||
                StartMoleculePosition.x + xDirVal < CatalystController.MinXCoord)
                xDirVal *= -1;
            if (StartMoleculePosition.y + yDirVal > CatalystController.MaxYCoord ||
                StartMoleculePosition.y + yDirVal < CatalystController.MinYCoord)
                yDirVal *= -1;
            if (StartMoleculePosition.z + zDirVal > CatalystController.MaxZCoord ||
                StartMoleculePosition.z + zDirVal < CatalystController.MinZCoord)
                zDirVal *= -1;
            
            
            if (type == MoleculeType.O)
                NewMoleculePosition = StartMoleculePosition + new Vector3(xDirVal, 0.0f, zDirVal);
            else
                NewMoleculePosition = StartMoleculePosition + new Vector3(xDirVal, yDirVal, zDirVal);
            NewMoleculeRotation = Quaternion.Euler(Random.Range(-180.0f, 180.0f),Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
        }

    }
}