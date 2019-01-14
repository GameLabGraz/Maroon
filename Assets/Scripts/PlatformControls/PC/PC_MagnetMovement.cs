using PlatformControls.BaseControls;
using UnityEngine;

public class PC_MagnetMovement : MagnetMovement
{
    private void OnMouseDrag()
    {       
        var mousePosScreen = Input.mousePosition;
        mousePosScreen.z = MagnetSpeed;

        if (Camera.main == null) return;
        var mousePos = Camera.main.ScreenToWorldPoint(mousePosScreen);

        var target = transform.position;
        target.x = mousePos.x;

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
