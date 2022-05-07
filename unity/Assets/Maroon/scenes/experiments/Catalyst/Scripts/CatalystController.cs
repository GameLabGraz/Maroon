using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using GameLabGraz.VRInteraction;
using Maroon.Physics;
using Maroon.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Maroon.Chemistry.Catalyst
{
    public enum ExperimentVariation
    {
        LangmuirHinshelwood,
        MarsVanKrevelen
    }

    public enum ExperimentStages
    {
        Init,
        CODesorb,
        COAdsorb,
        O2Adsorb_O2Dissociate,
        OFillSurface,
        OReactCO_CO2Desorb
    }

    public class CatalystController : MonoBehaviour
    {
        [Header("Simulation Parameters")]
        [SerializeField] bool isVrVersion;
        [SerializeField] QuantityFloat temperature;
        [SerializeField] QuantityFloat partialPressure;
        [SerializeField] int numberSpawnedO2Molecules;
        [SerializeField] int numberSpawnedCOMolecules;
        
        [Header("Catalyst specific objects")]
        [SerializeField] CatalystReactor catalystReactor;
        [SerializeField] CatalystSurface catalystSurfacePrefab;
        [SerializeField] GameObject catalystReactionBoxGameObject;
        [SerializeField] Transform catalystReactionBoxPlayerSpawnTransform;
        [SerializeField] Transform experimentRoomPlayerSpawnTransform;
        [SerializeField] Transform catalystSurfaceSpawnTransform;

        [Header("Molecule Prefabs")]
        [SerializeField] Molecule oMoleculePrefab;
        [SerializeField] Molecule o2MoleculePrefab;
        [SerializeField] Molecule coMoleculePrefab;
        [SerializeField] Molecule co2MoleculePrefab;

        [Header("Player specific objects")]
        [SerializeField] GameObject player;

        [Header("UI Elements")]
        [SerializeField] TextMeshProUGUI stepWiseEnableText;
        [SerializeField] TextMeshProUGUI currentStepText;
        [SerializeField] TextMeshProUGUI interactiveSimulationText;
        [SerializeField] TextMeshProUGUI turnOverRateText;
        [SerializeField] QuantityPropertyView temperatureView;
        [SerializeField] QuantityPropertyView partialPressureView;
        [SerializeField] VRLinearDrive temperatureViewVr;
        [SerializeField] VRLinearDrive partialPressureViewVr;
        [SerializeField] Image graphImage;
        [SerializeField] WhiteboardController whiteboardController;
        [SerializeField] Sprite graphLangmuirSprite;
        [SerializeField] Sprite graphVanKrevelenSprite;
        [SerializeField] CatalystVrControlPanel _controlPanel;
        [SerializeField] XCharts.LineChart lineChartLangmuir;
        [SerializeField] XCharts.LineChart lineChartVanKrevelen;

        private int _freedMoleculeCounter = 0;
        private List<Vector3> _platSpawnPoints = new List<Vector3>();
        private List<Vector3> _activePlatSpawnPoints = new List<Vector3>();
        private List<Vector3> _coSpawnPoints = new List<Vector3>();
        private List<Vector3> _oSpawnPoints = new List<Vector3>();
        private List<Molecule> _activeMolecules = new List<Molecule>();
        private CatalystSurface _catalystSurface;
        
        private float _currentTurnOverRate = 0.0f;
        private bool _doStepWiseSimulation = false;
        private bool _doInteractiveSimulation = true;

        private System.Action onReactionStart;

        private float _minXValLocal = 0.0f;
        private float _maxXValLocal = 0.0f;
        private float _minZValLocal = 0.0f;
        private float _maxZValLocal = 0.0f;
        
        private static readonly Regex WhiteSpaces = new Regex(@"\s+");

        private static readonly int[][] TemperatureStageValues = new int [][]
        {
            new int[] {250, 275, 300, 325, 350, 375, 400, 425, 450 },
            new int[] {321, 334, 348, 363}
        };
        
        private static readonly float[][] PartialPressureValues = new float[][]
        {
            new float[] {0.01f, 0.02f, 0.04f, 0.2f},
            new float[] {0.001f, 0.002f, 0.004f, 0.008f, 0.014f, 0.026f}
        };
        
        private static readonly float[][][] TurnOverRates = new float[][][]
        {
            new float[][]
            {
                new float[] { 0f, 0f, 0.047619048f, 0.285714286f },
                new float[] { 0f, 0.047619048f, 0.142857143f, 0.666666667f },
                new float[] { 0f, 0.095238095f, 0.238095238f, 1.19047619f },
                new float[] { 0.047619048f, 0.19047619f, 0.380952381f, 1.952380952f },
                new float[] { 0.095238095f, 0.285714286f, 0.571428571f, 2.952380952f },
                new float[] { 0.19047619f, 0.380952381f, 0.80952381f, 4.19047619f },
                new float[] { 0.285714286f, 0.571428571f, 1.142857143f, 5.904761905f },
                new float[] { 0.333333333f, 0.714285714f, 1.428571429f, 7.428571429f },
                new float[] { 0.380952381f, 0.80952381f, 1.619047619f, 8.571428571f }
            },
            new float[][]
            {
                new float[] { 0.042146f, 0.095785f, 0.12644f, 0.18008f, 0.23372f, 0.29502f },
                new float[] { 0.16475f, 0.341f, 0.4636f, 0.61686f, 0.75479f, 0.83908f },
                new float[] { 0.26437f, 0.54023f, 0.71648f, 1.023f, 1.1686f, 1.3678f },
                new float[] { 0.63985f, 1.1456f, 1.5747f, 2.3563f, 2.9234f, 3.6743f }
            }
        };

        private static readonly List<ExperimentStages> HinshelwoodStages = new List<ExperimentStages>()
        {
            ExperimentStages.Init,
            ExperimentStages.CODesorb,
            ExperimentStages.O2Adsorb_O2Dissociate,
            ExperimentStages.OReactCO_CO2Desorb
        };

        private static readonly List<ExperimentStages> KrevelenStages = new List<ExperimentStages>()
        {
            ExperimentStages.Init,
            ExperimentStages.COAdsorb,
            ExperimentStages.OReactCO_CO2Desorb,
            ExperimentStages.O2Adsorb_O2Dissociate,
            ExperimentStages.OFillSurface
        };

        //public const float FixedMoleculeYDist = 0.28f - 0.075f;
        public const float PlatinumScale = 0.14f;

        public static bool IsVrVersion;
        public static bool DoStepWiseSimulation = false;
        public static ExperimentStages CurrentExperimentStage = ExperimentStages.Init;
        public static ExperimentVariation ExperimentVariation = ExperimentVariation.LangmuirHinshelwood;

        // plane above surface where molecules can be spawned, and move
        public static float MinXCoord = 0.0f;
        public static float MaxXCoord = 0.0f;
        public static float MaxZCoord = 0.0f;
        public static float MinZCoord = 0.0f;
        // height above surface max and min coordinates
        public static float MinYCoord = 0.0f;
        public static float MaxYCoord = 0.0f;

        private void Awake()
        {
            var coordFile = Resources.Load<TextAsset>("SurfaceData/Pt-111");
            FillSpawnPoints(coordFile);
            coordFile = Resources.Load<TextAsset>("SurfaceData/Co3O4_111");
            FillSpawnPoints(coordFile);
            
            var surfaceCoords = Resources.Load<TextAsset>("SurfaceData/Pt-111-active_surface");
            FillActiveSurfaceSpawnPoints(surfaceCoords);
        }

        private void Start()
        {
            catalystReactionBoxGameObject.SetActive(false);
            IsVrVersion = isVrVersion;

            catalystReactor.OnReactorFilled.AddListener(StartExperiment);
            graphImage.gameObject.SetActive(false);
            lineChartLangmuir.gameObject.SetActive(false);
            lineChartVanKrevelen.gameObject.SetActive(false);
        }

        private void StartExperiment()
        {
            if (_doInteractiveSimulation)
                HandleCatalystSurfaceSetup();
            DrawSimulationGraphs();
        }

        private void OnDestroy()
        {
            catalystReactor.OnReactorFilled.RemoveListener(SpawnCatalystSurfaceObject);
        }
        
        /**
         * Fill platin, cobalt, and O spawn point lists.
         * <param name="coordFile"> CSV file with coordinates </param>
         */
        private void FillSpawnPoints(TextAsset coordFile)
        {
            string[] lines = coordFile.text.Split('\n');
            foreach (var line in lines)
            {
                string trimmedLine = line.TrimStart(); // O entry have a leading whitespace
                string[] lineValues = WhiteSpaces.Replace(trimmedLine, ",").Split(',');
                if (lineValues.Length >= 4)
                {
                    Vector3 spawnPoint = new Vector3(
                        float.Parse(lineValues[1], CultureInfo.InvariantCulture.NumberFormat),
                        float.Parse(lineValues[2], CultureInfo.InvariantCulture.NumberFormat),
                        float.Parse(lineValues[3], CultureInfo.InvariantCulture.NumberFormat)
                    );

                    if (lineValues[0].Equals("Pt"))
                    {
                        _platSpawnPoints.Add(spawnPoint);
                    }
                    else if (lineValues[0].Equals("Co"))
                    {
                        _coSpawnPoints.Add(spawnPoint);
                    }
                    else if (lineValues[0].Equals("O"))
                    {
                        _oSpawnPoints.Add(spawnPoint);
                    }
                }
            }
        }

        private void FillActiveSurfaceSpawnPoints(TextAsset coordFile)
        {
            string[] lines = coordFile.text.Split('\n');
            foreach (var line in lines)
            {
                string trimmedLine = line.TrimStart(); // O entry have a leading whitespace
                string[] lineValues = WhiteSpaces.Replace(trimmedLine, ",").Split(',');
                if (lineValues.Length >= 4)
                {
                    Vector3 spawnPoint = new Vector3(
                        float.Parse(lineValues[1], CultureInfo.InvariantCulture.NumberFormat),
                        float.Parse(lineValues[2], CultureInfo.InvariantCulture.NumberFormat),
                        float.Parse(lineValues[3], CultureInfo.InvariantCulture.NumberFormat)
                    );

                    if (lineValues[0].Equals("Pt"))
                    {
                        _activePlatSpawnPoints.Add(spawnPoint);
                    }
                }
            }
        }

        private void HandleCatalystSurfaceSetup()
        {
            catalystReactionBoxGameObject.SetActive(true);
            player.transform.position = catalystReactionBoxPlayerSpawnTransform.position;
            SpawnCatalystSurfaceObject();
        }

        /**
         * Spawns the catalyst reaction surface and call the setup coordinates method for the
         * selected variant.
         */
        private void SpawnCatalystSurfaceObject()
        {
            EnsureCleanSurface();

            _catalystSurface = Instantiate(catalystSurfacePrefab, catalystSurfaceSpawnTransform);
            if (ExperimentVariation.Equals(ExperimentVariation.LangmuirHinshelwood))
            {
                _catalystSurface.SetupCoordsLangmuir(_platSpawnPoints,_activePlatSpawnPoints,list =>
                    {
                        _activeMolecules = list;
                        foreach (var molecule in _activeMolecules)
                        {
                            onReactionStart += molecule.ReactionStart;
                            if (molecule.Type == MoleculeType.Pt) continue;
                            molecule.TemperatureChanged(temperature);
                            molecule.PressureChanged(partialPressure);
                            temperature.onValueChanged.AddListener(molecule.TemperatureChanged);
                            partialPressure.onValueChanged.AddListener(molecule.PressureChanged);
                        }
                        SetMovementCoordinateMinMax();
                        SpawnReactionMaterial(true, true);
                    },
                    OnMoleculeFreed);
            }
            else if (ExperimentVariation.Equals(ExperimentVariation.MarsVanKrevelen))
            {
                _catalystSurface.SetupCoordsKrevelen(_coSpawnPoints, _oSpawnPoints, list =>
                    {
                        _activeMolecules = list;
                        foreach (var molecule in _activeMolecules)
                        {
                            onReactionStart += molecule.ReactionStart;
                            if (molecule.Type == MoleculeType.Co) continue;
                            if (molecule.Type == MoleculeType.O)
                                molecule.OnCO2Created += CreateCO2;
                            molecule.TemperatureChanged(temperature);
                            molecule.PressureChanged(partialPressure);
                            temperature.onValueChanged.AddListener(molecule.TemperatureChanged);
                            partialPressure.onValueChanged.AddListener(molecule.PressureChanged);
                        }
                        SetMovementCoordinateMinMax();
                        SpawnReactionMaterial(true, true);
                        onReactionStart?.Invoke();
                    });
            }
            if (_controlPanel)
                _controlPanel.Setup(Mathf.Min(MaxXCoord - MinXCoord, MaxZCoord - MinZCoord));
        }

        /**
         * Ensures that the catalyst surface is cleared when selecting another variant by
         * deleting all child objects from the catalyst surface spawn transform parent.
         */
        private void EnsureCleanSurface()
        {
            if (catalystSurfaceSpawnTransform.childCount == 0) return;
            
            foreach (Transform childTransform in catalystSurfaceSpawnTransform.GetChild(0).transform)
            {
                Molecule molecule = childTransform.GetComponent<Molecule>();
                if (molecule != null)
                {
                    onReactionStart -= molecule.ReactionStart;
                    // do we also need to remove OnDissociate?
                    temperature.onValueChanged.RemoveListener(molecule.TemperatureChanged);
                    partialPressure.onValueChanged.RemoveListener(molecule.PressureChanged);
                    _activeMolecules.Remove(molecule);
                    Destroy(molecule.gameObject);
                }
            }
            Destroy(catalystSurfaceSpawnTransform.GetChild(0).gameObject);

            _activeMolecules.Clear();
        }

        /**
         * Spawns reaction material (O atoms and CO molecules).
         * Called once at start. Can subsequently be called via button click but then
         * only O atoms are spawned.
         */
        private void SpawnReactionMaterial(bool spawnO2, bool spawnCO)
        {
            Transform catalystSurfaceTransform = _catalystSurface.gameObject.transform;
            if (spawnO2)
            {
                for (int i = 0; i < numberSpawnedO2Molecules; i++)
                {
                    Vector3 spawnPos = new Vector3(Random.Range(_minXValLocal, _maxXValLocal), Random.Range(0.5f, 2.0f), Random.Range(_minZValLocal, _maxZValLocal));
                    Quaternion spawnRot = Quaternion.Euler(Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
                    Molecule molecule = Instantiate(o2MoleculePrefab, catalystSurfaceTransform);
                    molecule.gameObject.transform.localPosition = spawnPos;
                    molecule.gameObject.transform.localRotation = spawnRot;
                    molecule.OnDissociate += DissociateO2;
                    molecule.State = MoleculeState.Moving;
                    onReactionStart += molecule.ReactionStart;
                    AddMoleculeToActiveList(molecule);
                }
            }

            if (spawnCO)
            {
                for (int i = 0; i < numberSpawnedCOMolecules; i++)
                {
                    Vector3 spawnPos = new Vector3(Random.Range(_minXValLocal, _maxXValLocal), Random.Range(0.5f, 2.0f), Random.Range(_minZValLocal, _maxZValLocal));
                    Quaternion spawnRot = Quaternion.Euler(Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
                    Molecule molecule = Instantiate(coMoleculePrefab, catalystSurfaceTransform);
                    molecule.gameObject.transform.localPosition = spawnPos;
                    molecule.gameObject.transform.localRotation = spawnRot;
                    molecule.State = MoleculeState.Moving;
                    onReactionStart += molecule.ReactionStart;
                    AddMoleculeToActiveList(molecule);
                }
            }
        }

        /**
         * Called via callback from O2 molecules.
         * Removes O2 molecule and creates 2 O atoms.
         */
        private void DissociateO2(Molecule o2Molecule)
        {
            o2Molecule.OnDissociate -= DissociateO2;
            Transform parentTransform = o2Molecule.transform.parent;
            Vector3 o2Position = o2Molecule.transform.position;
            o2Molecule.ConnectedMolecule.ConnectedMolecule = null;
            RemoveMoleculeFromActiveList(o2Molecule);
            Destroy(o2Molecule.gameObject);
            List<Molecule> oMolecules = new List<Molecule>()
            {
                Instantiate(oMoleculePrefab, parentTransform),
                Instantiate(oMoleculePrefab, parentTransform)
            };

            bool alternate = false;
            foreach (Molecule molecule in oMolecules)
            {
                molecule.OnCO2Created += CreateCO2;
                molecule.State = MoleculeState.Moving;
                (molecule as OMolecule).CreateO2 += CreateO2;
                if (ExperimentVariation == ExperimentVariation.MarsVanKrevelen)
                    (molecule as OMolecule).SetDarkMaterial();
                molecule.gameObject.transform.position = new Vector3(alternate ? o2Position.x + PlatinumScale / 3.0f : o2Position.x - PlatinumScale / 3.0f, o2Position.y - 0.07f, o2Position.z);
                AddMoleculeToActiveList(molecule);
                alternate = !alternate;
            }
        }

        /**
         * Callback that is called from CO molecules in the Langmuir method when these
         * molecules haven been removed by hand.
         * Increments a counter and starts the reaction when four molecules have been removed.
         */
        private void OnMoleculeFreed()
        {
            if (_freedMoleculeCounter == 4)
            {
                onReactionStart?.Invoke();
            }
            _freedMoleculeCounter++;
        }

        /**
         * Creates CO2 molecule. Called from O atoms when they are near a CO molecule that
         * they have been drawn to.
         * Also called when two O atoms collide (todo implement this).
         */
        private void CreateCO2(Molecule oMolecule, Molecule coMolecule)
        {
            oMolecule.OnCO2Created -= CreateCO2;
            RemoveMoleculeFromActiveList(oMolecule);
            RemoveMoleculeFromActiveList(coMolecule);
            Transform parentTransform = coMolecule.gameObject.transform.parent;
            Vector3 coPosition = coMolecule.gameObject.transform.position;
            Quaternion coRotation = coMolecule.gameObject.transform.rotation;
            
            coMolecule.ConnectedMolecule.ActivateDrawingCollider(true);
            coMolecule.ConnectedMolecule = null;

            Destroy(oMolecule.gameObject);
            Destroy(coMolecule.gameObject);

            Molecule co2Molecule = Instantiate(co2MoleculePrefab, parentTransform);
            co2Molecule.gameObject.transform.position = coPosition;
            co2Molecule.gameObject.transform.rotation = coRotation;
        }

        /**
         * Create O2 molecule. Called when two O atoms are colliding.
         */
        private void CreateO2(Molecule oMolecule1, Molecule oMolecule2)
        {
            Transform parentTransform = oMolecule1.gameObject.transform.parent;
            Vector3 oPosition = oMolecule1.gameObject.transform.position;
            RemoveMoleculeFromActiveList(oMolecule1);
            RemoveMoleculeFromActiveList(oMolecule2);
            
            Destroy(oMolecule1.gameObject);
            Destroy(oMolecule2.gameObject);

            Molecule o2Molecule = Instantiate(o2MoleculePrefab, parentTransform);
            o2Molecule.gameObject.transform.position = oPosition;
            o2Molecule.OnDissociate += DissociateO2;
            o2Molecule.State = MoleculeState.Moving;
            AddMoleculeToActiveList(o2Molecule);
        }

        private void AddMoleculeToActiveList(Molecule molecule)
        {
            molecule.TemperatureChanged(temperature);
            molecule.PressureChanged(partialPressure);
            temperature.onValueChanged.AddListener(molecule.TemperatureChanged);
            partialPressure.onValueChanged.AddListener(molecule.PressureChanged);
            _activeMolecules.Add(molecule);
        }

        private void RemoveMoleculeFromActiveList(Molecule molecule)
        {
            onReactionStart -= molecule.ReactionStart;
            temperature.onValueChanged.RemoveListener(molecule.TemperatureChanged);
            partialPressure.onValueChanged.RemoveListener(molecule.PressureChanged);
            _activeMolecules.Remove(molecule);
        }

        private void UpdateTurnOverRateUI()
        {
            turnOverRateText.text = _currentTurnOverRate.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        private void SetMovementCoordinateMinMax()
        {
            MinXCoord = _activeMolecules.Min(molecule => molecule.gameObject.transform.position.x) + 0.4f;
            MaxXCoord = _activeMolecules.Max(molecule => molecule.gameObject.transform.position.x) - 0.4f;
            MinZCoord = _activeMolecules.Min(molecule => molecule.gameObject.transform.position.z) + 0.4f;
            MaxZCoord = _activeMolecules.Max(molecule => molecule.gameObject.transform.position.z) - 0.4f;

            MinYCoord = _activeMolecules.Min(molecule => molecule.gameObject.transform.position.y) + 0.1f;
            MaxYCoord = 4.0f;
            
            _minXValLocal = _activeMolecules.Min(molecule => molecule.gameObject.transform.localPosition.x) + 0.4f;
            _maxXValLocal = _activeMolecules.Max(molecule => molecule.gameObject.transform.localPosition.x) - 0.4f;
            _minZValLocal = _activeMolecules.Min(molecule => molecule.gameObject.transform.localPosition.z) + 0.4f;
            _maxZValLocal = _activeMolecules.Max(molecule => molecule.gameObject.transform.localPosition.z) - 0.4f;
        }

        private void SetSimulationParametersMinMax(ExperimentVariation variation)
        {
            if (temperatureView && partialPressureView)
            {
                temperatureView.ClearUI();
                partialPressureView.ClearUI();
            }

            temperature.minValue =
                ExperimentVariation == ExperimentVariation.LangmuirHinshelwood
                ? TemperatureStageValues[(int)ExperimentVariation.LangmuirHinshelwood][0] - 273.15f
                : TemperatureStageValues[(int)ExperimentVariation.MarsVanKrevelen][0] - 273.15f;
            temperature.maxValue =
                ExperimentVariation == ExperimentVariation.LangmuirHinshelwood
                    ? TemperatureStageValues[(int)ExperimentVariation.LangmuirHinshelwood][TemperatureStageValues[(int)ExperimentVariation.LangmuirHinshelwood].Length - 1] - 273.15f
                    : TemperatureStageValues[(int)ExperimentVariation.MarsVanKrevelen][TemperatureStageValues[(int)ExperimentVariation.MarsVanKrevelen].Length - 1] - 273.15f;

            partialPressure.minValue =
                ExperimentVariation == ExperimentVariation.LangmuirHinshelwood
                    ? PartialPressureValues[(int)ExperimentVariation.LangmuirHinshelwood][0]
                    : PartialPressureValues[(int)ExperimentVariation.MarsVanKrevelen][0];
            partialPressure.maxValue =
                ExperimentVariation == ExperimentVariation.LangmuirHinshelwood
                    ? PartialPressureValues[(int)ExperimentVariation.LangmuirHinshelwood][PartialPressureValues[(int)ExperimentVariation.LangmuirHinshelwood].Length - 1]
                    : PartialPressureValues[(int)ExperimentVariation.MarsVanKrevelen][PartialPressureValues[(int)ExperimentVariation.MarsVanKrevelen].Length - 1];

            temperature.Value = temperature.minValue;
            partialPressure.Value = partialPressure.minValue;

            if (temperatureView && partialPressureView)
            {
                temperatureView.ShowUI();
                partialPressureView.ShowUI();
            }
            else if (temperatureViewVr && partialPressureViewVr)
            {
                temperatureViewVr.RecalibrateRange(temperature.minValue, temperature.maxValue);
                partialPressureViewVr.RecalibrateRange(partialPressure.minValue, partialPressure.maxValue);
                temperatureViewVr.ForceToValue(temperature.minValue);
                partialPressureViewVr.ForceToValue(partialPressure.minValue);
            }

        }

        private void DrawSimulationGraphs()
        {
            graphImage.gameObject.SetActive(false);
            if (ExperimentVariation == ExperimentVariation.LangmuirHinshelwood)
            {
                if (_doInteractiveSimulation)
                {
                    lineChartLangmuir.gameObject.SetActive(true);
                    lineChartLangmuir.RefreshChart();
                }
                
            }
            else
            {
                if (_doInteractiveSimulation)
                {
                    lineChartVanKrevelen.gameObject.SetActive(true);
                    lineChartVanKrevelen.RefreshChart();
                }
            }
        }

        public void StartSimulation()
        {
            Debug.Log("Start catalyst simulation");
            DoStepWiseSimulation = _doStepWiseSimulation;
            if (graphImage && _doInteractiveSimulation)
                graphImage.sprite = ExperimentVariation == ExperimentVariation.LangmuirHinshelwood
                    ? graphLangmuirSprite
                    : graphVanKrevelenSprite;
            else if (whiteboardController && _doInteractiveSimulation)
                whiteboardController.SelectLecture(ExperimentVariation == ExperimentVariation.LangmuirHinshelwood ? 0 : 1);
        }

        public void StopSimulation()
        {
            Debug.Log("Stop catalyst simulation");
        }

        public void Reset()
        {
            EnsureCleanSurface();

            if (graphImage)
                graphImage.sprite = null;
            
            catalystReactionBoxGameObject.SetActive(false);
            player.transform.position = experimentRoomPlayerSpawnTransform.position;
            
            lineChartLangmuir.gameObject.SetActive(false);
            lineChartVanKrevelen.gameObject.SetActive(false);
            
            _doStepWiseSimulation = false;
            _doInteractiveSimulation = true;
        }

        public void TemperatureChanged(float newTemperature)
        {
            if (!temperatureView && newTemperature != temperature.Value) // in vr scene - no QuantityView
                temperature.Value = newTemperature;
            // update turnover rates
            _currentTurnOverRate = TurnOverRates[(int)ExperimentVariation][GetTemperatureIndex(temperature.Value)][GetPartialPressureIndex(partialPressure.Value)];
            UpdateTurnOverRateUI();
        }

        public void PartialPressureChanged(float newPartialPressure)
        {
            if (!partialPressureView && newPartialPressure != partialPressure.Value) // in vr scene - no QuantityView
                partialPressure.Value = newPartialPressure;
            // update turnover rates
            _currentTurnOverRate = TurnOverRates[(int)ExperimentVariation][GetTemperatureIndex(temperature.Value)][GetPartialPressureIndex(partialPressure.Value)];
            UpdateTurnOverRateUI();
        }
        
        public void ChangExperimentVariation(int val)
        {
            ExperimentVariation = (ExperimentVariation)val;
            CurrentExperimentStage = ExperimentVariation == ExperimentVariation.LangmuirHinshelwood ? HinshelwoodStages[0] : KrevelenStages[0];
            if (currentStepText.gameObject.activeSelf)
                currentStepText.text = CurrentExperimentStage.ToString();
            SetSimulationParametersMinMax(ExperimentVariation);
        }

        public void SpawnO2ButtonClicked()
        {
            SpawnReactionMaterial(true, false);
        }

        public void SpawnCOButtonClicked()
        {
            SpawnReactionMaterial(false, true);
        }

        public void StepWiseSimulationValueChanged(bool val)
        {
            _doStepWiseSimulation = val;
            if (stepWiseEnableText)
                stepWiseEnableText.text = _doStepWiseSimulation ? "enabled" : "disabled";
            currentStepText.gameObject.SetActive(val);
            currentStepText.text = CurrentExperimentStage.ToString();
        }

        public void NextStepButtonClicked()
        {
            if (ExperimentVariation == ExperimentVariation.LangmuirHinshelwood)
            {
                if (CurrentExperimentStage == HinshelwoodStages[HinshelwoodStages.Count - 1])
                    CurrentExperimentStage = HinshelwoodStages[1];
                else
                    CurrentExperimentStage = HinshelwoodStages[HinshelwoodStages.IndexOf(CurrentExperimentStage) + 1];
            }
            else if (ExperimentVariation == ExperimentVariation.MarsVanKrevelen)
            {
                if (CurrentExperimentStage == KrevelenStages[KrevelenStages.Count - 1])
                    CurrentExperimentStage = KrevelenStages[1];
                else
                    CurrentExperimentStage = KrevelenStages[KrevelenStages.IndexOf(CurrentExperimentStage) + 1];
            }

            currentStepText.text = CurrentExperimentStage.ToString();
        }

        public void InteractiveSimulationValueChanged(bool val)
        {
            _doInteractiveSimulation = val;
            if (interactiveSimulationText)
                interactiveSimulationText.text = _doInteractiveSimulation ? "enabled" : "disabled";
        }

        private static int GetTemperatureIndex(float temperatureValue)
        {
            // add 273.16 instead of 273.15 to always get at least the first element index
            int idx = Array.IndexOf(TemperatureStageValues[(int)ExperimentVariation], TemperatureStageValues[(int)ExperimentVariation].TakeWhile(num => num <= temperatureValue + 273.16f).LastOrDefault());
            if (idx < 0)
                idx = 0;
            return idx;
        }

        private static int GetPartialPressureIndex(float partialPressureValue)
        {
            int idx = Array.IndexOf(PartialPressureValues[(int)ExperimentVariation], PartialPressureValues[(int)ExperimentVariation].TakeWhile(num => num <= partialPressureValue + 0.00001f).LastOrDefault());
            if (idx < 0)
                idx = 0;
            return idx;
        }

        public static float GetTurnOverFrequency(float temperature, float partialPressure)
        {
            return TurnOverRates[(int)ExperimentVariation][GetTemperatureIndex(temperature)][GetPartialPressureIndex(partialPressure)];
        }
    }
}