#pragma strict

function Start () {

}

function Update () {
    GetComponent.<Light>().intensity = Random.Range(0.35f, 0.45f);
}