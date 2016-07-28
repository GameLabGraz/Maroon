#pragma strict

var pullForce : float = 1.0f;

private var touching : boolean = false;
// private var grabPoint : Vector3;
// private var collider2 : SphereCollider;
private var startPoint : Vector2;

function Start() {
//     collider2 = gameObject.AddComponent(SphereCollider);
//     collider2.center = gameObject.GetComponent(BoxCollider).center;
//     collider2.radius = 15.0f;
//     collider2.isTrigger = true;
}

function OnMouseDown() {
//     var ray : Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//     var hit : RaycastHit;
//     if (collider2.Raycast(ray, hit, Mathf.Infinity)) {
//         touching = true;
//         grabPoint = transform.InverseTransformPoint(hit.point);
//     }

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
        
//         var ray : Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//         var hit : RaycastHit;
//         if (collider2.Raycast(ray, hit, Mathf.Infinity)) {
//             rigidbody.velocity *= 0.5f;  // seriously overdampen this spring force to give the user more control
//             var grabGlobal : Vector3 = transform.TransformPoint(grabPoint);
//             var force : Vector3 = pullForce*(hit.point - grabGlobal);
//             force.x = 0.0f;
//             Debug.Log(force);
//             // force -= ray.direction * Vector3.Dot(force, ray.direction);
//             rigidbody.AddForceAtPosition(force, grabGlobal);
//         }
    }
}
