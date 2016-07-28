#pragma strict

var initialVelocity : Vector3;
var initialAngularVelocity : Vector3;

function Start() {
    GetComponent.<Rigidbody>().velocity = initialVelocity;
    GetComponent.<Rigidbody>().angularVelocity = initialAngularVelocity;
}
