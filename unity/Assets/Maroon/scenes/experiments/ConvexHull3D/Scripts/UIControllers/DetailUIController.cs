using System.Collections.Generic;
using GEAR.Localization;
using Maroon.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Slider = GameLabGraz.UI.Slider;

namespace Maroon.ComputerScience.ConvexHull3D
{
    public class DetailUIController : MonoBehaviour
    {

        [Header("ConvexHull")]
        [SerializeField] private ExperimentController   experimentController;
        [SerializeField] private DetailModeController   detailController;


        [Header("Settings")]
        [SerializeField] private TMP_Dropdown           algorithmDropdown;
        [SerializeField] private Slider                 pointCountSlider;
        [SerializeField] private Slider                 speedSlider;
        [SerializeField] private Toggle                 showSearchLinesToggle;
        [SerializeField] private Toggle                 showFacesToggle;
        [SerializeField] private Button                 generatePointsButton;
        [SerializeField] private Button                 battleModeButton;

        [SerializeField] private TextMeshProUGUI        facesAndStepsLabel;
        [SerializeField] private TextMeshProUGUI        descriptionText;

        private ConvexHull3D CenterHull => detailController.CenterHull;

        //----------------------------------------------------------------------------------

        private void Start()
        {
            CenterHull.OnStep            += HandleStep;
            CenterHull.OnSimulationReset += RefreshFacesAndStepsLabel;

            algorithmDropdown.onValueChanged.AddListener(OnAlgorithmDropdownChanged);
            generatePointsButton.onClick.AddListener(CenterHull.GeneratePoints);

            pointCountSlider.maxValue = 50;
            speedSlider.maxValue = 10;

            pointCountSlider.onValueChanged.AddListener(detailController.SetPointCount);
            speedSlider.onValueChanged.AddListener(detailController.SetSpeed);

            showSearchLinesToggle.onValueChanged.AddListener(OnShowSearchLinesChanged);
            showFacesToggle.onValueChanged.AddListener(OnShowFacesChanged);

            battleModeButton.onClick.AddListener(experimentController.EnterBattleMode);

            PopulateAlgorithmDropdown();
            SetDescription();
            
            RefreshFacesAndStepsLabel();

            LanguageManager.Instance.OnLanguageChanged.AddListener(OnLanguageChanged);
        }

        private void OnDestroy()
        {
            CenterHull.OnStep            -= HandleStep;
            CenterHull.OnSimulationReset -= RefreshFacesAndStepsLabel;

            algorithmDropdown.onValueChanged.RemoveListener(OnAlgorithmDropdownChanged);
            generatePointsButton.onClick.RemoveListener(CenterHull.GeneratePoints);

            pointCountSlider.onValueChanged.RemoveListener(detailController.SetPointCount);
            speedSlider.onValueChanged.RemoveListener(detailController.SetSpeed);

            showSearchLinesToggle.onValueChanged.RemoveListener(OnShowSearchLinesChanged);
            showFacesToggle.onValueChanged.RemoveListener(OnShowFacesChanged);

            battleModeButton.onClick.RemoveListener(experimentController.EnterBattleMode);

            LanguageManager.Instance.OnLanguageChanged.RemoveListener(OnLanguageChanged);
        }

        //----------------------------------------------------------------------------------

        private void HandleStep(int _) => RefreshFacesAndStepsLabel();
        
        private void OnLanguageChanged(SystemLanguage language)
        {
            SetDescription();
            RefreshFacesAndStepsLabel();
        }

        private void OnAlgorithmDropdownChanged(int index)
        {
            CenterHull.SetAlgorithmFromDropdown(index);

            SetDescription();
            RefreshFacesAndStepsLabel();
        }

        private void OnShowSearchLinesChanged(bool value)
        {
            CenterHull.ShowSearchLines = value;
        }
        
        private void OnShowFacesChanged(bool value)
        {
            CenterHull.ShowFaces = value;
        }

        private void SetDescription()
        {
            string key = CenterHull.SelectedAlgorithmIndex switch
            {
                0 => "Gift Wrapping Description",
                1 => "Incremental Description",
                2 => "QuickHull Description",
                _ => ""
            };

            descriptionText.text = LanguageManager.Instance.GetString(key);
        }

        private void RefreshFacesAndStepsLabel()
        {
            facesAndStepsLabel.text = LanguageManager.Instance.GetString("CH_FacesPrefix") + CenterHull.HullFaceCount + "\n" +
                                    LanguageManager.Instance.GetString("CH_StepsPrefix") + CenterHull.StepCount;
        }

        private void PopulateAlgorithmDropdown()
        {
            algorithmDropdown.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>();

            foreach(var algo in CenterHull.Algorithms)
            {
                options.Add(new TMP_Dropdown.OptionData(algo.Name));
            }

            algorithmDropdown.AddOptions(options);
            algorithmDropdown.SetValueWithoutNotify(CenterHull.SelectedAlgorithmIndex);
            algorithmDropdown.RefreshShownValue();
        }
    }
}