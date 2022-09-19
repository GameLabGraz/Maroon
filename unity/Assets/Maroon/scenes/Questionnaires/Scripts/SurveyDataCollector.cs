using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LimeSurveyData
{
    [CreateAssetMenu(fileName = "SurveyData", menuName = "LimeSurvey/SurveyDataCollector")]
    public class SurveyDataCollector : ScriptableObject
    {
        [Tooltip("The ID LimeSurvey returns once the pre-questionnaire was submitted.")]
        public int ResponseID;

        [Tooltip("Collection of the visited scenes including their data collections.")]
        //key is the scene name, scene info collects all info of the user from the scene
        //(list because user could visit the scene multiple times)
        public Dictionary<string, List<SurveySceneInfo>> Scenes = new Dictionary<string, List<SurveySceneInfo>>();

        public void AddSceneInfo(SurveySceneInfo surveySceneInfo)
        {
            if (!Scenes.ContainsKey(surveySceneInfo.SceneName))
            {
               Scenes.Add(surveySceneInfo.SceneName, new List<SurveySceneInfo>()); 
            }
            
            Scenes[surveySceneInfo.SceneName].Add(surveySceneInfo);
        }
    }
}