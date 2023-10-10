using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeighableObject : MonoBehaviour, IResetObject
{

    public decimal starting_weight;
    private decimal weight;

    private void Awake()
    {
        ResetObject();
    }

    public decimal getWeight()
    {
        return weight;
    }

    public void setWeight(decimal new_weight)
    {
        weight = new_weight;
    }

    public void resetWeight()
    {
        weight = starting_weight;
    }

    public void ResetObject()
    {
        resetWeight();
    }

}
