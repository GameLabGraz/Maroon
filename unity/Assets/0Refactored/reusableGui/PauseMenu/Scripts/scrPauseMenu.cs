using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrPauseMenu : MonoBehaviour
{
    public static bool IsPaused = false;
    [SerializeField] private GameObject PauseMenuCanvas;

    // Start is called before the first frame update
    void Start()
    {
        this.PauseMenuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(scrPauseMenu.IsPaused)
            {
                this.ClosePauseMenu();
            }
            else
            {
                this.OpenPauseMenu();
            }
        }
    }

    private void OpenPauseMenu()
    {
        this.PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0;
        scrPauseMenu.IsPaused = true;
    }

    public void ClosePauseMenu()
    {
        this.PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1;
        scrPauseMenu.IsPaused = false;
    }
}
