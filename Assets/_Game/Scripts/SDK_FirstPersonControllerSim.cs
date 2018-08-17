using UnityEngine;
using VRTK;

public class SDK_FirstPersonControllerSim : MonoBehaviour {
    [HideInInspector] public bool selected;

    protected VRTK_VelocityEstimator cachedVelocityEstimator;

    public Vector3 GetVelocity() {
        SetCaches();
        return cachedVelocityEstimator.GetVelocityEstimate();
    }

    public Vector3 GetAngularVelocity() {
        SetCaches();
        return cachedVelocityEstimator.GetAngularVelocityEstimate();
    }

    protected virtual void OnEnable() {
        SetCaches();
    }

    protected virtual void SetCaches() {
        if (cachedVelocityEstimator == null) {
            cachedVelocityEstimator = (GetComponent<VRTK_VelocityEstimator>() != null
                    ? GetComponent<VRTK_VelocityEstimator>()
                    : gameObject.AddComponent<VRTK_VelocityEstimator>());
        }
    }
}