using System.Collections.Generic;
using Localization;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class PC_LocalizedDropDown : MonoBehaviour
{
    [SerializeField]
    private List<string> _optionKeys = new List<string>();

    private void Start()
    {
        var dropdown = GetComponent<Dropdown>();
        dropdown.ClearOptions();

        foreach (var optionKey in _optionKeys)
        {
            dropdown.options.Add(new Dropdown.OptionData
            {
                text = LanguageManager.Instance.GetString(optionKey)
            });
        }

        dropdown.RefreshShownValue();
    }
}
