using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LimeSurveyData
{
    [Serializable] public class ExperimentEntry
    {
        public string SceneName;
        public List<ExperimentMeasurements> measurements;
    }

    [CreateAssetMenu(fileName = "SurveyData", menuName = "LimeSurvey/ExperimentData")]
    public class ExperimentData : ScriptableObject
    {
        [Tooltip("The ID LimeSurvey returns once the pre-questionnaire was submitted.")]
        public int ResponseID;

        [Tooltip("Collection of the visited scenes including their data collections.")]
        //(list because user could visit the scene multiple times)
        [SerializeField] private List<ExperimentEntry> Scenes = new List<ExperimentEntry>();

        public void AddSceneInfo(ExperimentMeasurements surveySceneInfo)
        {
            if (Scenes.All(sceneInfo => sceneInfo.SceneName != surveySceneInfo.SceneName))
            {
                Scenes.Add(new ExperimentEntry
                {
                    SceneName = surveySceneInfo.SceneName,
                    measurements = new List<ExperimentMeasurements> { surveySceneInfo }
                });
            }
            else
            {
                Scenes.Find(sceneInfo => sceneInfo.SceneName == surveySceneInfo.SceneName)
                    .measurements.Add(surveySceneInfo);
            }
        }
    }

    public class HuygensPrincipleData : ExperimentData
    {

    }

}