using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeighableObject : MonoBehaviour, IResetObject
{

    public float starting_weight;
    private float weight;

    private void Awake()
    {
        ResetObject();
    }

    public float getWeight()
    {
        return weight;
    }

    public void setWeight(float new_weight)
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
