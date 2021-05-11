using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using Dropdown = Maroon.UI.Dropdown;
using Slider = Maroon.UI.Slider;
using InputField = Maroon.UI.InputField;

public class SortingNetworkSync : ExperimentNetworkSync
{
    #region ExperimentInputs
    
    //Control Buttons
    [SerializeField]
    private Button _resetButton;
    
    [SerializeField]
    private Button _playButton;

    [SerializeField]
    private Button _pauseButton;

    [SerializeField]
    private Button _stepBackwardButton;
    
    [SerializeField]
    private Button _stepForwardButton;
    
    //Detail Mode Options
    [SerializeField] private Dropdown _detailAlgorithmDropDown;
    
    [SerializeField] private Slider _detailSizeSlider;
    
    [SerializeField] private InputField _detailSizeInputField;
    
    [SerializeField] private Button _battleModeButton;
    
    //Battle Mode Options
    [SerializeField] private Dropdown _battleLeftAlgorithmDropDown;
    
    [SerializeField] private Dropdown _battleRightAlgorithmDropDown;
    
    [SerializeField] private Slider _battleSpeedSlider;
    
    [SerializeField] private InputField _battleSpeedInputField;
    
    [SerializeField] private Dropdown _battleArrangementDropDown;
    
    [SerializeField] private Button _detailModeButton;
    
    [SerializeField] private Button _detailViewLeftButton;
    
    [SerializeField] private Button _detailViewRightButton;

    [SerializeField] private GameObject _leftClickHereText;
    [SerializeField] private GameObject _rightClickHereText;
    
    //Quiz
    [SerializeField] private Button _resetScoreButton;

    #endregion

    [SerializeField] private SortingController sortingController;

    [SerializeField] private GameObject quizScorePrefab;

    [Server]
    public override void ClientLoadedScene(NetworkConnection conn)
    {
        List<int> detailArray = sortingController.GetDetailArray();
        List<int> battleOrder = sortingController.GetBattleOrder();
        RpcUpdateClientInformation(conn, detailArray.ToArray(), battleOrder.ToArray());
    }

    [TargetRpc]
    private void RpcUpdateClientInformation(NetworkConnection client, int[] array, int[] order)
    {
        if (!isServer)
        {
            sortingController.SetDetailArray(array.ToList());
            sortingController.SetBattleOrder(order.ToList());
        }
    }

    protected override void Start()
    {
        base.Start();

        SpawnMyQuizPlayer(Maroon.NetworkManager.Instance.PlayerName);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        sortingController.GoOffline();
    }
    
    [Command(ignoreAuthority = true)]
    private void SpawnMyQuizPlayer(string playerName)
    {
        GameObject newQuizScore = Instantiate(quizScorePrefab);
        NetworkServer.Spawn(newQuizScore, Maroon.NetworkManager.Instance.GetConnectionByName(playerName));
    }

    public void SynchronizeArray(List<int> array)
    {
        if (Maroon.NetworkManager.Instance.IsInControl)
        {
            CmdSyncArray(array.ToArray());
        }
    }

    [Command(ignoreAuthority = true)]
    private void CmdSyncArray(int[] array)
    {
        RpcSyncArray(array);
    }

    [ClientRpc]
    private void RpcSyncArray(int[] array)
    {
        if (!Maroon.NetworkManager.Instance.IsInControl)
        {
            sortingController.SetDetailArray(array.ToList());
        }
    }
    
    public void SynchronizeBattleOrder(List<int> order)
    {
        if (Maroon.NetworkManager.Instance.IsInControl)
        {
            CmdSyncBattleOrder(order.ToArray());
        }
    }

    [Command(ignoreAuthority = true)]
    private void CmdSyncBattleOrder(int[] order)
    {
        RpcSyncBattleOrder(order);
    }

    [ClientRpc]
    private void RpcSyncBattleOrder(int[] order)
    {
        if (!Maroon.NetworkManager.Instance.IsInControl)
        {
            sortingController.SetBattleOrder(order.ToList());
        }
    }

    #region ControlHandling

    protected override void OnGetControl()
    {
        _resetButton.interactable = true;
        _playButton.interactable = true;
        _pauseButton.interactable = true;
        _stepBackwardButton.interactable = true;
        _stepForwardButton.interactable = true;
        
        _detailAlgorithmDropDown.interactable = true;
        _detailSizeSlider.interactable = true;
        _detailSizeInputField.interactable = true;
        _battleModeButton.interactable = true;
        
        _battleLeftAlgorithmDropDown.interactable = true;
        _battleRightAlgorithmDropDown.interactable = true;
        _battleSpeedSlider.interactable = true;
        _battleSpeedInputField.interactable = true;
        _battleArrangementDropDown.interactable = true;
        _detailModeButton.interactable = true;
        _detailViewLeftButton.interactable = true;
        _detailViewRightButton.interactable = true;
        
        _leftClickHereText.SetActive(true);
        _rightClickHereText.SetActive(true);

        _resetScoreButton.interactable = true;
    }

    protected override void OnLoseControl()
    {
        _resetButton.interactable = false;
        _playButton.interactable = false;
        _pauseButton.interactable = false;
        _stepBackwardButton.interactable = false;
        _stepForwardButton.interactable = false;
        
        _detailAlgorithmDropDown.interactable = false;
        _detailSizeSlider.interactable = false;
        _detailSizeInputField.interactable = false;
        _battleModeButton.interactable = false;
        
        _battleLeftAlgorithmDropDown.interactable = false;
        _battleRightAlgorithmDropDown.interactable = false;
        _battleSpeedSlider.interactable = false;
        _battleSpeedInputField.interactable = false;
        _battleArrangementDropDown.interactable = false;
        _detailModeButton.interactable = false;
        _detailViewLeftButton.interactable = false;
        _detailViewRightButton.interactable = false;
        
        _leftClickHereText.SetActive(false);
        _rightClickHereText.SetActive(false);

        _resetScoreButton.interactable = false;
    }

    #endregion

    #region Listeners

    protected override void AddListeners()
    {
        base.AddListeners();
        
        _resetButton.onClick.AddListener(ResetButtonClicked);
        //Play Pause Button already Mirrored with Simulation Mirroring!
        _stepBackwardButton.onClick.AddListener(StepBackwardButtonClicked);
        _stepForwardButton.onClick.AddListener(StepForwardButtonClicked);
        
        _detailAlgorithmDropDown.onValueChanged.AddListener(DetailAlgorithmDropDownChanged);
        _detailSizeSlider.onValueChanged.AddListener(DetailSizeSliderChanged);
        _detailSizeInputField.onEndEdit.AddListener(DetailSizeInputFieldEndEdit);
        _battleModeButton.onClick.AddListener(BattleModeButtonClicked);
        
        _battleLeftAlgorithmDropDown.onValueChanged.AddListener(BattleLeftAlgorithmDropDownChanged);
        _battleRightAlgorithmDropDown.onValueChanged.AddListener(BattleRightAlgorithmDropDownChanged);
        _battleSpeedSlider.onValueChanged.AddListener(BattleSpeedSliderChanged);
        _battleSpeedInputField.onEndEdit.AddListener(BattleSpeedInputFieldEndEdit);
        _battleArrangementDropDown.onValueChanged.AddListener(BattleArrangementDropDownChanged);
        _detailModeButton.onClick.AddListener(DetailModeButtonClicked);
        _detailViewLeftButton.onClick.AddListener(DetailViewLeftButtonClicked);
        _detailViewRightButton.onClick.AddListener(DetailViewRightButtonClicked);
        
        _resetScoreButton.onClick.AddListener(ResetScoreButtonClicked);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        
        _resetButton.onClick.RemoveListener(ResetButtonClicked);
        //Play Pause Button already Mirrored with Simulation Mirroring!
        _stepBackwardButton.onClick.RemoveListener(StepBackwardButtonClicked);
        _stepForwardButton.onClick.RemoveListener(StepForwardButtonClicked);
        
        _detailAlgorithmDropDown.onValueChanged.RemoveListener(DetailAlgorithmDropDownChanged);
        _detailSizeSlider.onValueChanged.RemoveListener(DetailSizeSliderChanged);
        _detailSizeInputField.onEndEdit.RemoveListener(DetailSizeInputFieldEndEdit);
        _battleModeButton.onClick.RemoveListener(BattleModeButtonClicked);
        
        _battleLeftAlgorithmDropDown.onValueChanged.RemoveListener(BattleLeftAlgorithmDropDownChanged);
        _battleRightAlgorithmDropDown.onValueChanged.RemoveListener(BattleRightAlgorithmDropDownChanged);
        _battleSpeedSlider.onValueChanged.RemoveListener(BattleSpeedSliderChanged);
        _battleSpeedInputField.onEndEdit.RemoveListener(BattleSpeedInputFieldEndEdit);
        _battleArrangementDropDown.onValueChanged.RemoveListener(BattleArrangementDropDownChanged);
        _detailModeButton.onClick.RemoveListener(DetailModeButtonClicked);
        _detailViewLeftButton.onClick.RemoveListener(DetailViewLeftButtonClicked);
        _detailViewRightButton.onClick.RemoveListener(DetailViewRightButtonClicked);
        
        _resetScoreButton.onClick.RemoveListener(ResetScoreButtonClicked);
    }

    #endregion

    #region InputHandlers

    //RESET BUTTON
    private void ResetButtonClicked()
    {
        SyncEvent(nameof(ResetButtonActivated));
    }

    private IEnumerator ResetButtonActivated()
    {
        _resetButton.onClick.Invoke();
        OnLoseControl();
        yield return null;
    }
    
    //STEP BACKWARD BUTTON
    private void StepBackwardButtonClicked()
    {
        SyncEvent(nameof(StepBackwardButtonActivated));
    }

    private IEnumerator StepBackwardButtonActivated()
    {
        _stepBackwardButton.onClick.Invoke();
        _stepBackwardButton.interactable = false;
        yield return null;
    }
    
    //STEP FORWARD BUTTON
    private void StepForwardButtonClicked()
    {
        SyncEvent(nameof(StepForwardButtonActivated));
    }

    private IEnumerator StepForwardButtonActivated()
    {
        _stepForwardButton.onClick.Invoke();
        _stepForwardButton.interactable = false;
        yield return null;
    }
    
    //Detail Mode
    
    private void DetailAlgorithmDropDownChanged(int value)
    {
        SyncEvent(nameof(DetailAlgorithmDropDownActivated), value);
    }

    private void DetailAlgorithmDropDownActivated(int value)
    {
        _detailAlgorithmDropDown.value = value;
        _detailAlgorithmDropDown.onValueChanged.Invoke(value);
    }
    
    private void DetailSizeSliderChanged(float value)
    {
        SyncEvent(nameof(DetailSizeSliderActivated), value);
    }

    private void DetailSizeSliderActivated(float value)
    {
        _detailSizeSlider.SetSliderValue(value);
        _detailSizeSlider.onValueChanged.Invoke(value);
    }

    private void DetailSizeInputFieldEndEdit(string valueString)
    {
        SyncEvent(nameof(DetailSizeInputFieldActivated), valueString);
    }

    private void DetailSizeInputFieldActivated(string valueString)
    {
        _detailSizeInputField.onEndEdit.Invoke(valueString);
    }
    
    private void BattleModeButtonClicked()
    {
        SyncEvent(nameof(BattleModeButtonActivated));
    }

    private void BattleModeButtonActivated()
    {
        _battleModeButton.onClick.Invoke();
    }
    
    //Battle Mode
    
    private void BattleLeftAlgorithmDropDownChanged(int value)
    {
        SyncEvent(nameof(BattleLeftAlgorithmDropDownActivated), value);
    }

    private void BattleLeftAlgorithmDropDownActivated(int value)
    {
        _battleLeftAlgorithmDropDown.value = value;
        _battleLeftAlgorithmDropDown.onValueChanged.Invoke(value);
    }
    
    private void BattleRightAlgorithmDropDownChanged(int value)
    {
        SyncEvent(nameof(BattleRightAlgorithmDropDownActivated), value);
    }

    private void BattleRightAlgorithmDropDownActivated(int value)
    {
        _battleRightAlgorithmDropDown.value = value;
        _battleRightAlgorithmDropDown.onValueChanged.Invoke(value);
    }
    
    private void BattleSpeedSliderChanged(float value)
    {
        SyncEvent(nameof(BattleSpeedSliderActivated), value);
    }

    private void BattleSpeedSliderActivated(float value)
    {
        _battleSpeedSlider.SetSliderValue(value);
        _battleSpeedSlider.onValueChanged.Invoke(value);
    }

    private void BattleSpeedInputFieldEndEdit(string valueString)
    {
        SyncEvent(nameof(BattleSpeedInputFieldActivated), valueString);
    }

    private void BattleSpeedInputFieldActivated(string valueString)
    {
        _battleSpeedInputField.onEndEdit.Invoke(valueString);
    }
    
    private void BattleArrangementDropDownChanged(int value)
    {
        SyncEvent(nameof(BattleArrangementDropDownActivated), value);
    }

    private void BattleArrangementDropDownActivated(int value)
    {
        _battleArrangementDropDown.value = value;
        _battleArrangementDropDown.onValueChanged.Invoke(value);
    }
    
    private void DetailModeButtonClicked()
    {
        SyncEvent(nameof(DetailModeButtonActivated));
    }

    private void DetailModeButtonActivated()
    {
        _detailModeButton.onClick.Invoke();
    }
    
    private void DetailViewLeftButtonClicked()
    {
        SyncEvent(nameof(DetailViewLeftButtonActivated));
    }

    private void DetailViewLeftButtonActivated()
    {
        _detailViewLeftButton.onClick.Invoke();
    }
    
    private void DetailViewRightButtonClicked()
    {
        SyncEvent(nameof(DetailViewRightButtonActivated));
    }

    private void DetailViewRightButtonActivated()
    {
        _detailViewRightButton.onClick.Invoke();
    }
    
    //Quiz
    private void ResetScoreButtonClicked()
    {
        SyncEvent(nameof(ResetScoreButtonActivated));
    }

    private void ResetScoreButtonActivated()
    {
        _resetScoreButton.onClick.Invoke();
    }

    #endregion
}
