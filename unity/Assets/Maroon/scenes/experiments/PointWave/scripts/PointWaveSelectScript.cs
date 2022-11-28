using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointWaveSelectScript : MonoBehaviour
{
    public enum SelectObjectType
    {
        SourceSelect,
        VisualizationPlaneSelect,
    }
    
    public List<GameObject> highlightObjects = new List<GameObject>();
    public SelectObjectType type;
    public string nameKey;



    //Testing mouse detection 

    public PointWaveWaterPlane waterPlane;
    private PointWavePoolHandler _waveLogic;
    private void OnDisable()
    {
        if (!_waveLogic || _waveLogic.GetComponent<PointWave_SelectionHandler>().selectedObject != this) return;
        Deselect();
        DeselectMe();
    }
 

    private void OnMouseDown()
    {
        if (!_waveLogic)
        {
            var obj = GameObject.Find("PoolHandler");
            if (obj)
                _waveLogic = obj.GetComponent<PointWavePoolHandler>();
        }
        Debug.LogWarning("here");
        //    Debug.LogWarning(Input.mousePosition);
        Vector3 worldPosition = new Vector3();
        Camera c = Camera.main;
        Ray ray = c.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
           // Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
            Debug.Log("normal :" + hit.normal);
            //hit.collider;
            if(hit.transform.gameObject == GameObject.Find("innerBathub")) // can add boundary maybe
            {
              var test =   hit.point;
                Debug.Log("test x: "+ test.x);
                Debug.Log("test y: "+ test.y);
                Debug.Log("test z: "+ test.z);
                worldPosition = hit.point;
                BoxCollider col = GetComponent<BoxCollider>();
            //    Debug.Log(col.size.x/2);

                var data =  transform.InverseTransformPoint(worldPosition);
              //  Debug.Log("Data : " + data);
                float x = 0;
                float z = 0;

                x = (data.x / (col.size.x / 2));
                z = (data.z / (col.size.z / 2));

           


                
                worldPosition = data;
                worldPosition.x = x;
                worldPosition.z = z;
                //GetComponent<GameObject.bounds.size;
                //   Debug.Log("BATHTUB");
            }
            Debug.Log(hit.transform.gameObject);

        //    Debug.Log("Area from normal/direction : " + CalculateFacingArea(selcectedMesh, hit.normal));
          //  return CalculateFacingArea(selcectedMesh, hit.normal);
        }
        Debug.Log(hit.ToString());
     //   return 0f;
    
        //worldPosition = Input.mousePosition;
        Debug.LogWarning(worldPosition);

        waterPlane.AddMouseData(worldPosition);
     //   var test =  _waveLogic.GetComponent<PointWaveWaterPlane>();
       // test.AddMouseData(Input.mousePosition);
        Select();
    }

    public void Update()
    {
        waterPlane.UpdateMeshData();
    }



    public void Select()
    {        
        if (!_waveLogic)
        {
            var obj = GameObject.Find("PoolHandler");
            if (obj)
                _waveLogic = obj.GetComponent<PointWavePoolHandler>();
        }
        var selectHandler = _waveLogic.GetComponent<PointWave_SelectionHandler>();
        selectHandler.SelectObject(this);
        
        foreach (var highlightObj in highlightObjects)
        {
            var mats = highlightObj.GetComponent<MeshRenderer>().sharedMaterials.ToList();
            if(!mats.Contains(selectHandler.highlightMaterial))
                mats.Add(selectHandler.highlightMaterial);
            highlightObj.GetComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
        }
    }
    
    public void Deselect()
    {
        var selectHandler = _waveLogic.GetComponent<PointWave_SelectionHandler>();
        foreach (var highlightObj in highlightObjects)
        {
            var mats = highlightObj.GetComponent<MeshRenderer>().sharedMaterials.ToList();
            mats.Remove(selectHandler.highlightMaterial);
            highlightObj.GetComponent<MeshRenderer>().sharedMaterials = mats.ToArray();
        }
    }

    public void DeselectMe()
    {
        if (!_waveLogic) return;
        var handler = _waveLogic.GetComponent<PointWave_SelectionHandler>(); 
        handler.SelectObject(null);
    }
    
    public void PositionChanged()
    {
        if (!_waveLogic)
        {
            var obj = GameObject.Find("PoolHandler");
            if (obj)
                _waveLogic = obj.GetComponent<PointWavePoolHandler>();
        }

        var handler = _waveLogic.GetComponent<PointWave_SelectionHandler>();
        if(handler && handler.selectedObject == this)
            handler.PositionChanged();
    }
    
    private void OnDestroy()
    {
        if(!_waveLogic) return;
        var handler = _waveLogic.GetComponent<PointWave_SelectionHandler>();
        if (handler && handler.selectedObject == this)
            handler.SelectObject(null);
    }
}
