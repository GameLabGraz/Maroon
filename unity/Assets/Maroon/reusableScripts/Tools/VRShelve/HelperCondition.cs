using UnityEngine;
using UnityEngine.Events;

public class HelperCondition : MonoBehaviour
{
    public int intValue = 0;
    public float floatValue = 0f;
    public float epsilon = 0.00001f;
    public bool startCondition = false;

    public UnityEvent onConditionTrue;
    public UnityEvent onConditionFalse;

    private bool _conditionState = false;

    protected void Start()
    {
        _conditionState = startCondition;

        if (_conditionState)
        {
            onConditionTrue.Invoke();
        }
        else
        {
            onConditionFalse.Invoke();
        }
    }

    public void OnIntEqual(int other)
    {
        ChangeConditionState(other == intValue);
    }

    public void OnIntGreaterThan(int other)
    {
        ChangeConditionState(other > intValue);
    }

    public void OnIntGreaterEqualThan(int other)
    {
        ChangeConditionState(other >= intValue);
    }
    
    public void OnIntSmallerThan(int other)
    {
        ChangeConditionState(other < intValue);
    }

    public void OnIntSmallerEqualThan(int other)
    {
        ChangeConditionState(other <= intValue);
    }

    public void OnFloatEqual(float other)
    {
        ChangeConditionState(Mathf.Abs(other - floatValue) < epsilon);
    }

    public void OnFloatGreaterThan(float other)
    {
        ChangeConditionState(other > floatValue);
    }

    public void OnFloatSmallerThan(float other)
    {
        ChangeConditionState(other < floatValue);
    }

    private void ChangeConditionState(bool newVal)
    {
        if (newVal == _conditionState) return;
        
        _conditionState = newVal;
        if(_conditionState)
            onConditionTrue.Invoke();
        else 
            onConditionFalse.Invoke();
    }
    
}
