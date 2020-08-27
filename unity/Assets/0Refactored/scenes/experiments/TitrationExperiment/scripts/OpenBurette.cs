using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBurette : MonoBehaviour {

    public GameObject particleObj;
    public GameObject handle;
    public bool open = false;
    private ParticleSystem particleSys;

    private Vector3 openHandlePos = new Vector3(-1.34306f, -0.8691311f, 1.35f);
    private Vector3 closedHandlePos = new Vector3(1.645245f, -0.8691306f, 4.530176f);
    private Vector3 openHandleRot = new Vector3(-89.97201f, 0f, -87.784f);
    private Vector3 closedHandleRot = new Vector3(-89.98f, 0f, 0f);

    public bool interactable = false;


    private void Start()
    {
        particleSys = particleObj.GetComponent<ParticleSystem>();
        particleSys.Stop();
    }

    private void OnMouseDown()
    {
        if (interactable)
        {
            particleSys.Play();
            handle.transform.localRotation = Quaternion.Euler(openHandleRot);
            handle.transform.localPosition = openHandlePos;
            open = true;
        }
    }

    private void OnMouseUp()
    {
        if (interactable)
        {
            particleSys.Stop();
            handle.transform.localRotation = Quaternion.Euler(closedHandleRot);
            handle.transform.localPosition = closedHandlePos;
            open = false;
        }
    }
}
