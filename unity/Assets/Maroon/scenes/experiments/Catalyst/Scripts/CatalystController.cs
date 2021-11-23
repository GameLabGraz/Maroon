using System.Collections;
using System.Collections.Generic;
using Maroon.Physics;
using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts
{
    public class CatalystController : MonoBehaviour
    {
        [Header("Simulation Parameters")]
        [SerializeField] QuantityFloat temperature;
        [SerializeField] CatalystSurfaceSize catalystSurfaceSize = CatalystSurfaceSize.Small;
        
        [Header("Catalyst specific objects")]
        [SerializeField] CatalystReactor catalystReactor;
        [SerializeField] CatalystSurface catalystSurfacePrefab;
        [SerializeField] Transform catalystSurfaceSpawnTransform;

        [SerializeField] Molecule oMoleculePrefab;
        [SerializeField] Molecule o2MoleculePrefab;
        [SerializeField] Molecule coMoleculePrefab;
        [SerializeField] Molecule co2MoleculePrefab;
        [SerializeField] int numSpawnedMolecules;

        [Header("Player specific objects")]
        [SerializeField] GameObject player;
        
        private CatalystSurface _catalystSurface;
        private List<Molecule> _activeMolecules = new List<Molecule>();

        public const float FixedMoleculeYDist = 0.28f;
        public const float PlatinumScale = 0.14f;

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
            _catalystSurface.Setup((int)catalystSurfaceSize, list =>
            {
                _activeMolecules = list;
                StartCoroutine(SpawnCatalystReactionMaterial());
            });
        }

        private IEnumerator SpawnCatalystReactionMaterial()
        {
            Transform catalystSurfaceTransform = _catalystSurface.GetComponentInChildren<CatalystSurface>().transform;
            yield return new WaitForSeconds(2.0f); // use simple delay for now
            Molecule[] prefabs = new Molecule[] { coMoleculePrefab, o2MoleculePrefab };
            float maxOffset = PlatinumScale * (int)catalystSurfaceSize;
            for (int i = 0; i < numSpawnedMolecules; i++)
            {
                Vector3 spawnPos = new Vector3(Random.Range(0.1f, 0.1f + maxOffset), Random.Range(0.5f, 1.3f), Random.Range(0.1f, 0.1f + maxOffset));
                Quaternion spawnRot = Quaternion.Euler(Random.Range(-180.0f, 180.0f),Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
                Molecule molecule = Instantiate(prefabs[Random.Range(0, prefabs.Length)], catalystSurfaceTransform);
                molecule.gameObject.transform.localPosition = spawnPos;
                molecule.gameObject.transform.localRotation = spawnRot;
                AddMoleculeToActiveList(molecule);
                if (molecule.Type == MoleculeType.O2)
                {
                    molecule.OnDissociate += DissociateO2;
                }
            }
        }

        private void DissociateO2(Molecule o2Molecule)
        {
            o2Molecule.OnDissociate -= DissociateO2;
            RemoveMoleculeFromActiveList(o2Molecule);
            Transform parentTransform = o2Molecule.transform.parent;
            Vector3 o2Position = o2Molecule.transform.position;
            o2Molecule.ConnectedMolecule.ConnectedMolecule = null;
            temperature.onValueChanged.RemoveListener(o2Molecule.TemperatureChanged);
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
                molecule.IsFixedMolecule = true;
                molecule.gameObject.transform.position = new Vector3(alternate ? o2Position.x + PlatinumScale / 4.0f : o2Position.x - PlatinumScale / 4.0f, o2Position.y - 0.04f, o2Position.z);
                AddMoleculeToActiveList(molecule);
                alternate = !alternate;
            }
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
            temperature.onValueChanged.AddListener(molecule.TemperatureChanged);
            _activeMolecules.Add(molecule);
        }

        private void RemoveMoleculeFromActiveList(Molecule molecule)
        {
            temperature.onValueChanged.RemoveListener(molecule.TemperatureChanged);
            _activeMolecules.Remove(molecule);
        }
    }
}