using Maroon.ScoreView;
using UnityEngine;

[RequireComponent(typeof(UnlockableDrawer))]
public class ScoreUnlockDrawerHandler : MonoBehaviour
{
    private UnlockableDrawer _unlockableDrawer;
    private void Start()
    {
        _unlockableDrawer = GetComponent<UnlockableDrawer>();
    }

    public void UnlockDrawer(ExperimentScore experimentScore)
    {
        if (experimentScore.Score >= experimentScore.MaxScore / 3)
            _unlockableDrawer.LockLevel = 1;
        else if (experimentScore.Score >= experimentScore.MaxScore * 2 / 3)
            _unlockableDrawer.LockLevel = 2;
        else if (experimentScore.Score >= experimentScore.MaxScore)
            _unlockableDrawer.LockLevel = 3;
        else
            _unlockableDrawer.LockLevel = 0;
    }
}
