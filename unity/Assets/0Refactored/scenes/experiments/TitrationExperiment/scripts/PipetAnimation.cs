using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipetAnimation : MonoBehaviour, IResetObject
{

    Animator pipetAnimator;
    public GameObject analyt;
    ShowFluid showFluid;

    Animator dropperAnimator;

    void Start ()
    {
        pipetAnimator = GetComponent<Animator>();
        showFluid = ShowFluid.Instance;
        dropperAnimator = GameObject.Find("Dropper").GetComponent<Animator>();
	}
	
    // Gamecontroller decides if analyt is acid oder base
    public void setPipetBool(bool param)
    {
        if (param)
            pipetAnimator.SetTrigger("takeAcidTrigger");
        else
            pipetAnimator.SetTrigger("takeBaseTrigger");

        dropperAnimator.ResetTrigger("forceIndicatorExit");
    }

    public void activateIndicator()
    {
        dropperAnimator.SetTrigger("takeIndicatorTrigger");
    }

    public void resetPipet(bool param)
    {
        if (param)
        {
            pipetAnimator.SetTrigger("forceExit");
            dropperAnimator.SetTrigger("forceIndicatorExit");
        }
    }

    public void resetTrigger(string name)
    {
        analyt.GetComponent<Animator>().ResetTrigger(name);
    }

    public void playAnalytAnimation()
    {
        analyt.GetComponent<Animator>().SetTrigger("put");
    }
    
    public void ResetObject()
    {
        //resetAnalytAnimation
        analyt.GetComponent<Animator>().SetTrigger("reset");
        showFluid.meshRend.enabled = false;
        showFluid.changeMeshMaterial(colorEnum.colorless);
    }
}
