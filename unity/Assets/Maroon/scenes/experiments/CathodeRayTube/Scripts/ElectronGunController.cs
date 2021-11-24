using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maroon.Physics.CathodeRayTube
{
    public class ElectronGunController : PausableObject
    {
        [SerializeField] private GameObject Electron;
        [SerializeField] private float ElectronSpeed;
        private GameObject TempElectron = null;

        protected override void HandleUpdate()
        {
            if (TempElectron == null)
            {
                TempElectron = Instantiate(Electron, transform.position, transform.rotation);
                TempElectron.transform.name = "Electron";
                TempElectron.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, -ElectronSpeed, 0));
            }
        }
        
        protected override void HandleFixedUpdate()
        {

        }
    }
}
