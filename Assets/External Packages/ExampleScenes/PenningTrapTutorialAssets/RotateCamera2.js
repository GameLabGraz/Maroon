#pragma strict

var scrollAround : GameObject;

private var scrollingScreen : boolean = false;
private var scrollStart : Vector2;

function Update() {
    if (Input.GetMouseButton(0)  &&  !scrollingScreen) {
        scrollingScreen = true;
        scrollStart = Input.mousePosition;
    }
    if (!Input.GetMouseButton(0)) {
        scrollingScreen = false;
    }

    if (scrollingScreen) {
        var diffx : float = Input.mousePosition.x - scrollStart.x;
        
        transform.RotateAround(scrollAround.transform.position, Vector3.up, 0.1f * diffx);
        if (Mathf.Abs(transform.position.x) > 80.0f) {
            // put it back
            transform.RotateAround(scrollAround.transform.position, Vector3.up, -0.1f * diffx);
        }
        scrollStart = Input.mousePosition;
    }
}