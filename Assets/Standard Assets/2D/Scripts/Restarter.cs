using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityStandardAssets._2D
{
    public class Restarter : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                //Application.LoadLevel(Application.loadedLevelName);

                // fix 2 warnings
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            }
        }
    }
}
