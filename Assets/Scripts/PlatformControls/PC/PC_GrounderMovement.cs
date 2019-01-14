using PlatformControls.BaseControls;
using UnityEngine;

namespace PlatformControls.PC
{
    public class PC_GrounderMovement : GrounderMovement
    {
        [SerializeField] private bool _invertInput = false;

        private void Update()
        {       
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                if(_invertInput)
                    Move(Vector3.right, MaxMovementLeft);
                else
                    Move(Vector3.left, MaxMovementRight);
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                if (_invertInput)
                    Move(Vector3.left, MaxMovementLeft);
                else
                    Move(Vector3.right, MaxMovementRight);
            }
        }
    }
}
