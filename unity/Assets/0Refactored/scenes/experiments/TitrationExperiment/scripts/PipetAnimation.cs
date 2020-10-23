using UnityEngine;

public class PipetAnimation : MonoBehaviour, IResetObject
{

    Animator pipetAnimator;
    public GameObject analyte;
    ShowFluid showFluid;
    Animator dropperAnimator;

    void Start ()
    {
        pipetAnimator = GetComponent<Animator>();
        showFluid = ShowFluid.Instance;
        dropperAnimator = GameObject.Find("Dropper").GetComponent<Animator>();
	}
	
    // Gamecontroller decides if analyte is acid oder base
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
        analyte.GetComponent<Animator>().ResetTrigger(name);
    }

    public void playAnalyteAnimation()
    {
        analyte.GetComponent<Animator>().SetTrigger("put");
    }

    public void ResetObject()
    {
        //resetAnalyteAnimation
        analyte.GetComponent<Animator>().SetTrigger("reset");
        showFluid.meshRend.enabled = false;
        showFluid.changeMeshMaterial(colorEnum.colorless);
    }
}
