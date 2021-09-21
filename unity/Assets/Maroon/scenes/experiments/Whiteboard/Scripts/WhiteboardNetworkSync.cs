using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class WhiteboardNetworkSync : ExperimentNetworkSync
{
    #region ExperimentInputs

    [SerializeField] private Button _previousButton;
    
    [SerializeField] private Button _listButton;
    
    [SerializeField] private Button _nextButton;
    
    [SerializeField] private WhiteboardController _whiteboardController;
    
    [SerializeField] private GuiWhiteboard _guiWhiteboard;
    
    #endregion
    
    [SyncVar(hook = "OnLectureIndexChanged")] private int _currentLectureIndex;

    #region ControlHandlers

    protected override void OnGetControl()
    {
        _previousButton.interactable = true;
        _listButton.interactable = true;
        _nextButton.interactable = true;
    }

    protected override void OnLoseControl()
    {
        _previousButton.interactable = false;
        _listButton.interactable = false;
        _nextButton.interactable = false;

        _guiWhiteboard.HideMenu();
    }

    #endregion

    #region Listeners

    protected override void AddListeners()
    {
        base.AddListeners();
        
        _previousButton.onClick.AddListener(OnPreviousButtonClicked);
        //Menu only needed on Client in Control!
        _nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        
        _previousButton.onClick.RemoveListener(OnPreviousButtonClicked);
        _nextButton.onClick.RemoveListener(OnNextButtonClicked);
    }

    #endregion

    #region InputHandlers

    private Lecture _currentLecture;
    private void Update()
    {
        if (Maroon.NetworkManager.Instance.IsInControl)
        {
            if (_whiteboardController.SelectedLecture != _currentLecture)
            {
                CmdSetLectureIndex(_whiteboardController.Lectures.IndexOf(_whiteboardController.SelectedLecture));
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdSetLectureIndex(int i)
    {
        _currentLectureIndex = i;
    }

    private void OnLectureIndexChanged(int oldVal, int newVal)
    {
        _currentLectureIndex = newVal;
        _currentLecture = _whiteboardController.Lectures[newVal];
        _whiteboardController.SelectLecture(newVal);
        _whiteboardController.Refresh();
    }

    private void OnPreviousButtonClicked()
    {
        SyncEvent(nameof(PreviousButtonActivated));
    }

    private IEnumerator PreviousButtonActivated()
    {
        _previousButton.onClick.Invoke();
        yield return null;
    }
    
    private void OnNextButtonClicked()
    {
        SyncEvent(nameof(NextButtonActivated));
    }

    private IEnumerator NextButtonActivated()
    {
        _nextButton.onClick.Invoke();
        yield return null;
    }
    
    #endregion
}
