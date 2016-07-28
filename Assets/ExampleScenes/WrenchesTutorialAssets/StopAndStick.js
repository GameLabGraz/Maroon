#pragma strict

var wrenches : GameObject[];

function OnTriggerEnter(other : Collider) {
    var itsAWrench : boolean = false;
    for (var wrench in wrenches) {
        if (other == wrench.GetComponent.<Collider>()) {
            itsAWrench = true;
            break;
        }
    }
    if (itsAWrench) {
        var x : float = Vector3.Dot(other.gameObject.transform.TransformPoint(Vector3.up).normalized, Vector3.right);
        var y : float = Vector3.Dot(other.gameObject.transform.TransformPoint(Vector3.up).normalized, Vector3.up);
        
        other.gameObject.transform.eulerAngles = Vector3(0.0f, 90.0f, (180.0/Mathf.PI)*Mathf.Atan2(y, x));
        other.gameObject.transform.position.x = -2.4f;
        other.gameObject.GetComponent.<Rigidbody>().isKinematic = true;
        other.gameObject.transform.position.y -= Mathf.Min(other.gameObject.GetComponent.<Collider>().bounds.min.y, 0.0f);
    }
}