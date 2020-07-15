using System;
using System.Collections.Generic;
using Localization;
using TMPro;
using UnityEngine;



public class PC_Dropdown : TMP_Dropdown
{
    // Items that will be visible when the dropdown is shown.
    // We box this into its own class so we can use a Property Drawer for it.
    [SerializeField]
    private LocalizedOptionDataList m_keys = new LocalizedOptionDataList();
    
    public List<LocalizedOptionData> keys
    {
        get => m_keys.optionKeys;
        set { m_keys.optionKeys = value;
            options.AddRange(value);
            RefreshShownValue(); }
    }
    
    
    
    [Serializable]
    public class LocalizedOptionData : OptionData
    {
        [SerializeField]
        private string m_Key;
        
        /// <summary>
        /// The text associated with the option.
        /// </summary>
        public string key
        {
            get => m_Key;
            set
            {
                m_Key = value;
                UpdateKey();
            } 
        }
       
        public LocalizedOptionData(string key, string text) : base(text)
        {
            this.key = key;
        }
        
        public LocalizedOptionData(string key, string text, Sprite image) : base(text, image)
        {
            this.key = key;
        }

        public void UpdateKey()
        {
            if (!LanguageManager.Instance) return;
            text = LanguageManager.Instance.GetString(key);
        }
    }

    [Serializable]
    /// <summary>
    /// Class used internally to store the list of options for the dropdown list.
    /// </summary>
    /// <remarks>
    /// The usage of this class is not exposed in the runtime API. It's only relevant for the PropertyDrawer drawing the list of options.
    /// </remarks>
    public class LocalizedOptionDataList : OptionDataList
    {
        [SerializeField]
        private List<LocalizedOptionData> m_OptionKeys;
        
        /// <summary>
        /// The list of options for the dropdown list.
        /// </summary>
        public List<LocalizedOptionData> optionKeys { get => m_OptionKeys;
            set => m_OptionKeys = value;
        }

        public LocalizedOptionDataList()
        {
            optionKeys = new List<LocalizedOptionData>();
        }
    }
    
    protected override void Start()
    {
        base.Start();

        foreach(var key in keys)
            key.UpdateKey();

        if (keys.Count > 0)
        {
            options.Clear();
            options.AddRange(keys);
        }
        
        RefreshShownValue();
    }
    
    
    public void AddOptions(List<LocalizedOptionData> options)
    {
        keys.AddRange(options);
        this.options.Clear();
        this.options.AddRange(keys);
        RefreshShownValue();
    }
    
}