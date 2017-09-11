#pragma strict

function FixedUpdate() {
    var up : Vector3 = transform.TransformDirection(Vector3(0.0f, 3.2f, 0.0f));
    GetComponent.<Rigidbody>().AddTorque(Vector3(-3.0f * up.z, 0.0f, 3.0f * up.x));
}