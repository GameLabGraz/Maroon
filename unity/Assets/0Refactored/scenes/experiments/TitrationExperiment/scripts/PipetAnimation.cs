using UnityEngine;

public class PipetAnimation : MonoBehaviour, IResetObject
{

    private Animator pipetAnimator;
    private ShowFluid showFluid;
    private Animator dropperAnimator;

    [SerializeField] private GameObject analyte;


    void Start ()
    {
        pipetAnimator = GetComponent<Animator>();
        showFluid = ShowFluid.Instance;
        dropperAnimator = GameObject.Find("Dropper").GetComponent<Animator>();
    }
    
    // Gamecontroller decides if analyte is acid oder base
    public void SetPipetBool(bool param)
    {
        if (param)
            pipetAnimator.SetTrigger("takeAcidTrigger");
        else
            pipetAnimator.SetTrigger("takeBaseTrigger");

        dropperAnimator.ResetTrigger("forceIndicatorExit");
    }

    public void ActivateIndicator()
    {
        dropperAnimator.SetTrigger("takeIndicatorTrigger");
    }

    public void ResetPipet(bool param)
    {
        if (param)
        {
            pipetAnimator.SetTrigger("forceExit");
            dropperAnimator.SetTrigger("forceIndicatorExit");
        }
    }

    public void ResetTrigger(string name)
    {
        analyte.GetComponent<Animator>().ResetTrigger(name);
    }

    public void PlayAnalyteAnimation()
    {
        analyte.GetComponent<Animator>().SetTrigger("put");
    }

    public void ResetObject()
    {
        // handle of burette is not interactable
        analyte.GetComponent<SetBuretteInteractive>().DisableBuretteTap();

        // resetAnalyteAnimation
        analyte.GetComponent<Animator>().SetTrigger("reset");
        showFluid.DisableMeshRenderer();
        showFluid.ChangeMeshMaterial(colorEnum.colorless);
    }
}
