using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Maroon.Physics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Maroon.scenes.experiments.Catalyst.Scripts
{
    public class CatalystController : MonoBehaviour
    {
        [Header("Simulation Parameters")]
        [SerializeField] QuantityFloat temperature;
        [SerializeField] QuantityFloat partialPressure;
        //[SerializeField] CatalystSurfaceSize catalystSurfaceSize = CatalystSurfaceSize.Small;
        
        [Header("Catalyst specific objects")]
        [SerializeField] CatalystReactor catalystReactor;
        [SerializeField] CatalystSurface catalystSurfacePrefab;
        [SerializeField] Transform catalystSurfaceSpawnTransform;

        [SerializeField] Molecule oMoleculePrefab;
        [SerializeField] Molecule o2MoleculePrefab;
        [SerializeField] Molecule coMoleculePrefab;
        [SerializeField] Molecule co2MoleculePrefab;
        [SerializeField] int numberSpawnedO2Molecules;
        [SerializeField] int numberSpawnedCOMolecules;

        [Header("Player specific objects")]
        [SerializeField] GameObject player;

        private int freedMoleculeCounter = 0;
        private List<Vector3> _platSpawnPoints = new List<Vector3>();
        private List<Molecule> _activeMolecules = new List<Molecule>();
        private CatalystSurface _catalystSurface;

        private System.Action onReactionStart;

        public const float FixedMoleculeYDist = 0.28f - 0.075f;
        public const float PlatinumScale = 0.14f;

        private void Awake()
        {
            var coordFile = Resources.Load<TextAsset>("Pt-111");
            string[] lines = coordFile.text.Split('\n');
            foreach (var line in lines)
            {
                string[] lineValues = line.Split(',');
                if (lineValues.Length == 3)
                {
                    _platSpawnPoints.Add(
                        new Vector3(
                            float.Parse(lineValues[0], CultureInfo.InvariantCulture.NumberFormat), 
                            float.Parse(lineValues[1], CultureInfo.InvariantCulture.NumberFormat), 
                            float.Parse(lineValues[2], CultureInfo.InvariantCulture.NumberFormat)
                            )
                        );
                }
            }
        }

        private void Start()
        {
            SimulationController.Instance.OnStart.AddListener(StartSimulation);
            SimulationController.Instance.OnStop.AddListener(StopSimulation);

            catalystReactor.OnSpawnCatalystSurface += SpawnCatalystSurfaceObject;
            SpawnCatalystSurfaceObject();
        }

        private void OnDestroy()
        {
            SimulationController.Instance.OnStart.RemoveListener(StartSimulation);
            SimulationController.Instance.OnStop.RemoveListener(StopSimulation);
            
            catalystReactor.OnSpawnCatalystSurface -= SpawnCatalystSurfaceObject;
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
            _catalystSurface = Instantiate(catalystSurfacePrefab, catalystSurfaceSpawnTransform);
            _catalystSurface.SetupCoords(_platSpawnPoints, list => 
                {
                    _activeMolecules = list;
                    foreach (var molecule in _activeMolecules)
                    {
                        onReactionStart += molecule.ReactionStart;
                    }
                    SpawnReactionMaterial();
                },
                OnMoleculeFreed);
        }

        private void SpawnReactionMaterial()
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
            AddMoleculeToActiveList(co2Molecule);
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
            temperature.onValueChanged.RemoveListener(molecule.TemperatureChanged);
            partialPressure.onValueChanged.RemoveListener(molecule.PressureChanged);
            _activeMolecules.Remove(molecule);
        }
    }
}