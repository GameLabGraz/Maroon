using PlatformControls.BaseControls;
using UnityEngine;

namespace PlatformControls.PC
{
    public class PC_MouseMovement : Movement
    {
        [SerializeField]
        private Vector3 _moveAxis = Vector3.forward;

        private void OnMouseDrag()
        {
            var mousePosScreen = Input.mousePosition;
            mousePosScreen.z = Speed;

            if (Camera.main == null) return;
            var mousePos = Camera.main.ScreenToWorldPoint(mousePosScreen);

            var target = Vector3.Scale(Vector3.one - _moveAxis, transform.position) + Vector3.Scale(_moveAxis, mousePos);
            Move(target);
        }

        private void OnMouseDown()
        {
            StartMoving();
        }

        private void OnMouseUp()
        {
            StopMoving();
        }
    }
}
