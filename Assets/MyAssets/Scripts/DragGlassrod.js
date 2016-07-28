#pragma strict

var pullForce : float = 1.0f;

private var touching : boolean = false;
private var startPoint : Vector2;

function Start() {
}

function OnMouseDown() {
    touching = true;
    startPoint = Input.mousePosition;
}

function FixedUpdate() {
    if (!Input.GetMouseButton(0)) {
        touching = false;
    }
    else if (touching) {
        var change : Vector2 = Input.mousePosition - startPoint;
        startPoint = Input.mousePosition;
        GetComponent.<Rigidbody>().AddTorque(Vector3(0.0f, -0.05f*change.x, 0.05f*change.y), ForceMode.Impulse);
        
        GetComponent.<Rigidbody>().AddForce(-0.05f * change.magnitude * Camera.main.ScreenPointToRay(Input.mousePosition).direction);
    }
}
