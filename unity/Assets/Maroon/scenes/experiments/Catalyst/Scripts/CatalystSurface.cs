using System.Collections.Generic;
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
        [SerializeField] private GameObject boundaryXMin;
        [SerializeField] private GameObject boundaryXMax;
        [SerializeField] private GameObject boundaryZMin;
        [SerializeField] private GameObject boundaryZMax;
        
        private float _spaceBetweenMolecules;

        public void SetupCoords(List<Vector3> platCoords)
        {
            
            foreach (var coord in platCoords)
            {
                Molecule platMolecule = Instantiate(platinumMoleculePrefab, surfaceLayerParent);
                platMolecule.transform.position = coord / 20.0f;
                platMolecule.State = MoleculeState.Fixed;
            }
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
    }
}