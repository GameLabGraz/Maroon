using PlatformControls.BaseControls;
using UnityEngine;
using Util;

namespace PlatformControls.PC
{
    public class PC_EnterExperiment : EnterScene
    {
        private void OnTriggerStay(Collider other)
        {
            if (!PlayerUtil.IsPlayer(other.gameObject))
                return;

            if (Input.GetKey(KeyCode.Return))
                Enter();
        }
    }
}
