using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class EnterButton : VRTK_InteractableObject
{
    float spinSpeed = 0f;
    Transform rotator;
    public string LevelName; //= "VanDeGraaffExperiment2";

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);
        spinSpeed = 360f;

        Debug.Log("ENTER BUTTON pressed, will load " + this.LevelName);
        SceneManager.LoadScene(this.LevelName);
    }

    public override void StopUsing(GameObject usingObject)
    {
        base.StopUsing(usingObject);
        spinSpeed = 0f;
    }

    protected override void Start()
    {
        base.Start();
        rotator = transform.Find("Capsule");
    }

    protected override void Update()
    {
        rotator.transform.Rotate(new Vector3(spinSpeed * Time.deltaTime, 0f, 0f));
    }

    //helper fct, unused for now
    private void loadLevel(string name)
    {
        Debug.Log("ENTER BUTTON pressed, will load " + name);
        SceneManager.LoadScene(name);
    }
}
