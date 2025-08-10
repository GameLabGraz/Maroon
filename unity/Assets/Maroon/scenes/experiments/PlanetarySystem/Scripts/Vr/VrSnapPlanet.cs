using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Maroon.Experiments.PlanetarySystem
{
    public class VRSnapPlanet : MonoBehaviour
    {
        public Transform sortingPlanetTarget;
        public float snapDistance = 0.5f;
        public AudioSource audioSource;
        public AudioClip dropClip;

        public void TrySnap()
        {
            float dist = Vector3.Distance(transform.position, sortingPlanetTarget.position);
            if (dist <= snapDistance)
            {
                transform.SetParent(sortingPlanetTarget);
                transform.position = sortingPlanetTarget.position;
                transform.localScale = Vector3.one * 1.02f;

                if (audioSource != null && dropClip != null)
                    audioSource.PlayOneShot(dropClip);

                Debug.Log("VR Snap successful!");
            }
            else
            {
                Debug.Log("VR Snap failed: too far away.");
            }
        }
    }
}