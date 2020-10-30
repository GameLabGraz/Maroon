using UnityEngine;

public class PipetAnimation : MonoBehaviour, IResetObject
{
    [SerializeField] private OpenBurette burette;

    [SerializeField] private Animator pipetAnimator;
    [SerializeField] private Animator dropperAnimator;
    [SerializeField] private Animator analyteAnimator;

    private static readonly int TakeAcidTrigger = Animator.StringToHash("takeAcidTrigger");
    private static readonly int TakeBaseTrigger = Animator.StringToHash("takeBaseTrigger");
    private static readonly int ForceIndicatorExit = Animator.StringToHash("forceIndicatorExit");
    private static readonly int TakeIndicatorTrigger = Animator.StringToHash("takeIndicatorTrigger");
    private static readonly int ForceExit = Animator.StringToHash("forceExit");
    private static readonly int Reset = Animator.StringToHash("reset");
    private static readonly int Put = Animator.StringToHash("put");

    private ShowFluid _showFluid;
    
    private void Start ()
    {
        _showFluid = ShowFluid.Instance;
    }
    
    // TitrationController decides if analyte is acid oder base
    public void SetPipetBool(bool param)
    {
        if (param)
            pipetAnimator.SetTrigger(TakeAcidTrigger);
        else
            pipetAnimator.SetTrigger(TakeBaseTrigger);

        dropperAnimator.ResetTrigger(ForceIndicatorExit);
    }

    public void ActivateIndicator()
    {
        dropperAnimator.SetTrigger(TakeIndicatorTrigger);
    }

    public void ResetPipet(bool param)
    {
        if (param)
        {
            pipetAnimator.SetTrigger(ForceExit);
            dropperAnimator.SetTrigger(ForceIndicatorExit);
        }
    }

    public void ResetTrigger(string trigger)
    {
        analyteAnimator.ResetTrigger(trigger);
    }

    public void PlayAnalyteAnimation()
    {
        analyteAnimator.SetTrigger(Put);
    }

    public void ResetObject()
    {
        // handle of burette is not interactable
        burette.interactable = false;

        // resetAnalyteAnimation
        analyteAnimator.SetTrigger(Reset);
        _showFluid.DisableMeshRenderer();
        _showFluid.ChangeMeshMaterial(colorEnum.Colorless);
    }
}
