#pragma strict

var pullForce : float = 10.0f;
var liftForce : float = 0.5f;
var dragHeight : float = 0.0f;
var dragMouseTargetLayer : int = 8;
private var touching : boolean = false;
private var grabPoint : Vector3;

function OnMouseDown() {
    var ray : Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    var hit : RaycastHit;
    if (Physics.Raycast(ray, hit, Mathf.Infinity, (1 << dragMouseTargetLayer))) {
        touching = true;
        grabPoint = transform.InverseTransformPoint(hit.point);
    }
}

function FixedUpdate() {
    if (!Input.GetMouseButton(0)) {
        touching = false;
    }
    else if (touching) {
        var ray : Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit : RaycastHit;
        if (Physics.Raycast(ray, hit, Mathf.Infinity, (1 << dragMouseTargetLayer))) {
            GetComponent.<Rigidbody>().velocity *= 0.5f;  // seriously overdampen this spring force to give the user more control
            var grabGlobal : Vector3 = transform.TransformPoint(grabPoint);
            grabGlobal += dragHeight*Vector3.up;
            GetComponent.<Rigidbody>().AddForceAtPosition(pullForce*(hit.point - grabGlobal) + liftForce*Vector3.up, grabGlobal, ForceMode.Impulse);
        }
        else {
            touching = false;
        }
    }
}
