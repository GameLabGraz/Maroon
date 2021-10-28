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
    
    private Disk _currentDisk = null;
    
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
        _currentDisk = newObject.GetComponent<Disk>();
        if (!_currentDisk) return;
        
        if (_currentDisk.GetActivateSnapObject())
        {
            _currentDisk.GetActivateSnapObject().SetActive(true);
        }

        var levelDisk = _currentDisk as LevelDisk;
        if (levelDisk && levelDisk.HasScene())
        {
            button.SetActive(true);
        }
    }
    
    public void OnDiskUnsnapped(VRSnapDropZone zone, GameObject newObject)
    {
        var disk = newObject.GetComponent<Disk>();
        if (disk != _currentDisk) return;
        
        if (disk.GetActivateSnapObject())
        {
            disk.GetActivateSnapObject().SetActive(false);
        }
            
        _currentDisk = null;
        button.SetActive(false);
    }

    public void LoadCurrentLevel()
    {
        var levelDisk = _currentDisk as LevelDisk;
        if (levelDisk && levelDisk.HasScene())
        {
            levelDisk.GoToScene();
        }
    }
    
}
