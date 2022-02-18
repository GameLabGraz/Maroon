using System.Collections.Generic;
using System.Linq;
using Maroon.scenes.experiments.Catalyst.Scripts.Molecules;
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
        [SerializeField] Molecule cobaltMoleculePrefab;
        [SerializeField] Molecule oxygenMoleculePrefab;
        [SerializeField] Transform surfaceLayerParent;
        
        private float _spaceBetweenMolecules;

        public void SetupCoords(List<Vector3> platCoords, 
            System.Action<List<Molecule>> onComplete, 
            System.Action onMoleculeFreed)
        {
            List<Molecule> platMolecules = new List<Molecule>();
            float maxYVal = -100.0f;
            for (int i = 0; i < platCoords.Count; i++)
            {
                Molecule platMolecule = Instantiate(platinumMoleculePrefab, surfaceLayerParent);
                platMolecule.transform.localPosition = platCoords[i] / 20.0f;
                platMolecule.State = MoleculeState.Fixed;

                // find top layer molecules based on y position
                if (maxYVal < platMolecule.transform.localPosition.y)
                {
                    maxYVal = platMolecule.transform.localPosition.y;
                }
                platMolecules.Add(platMolecule);
            }

            // only spawn co molecules on top layer
            List<Molecule> activeMolecules = new List<Molecule>();
            foreach (var platMolecule in platMolecules)
            {
                if (Mathf.Abs(platMolecule.transform.localPosition.y - maxYVal) < 0.01f)
                {
                    Molecule coMolecule = Instantiate(coMoleculePrefab, surfaceLayerParent);
                    coMolecule.State = MoleculeState.Fixed;

                    Vector3 moleculePos = platMolecule.transform.localPosition;
                    moleculePos.y += CatalystController.FixedMoleculeYDist;
                    coMolecule.transform.localPosition = moleculePos;

                    Quaternion moleculeRot = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                    coMolecule.transform.localRotation = moleculeRot;

                    platMolecule.ConnectedMolecule = coMolecule;
                    coMolecule.ConnectedMolecule = platMolecule;
                    coMolecule.OnMoleculeFreed += onMoleculeFreed;

                    activeMolecules.Add(coMolecule);
                    activeMolecules.Add(platMolecule);
                }
            }
            onComplete?.Invoke(activeMolecules);
        }

        public void SetupOtherCoords(List<Vector3> cobaltCoords, 
            List<Vector3> oCoords,
            System.Action<List<Molecule>> onComplete, 
            System.Action onMoleculeFreed)
        {
            List<Molecule> surfaceMolecules = new List<Molecule>();
            for (int i = 0; i < cobaltCoords.Count; i++)
            {
                Molecule cobaltMolecule = Instantiate(cobaltMoleculePrefab, surfaceLayerParent);
                cobaltMolecule.transform.localPosition = cobaltCoords[i] / 20.0f;
                cobaltMolecule.State = MoleculeState.Fixed;
                
                surfaceMolecules.Add(cobaltMolecule);
            }

            for (int i = 0; i < oCoords.Count / 2; i++)
            {
                Molecule oxygentMolecule = Instantiate(oxygenMoleculePrefab, surfaceLayerParent);
                oxygentMolecule.transform.localPosition = (oCoords[i] / 20.0f);
                oxygentMolecule.State = MoleculeState.Fixed;
                
                surfaceMolecules.Add(oxygentMolecule);
            }
            
            onComplete?.Invoke(surfaceMolecules);
        }
    }
}