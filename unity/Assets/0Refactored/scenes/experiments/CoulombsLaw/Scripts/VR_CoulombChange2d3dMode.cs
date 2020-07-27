using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK.Controllables;

public class VR_CoulombChange2d3dMode : MonoBehaviour
{
    public float ModeChangeDuration = 2f;
    public Image BlackeningScreen;
    
    private CoulombLogic _coulombLogic;

    private bool changedAtBeginning = false;

    enum Mode
    {
        cmWaiting,
        cmDarkeningFor,
        cmBrighteningFor
    }
    
    private float _currentTime = 0f;
    private Mode _currentMode;
    private bool _isNewMode3dMode = false;

    // Start is called before the first frame update
    void Start()
    {
        var obj = GameObject.Find("CoulombLogic");
        if (obj)
            _coulombLogic = obj.GetComponent<CoulombLogic>();

        _currentMode = Mode.cmWaiting;
    }

    private void Update()
    {
        if (_currentMode == Mode.cmWaiting) return;

        var col = BlackeningScreen.color;
        if (_currentMode == Mode.cmDarkeningFor)
        {
            col.a = (1f - _currentTime / ModeChangeDuration) * 2;

            if (_currentTime <= ModeChangeDuration / 2f)
            {
                col.a = 1f;

                _coulombLogic.OnSwitch3d2dMode(_isNewMode3dMode ? 1f : 0f);
                _currentMode = Mode.cmBrighteningFor;
            }
        }
        else if (_currentMode == Mode.cmBrighteningFor)
        {
            col.a = (_currentTime / ModeChangeDuration) * 2;

            if (_currentTime <= 0)
            {
                col.a = 0f;
                _currentMode = Mode.cmWaiting;
            }
        }
        
        Debug.Log("Color a = " + col.a);
        BlackeningScreen.color = col;


        _currentTime -= Time.deltaTime;
    }


    public void OnChangeTo2dMode(object o, ControllableEventArgs args)
    {
        ChangeMode(false);
    }
    
    public void OnChangeTo3dMode(object o, ControllableEventArgs args)
    {
        ChangeMode(true);
    }


    private void ChangeMode(bool newModeIs3d)
    {
        if (!changedAtBeginning)
        {
            changedAtBeginning = true;
            return;
        }

        if (_coulombLogic.IsIn3dMode() == newModeIs3d)
            return; //no need to change the current mode
        
        
        Debug.Log("Change Mode");
        _currentTime = ModeChangeDuration;
        _isNewMode3dMode = newModeIs3d;
        _currentMode = Mode.cmDarkeningFor;
    }
}
