using UnityEngine;

public class OpenBurette : MonoBehaviour 
{

    private GameObject handle;
    private ParticleSystem particleSys;

    private Vector3 openHandlePos = new Vector3(-0.0652f, 0f, -0.0689f);
    private Vector3 closedHandlePos;
    private Vector3 openHandleRot = new Vector3(0, -90f, 0);
    private Vector3 closedHandleRot;

    [HideInInspector]
    public bool interactable = false;
    [HideInInspector]
    public bool open = false;

    private void Start()
    {
        handle = gameObject.transform.GetChild(3).gameObject;
        particleSys = gameObject.transform.GetChild(5).gameObject.GetComponent<ParticleSystem>();
        particleSys.Stop();

        closedHandlePos = handle.transform.localPosition;
        closedHandleRot = handle.transform.localEulerAngles;
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
