using UnityEngine;
using System.Collections;

public class Eye : MonoBehaviour
{
    [Tooltip("The renderers holding the eye material. This in an aray to synchronise LOD levels if necesarry.")]
    public Renderer[] _tRenderers;
    [HideInInspector]
    public Material _tMaterial;

        [Header("PUPIL")]
    
    [Tooltip("The minimal size of the pupil.")]
    [Range(0.5f, 10.0f)]
    public float _fMinSize = 1.0f;

    [Tooltip("The maximal size of the pupil.")]
    [Range(0.5f, 10.0f)]
    public float _fMaxSize = 3.0f;

    [Tooltip("How fast does the pupil switch from its current value to the target value.")]
    [Range(0, 1)]
    public float _fChangeSpeed = .1f;

        [Header("LOOK AT")]

    [Tooltip("How fast does the eye rotate toward target look at.")]
    [Range(0, 1)]
    public float _fLookAtSpeed = .2f;

    [Tooltip("Angle limit applied to the eye so it doesn't look inside the skull.")]
    public float _fMoveAngle = 80.0f;

    [Tooltip("Strabismus applied to the eye.")]
    public Vector3 _tStrabismus;

    void Start()
    {
        _tMaterial = _tRenderers[0].material;

        for (int i = 0; i < _tRenderers.Length; i++)
            _tRenderers[i].material = _tMaterial;
    }

    public void SetPupil(float fValue)
    {
        _tMaterial.SetFloat("_PupilSize", Mathf.Lerp(_tMaterial.GetFloat("_PupilSize"), Mathf.Lerp(_fMinSize, _fMaxSize, fValue), _fChangeSpeed));
    }

    public void LookAt(Vector3 tTarget, Vector3 tForward, Vector3 tUp)
    {
        // Get Parent rotation

        Quaternion tParent = Quaternion.LookRotation(tForward, tUp);

        // Get Look rotation

        Vector3 tDir = (tTarget - transform.position).normalized;
        Quaternion tLook = Quaternion.LookRotation(tDir, tUp);

        // Clamp

        float fAngle = Vector3.Angle(tDir, tForward);

        if (fAngle > _fMoveAngle)
        {
            float fLerp = Mathf.Clamp01(_fMoveAngle / fAngle);
            tLook = Quaternion.Slerp(tParent, tLook, fLerp);
        }

        // Update rotation

        transform.rotation = Quaternion.Slerp(transform.rotation, tLook * Quaternion.Euler(_tStrabismus), _fLookAtSpeed);
    }
}