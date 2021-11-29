using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts
{
    public enum CatalystSurfaceSize
    {
        Small = 16,
        Medium = 32,
        Big = 64
    }

    public class CatalystSurface : MonoBehaviour
    {
        [SerializeField] Molecule platinumMoleculePrefab;
        [SerializeField] Molecule coMoleculePrefab;
        [SerializeField] Transform surfaceLayerParent;
        [SerializeField] int numSubLayers;
        [SerializeField] GameObject boundaryXMin;
        [SerializeField] GameObject boundaryXMax;
        [SerializeField] GameObject boundaryZMin;
        [SerializeField] GameObject boundaryZMax;
        [SerializeField] GameObject topLayerParent;
        
        private float _spaceBetweenMolecules;
        private List<Molecule> _topLayerMolecules = new List<Molecule>();

        public void SetupCoords(List<Vector3> platCoords, 
            System.Action<List<Molecule>> onComplete, 
            System.Action onMoleculeFreed,
            System.Action onReactionStart)
        {
            List<Molecule> activeMolecules = new List<Molecule>();
            foreach (var molecule in _topLayerMolecules)
            {
                Molecule coMolecule = Instantiate(coMoleculePrefab, surfaceLayerParent);
                coMolecule.State = MoleculeState.Fixed;
                            
                Vector3 moleculePos = molecule.transform.localPosition;
                moleculePos.y += CatalystController.FixedMoleculeYDist -0.075f;
                coMolecule.transform.localPosition = moleculePos;
                            
                Quaternion moleculeRot = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                coMolecule.transform.localRotation = moleculeRot;

                molecule.ConnectedMolecule = coMolecule;
                molecule.GetComponent<CapsuleCollider>().enabled = true;
                coMolecule.ConnectedMolecule = molecule;
                coMolecule.OnMoleculeFreed += onMoleculeFreed;
                onReactionStart += coMolecule.ReactionStart;
                            
                activeMolecules.Add(molecule);
                activeMolecules.Add(coMolecule);
            }
            onComplete?.Invoke(activeMolecules);
        }
        
        public void Setup(int surfaceSize, System.Action<List<Molecule>> onComplete)
        {
            float maxOffset = platinumMoleculePrefab.transform.GetChild(0).transform.localScale.x * surfaceSize;
            boundaryXMin.transform.localPosition = new Vector3(0.1f, 0.0f, 0.0f);
            boundaryZMin.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
            boundaryXMax.transform.localPosition = new Vector3(0.1f + maxOffset, 0.0f, 0.0f);
            boundaryZMax.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f + maxOffset);
            List<Molecule> activeMolecules = new List<Molecule>();
            _spaceBetweenMolecules = platinumMoleculePrefab.transform.GetChild(0).transform.localScale.x;
            for (int layerNum = 0; layerNum < numSubLayers; layerNum++)
            {
                Vector3 moleculePosition = surfaceLayerParent.position;
                moleculePosition.y += -layerNum * _spaceBetweenMolecules;
                for (int sizeX = 0; sizeX < surfaceSize; sizeX++)
                {
                    moleculePosition.x += _spaceBetweenMolecules;
                    for (int sizeZ = 0; sizeZ < surfaceSize; sizeZ++)
                    {
                        moleculePosition.z += _spaceBetweenMolecules;
                        Molecule platMolecule = Instantiate(platinumMoleculePrefab, surfaceLayerParent);
                        platMolecule.transform.position = moleculePosition;
                        platMolecule.State = MoleculeState.Fixed;
                        if (layerNum == 0)
                        {
                            Molecule coMolecule = Instantiate(coMoleculePrefab, surfaceLayerParent);
                            coMolecule.State = MoleculeState.Fixed;
                            
                            Vector3 moleculePos = platMolecule.transform.localPosition;
                            moleculePos.y = CatalystController.FixedMoleculeYDist;
                            coMolecule.transform.localPosition = moleculePos;
                            
                            Quaternion moleculeRot = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                            coMolecule.transform.localRotation = moleculeRot;

                            platMolecule.ConnectedMolecule = coMolecule;
                            platMolecule.GetComponent<CapsuleCollider>().enabled = true;
                            coMolecule.ConnectedMolecule = platMolecule;
                            
                            activeMolecules.Add(platMolecule);
                            activeMolecules.Add(coMolecule);
                        }
                    }
                    moleculePosition.z = surfaceLayerParent.position.z;
                }
                moleculePosition.x = surfaceLayerParent.transform.position.x;
            }
            onComplete?.Invoke(activeMolecules);
        }
            
        private void Awake()
        {
            _topLayerMolecules = topLayerParent.transform.GetComponentsInChildren<Molecule>().ToList();
        }
    }
}