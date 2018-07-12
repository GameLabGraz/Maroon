using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ViewingInfo : MonoBehaviour
{
    public Text viewingInfoText;
    public Image viewingInfoPanel;
    
	void Start () {
	
	}
	
	void Update ()
	{
	    viewingInfoText.text = Camera.main.transform.eulerAngles.ToString();
	}
}
