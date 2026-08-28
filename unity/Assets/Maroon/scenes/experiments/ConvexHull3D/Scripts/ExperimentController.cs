using GEAR.Localization;
using Maroon.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.ComputerScience.ConvexHull3D
{
    public class ExperimentController : MonoBehaviour, IResetObject
    {
        [Header("UI")]
        [SerializeField] private GameObject             normalUI;
        [SerializeField] private GameObject             battleUI;
        [SerializeField] private GameObject             battlePanels;
        [SerializeField] private GameObject             descriptionPanel;

        [Header("Camera")]
        [SerializeField] private SimpleCameraController cameraController;

        [Header("Controllers")]
        [SerializeField] private DetailModeController   detailController;
        [SerializeField] private BattleModeController   battleController;

        [Header("Helpi")]
        [SerializeField] private DialogueManager        dialogueManager;

        [Header("Controlls")]
        [SerializeField] private Button                 playButton;
        [SerializeField] private Button                 pauseButton;
        [SerializeField] private Button                 resetButton;
        [SerializeField] private Button                 nextStepButton;
        [SerializeField] private Button                 previousStepButton;

        private bool _inBattleMode;

        //----------------------------------------------------------------------------------

        private void Start()
        {
            detailController.OnDetailComplete   += OnDetailHullComplete;
            battleController.OnRaceComplete     += OnRaceComplete;

            EnterDetailMode();

            playButton.onClick.AddListener(Play);
            pauseButton.onClick.AddListener(Pause);
            resetButton.onClick.AddListener(Reset);

            nextStepButton.onClick.AddListener(NextStep);
            previousStepButton.onClick.AddListener(PreviousStep);

            SimulationController.Instance.onStartRunning.AddListener(OnSimulationStarted);
            SimulationController.Instance.onStopRunning.AddListener(OnSimulationStopped);
        }

        private void OnDestroy()
        {
            detailController.OnDetailComplete   -= OnDetailHullComplete;
            battleController.OnRaceComplete     -= OnRaceComplete;

            playButton.onClick.RemoveListener(Play);
            pauseButton.onClick.RemoveListener(Pause);
            resetButton.onClick.RemoveListener(Reset);

            nextStepButton.onClick.RemoveListener(NextStep);
            previousStepButton.onClick.RemoveListener(PreviousStep);

            SimulationController.Instance.onStartRunning.RemoveListener(OnSimulationStarted);
            SimulationController.Instance.onStopRunning.RemoveListener(OnSimulationStopped);
        }

        //----------------------------------------------------------------------------------

        public void EnterDetailMode()
        {
            _inBattleMode = false;

            battleController.LeftHullObject.SetActive(false);
            battleController.RightHullObject.SetActive(false);
            battleUI.SetActive(false);
            battlePanels.SetActive(false);

            detailController.CenterHullObject.SetActive(true);
            normalUI.SetActive(true);
            descriptionPanel.SetActive(true);
            nextStepButton.gameObject.SetActive(true);
            previousStepButton.gameObject.SetActive(true);

            cameraController.target = detailController.CenterHullTransform;
            cameraController.secondaryTarget = null;

            ShowHelp("EnterConvexHullExperiment");

            detailController.Enter();
        }

        public void EnterBattleMode()
        {
            _inBattleMode = true;

            detailController.CenterHullObject.SetActive(false);
            normalUI.SetActive(false);
            descriptionPanel.SetActive(false);
            nextStepButton.gameObject.SetActive(false);
            previousStepButton.gameObject.SetActive(false);

            battleController.LeftHullObject.SetActive(true);
            battleController.RightHullObject.SetActive(true);
            battleUI.SetActive(true);
            battlePanels.SetActive(true);
            
            cameraController.target = battleController.LeftHullTransform;
            cameraController.secondaryTarget = battleController.RightHullTransform;
            
            ShowHelp("EnterBattleMode");

            battleController.Enter();
        }


        public void Play()                  => SimulationController.Instance.StartSimulation();
        public void Pause()                 => SimulationController.Instance.StopSimulation();
        public void Reset()                 => SimulationController.Instance.ResetSimulation();
        public void NextStep()              => detailController.NextStep();
        public void PreviousStep()          => detailController.PreviousStep();

        private void OnDetailHullComplete() => SimulationController.Instance.StopSimulation();
        private void OnRaceComplete()       => SimulationController.Instance.StopSimulation();

        private void OnSimulationStarted()
        {
            if(_inBattleMode)
            {
                battleController.Play();
            }
            else
            {
                detailController.Play();
            }
        }

        private void OnSimulationStopped()
        {
            if(_inBattleMode)
            {
                battleController.Pause();
            }
            else
            {
                detailController.Pause();
            }
        }

        public void ResetObject()
        {
            if(_inBattleMode)
            {
                battleController.Reset();
            }
            else
            {
                detailController.Reset();
            }
        }

        private void ShowHelp(string key)
        {
            dialogueManager.ShowMessage(LanguageManager.Instance.GetString(key));
        }
    }
}
