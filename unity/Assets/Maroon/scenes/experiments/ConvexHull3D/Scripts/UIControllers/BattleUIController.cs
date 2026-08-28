using System.Collections.Generic;
using GEAR.Localization;
using Maroon.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.ComputerScience.ConvexHull3D
{
    public class BattleUIController : MonoBehaviour
    {
        [Header("Controllers")]
        [SerializeField] private ExperimentController   experimentController;
        [SerializeField] private BattleModeController   battleController;

        [Header("Settings")]
        [SerializeField] private TMP_Dropdown           leftAlgoDropdown;
        [SerializeField] private TMP_Dropdown           rightAlgoDropdown;
        [SerializeField] private Slider                 animationSpeedSlider;
        [SerializeField] private Button                 exitBattleButton;

        [Header("Labels")]
        [SerializeField] private TextMeshProUGUI        leftTitleLabel;
        [SerializeField] private TextMeshProUGUI        leftOperationsLabel;
        [SerializeField] private TextMeshProUGUI        rightTitleLabel;
        [SerializeField] private TextMeshProUGUI        rightOperationsLabel;

        //----------------------------------------------------------------------------------

        private void Start()
        {
            battleController.LeftHull.OnStep                += HandleLeftStep;
            battleController.LeftHull.OnSimulationReset     += RefreshLeft;

            battleController.RightHull.OnStep               += HandleRightStep;
            battleController.RightHull.OnSimulationReset    += RefreshRight;


            leftAlgoDropdown.onValueChanged.AddListener(OnLeftAlgoDropdownChanged);
            rightAlgoDropdown.onValueChanged.AddListener(OnRightAlgoDropdownChanged);

            animationSpeedSlider.onValueChanged.AddListener(battleController.SetSpeed);
            exitBattleButton.onClick.AddListener(experimentController.EnterDetailMode);

            LanguageManager.Instance.OnLanguageChanged.AddListener(OnLanguageChanged);

            PopulateDropdowns();
        }

        private void OnDestroy()
        {
            battleController.LeftHull.OnStep                -= HandleLeftStep;
            battleController.LeftHull.OnSimulationReset     -= RefreshLeft;

            battleController.RightHull.OnStep               -= HandleRightStep;
            battleController.RightHull.OnSimulationReset    -= RefreshRight;

            leftAlgoDropdown.onValueChanged.RemoveListener(OnLeftAlgoDropdownChanged);
            rightAlgoDropdown.onValueChanged.RemoveListener(OnRightAlgoDropdownChanged);

            animationSpeedSlider.onValueChanged.RemoveListener(battleController.SetSpeed);
            exitBattleButton.onClick.RemoveListener(experimentController.EnterDetailMode);

            LanguageManager.Instance.OnLanguageChanged.RemoveListener(OnLanguageChanged);
        }

        //----------------------------------------------------------------------------------

        private void OnLanguageChanged(SystemLanguage language)
        {
            RefreshLeft();
            RefreshRight();
        }

        private void HandleLeftStep(int _)  => RefreshLeft();
        private void HandleRightStep(int _) => RefreshRight();

        private void RefreshLeft()  => RefreshLabels(battleController.LeftHull, leftTitleLabel, leftOperationsLabel);
        private void RefreshRight() => RefreshLabels(battleController.RightHull, rightTitleLabel, rightOperationsLabel);


        private void RefreshLabels(ConvexHull3D currenthull, TextMeshProUGUI titleLabel, TextMeshProUGUI operationsLabel)
        {
            titleLabel.text = currenthull.CurrentAlgorithm.Name;
            operationsLabel.text = LanguageManager.Instance.GetString("CH_StepsPrefix") + currenthull.StepCount;
        }

        private void PopulateDropdowns()
        {
            PopulateAlgorithmDropdown(leftAlgoDropdown, battleController.LeftHull);
            PopulateAlgorithmDropdown(rightAlgoDropdown, battleController.RightHull);
        }

        private void OnLeftAlgoDropdownChanged(int index)
        {
            battleController.LeftHull.SetAlgorithmFromDropdown(index);
            battleController.RightHull.ClearHull();
        }

        private void OnRightAlgoDropdownChanged(int index)
        {
            battleController.RightHull.SetAlgorithmFromDropdown(index);
            battleController.LeftHull.ClearHull();
        }

        private void PopulateAlgorithmDropdown(TMP_Dropdown dropdown, ConvexHull3D currenthull)
        {
            dropdown.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>();
            
            foreach(var algo in currenthull.Algorithms)
            {
                options.Add(new TMP_Dropdown.OptionData(algo.Name));
            }

            dropdown.AddOptions(options);
            dropdown.SetValueWithoutNotify(currenthull.SelectedAlgorithmIndex);
            dropdown.RefreshShownValue();
        }
    }
}