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

        [SerializeField] Molecule o2MoleculePrefab;
        [SerializeField] Molecule coMoleculePrefab;
        [SerializeField] Molecule oMoleculePrefab;
        [SerializeField] int numSpawnedMolecules;

        [Header("Player specific objects")]
        [SerializeField] GameObject player;
        
        private CatalystSurface _catalystSurface;
        private List<Molecule> _activeMolecules = new List<Molecule>();

        public const float FixedMoleculeYDist = 0.18f; // todo move to own const class?

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
            for (int i = 0; i < numSpawnedMolecules; i++)
            {
                Vector3 spawnPos = new Vector3(Random.Range(0.2f, 3.2f), Random.Range(0.2f, 1.3f), Random.Range(0.2f, 3.2f));
                Quaternion spawnRot = Quaternion.Euler(Random.Range(-180.0f, 180.0f),Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
                Molecule molecule = Instantiate(prefabs[Random.Range(0, prefabs.Length)], catalystSurfaceTransform);
                molecule.gameObject.transform.localPosition = spawnPos;
                molecule.gameObject.transform.localRotation = spawnRot;
                _activeMolecules.Add(molecule);
                if (molecule.Type == MoleculeType.O2)
                {
                    molecule.OnDissociate += DissociateO2;
                }
            }
        }

        private void DissociateO2(Molecule o2Molecule)
        {
            _activeMolecules.Remove(o2Molecule);
            Transform parentTransform = o2Molecule.transform.parent;
            Vector3 o2Position = o2Molecule.transform.position;
            o2Molecule.ConnectedMolecule.ConnectedMolecule = null;
            Destroy(o2Molecule.gameObject);
            List<Molecule> oMolecules = new List<Molecule>()
            {
                Instantiate(oMoleculePrefab, parentTransform),
                Instantiate(oMoleculePrefab, parentTransform)
            };

            foreach (Molecule molecule in oMolecules)
            {
                molecule.IsFixedMolecule = true;
                molecule.transform.position = o2Position;
                _activeMolecules.Add(molecule);
            }
        }
    }
}