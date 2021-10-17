using System.Collections;
using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts
{
    public class CatalystController : MonoBehaviour
    {
        [Header("Catalyst specific objects")]
        [SerializeField] CatalystReactor catalystReactor;
        [SerializeField] CatalystSurface catalystSurfacePrefab;
        [SerializeField] Transform catalystSurfaceSpawnTransform;

        [SerializeField] Molecule o2MoleculePrefab;
        [SerializeField] Molecule coMoleculePrefab;
        [SerializeField] int numSpawnedMolecules;

        [Header("Player specific objects")]
        [SerializeField] GameObject player;
        
        private CatalystSurface _catalystSurface;

        private void Start()
        {
            SimulationController.Instance.OnStart.AddListener(StartSimulation);
            SimulationController.Instance.OnStop.AddListener(StopSimulation);

            catalystReactor.OnSpawnCatalystSurface += SpawnCatalystSurfaceObject;
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
            StartCoroutine(SpawnCatalystReactionMaterial());
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
            }
        }
    }
}