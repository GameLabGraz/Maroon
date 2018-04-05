using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AchievementController : MonoBehaviour {

    public Text achievementText;
    public int number;
    private string id;
    private int controlNumber;
    private const int UISPACE = 50;
    // Use this for initialization
    void Start ()

    {
        DontDestroyOnLoad(this.gameObject);
        //Get text from attached object
        number = GamificationManager.instance.howMuchSpawnedAchievementUIs;
        GamificationManager.instance.howMuchSpawnedAchievementUIs++;
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + (number * UISPACE), this.gameObject.transform.position.z);


    }

    public void SetText(string s, string i)
    {
        achievementText.text = s;
        id = i;
    }


    public void ChangePosition()
    {
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y - UISPACE, this.gameObject.transform.position.z);

    }

    public string getID()
    {
        return id;
    }

    // Update is called once per frame
    void Update ()
    {
    }
}
