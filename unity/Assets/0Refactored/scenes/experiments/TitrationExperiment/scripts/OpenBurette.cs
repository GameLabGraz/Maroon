using UnityEngine;

public class OpenBurette : MonoBehaviour 
{
    [SerializeField] private GameObject handle;
    [SerializeField] private ParticleSystem particleSys;

    private Vector3 _openHandlePos = new Vector3(-0.0652f, 0f, -0.0689f);
    private Vector3 _closedHandlePos;
    private Vector3 _openHandleRot = new Vector3(0, -90f, 0);
    private Vector3 _closedHandleRot;

    [HideInInspector]
    public bool interactable;
    [HideInInspector]
    public bool open;

    private void Start()
    {
        particleSys.Stop();

        _closedHandlePos = handle.transform.localPosition;
        _closedHandleRot = handle.transform.localEulerAngles;
    }

    private void OnMouseDown()
    {
        if (interactable)
        {
            particleSys.Play();
            handle.transform.localRotation = Quaternion.Euler(_openHandleRot);
            handle.transform.localPosition = _openHandlePos;
            open = true;
        }
    }

    private void OnMouseUp()
    {
        if (interactable)
        {
            particleSys.Stop();
            handle.transform.localRotation = Quaternion.Euler(_closedHandleRot);
            handle.transform.localPosition = _closedHandlePos;
            open = false;
        }
    }
}
