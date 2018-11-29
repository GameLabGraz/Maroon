using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class EnterButton : VRTK_InteractableObject
{
    //Transform rotator;
    public string LevelName; //= "VanDeGraaffExperiment2";

    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        base.StartUsing(usingObject);

        Debug.Log("ENTER BUTTON pressed, will load " + this.LevelName);
        SceneManager.LoadScene(this.LevelName);
    }

    private void Start()
    {
        //rotator = transform.Find("Capsule");
    }

    protected override void Update()
    {
        //rotator.transform.Rotate(new Vector3(spinSpeed * Time.deltaTime, 0f, 0f));
    }

    //helper fct, unused for now
    private void loadLevel(string name)
    {
        Debug.Log("ENTER BUTTON pressed, will load " + name);
        SceneManager.LoadScene(name);
    }
}
