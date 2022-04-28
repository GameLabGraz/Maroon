using System;
using System.Collections.Generic;
using System.Linq;
using DigitalRuby.ThunderAndLightning;
using Maroon.scenes.experiments.PerlinNoise.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Maroon.reusableGui.Experiment.Scripts.Runtime
{
    [Serializable]
    public class ButtonToggleEvent : UnityEvent<int>{}

    public class RadioButton : MonoBehaviour
    {
        [SerializeField] private int selected_index;
        [SerializeField] private List<string> buttons;
        [SerializeField] private bool horizontal = true;
        public ButtonToggleEvent OnSelect;

        
        private readonly List<Button> button_objects = new List<Button>();
        private Button selected;
        // Start is called before the first frame update
        void Start()
        {
            OnSelectButton(button_objects[selected_index]);
        }

        private void OnSelectButton(Button button)
        {
            Debug.Assert(button_objects.Contains(button), $"ButtonToggle: Selected button {button} is not in buttons list");
            
            if (selected) 
                selected.interactable = true;

            selected = button;
            selected.interactable = false;
            for (selected_index = 0; selected_index < button_objects.Count; selected_index++)
            {
                if(button_objects[selected_index] == button)
                    break;
            }

            OnSelect.Invoke(selected_index);
        }

        private void OnValidate()
        {
            if(PrefabModeIsActive())
                return;
            
            button_objects.Clear();
            for (var i = 1; i < transform.childCount; i++)
            {
                var go = transform.GetChild(i).gameObject;
                this.EndFrame(() => DestroyImmediate(go));
            }

            var button_prefab = transform.GetChild(0);
            foreach (var button_text in buttons)
            {
                var go = Instantiate(button_prefab, transform);
                Debug.Assert(go);
                var c = go.transform.GetChild(0);
                var text = c.GetComponent<TextMeshProUGUI>();
                if(text)
                    text.text = button_text;
                
                var button = go.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnSelectButton(button));
                
                go.gameObject.SetActive(true);

                button_objects.Add(button);
            }
            button_prefab.gameObject.SetActive(false);

            selected_index = selected_index.Clamp(0, button_objects.Count - 1);
        }
        
        bool PrefabModeIsActive()
        {
            return UnityEditor.Experimental.SceneManagement.
                PrefabStageUtility.GetCurrentPrefabStage() != null;
        }
    }
}
