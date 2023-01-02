using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using GameLabGraz.VRInteraction;
using Maroon.Physics;
using Maroon.UI;
using PrivateAccess;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XCharts;
using Random = UnityEngine.Random;

namespace Maroon.Chemistry.Catalyst
{
    public enum ExperimentVariation
    {
        LangmuirHinshelwood,
        MarsVanKrevelen,
        EleyRideal
    }

    public enum ExperimentStages
    {
        Init,
        CODesorb,
        O2Desorb,
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
        [SerializeField] Material catalystBoxMaterial;
        [SerializeField] ParticleSystem pressureParticleSystem;
        
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
        [SerializeField] WhiteboardController whiteboardController;
        [SerializeField] WhiteboardController whiteboardControllerBox;
        [SerializeField] List<Image> theoryImages; // ordered same as the experiment variants!
        [SerializeField] CatalystVrControlPanel controlPanel;
        [Header("order line charts the same as the experiment variant enum!")]
        [SerializeField] List<XCharts.LineChart> lineCharts;
        [SerializeField] List<XCharts.LineChart> lineChartsVRBox;
        [SerializeField] XCharts.GaugeChart progressChart;
        [SerializeField] GameObject questManagerLabObject;
        [Header("order quest manager objects the same as the experiment variant enum!")]
        [SerializeField] List<GameObject> questManagerVariantObjects;
        

        private int _freedMoleculeCounter = 0;
        private int _moleculesToFree = 4;
        private List<Vector3> _platSpawnPoints = new List<Vector3>();
        private List<Vector3> _activePlatSpawnPoints = new List<Vector3>();
        private List<Vector3> _coSpawnPoints = new List<Vector3>();
        private List<Vector3> _oSpawnPoints = new List<Vector3>();
        private List<Molecule> _activeMolecules = new List<Molecule>();
        private CatalystSurface _catalystSurface;
        
        private float _currentTurnOverRate = 0.0f;
        private bool _doStepWiseSimulation = false;
        private bool _doInteractiveSimulation = true;

        private bool _resetPlayer = false;

        private System.Action onReactionStart;

        private float _minXValLocal = 0.0f;
        private float _maxXValLocal = 0.0f;
        private float _minZValLocal = 0.0f;
        private float _maxZValLocal = 0.0f;

        private List<List<Serie>> _graphSeriesList = new List<List<Serie>>();
        
        private static float EleyTemperatureValue = 0;
        
        private static readonly Regex WhiteSpaces = new Regex(@"\s+");

        private static readonly int[][] TemperatureStageValues = new int [][]
        {
            new int[] { 250, 275, 300, 325, 350, 375, 400, 425, 450 }, // langmuir-hinshelwood
            new int[] { 321, 334, 348, 363 }, // mars van krevelen
            new int[] { 481, 499, 519, 533, 553 } // eley-rideal
        };
        
        private static readonly float[][] PartialPressureValues = new float[][]
        {
            new float[] { 0.01f, 0.02f, 0.04f, 0.2f }, // langmuir-hinshelwood
            new float[] { 0.001f, 0.002f, 0.004f, 0.008f, 0.014f, 0.026f }, // mars van krevelen
            // eley-rideal: pressure is different for each temperature stage
            // each row below corresponds on one temperature (first line = pressure values for 208 in temp values)
            new float[]
            {
                0.0010855f, 0.0019737f, 0.0062171f, 0.011053f, 0.001523f, 0f, 0f, // temperature 208
                0.00069079f, 0.0016118f, 0.0023355f, 0.0031579f, 0.0065461f, 0.008125f, 0.013257f, // temperature 226
                0.0010197f, 0.0022697f, 0.0028947f, 0.0042763f, 0.0098355f, 0.012434f, 0.015822f, // temperature 246
                0.0013816f, 0.0031908f, 0.0045395f, 0.0054934f, 0.0066447f, 0.0077961f, 0f, // temperature 260
                0.0019408f, 0.0025658f, 0.0036513f, 0.0046053f, 0.0055921f, 0.0070395f, 0.0089803f // temperature 280
            } 
        };
        
        private static readonly float[][][] TurnOverRates = new float[][][]
        {
            new float[][] // langmuir-hinshelwood
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
            new float[][] // mars van krevelen
            {
                new float[] { 0.042146f, 0.095785f, 0.12644f, 0.18008f, 0.23372f, 0.29502f },
                new float[] { 0.16475f, 0.341f, 0.4636f, 0.61686f, 0.75479f, 0.83908f },
                new float[] { 0.26437f, 0.54023f, 0.71648f, 1.023f, 1.1686f, 1.3678f },
                new float[] { 0.63985f, 1.1456f, 1.5747f, 2.3563f, 2.9234f, 3.6743f }
            },
            new float[][] // eley-rideal
            {
                new float[] { 0.000721806f, 0.001361116f, 0.001051789f, 0.000907325f, 0.00078371f, 0f, 0f },
                new float[] { 0.001381773f, 0.002351066f, 0.002722298f, 0.002887225f, 0.002206666f, 0.001938587f, 0.001381773f },
                new float[] { 0.001979835f, 0.003279112f, 0.004145254f, 0.00486706f, 0.003567847f, 0.002887225f, 0.002227323f },
                new float[] { 0.002598554f, 0.005258947f, 0.006846895f, 0.007424624f, 0.008682072f, 0.009775561f, 0f },
                new float[] { 0.004928964f, 0.006537697f, 0.008455499f, 0.010105415f, 0.011280884f, 0.012786207f, 0.014312831f }
            },
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

        private static readonly List<ExperimentStages> EleyStages = new List<ExperimentStages>()
        {
            ExperimentStages.Init,
            ExperimentStages.O2Adsorb_O2Dissociate,
            ExperimentStages.OReactCO_CO2Desorb
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
        
        public bool AreHinshelwoodMoleculesFreed { get => _freedMoleculeCounter == _moleculesToFree; }
        public bool HasInitialTempChanged { get => Mathf.Abs(temperature.Value - temperatureViewVr.initialValue) > 20.0f ; }

        public bool HasInitialPressureChanged
        {
            get => Mathf.Abs(partialPressure.Value - partialPressureViewVr.initialValue) > PartialPressureValues[(int) ExperimentVariation][1];
        }

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
            foreach (var chart in lineCharts)
            {
                chart.gameObject.SetActive(false);
            }
            progressChart.gameObject.SetActive(false);
            progressChart.series.list[0].data[0].data[1] = 0.0f;
            
            if (isVrVersion)
            {
                foreach (var chart in lineChartsVRBox)
                {
                    chart.gameObject.SetActive(false);
                }

                foreach (var questManagerObject in questManagerVariantObjects)
                {
                    questManagerObject.SetActive(false);
                }
            }
            else
            {
                foreach (var img in theoryImages)
                {
                   img.gameObject.SetActive(false); 
                }
            }

            foreach (var chart in lineCharts)
            {
                _graphSeriesList.Add(new List<Serie>(chart.series.list));
            }


        }

        private void StartExperiment()
        {
            if (_doInteractiveSimulation)
                HandleCatalystSurfaceSetup();
            if (!isVrVersion)
                DrawSimulationGraphsPC();
            else
                DrawSimulationGraphsVR();

            TryEnableQuestManager();
        }

        private void OnDestroy()
        {
            catalystReactor.OnReactorFilled.RemoveListener(StartExperiment);
            catalystBoxMaterial.color = Color.black;
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
            _resetPlayer = true;
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
            else if (ExperimentVariation.Equals(ExperimentVariation.EleyRideal))
            {
                _catalystSurface.SetupCoordsEley(_platSpawnPoints,_activePlatSpawnPoints,list =>
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
                        onReactionStart?.Invoke();
                    });
            }
            if (controlPanel)
                controlPanel.Setup(Mathf.Min(MaxXCoord - MinXCoord, MaxZCoord - MinZCoord), _doStepWiseSimulation);
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
                    if (ExperimentVariation == ExperimentVariation.MarsVanKrevelen)
                        (molecule as O2Molecule)?.SetDarkMaterial();
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
                    molecule.ActivateDrawingCollider(true);
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
                    (molecule as OMolecule)?.SetDarkMaterial();
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
            _freedMoleculeCounter++;
            if (_freedMoleculeCounter == _moleculesToFree)
            {
                onReactionStart?.Invoke();
            }
        }

        /**
         * Creates CO2 molecule. Called from O atoms when they are near a CO molecule that
         * they have been drawn to.
         */
        private void CreateCO2(Molecule oMolecule, Molecule coMolecule)
        {
            oMolecule.OnCO2Created -= CreateCO2;
            RemoveMoleculeFromActiveList(oMolecule);
            RemoveMoleculeFromActiveList(coMolecule);
            Transform parentTransform = coMolecule ? coMolecule.gameObject.transform.parent : oMolecule.gameObject.transform.parent;
            Vector3 coPosition = coMolecule ? coMolecule.gameObject.transform.position : oMolecule.gameObject.transform.position;
            Quaternion coRotation = coMolecule ? coMolecule.gameObject.transform.rotation : oMolecule.gameObject.transform.rotation;
            if (coMolecule)
            {
                if (coMolecule.ConnectedMolecule) // eley-rideal CO has no connected molecule
                {
                    coMolecule.ConnectedMolecule.ActivateDrawingCollider(true);
                    coMolecule.ConnectedMolecule = null;
                }
                Destroy(coMolecule.gameObject);
            }

            Destroy(oMolecule.gameObject);

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
            if (ExperimentVariation == ExperimentVariation.EleyRideal)
                turnOverRateText.text = (100 * TurnOverRates[(int) ExperimentVariation][GetTemperatureIndex(temperature)][GetPartialPressureIndex(partialPressure)]).ToString(CultureInfo.InvariantCulture.NumberFormat);
            else
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
            if (!isVrVersion)
            {
                temperatureView.ClearUI();
                partialPressureView.ClearUI();
            }

            // use faster direct array access since temperature is sorted
            temperature.minValue = TemperatureStageValues[(int) variation][0] - 273.15f;
            temperature.maxValue = TemperatureStageValues[(int) variation][TemperatureStageValues[(int) variation].Length - 1] - 273.15f;
            // use min max due to eley-rideal partial pressure not being sorted
            partialPressure.minValue = PartialPressureValues[(int) variation].Min();
            partialPressure.maxValue = PartialPressureValues[(int) variation].Max();

            temperature.Value = temperature.minValue;
            partialPressure.Value = partialPressure.minValue;

            if (isVrVersion)
            {
                temperatureViewVr.SetMinMax(temperature.minValue, temperature.maxValue);
                partialPressureViewVr.SetMinMax(partialPressure.minValue, partialPressure.maxValue);
                temperatureViewVr.ForceToValue(temperature.minValue);
                partialPressureViewVr.ForceToValue(partialPressure.minValue);
            }
            else
            {
                temperatureView.ShowUI();
                partialPressureView.ShowUI();
            }

        }

        private void DrawSimulationGraphsPC()
        {
            LineChart lineChart = lineCharts[(int) ExperimentVariation];
            lineChart.gameObject.SetActive(true);
            if (_doInteractiveSimulation)
            {
                lineChart.RefreshChart();
            }
            else
            {
                lineChart.series.RemoveAll();
                StartCoroutine(CoDrawSimulationGraphs(lineChart, _graphSeriesList[(int)ExperimentVariation], 1.6f));
                progressChart.gameObject.SetActive(true);
                StartCoroutine(CoDrawProgressGraph());
            }
        }
        
        private void DrawSimulationGraphsVR()
        {
            if (_doInteractiveSimulation)
            {
                LineChart lineChartVRBox = lineChartsVRBox[(int) ExperimentVariation];
                lineChartVRBox.gameObject.SetActive(true);
                lineChartVRBox.RefreshChart();
            }
            else
            {
                LineChart lineChart = lineCharts[(int) ExperimentVariation];
                lineChart.gameObject.SetActive(true);
                lineChart.series.RemoveAll();
                StartCoroutine(CoDrawSimulationGraphs(lineChart, _graphSeriesList[(int)ExperimentVariation], 1.6f));
                progressChart.gameObject.SetActive(true);
                StartCoroutine(CoDrawProgressGraph());
            }
        }

        private IEnumerator CoDrawSimulationGraphs(LineChart lineChart, List<Serie> initialSeries, float waitTime)
        {
            int serieCount = 0;
            foreach (var initialSerie in initialSeries)
            {
                Serie serie = lineChart.AddSerie(initialSerie.type, initialSerie.name, initialSerie.show);
                serie.lineType = initialSerie.lineType;
                PrivateAccessExtension.SetFieldValue(serie, "showDataDimension", initialSerie.showDataDimension);
                var serieData = initialSerie.data;
                foreach (var data in serieData)
                {
                    lineChart.AddData(serieCount, new List<float>(data.data));
                    lineChart.RefreshChart();
                    yield return new WaitForSeconds(waitTime);
                }
                serieCount++;
            }
        }

        private IEnumerator CoDrawProgressGraph()
        {
            var waitTime = 0f;
            if (ExperimentVariation == ExperimentVariation.LangmuirHinshelwood)
                waitTime = 4f * 9f * 1.6f; // 4 series * 9 data entries -> 1.6 seconds per entry
            else if (ExperimentVariation == ExperimentVariation.MarsVanKrevelen)
                waitTime = 4f * 6f * 1.6f; // 4 series * 6 data entries -> 1.6 seconds per entry
            else if (ExperimentVariation == ExperimentVariation.EleyRideal)
                waitTime = (5f + 6f + 3f * 7f) * 1.6f; // 5 series (1 with 5 data points, 1 with 6 data points, and 3 with 7 data points) -> 1.6 seconds per entry
            waitTime /= 360;
            var progress = 0.0f;
            while (progress < 360f)
            {
                progress += 1.0f;
                progressChart.series.list[0].data[0].data[1] = progress;
                progressChart.RefreshChart();
                yield return new WaitForSeconds(waitTime);
            }
        }
        
        private void RestoreLineGraphObjects()
        {
            for (int i = 0; i < lineCharts.Count; i++)
            {
                LineChart lineChart = lineCharts[i];
                List<Serie> initialSeries = _graphSeriesList[i];
                if (lineChart == null) return;
                lineChart.series.RemoveAll();
                for (int j = 0; j < initialSeries.Count; j++)
                {
                    Serie serie = lineChart.AddSerie(initialSeries[j].type, initialSeries[j].name, initialSeries[j].show);
                    serie.lineType = initialSeries[j].lineType;
                    PrivateAccessExtension.SetFieldValue(serie, "showDataDimension", initialSeries[j].showDataDimension);
                    var serieData = initialSeries[j].data;
                    foreach (var data in serieData)
                    {
                        lineChart.AddData(j, new List<float>(data.data));
                    }
                }
            }
        }

        private void TryEnableQuestManager()
        {
            if (isVrVersion && _doInteractiveSimulation)
            {
                questManagerVariantObjects[(int) ExperimentVariation].SetActive(true);
            }
        }

        public void StartSimulation()
        {
            Debug.Log("Start catalyst simulation");
            DoStepWiseSimulation = _doStepWiseSimulation;
            
            SetSimulationParametersMinMax(ExperimentVariation);

            if (isVrVersion)
            {
                whiteboardController.SelectLecture(0);
                whiteboardControllerBox.SelectLecture(0);
            }
            else
            {
                theoryImages[(int)ExperimentVariation].gameObject.SetActive(true);
            }
        }

        public void StopSimulation()
        {
            Debug.Log("Stop catalyst simulation");
        }

        public void Reset()
        {
            _freedMoleculeCounter = 0;
            EnsureCleanSurface();

            catalystReactionBoxGameObject.SetActive(false);
            if (_resetPlayer)
                player.transform.position = experimentRoomPlayerSpawnTransform.position;
            _resetPlayer = false;
            

            StopAllCoroutines();
            RestoreLineGraphObjects();
            foreach (var chart in lineCharts)
            {
                chart.gameObject.SetActive(false);
            }
            progressChart.gameObject.SetActive(false);
            progressChart.series.list[0].data[0].data[1] = 0.0f;
            if (isVrVersion)
            {
                foreach (var chart in lineChartsVRBox)
                {
                    chart.gameObject.SetActive(false);
                }
                questManagerLabObject.GetComponent<QuestManager.QuestManager>().ResetQuests();
                foreach (var questManagerVariantObject in questManagerVariantObjects)
                {
                    questManagerVariantObject.GetComponent<QuestManager.QuestManager>().ResetQuests();
                    questManagerVariantObject.SetActive(false);
                }
                if (controlPanel)
                    controlPanel.ResetToInitialPosition();
            }
            else
            {
                foreach (var img in theoryImages)
                {
                    img.gameObject.SetActive(false); 
                }
            }

            _doStepWiseSimulation = false;
            _doInteractiveSimulation = true;
            
            catalystBoxMaterial.color = Color.black;
        }

        public void TemperatureChanged(float newTemperature)
        {
            if (!temperatureView && newTemperature != temperature.Value) // in vr scene - no QuantityView
                temperature.Value = newTemperature;

            if (catalystBoxMaterial != null)
            {
                float redVal = Mathf.Clamp(newTemperature / 2, 0.0f, 80.0f);
                Color currentColor = catalystBoxMaterial.color;
                currentColor.r = redVal / 255.0f;
                catalystBoxMaterial.color = currentColor;
            }
            // update turnover rates
            _currentTurnOverRate = TurnOverRates[(int)ExperimentVariation][GetTemperatureIndex(temperature.Value)][GetPartialPressureIndex(partialPressure.Value)];
            UpdateTurnOverRateUI();
        }

        public void PartialPressureChanged(float newPartialPressure)
        {
            if (!partialPressureView && newPartialPressure != partialPressure.Value) // in vr scene - no QuantityView
                partialPressure.Value = newPartialPressure;
            // update turnover rates
            if (pressureParticleSystem != null)
            {
                // adjust speed for newly spawned particle
                float pressureStep = partialPressure.Value / partialPressure.maxValue; // % of max
                var main = pressureParticleSystem.main;
                main.simulationSpeed = Mathf.Clamp(pressureStep * 2, 0.2f, 2.0f); // speed should go from 0.2 to 2
            }
            _currentTurnOverRate = TurnOverRates[(int)ExperimentVariation][GetTemperatureIndex(temperature.Value)][GetPartialPressureIndex(partialPressure.Value)];
            UpdateTurnOverRateUI();
        }
        
        public void ChangExperimentVariation(int val)
        {
            ExperimentVariation = (ExperimentVariation)val;
            if (ExperimentVariation == ExperimentVariation.LangmuirHinshelwood)
                CurrentExperimentStage = HinshelwoodStages[0];
            else if (ExperimentVariation == ExperimentVariation.MarsVanKrevelen)
                CurrentExperimentStage = KrevelenStages[0];
            else if (ExperimentVariation == ExperimentVariation.EleyRideal)
                CurrentExperimentStage = EleyStages[0];
            
            if (currentStepText.gameObject.activeSelf)
                currentStepText.text = CurrentExperimentStage.ToString();
            SetSimulationParametersMinMax(ExperimentVariation);
            
            if (isVrVersion)
            {
                whiteboardController.SelectLecture(0);
                whiteboardControllerBox.SelectLecture(0);
                whiteboardController.Refresh();
                whiteboardControllerBox.Refresh();
            }
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
            else if (ExperimentVariation == ExperimentVariation.EleyRideal)
            {
                if (CurrentExperimentStage == EleyStages[EleyStages.Count - 1])
                    CurrentExperimentStage = EleyStages[1];
                else
                    CurrentExperimentStage = EleyStages[EleyStages.IndexOf(CurrentExperimentStage) + 1];
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
            
            EleyTemperatureValue = temperatureValue;
            return idx;
        }

        private static int GetPartialPressureIndex(float partialPressureValue)
        {
            int idx = 0;
            if (ExperimentVariation == ExperimentVariation.EleyRideal)
            {
                int tempStage = GetTemperatureIndex(EleyTemperatureValue);
                int pressureValsPerTemp = PartialPressureValues[(int) ExperimentVariation].Length /
                                          TemperatureStageValues[(int) ExperimentVariation].Length;
                // get offset into the pressure array row
                // stage * pressure vals per temp = first of correct row
                int offsetIntoPressureArray = tempStage * pressureValsPerTemp;

                // get slice of array for the correct temperature row with 7 values
                var arraySlice = new ArraySegment<float>(PartialPressureValues[(int) ExperimentVariation],
                    offsetIntoPressureArray, pressureValsPerTemp).ToArray();
                idx = Array.IndexOf(arraySlice, arraySlice.OrderBy(num => num).TakeWhile(num => num <= partialPressureValue + 0.00001f).LastOrDefault());
            }
            else 
                idx = Array.IndexOf(PartialPressureValues[(int)ExperimentVariation], PartialPressureValues[(int)ExperimentVariation].TakeWhile(num => num <= partialPressureValue + 0.00001f).LastOrDefault());
            if (idx < 0)
                idx = 0;
            return idx;
        }

        public static float GetTurnOverFrequency(float temperature, float partialPressure)
        {
            if (ExperimentVariation == ExperimentVariation.EleyRideal)
                return 100 * TurnOverRates[(int) ExperimentVariation][GetTemperatureIndex(temperature)][GetPartialPressureIndex(partialPressure)];
            return TurnOverRates[(int)ExperimentVariation][GetTemperatureIndex(temperature)][GetPartialPressureIndex(partialPressure)];
        }
    }
}