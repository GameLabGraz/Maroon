using System.Collections.Generic;
using UnityEngine;

namespace Maroon.scenes.experiments.Catalyst.Scripts
{
    public enum CatalystSurfaceSize
    {
        Small = 64,
        Medium = 128,
        Big = 256
    }
    public class CatalystSurface : MonoBehaviour
    {
        [SerializeField] GameObject platinumMoleculePrefab;
        [SerializeField] CatalystSurfaceSize surfaceSize;
        [SerializeField] Transform surfaceLayerParent;
        [SerializeField] int numSubLayers;
        
        private float _spaceBetweenMolecules;
        
        private void Start()
        {
            _spaceBetweenMolecules = platinumMoleculePrefab.transform.localScale.x;
            for (int layerNum = 0; layerNum < numSubLayers; layerNum++)
            {
                Vector3 moleculePosition = surfaceLayerParent.position;
                moleculePosition.y += -layerNum * _spaceBetweenMolecules;
                for (int sizeX = 0; sizeX < (int)surfaceSize / 4; sizeX++)
                {
                    moleculePosition.x += _spaceBetweenMolecules;
                    for (int sizeZ = 0; sizeZ < (int)surfaceSize / 4; sizeZ++)
                    {
                        moleculePosition.z += _spaceBetweenMolecules;
                        GameObject platMolecule = Instantiate(platinumMoleculePrefab, surfaceLayerParent);
                        platMolecule.transform.position = moleculePosition;
                    }
                    moleculePosition.z = surfaceLayerParent.position.z;
                }
                moleculePosition.x = surfaceLayerParent.transform.position.x;
            }
        }
    }
}