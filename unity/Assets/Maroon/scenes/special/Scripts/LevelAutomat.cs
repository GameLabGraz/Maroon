﻿using System;
using System.Collections;
using System.Collections.Generic;
using GEAR.Localization.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LevelAutomat : MonoBehaviour
{
    [Serializable]
    public struct CategoryMaterial
    {
        public string categoryName;
        public Material material;
        public Material highlightMaterial;
    }
    
    [SerializeField] private TextMeshPro display;
    private string _displayString = "";
    [SerializeField] private GameObject diskParent;
    [SerializeField] private GameObject diskPrefab;
    [SerializeField] private List<Transform> defaultPositions;
    [SerializeField] private Transform outputPosition;
    
    [Header("Additional Loading Scenes")]
    [SerializeField] private Maroon.CustomSceneAsset targetLabScenePC;
    [SerializeField] private Maroon.CustomSceneAsset targetLabSceneVR;

    [Header("Materials and Representation")] 
    [SerializeField] private Material generalLabMaterial;
    [SerializeField] private List<CategoryMaterial> categoryMaterials;


    [Header("Debug Variables")] 
    [SerializeField] private bool showNext = false;
    [SerializeField] private bool showPrev = false;
    [SerializeField] private bool createDisk = false;
    [SerializeField] private bool addChar = false;
    [SerializeField] private string myChar = "1";
    [SerializeField] private bool clearChars = false;
    

    private Dictionary<int, List<GameObject>> diskCollection = new Dictionary<int, List<GameObject>>(); // <Page, Disk>

    private int _pageCount = 0;
    private int _currentPage = 0;

    private float _currentTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        FillAutomat();
    }

    // Update is called once per frame
    void Update()
    {
        _currentTime += Time.deltaTime;
        if (showNext)
        {
            ShowNextPage();
            showNext = false;
        }

        if (showPrev)
        {
            ShowPrevPage();
            showPrev = false;
        }

        if (addChar)
        {
            StartCoroutine(CreateDebugDisks());
            
            addChar = false;
            createDisk = false;
        }

        // if (createDisk)
        // {
        //     CreateEnteredDisk();
        //     createDisk = false;
        // }

        if (clearChars)
        {
            OnClear();
            clearChars = false;
        }
    }
    
    public IEnumerator CreateDebugDisks()
    {
        yield return new WaitForSeconds(5);
        
        for (int i = 0; i < myChar.Length; ++i)
        {
            OnAddChar(myChar[i].ToString());

            yield return new WaitUntil(() =>
            {
                if (_currentTime <= 0.3f) return false;
                _currentTime = 0f;
                return true;
            });
            if (i % 2 == 1)
            {
                CreateEnteredDisk();
                
                yield return new WaitUntil(() =>
                {
                    if (_currentTime <= 0.3f) return false;
                    _currentTime = 0f;
                    return true;
                });
                OnClear();

                // yield return new WaitForSeconds(1);
            }
        }

        
        // 233122332211223131333223121212
        
    }

    public void OnAddChar(string c)
    {
        Debug.Assert(c.Length == 1);
        if (_displayString.Length >= 2) return;
        _displayString += c;
        display.text = _displayString;
    }

    public void OnClear()
    {
        _displayString = "";
        display.text = _displayString;
    }

    private Material GetMaterialForCategory(string categoryName, bool highlight = false)
    {
        foreach (var catMat in categoryMaterials)
        {
            if (catMat.categoryName.Equals(categoryName))
                return highlight ? catMat.highlightMaterial : catMat.material;
        }
        
        Debug.Assert(false, "Material for Category '" + categoryName + "' not found.");
        return null;
    }
    //
    // protected void 

    protected GameObject CreateSceneDisk(Maroon.CustomSceneAsset scene, Vector3 position, int currentPage, Material mat, Material highlightMat)
    {
        var sceneDisk = Instantiate(diskPrefab, position, Quaternion.identity, diskParent.transform);
        Debug.Assert(sceneDisk);
        
        var disk = sceneDisk.GetComponent<LevelDisk>();
        Debug.Assert(disk);

        disk.SetupExperiment(scene);
        disk.Setup(mat, highlightMat, scene.SceneNameWithoutPlatformExtension);

        return sceneDisk;
    }

    protected GameObject CreateGeneralDisk(Maroon.SceneCategory category, Vector3 position, int currentPage)
    {
        var generalDisk = Instantiate(diskPrefab, position, Quaternion.identity, diskParent.transform);
        Debug.Assert(generalDisk);
        
        var disk = generalDisk.GetComponent<LevelDisk>();
        Debug.Assert(disk);

        disk.SetupLaboratory(category, Maroon.PlatformManager.Instance.CurrentPlatformIsVR ? targetLabSceneVR : targetLabScenePC);
        disk.Setup(generalLabMaterial, GetMaterialForCategory(category.Name, true),
            category.Name + " Lab");

        return generalDisk;
    }

    protected void FreezeObject(GameObject obj)
    {
        var rb = obj.GetComponent<Rigidbody>();
        if (rb)
            rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void FillAutomat()
    {
        var platformSceneType = Maroon.PlatformManager.Instance.SceneTypeBasedOnPlatform;
        var categories = Maroon.SceneManager.Instance.getSceneCategories(platformSceneType);
        // var categories = Maroon.SceneManager.Instance.getSceneCategories(SceneType.Standard);

        int currentTransform = 0;
        int currentPage = 0;

        foreach (var cat in categories)
        {
            //general disk for the whole e.g. 'physics lab'
            var generalDisk = CreateGeneralDisk(cat, defaultPositions[currentTransform].position, currentPage);
            if(currentTransform == 0)
                diskCollection.Add(currentPage, new List<GameObject>());
            diskCollection[currentPage].Add(generalDisk);
            FreezeObject(generalDisk);
            generalDisk.SetActive(currentPage == 0);

            currentTransform++;
            if (currentTransform >= 9)
            {
                currentTransform = 0;
                currentPage++;
            }

            var categoryMat = GetMaterialForCategory(cat.Name);
            var categoryHighlightMat = GetMaterialForCategory(cat.Name, true);

            int i = 0;
            foreach (var scene in cat.Scenes)
            {
                ++i;
                if (i == 5)
                    break;
                
                var sceneDisk = CreateSceneDisk(scene, defaultPositions[currentTransform].position, currentPage,
                    categoryMat, categoryHighlightMat);
                if(currentTransform == 0)
                    diskCollection.Add(currentPage, new List<GameObject>());
                diskCollection[currentPage].Add(sceneDisk);
                FreezeObject(sceneDisk);
                sceneDisk.SetActive(currentPage == 0);

                currentTransform++;
                if (currentTransform >= 9)
                {
                    currentTransform = 0;
                    currentPage++;
                }
            }
        }

        _pageCount = currentPage + 1;
    }

    public void ShowNextPage()
    {
        if (_pageCount == 1)
            return;
        
        Debug.Assert(diskCollection.ContainsKey(_currentPage));
        foreach(var disk in diskCollection[_currentPage])
            disk.SetActive(false);

        _currentPage++;
        if (_currentPage >= _pageCount)
            _currentPage = 0;

        Debug.Assert(diskCollection.ContainsKey(_currentPage));
        foreach(var disk in diskCollection[_currentPage])
            disk.SetActive(true);
    }

    public void ShowPrevPage()
    {
        if (_pageCount == 1)
            return;
        
        Debug.Assert(diskCollection.ContainsKey(_currentPage));
        foreach(var disk in diskCollection[_currentPage])
            disk.SetActive(false);

        _currentPage--;
        if (_currentPage < 0)
            _currentPage = _pageCount - 1;

        Debug.Assert(diskCollection.ContainsKey(_currentPage));
        foreach(var disk in diskCollection[_currentPage])
            disk.SetActive(true);
    }

    protected int GetIndexFromDisplayString()
    {
        switch (_displayString)
        {
            case "11": return 0;
            case "12": return 1;
            case "13": return 2;
            case "21": return 3;
            case "22": return 4;
            case "23": return 5;
            case "31": return 6;
            case "32": return 7;
            case "33": return 8;
        }

        return -1;
    }

    public void CreateEnteredDiskIfTrue(bool val)
    {
        if (!val)
            return;
        CreateEnteredDisk();
    }
    
    public void CreateEnteredDisk()
    {
        var idx = GetIndexFromDisplayString();
        Debug.Assert(diskCollection.ContainsKey(_currentPage));
        if (idx == -1 || idx >= diskCollection[_currentPage].Count)
            return; // no disk to generate here

        var originalDisk = diskCollection[_currentPage][idx];
        Debug.Assert(originalDisk);

        var newDisk = Instantiate(originalDisk, outputPosition.position, outputPosition.rotation, transform.parent);
        Debug.Assert(newDisk);

        var rb = newDisk.GetComponent<Rigidbody>();
        if (rb)
            rb.constraints = RigidbodyConstraints.None;
        
        OnClear();
    }
}
