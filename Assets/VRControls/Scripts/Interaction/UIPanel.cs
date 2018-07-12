using UnityEngine;
using System.Collections;

public class UIPanel : MonoBehaviour
{
    public GameObject panel;
    protected bool isOpen = true;

    public virtual void open()
    {
        if (isOpen)
            return;

        isOpen = true;
        panel.SetActive(true);
    }

    public virtual void close()
    {
        if (!isOpen)
            return;

        isOpen = false;
        panel.SetActive(false);
    }
}
