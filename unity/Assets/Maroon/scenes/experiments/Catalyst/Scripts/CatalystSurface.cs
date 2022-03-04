﻿using System.Collections;
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
        [SerializeField] ODrawingSpot oDrawingSpotPrefab;

        private float _spaceBetweenMolecules;

        /**
         * Instantiate surface atoms / molecules of the Langmuir variant based on the given coordinates.
         * <param name="platCoords"> Coordinates of surface atoms </param>
         * <param name="activePlatCoords"> Coordinates of surface atoms that should have CO attached to them </param>
         * <param name="onComplete"> Action that should be called once all surface atoms / molecules have
         * been instantiated. Returns the list of instantiated molecules to the
         * CatalystController. </param>
         * <param name="onMoleculeFreed"> Action that the CO molecules subscribe to. Called when CO molecules
         * are removed from the surface by hand (only happens for the first four). </param>
         */
        public void SetupCoordsLangmuir(List<Vector3> platCoords,
            List<Vector3> activePlatCoords,
            System.Action<List<Molecule>> onComplete, 
            System.Action onMoleculeFreed)
        {
            List<Molecule> platMolecules = new List<Molecule>();
            for (int i = 0; i < platCoords.Count; i++)
            {
                
                Molecule platMolecule = Instantiate(platinumMoleculePrefab, surfaceLayerParent);
                platMolecule.transform.localPosition = platCoords[i] / 20.0f;
                platMolecule.State = MoleculeState.Fixed;

                if (activePlatCoords.Contains(platCoords[i]))
                    platMolecules.Add(platMolecule);
                else
                    platMolecule.gameObject.GetComponent<Molecule>().enabled = false;
            }

            // only spawn co molecules on top layer
            List<Molecule> activeMolecules = new List<Molecule>();
            foreach (var platMolecule in platMolecules)
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
            onComplete?.Invoke(activeMolecules);
        }

        /**
         * Instantiate surface atoms of the van Krevelen variant based on the given coordinates.
         * <param name="cobaltCoords"> Coordinates of cobalt surface atoms </param>
         * <param name="oCoords"> Coordinates of O surface atoms </param>
         * <param name="onComplete"> Action that should be called once all surface atoms / molecules have
         * been instantiated. Returns the list of instantiated molecules to the
         * CatalystController. </param>
         */
        public void SetupCoordsKrevelen(List<Vector3> cobaltCoords, 
            List<Vector3> oCoords,
            System.Action<List<Molecule>> onComplete)
        {
            List<Molecule> surfaceMolecules = new List<Molecule>();
            float maxYVal = cobaltCoords.Max(vector => vector.y);
            for (int i = 0; i < cobaltCoords.Count; i++)
            {
                Molecule cobaltMolecule = Instantiate(cobaltMoleculePrefab, surfaceLayerParent);
                cobaltMolecule.transform.localPosition = cobaltCoords[i] / 20.0f + new Vector3(1.0f, 0.0f, 3.5f); // todo remove offsets when i get centered coords
                cobaltMolecule.State = MoleculeState.Fixed;
                
                if (Mathf.Abs(cobaltCoords[i].y - maxYVal) < 0.01f)
                {
                    surfaceMolecules.Add(cobaltMolecule);
                }
                else
                {
                    cobaltMolecule.gameObject.GetComponent<Molecule>().enabled = false;
                }
                
            }

            maxYVal = oCoords.Max(vector => vector.y);
            
            for (int i = 0; i < oCoords.Count; i++)
            {
                // for now just spawn the top o2 molecules
                if (Mathf.Abs(oCoords[i].y - maxYVal) < 2.0f)
                {
                    Molecule oxygenMolecule = Instantiate(oxygenMoleculePrefab, surfaceLayerParent);
                    oxygenMolecule.transform.localPosition = (oCoords[i] / 20.0f) + new Vector3(1.0f, 0.0f, 3.5f); // todo remove offsets when i get centered coords
                    oxygenMolecule.State = MoleculeState.InSurfaceDrawingSpot;
                    // set drawing spot so we can refill O molecule at same position later
                    ODrawingSpot oDrawingSpot = Instantiate(oDrawingSpotPrefab, surfaceLayerParent);
                    oDrawingSpot.transform.localPosition = (oCoords[i] / 20.0f) + new Vector3(1.0f, 0.0f, 3.5f); // todo remove offsets when i get centered coords
                    oDrawingSpot.SetAttachedMolecule(oxygenMolecule);
                    surfaceMolecules.Add(oxygenMolecule);
                }
            }
            
            onComplete?.Invoke(surfaceMolecules);
            StartCoroutine(SpawnODelayed(oCoords));
        }

        public IEnumerator SpawnODelayed(List<Vector3> oCoords)
        {
            yield return new WaitForSeconds(0.5f);
            float maxYVal = oCoords.Max(vector => vector.y);
            for (int i = 0; i < oCoords.Count; i++)
            {
                // spawn the rest of the molecules
                if (Mathf.Abs(oCoords[i].y - maxYVal) > 2.0f)
                {
                    Molecule oxygenMolecule = Instantiate(oxygenMoleculePrefab, surfaceLayerParent);
                    oxygenMolecule.transform.localPosition = (oCoords[i] / 20.0f) + new Vector3(1.0f, 0.0f, 3.5f); // todo remove offsets when i get centered coords
                    oxygenMolecule.State = MoleculeState.Fixed;
                    oxygenMolecule.gameObject.GetComponent<Molecule>().enabled = false;
                }

                if (i % 500 == 0)
                    yield return null;
            }
        }
    }
}