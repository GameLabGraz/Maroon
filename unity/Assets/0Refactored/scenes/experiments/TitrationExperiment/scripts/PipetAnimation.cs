using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipetAnimation : MonoBehaviour, IResetObject
{

    Animator pipetAnimator;
    public GameObject analyt;
    ShowFluid showFluid;

    void Start ()
    {
        pipetAnimator = GetComponent<Animator>();
        showFluid = GetComponent<ShowFluid>();
	}
	
    // Gamecontroller decides if analyt is acid oder base
    public void setPipetBool(bool param)
    {
        if (param)
            pipetAnimator.SetTrigger("takeAcidTrigger");
        else
            pipetAnimator.SetTrigger("takeBaseTrigger");
    }

    public void resetPipet(bool param)
    {
        if (param)
        {
            pipetAnimator.SetTrigger("forceExit");
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

    //resetAnalytAnimation
    
    public void ResetObject()
    {
        analyt.GetComponent<Animator>().SetTrigger("reset");
        showFluid.meshRend.enabled = false;
        showFluid.changeMeshMaterial(colorEnum.colorless);
    }
}
