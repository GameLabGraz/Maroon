#pragma strict

function Start() {
    GetComponent.<Rigidbody>().inertiaTensor = new Vector3(0.050, 0.030, 0.078);
}
