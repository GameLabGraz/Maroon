using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Maroon.Physics;
using Maroon.scenes.experiments.Catalyst.Scripts.Molecules;
using Maroon.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maroon.scenes.experiments.Catalyst.Scripts
{
    public class CatalystController : MonoBehaviour
    {
        [Header("Simulation Parameters")]
        [SerializeField] Dropdown variantDropdown;
        [SerializeField] QuantityFloat temperature;
        [SerializeField] QuantityFloat partialPressure;
        [SerializeField] int numberSpawnedO2Molecules;
        [SerializeField] int numberSpawnedCOMolecules;
        
        [Header("Catalyst specific objects")]
        [SerializeField] CatalystReactor catalystReactor;
        [SerializeField] CatalystSurface catalystSurfacePrefab;
        [SerializeField] Transform catalystSurfaceSpawnTransform;

        [Header("Molecule Prefabs")]
        [SerializeField] Molecule oMoleculePrefab;
        [SerializeField] Molecule o2MoleculePrefab;
        [SerializeField] Molecule coMoleculePrefab;
        [SerializeField] Molecule co2MoleculePrefab;
        [SerializeField] Molecule cobaltMoleculePrefab;

        [Header("Player specific objects")]
        [SerializeField] GameObject player;

        [Header("UI Elements")]
        [SerializeField] InputField turnOverRateInputField;

        private int freedMoleculeCounter = 0;
        private List<Vector3> _platSpawnPoints = new List<Vector3>();
        private List<Vector3> _coSpawnPoints = new List<Vector3>();
        private List<Vector3> _oSpawnPoints = new List<Vector3>();
        private List<Molecule> _activeMolecules = new List<Molecule>();
        private CatalystSurface _catalystSurface;
        
        private float _currentTurnOverRate = 0.0f;

        private static readonly int[] TemperatureStageValues = new[] { 250, 275, 300, 325, 350, 375, 400, 425, 450 };
        private static readonly float[] PartialPressureValues = new[] { 0.01f, 0.02f, 0.04f, 0.2f };
        public static readonly float[][] TurnOverRates = new float[][]
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
        };
        
        

        private System.Action onReactionStart;

        private static readonly Regex WhiteSpaces = new Regex(@"\s+");

        public const float FixedMoleculeYDist = 0.28f - 0.075f;
        public const float PlatinumScale = 0.14f;

        private void Awake()
        {
            var coordFile = Resources.Load<TextAsset>("Pt-111");
            FillSpawnPoints(coordFile);
            coordFile = Resources.Load<TextAsset>("Co3O4_111");
            FillSpawnPoints(coordFile);
        }

        private void Start()
        {
            SimulationController.Instance.OnStart.AddListener(StartSimulation);
            SimulationController.Instance.OnStop.AddListener(StopSimulation);

            catalystReactor.OnSpawnCatalystSurface += SpawnCatalystSurfaceObject;
            
            temperature.onValueChanged.AddListener(TemperatureChanged);
            partialPressure.onValueChanged.AddListener(PartialPressureChanged);
            
            SpawnCatalystSurfaceObject();
        }

        private void OnDestroy()
        {
            SimulationController.Instance.OnStart.RemoveListener(StartSimulation);
            SimulationController.Instance.OnStop.RemoveListener(StopSimulation);
            
            temperature.onValueChanged.RemoveListener(TemperatureChanged);
            partialPressure.onValueChanged.RemoveListener(PartialPressureChanged);
            
            catalystReactor.OnSpawnCatalystSurface -= SpawnCatalystSurfaceObject;
        }
        
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
        

        private void StartSimulation()
        {
            Debug.Log("Start catalyst simulation");
        }

        private void StopSimulation()
        {
            Debug.Log("Stop catalyst simulation");
        }

        private void SpawnCatalystSurfaceObject()
        {
            EnsureCleanSurface();

            _catalystSurface = Instantiate(catalystSurfacePrefab, catalystSurfaceSpawnTransform);
            if (variantDropdown.value == 0)
            {
                _catalystSurface.SetupCoords(_platSpawnPoints,list =>
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
                        SpawnReactionMaterial();
                    },
                    OnMoleculeFreed);
            }
            else if (variantDropdown.value == 1)
            {
                _catalystSurface.SetupOtherCoords(_coSpawnPoints, _oSpawnPoints, list =>
                    {
                        
                    },
                    OnMoleculeFreed);
            }
        }

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

        private void SpawnReactionMaterial(bool isSpawnButtonClicked = false)
        {
            Transform catalystSurfaceTransform = _catalystSurface.gameObject.transform;
            for (int i = 0; i < numberSpawnedO2Molecules; i++)
            {
                Vector3 spawnPos = new Vector3(Random.Range(-0.8f, 2.2f), Random.Range(0.5f, 2.0f), Random.Range(0.3f, -2.0f));
                Quaternion spawnRot = Quaternion.Euler(Random.Range(-180.0f, 180.0f),Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
                Molecule molecule = Instantiate(o2MoleculePrefab, catalystSurfaceTransform);
                molecule.gameObject.transform.localPosition = spawnPos;
                molecule.gameObject.transform.localRotation = spawnRot;
                molecule.OnDissociate += DissociateO2;
                molecule.State = MoleculeState.Moving;
                onReactionStart += molecule.ReactionStart;
                AddMoleculeToActiveList(molecule);
            }

            if (isSpawnButtonClicked) return;
            
            for (int i = 0; i < numberSpawnedCOMolecules; i++)
            {
                Vector3 spawnPos = new Vector3(Random.Range(-0.8f, 2.2f), Random.Range(0.5f, 2.0f), Random.Range(0.3f, -2.0f));
                Quaternion spawnRot = Quaternion.Euler(Random.Range(-180.0f, 180.0f),Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
                Molecule molecule = Instantiate(coMoleculePrefab, catalystSurfaceTransform);
                molecule.gameObject.transform.localPosition = spawnPos;
                molecule.gameObject.transform.localRotation = spawnRot;
                molecule.State = MoleculeState.Moving;
                onReactionStart += molecule.ReactionStart;
                AddMoleculeToActiveList(molecule);
            }
        }

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
                molecule.State = MoleculeState.Fixed;
                molecule.gameObject.transform.position = new Vector3(alternate ? o2Position.x + PlatinumScale / 4.0f : o2Position.x - PlatinumScale / 4.0f, o2Position.y - 0.06f, o2Position.z);
                AddMoleculeToActiveList(molecule);
                alternate = !alternate;
            }
        }

        private void OnMoleculeFreed()
        {
            if (freedMoleculeCounter == 4)
            {
                onReactionStart?.Invoke();
            }
            freedMoleculeCounter++;
        }

        private void CreateCO2(Molecule oMolecule, Molecule coMolecule)
        {
            oMolecule.OnCO2Created -= CreateCO2;
            RemoveMoleculeFromActiveList(oMolecule);
            RemoveMoleculeFromActiveList(coMolecule);
            Transform parentTransform = coMolecule.gameObject.transform.parent;
            Vector3 coPosition = coMolecule.gameObject.transform.position;
            
            coMolecule.ConnectedMolecule.ActivateDrawingCollider(true);
            coMolecule.ConnectedMolecule = null;
            
            Destroy(oMolecule.gameObject);
            Destroy(coMolecule.gameObject);

            Molecule co2Molecule = Instantiate(co2MoleculePrefab, parentTransform);
            co2Molecule.gameObject.transform.position = coPosition;
            // todo rotate co2 in up facing position
            co2Molecule.MoveOutCO2();
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

        private void TemperatureChanged(float newTemperature)
        {
            // update turnover rates
            _currentTurnOverRate = TurnOverRates[GetTemperatureIndex(temperature.Value)][GetPartialPressureIndex(partialPressure.Value)];
            UpdateTurnOverRateUI();
        }

        private void PartialPressureChanged(float newPartialPressure)
        {
            // update turnover rates
            _currentTurnOverRate = TurnOverRates[GetTemperatureIndex(temperature.Value)][GetPartialPressureIndex(partialPressure.Value)];
            UpdateTurnOverRateUI();
        }

        private void UpdateTurnOverRateUI()
        {
            turnOverRateInputField.text = _currentTurnOverRate.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        public void SpawnO2ButtonClicked()
        {
            SpawnReactionMaterial(true);
        }
        
        
        public static int GetTemperatureIndex(float temperatureValue)
        {
            // add 273.16 instead of 273.15 to always get at least the first element index
            return Array.IndexOf(TemperatureStageValues, TemperatureStageValues.TakeWhile(num => num <= temperatureValue + 273.16f).Last());
        }

        public static int GetPartialPressureIndex(float partialPressureValue)
        {
            return Array.IndexOf(PartialPressureValues, PartialPressureValues.TakeWhile(num => num <= partialPressureValue + 0.00001f).Last());
        }
    }
}