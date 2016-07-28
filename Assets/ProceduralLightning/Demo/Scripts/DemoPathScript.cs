//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;

namespace DigitalRuby.ThunderAndLightning
{
    public class DemoPathScript : MonoBehaviour
    {
        public GameObject Crate;

        private void Start()
        {
            Crate.GetComponent<Rigidbody>().angularVelocity = new Vector3(0.2f, 0.3f, 0.4f);
        }
    }
}
