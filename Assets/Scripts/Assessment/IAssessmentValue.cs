using Evaluation.UnityInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class IAssessmentValue : MonoBehaviour
{
    public bool ContinuousUpdate = false;
    public abstract GameEvent GameEvent {  get; set; }
}
