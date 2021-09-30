using System;
using System.Collections;
using System.Collections.Generic;
using Maroon;
using UnityEngine;

public class LevelDisk : MonoBehaviour
{
    public bool isLabScene = false;
    public Maroon.CustomSceneAsset ExperimentScene;
    public string categoryName;

    private Maroon.SceneCategory _category;

    protected void Start()
    {
        if (categoryName != "")
        {
            _category = Maroon.SceneManager.Instance.getSceneCategoryByName(categoryName, SceneType.VR);
        }
    }

    public bool HasScene()
    {
        if (isLabScene)
        {
            return ExperimentScene != null && _category != null;
        }
        return ExperimentScene != null;
    }

    public void GoToScene()
    {
        if (!HasScene())
            return;

        if (isLabScene)
        {
            Maroon.SceneManager.Instance.ActiveSceneCategory = _category;
        }
        Maroon.SceneManager.Instance.LoadSceneRequest(ExperimentScene);
    }
}
