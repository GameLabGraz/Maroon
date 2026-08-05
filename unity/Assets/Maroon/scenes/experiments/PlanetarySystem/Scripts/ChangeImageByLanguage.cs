using GEAR.Localization;
using UnityEngine;


namespace Maroon.Experiments.PlanetarySystem
{
    public class ChangeImageByLanguage : MonoBehaviour
    {
        [Header("PlanetarySorting Game Planet Chart Image")]
        [SerializeField] private Material sortingGamePlanetChartMaterial;
        [SerializeField] private Texture2D sortingGamePlanetChartDETexture;
        [SerializeField] private Texture2D sortingGamePlanetChartENGTexture;

        [Header("Start Planetary Sorting Game Image")]
        [SerializeField] private Material startSortingGameMaterial;
        [SerializeField] private Texture2D startSortingGameDETexture;
        [SerializeField] private Texture2D startSortingGameENGTexture;

        [Header("Start Planetary System Simulation Image")]
        [SerializeField] private Material startSimulationMaterial;
        [SerializeField] private Texture2D startSimulationDETexture;
        [SerializeField] private Texture2D startSimulationENGTexture;

        //---------------------------------------------------------------------------------------

        /// <summary>
        /// set up listener for language change event
        /// set the initial image
        /// </summary>
        private void Start()
        {
            LanguageManager.Instance.OnLanguageChanged.AddListener(ChangeImageLanguage);
            RefreshMaterials();
        }


        /// <summary>
        /// change image textures depending on language
        /// </summary>
        /// <param name="lang"></param>
        private void ChangeImageLanguage(SystemLanguage lang)
        {
            if (lang.Equals(SystemLanguage.German))
            {
                sortingGamePlanetChartMaterial.mainTexture = sortingGamePlanetChartDETexture;
                startSortingGameMaterial.mainTexture = startSortingGameDETexture;
                //same texture in DE and ENG
                startSimulationMaterial.mainTexture = startSimulationDETexture;
            }
            else
            {
                sortingGamePlanetChartMaterial.mainTexture = sortingGamePlanetChartENGTexture;
                startSortingGameMaterial.mainTexture = startSortingGameENGTexture;
                startSimulationMaterial.mainTexture = startSimulationENGTexture;
            }
        }


        /// <summary>
        /// Call ChangeImageLanguage with the current language
        /// </summary>
        public void RefreshMaterials()
        {
            ChangeImageLanguage(LanguageManager.Instance.CurrentLanguage);
        }


        /// <summary>
        /// remove language change listener
        /// </summary>
        private void OnDestroy()
        {
            LanguageManager.Instance.OnLanguageChanged.RemoveListener(ChangeImageLanguage);
        }
    }
}
