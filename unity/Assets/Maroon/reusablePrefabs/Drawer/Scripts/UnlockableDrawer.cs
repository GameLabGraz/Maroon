using System;
using System.Collections;
using System.Collections.Generic;
using GameLabGraz.VRInteraction;
using PrivateAccess;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(VRLinearDrive))]
[RequireComponent(typeof(LockedDrawer))]
public class UnlockableDrawer : MonoBehaviour
{
    [Serializable]
    public struct UnlockableLevel
    {
        public List<GameObject> controls;
    }

    [SerializeField] protected bool lockingUsed = false;
    [SerializeField] protected int initialLockLevel = 0;
    [SerializeField] protected VRScrollingHelper scrollHelper;
    [SerializeField] protected List<UnlockableLevel> levels = new List<UnlockableLevel>();

    [Header("Debug")] 
    public bool DebugOutput = false;
    [Range(0, 3)]
    public int DebugLockLevel = 0;

    private int _currentLockLevel = -1;
    private VRInteractable _interactable;
    private SteamVR_Skeleton_Poser _skeletonPoser;
    private LockedDrawer _lockedDrawer;
    private bool _lockEnabled = true;
    
    public int LockLevel
    {
        get => _currentLockLevel;
        set
        {
            Debug.Assert(0 <= value && value <= levels.Count);
            if (value == _currentLockLevel) return;
            _currentLockLevel = DebugLockLevel = value;
            UpdateLockLevel();
        }
    }
    
    void Start()
    {
        _interactable = GetComponent<VRInteractable>();
        _skeletonPoser = GetComponent<SteamVR_Skeleton_Poser>();
        _lockedDrawer = GetComponent<LockedDrawer>();
        Debug.Assert(_interactable != null);
        Debug.Assert(_lockedDrawer != null);

        if (!lockingUsed)
        {
            initialLockLevel = DebugLockLevel = levels.Count; //set to max level so nothing is locked
        }
        LockLevel = initialLockLevel;
        _lockEnabled = lockingUsed;
    }

    
    private void Update()
    {
        if (_lockEnabled && LockLevel != DebugLockLevel)
        {
            LockLevel = DebugLockLevel;
            DebugLockLevel = LockLevel;
        }
    }

    public void EnableLock(bool enable)
    {
        _lockEnabled = enable;
    }
    
    public void EnableLock(bool enable, int lockLevel)
    {
        EnableLock(enable);
        LockLevel = lockLevel;
    }
    
    protected void UpdateLockLevel()
    {
        if (!_lockEnabled)
            return;
        
        if(DebugOutput)
            Debug.Log("Update Lock Level: " + LockLevel);
        
        if (_skeletonPoser)
            _skeletonPoser.enabled = (LockLevel != 0);

        _interactable.interactable = (LockLevel != 0);
        _lockedDrawer.enabled = (LockLevel == 0);

        var activeCnt = 0;
        
        for (var i = 0; i < levels.Count; ++i)
        {
            foreach(var obj in levels[i].controls)
            {
                obj.SetActive(i <= LockLevel - 1);
                if (i <= LockLevel - 1)
                    activeCnt++;
            }
        }
        
        if(DebugOutput)
            Debug.Log("Active Controls: " + activeCnt);
        
        if (scrollHelper)
            scrollHelper.RecalculateScrolling(activeCnt);
    }
}
