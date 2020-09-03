using UnityEngine;

public class scrMenu : MonoBehaviour
{
    // #################################################################################################################
    // Members

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Open/Close

    [SerializeField] private bool EnableEscKey = false;

    [SerializeField] private bool EnableOnStart = false;

    [SerializeField] private bool RemoveExtraColumnsOnClose = true;

    private bool IsOpen = false;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Game Object Links

    [SerializeField] private GameObject Canvas;

    [SerializeField] private Transform Columns;

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Prefab Links
    public GameObject MainColumn;
    
    // #################################################################################################################
    // Methods

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Setup

    private void Start()
    {
        // Hide menu
        this.Canvas.SetActive(false);

        // Delete columns
        while(this.Columns.transform.childCount > 0)
        {
            DestroyImmediate(this.Columns.GetChild(0).gameObject);
        }

        // Add main column of this menu (e.g. Pause Menu column)
        this.AddMenuColumn(MainColumn);

        // Open if selected
        if(this.EnableOnStart)
        {
            this.OpenMenu();
        }
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Open/Close Menu

    // TODO use keymap instead of this
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && this.EnableEscKey)
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

        if(this.RemoveExtraColumnsOnClose)
        {
            this.RemoveAllMenuColumnsButFirst();
        }
    }

    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // Columns

    public void AddMenuColumn(GameObject column)
    {
        GameObject new_column;
        new_column = Instantiate(column) as GameObject;
        new_column.transform.SetParent(this.Columns, false);
    }

    public void RemoveMenuColumn(GameObject column)
    {
        Destroy(column);
    }

    public void RemoveAllMenuColumnsButFirst()
    {
        while(this.Columns.transform.childCount > 1)
        {
            DestroyImmediate(this.Columns.GetChild(1).gameObject);
        }
    }
}
