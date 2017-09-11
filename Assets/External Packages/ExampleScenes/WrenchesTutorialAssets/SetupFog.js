#pragma strict

function Start() {
    RenderSettings.fog = true;
    RenderSettings.fogMode = FogMode.Linear;
    RenderSettings.fogColor = Color.black;
    RenderSettings.fogStartDistance = 0.0f;
    RenderSettings.fogEndDistance = 15.0f;
    
    GameObject.Find("RotatingPlane").GetComponent.<Rigidbody>().inertiaTensor = new Vector3(10.0f, 10.0f, 10.0f);
}
