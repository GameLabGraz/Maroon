using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class scrMenu : MonoBehaviour
{
    // #################################################################################################################
    // Members
    [SerializeField] private bool OpenOnEsc = false;

    private bool IsOpen = false;

    [SerializeField] private GameObject Canvas;

    private Transform Columns;

    public GameObject MainColumn;
    
    // #################################################################################################################
    // Methods

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Setup

    private void Start()
    {
        // Hide menu
        this.Canvas.SetActive(false);

        // Init Columns
        this.Columns = Canvas.transform.Find("Columns");

        // Delete columns
        while(this.Columns.childCount > 0)
        {
            DestroyImmediate(this.Columns.GetChild(0).gameObject);
        }

        // Add main column of this menu (e.g. Pause Menu column)
        this.AddMenuColumn(MainColumn);
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Open/Close Menu

    // TODO use keymap instead of this
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(this.IsOpen)
            {   
                this.CloseMenu();
            }
            else
            {
                this.OpenMenu();
            }
        }
    }

    public void OpenMenu()
    {
        this.Canvas.SetActive(true);
        this.IsOpen = true;
    }

    public void CloseMenu()
    {
        this.Canvas.SetActive(false);
        this.IsOpen = false;    
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Columns

    public void AddMenuColumn(GameObject column)
    {
        GameObject new_column;
        new_column = Instantiate(column) as GameObject;
        new_column.transform.parent = this.Columns; 
    }

    public void RemoveMenuColumn(GameObject column)
    {
        Destroy(column);
    }
}
