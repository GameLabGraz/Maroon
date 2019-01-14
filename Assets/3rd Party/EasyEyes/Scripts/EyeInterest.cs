using UnityEngine;
using System.Collections;

public class EyeInterest : MonoBehaviour
{
    [Tooltip("How interesting is this object to eye managers ? Manager always look at the highest value in range.")]
    public int _iInterestPriority;

    [Tooltip("Affinity value is used to affect eye pupil. The higher the value, the more open the pupil.")]
    [Range(0,1)]
    public float _fAffinity;

    [Tooltip("How much in front this object has to be to be seen ? 0 will never be seen. 90 will be seen if in front or on the side. 180 will always be seen.")]
    [Range(0,180)]
    public float _fNecessaryFrontSolidAngle = 180.0f;
}
