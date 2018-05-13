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
    private string dialogeKey;
    // Use this for initialization
    void Start ()

    {
        DontDestroyOnLoad(this.gameObject);
        //Get text from attached object
        number = GamificationManager.instance.HowMuchSpawnedAchievementUIs;
        GamificationManager.instance.HowMuchSpawnedAchievementUIs++;
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + (number * UISPACE), this.gameObject.transform.position.z);
    }

    //Update text in case of language switch
    public void SetText()
    {
        achievementText.text = GamificationManager.instance.l_manager.GetString(dialogeKey);
    }

    public void setDialogue(string d, string i)
    {
        dialogeKey = d;
        id = i;
    }

    public string getDialogueKey()
    {
        return dialogeKey;
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
        SetText();
    }
}
