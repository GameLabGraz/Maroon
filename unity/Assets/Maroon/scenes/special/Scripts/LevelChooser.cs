using System.Collections;
using System.Collections.Generic;
using GEAR.VRInteraction;
using UnityEngine;

public class LevelChooser : MonoBehaviour
{
    [SerializeField]
    private VRSnapDropZone snapZone;
    [SerializeField]
    private GameObject button;
    
    private LevelDisk _currentDisk = null;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(snapZone && button);

        button.SetActive(false);
        
        snapZone.onSnap.AddListener(OnDiskSnapped);
        snapZone.onUnsnap.AddListener(OnDiskUnsnapped);
    }

    public void OnDiskSnapped(VRSnapDropZone zone, GameObject newObject)
    {
        _currentDisk = newObject.GetComponent<LevelDisk>();
        Debug.Assert(_currentDisk);
        button.SetActive(_currentDisk != null);
    }
    
    public void OnDiskUnsnapped(VRSnapDropZone zone, GameObject newObject)
    {
        var tmp = newObject.GetComponent<LevelDisk>();
        if (tmp == _currentDisk)
        {
            _currentDisk = null;
            button.SetActive(true);
        }
    }

    public void LoadCurrentLevel()
    {
        Debug.Log("Load Current Level");
        Debug.Assert(_currentDisk && _currentDisk.HasScene());
        _currentDisk.GoToScene();
    }
    
}
