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
        
        private float _spaceBetweenMolecules;
        
        public void Setup(int surfaceSize, System.Action<List<Molecule>> onComplete)
        {
            List<Molecule> activeMolecules = new List<Molecule>();
            _spaceBetweenMolecules = platinumMoleculePrefab.transform.localScale.x;
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
                        platMolecule.IsFixedMolecule = true;
                        if (layerNum == 0)
                        {
                            Molecule coMolecule = Instantiate(coMoleculePrefab, surfaceLayerParent);
                            coMolecule.IsFixedMolecule = true;
                            Vector3 moleculePos = platMolecule.transform.localPosition;
                            moleculePos.y = CatalystController.FixedMoleculeYDist;
                            coMolecule.transform.localPosition = moleculePos;
                            Quaternion moleculeRot = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                            coMolecule.transform.localRotation = moleculeRot;

                            platMolecule.ConnectedMolecule = coMolecule;
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