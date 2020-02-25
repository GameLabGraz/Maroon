using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EyeManager : MonoBehaviour
{
    // Change the radius of the collider attached to affect the range of detection of points of interest.
    // If no collider is attached, trigger callbacks will not be called, and so the eyes will never look at any point of interest.
    // For the callbacks to be called, this object must also have a rigidbody, or be a child of an object having one.

    [Tooltip("Eyes that will be managed by this script. Only use eyes mounted on this manager, as this will be the reference for look at and pupil opening.")]
    public Eye[] _tEyes;

    [Tooltip("Light used as reference for pupil opening.")]
    public Transform _tLight;

    [Tooltip("How much does the light affect the pupil opening ?")]
    [Range(0,1)]
    public float _fLightInfluence = 1.0f;

    [Tooltip("How much does the twitching affect the pupil opening ?")]
    [Range(0, 1)]
    public float _fPupilTwitchInfluence = .2f;

    [Tooltip("The minimum delay before the next twitch in the pupil.")]
    public float _fMinChangeDelay = .1f;

    [Tooltip("The maximum delay before the next twitch in the pupil.")]
    public float _fMaxChangeDelay = .5f;

    private float _fCurrentChangeDelay = 0;
    private float _fCurrentTime = 0;
    private float _fCurrentPupil = 0;

    private List<EyeInterest> _tInterest = new List<EyeInterest>();

    void Start()
    {
        if(_tLight == null)
            Debug.Log("No light was referenced on " + name + ". Pupil will not changed based on light orientation.", this.gameObject);
    }

    void Update ()
    {
        // If delay is passed
        if(_fCurrentTime >= _fCurrentChangeDelay)
        {
            // Reset timer
            _fCurrentTime = 0.0f;

            // Get new delay
            _fCurrentChangeDelay = Random.Range(_fMinChangeDelay, _fMaxChangeDelay);

            // Get new pupil opening
            _fCurrentPupil = Random.value;
        }

        // Get current target and affinity
        float fAffinity;
        Vector3 tTarget = GetTarget(out fAffinity);

        // Update all the eyes
        for( int i = 0; i < _tEyes.Length; i++ )
        {
            // Look at target
            _tEyes[i].LookAt(tTarget, transform.forward, transform.up);

            // Get pupil based on light
            float fDot = _tLight != null ? Vector3.Dot(_tEyes[i].transform.forward, _tLight.forward) * .5f + .5f : 0.0f;

            // Update pupil value
            _tEyes[i].SetPupil((fDot * _fLightInfluence + _fCurrentPupil * _fPupilTwitchInfluence) / (_fLightInfluence + _fPupilTwitchInfluence) + fAffinity);
        }

        // Update timer
        _fCurrentTime += Time.deltaTime;
    }

    Vector3 GetTarget(out float fAffinity)
    {
        // Init affinity
        fAffinity = 0;

        // If list is empty, return target far in front of the eyes
        if (_tInterest.Count == 0)
            return transform.position + transform.forward * 1000;

        // Init value to search trough the list
        int iBestTarget = 0;
        int iBestInterest = int.MinValue;

        // Search trough the list
        for( int i = 0; i < _tInterest.Count; i++ )
        {
            // Check if target is in necessary solid angle
            Vector3 tDir = _tInterest[iBestTarget].transform.position - transform.position;
            if (Vector3.Angle(transform.forward, tDir) > _tInterest[i]._fNecessaryFrontSolidAngle)
                continue;

            // If target is more interesting than current best
            if (_tInterest[i]._iInterestPriority > iBestInterest)
            {
                // Update values
                iBestTarget = i;
                iBestInterest = _tInterest[i]._iInterestPriority;
                fAffinity = _tInterest[i]._fAffinity;
            }
        }

        return _tInterest[iBestTarget].transform.position;
    }

    void OnTriggerEnter(Collider tOther)
    {
        // If interest point, add it to the list
        EyeInterest tInterest = tOther.GetComponent<EyeInterest>();
        if (tInterest != null)
            _tInterest.Add(tInterest);
    }

    void OnTriggerExit(Collider tOther)
    {
        // If interest point, remove it from the list
        EyeInterest tInterest = tOther.GetComponent<EyeInterest>();
        if (tInterest != null)
            _tInterest.Remove(tInterest);
    }
}