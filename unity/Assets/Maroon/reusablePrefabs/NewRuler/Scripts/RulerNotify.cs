using System;
using UnityEngine;

public class RulerNotify : MonoBehaviour
{
    public event Action onRulerPinEnable;
    public event Action onRulerPinDisable;

    private void OnDisable()
    {
        if (onRulerPinDisable != null)
            onRulerPinDisable();
    }

    private void OnEnable()
    {
        if (onRulerPinEnable != null)
            onRulerPinEnable();
    }
}
