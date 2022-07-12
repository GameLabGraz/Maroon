using System;
using System.Collections.Generic;
using System.Linq;
using Maroon.scenes.experiments.PerlinNoise.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Maroon.reusableGui.Experiment.Scripts.Runtime
{
    [Serializable]
    public class ButtonToggleEvent : UnityEvent<int, string>
    {
    }

    public class RadioButton : MonoBehaviour
    {
        public int selected_index { get; private set; }
        [SerializeField] private List<string> buttons;
        public ButtonToggleEvent OnSelect;


        private List<Button> button_objects = new List<Button>();
        private Button selected;

        // Start is called before the first frame update
        void Start()
        {
            Refresh();
            OnSelectButton(selected_index);
        }

        public void SetButtons(List<string> b)
        {
            buttons = b;
            Refresh();
        }

        private void OnSelectButton(int index)
        {
            Debug.Assert(button_objects.IsValidIndex(index),
                $"ButtonToggle: Selected button id {index} is not in buttons list");

            selected_index = index;
            if (selected)
                selected.interactable = true;

            selected = button_objects[selected_index];
            selected.interactable = false;

            OnSelect.Invoke(selected_index, buttons[selected_index]);
        }

        private void OnValidate()
        {
            if (PrefabModeIsActive() || !gameObject.activeInHierarchy)
                return;
            Refresh();
        }

        private void Refresh()
        {
            if (button_objects == null)
                button_objects = new List<Button>();
            if (button_objects.Count == 0) //should not happen
                button_objects.AddRange(transform.GetComponentsInChildren<Button>());

            var current_size = transform.childCount;
            var target_size = Math.Max(buttons.Count, 1); //we need to keep at least one button as a template

            if (button_objects.Count != current_size ||
                button_objects.Any(b => b == null))
            {
                Debug.LogWarning("RadioButton OnValidate: something went wrong, reset");
                button_objects.Clear();
                for (int i = 1; i < current_size; i++)
                {
                    var go = transform.GetChild(i).gameObject;
                    this.EndFrame(() => DestroyImmediate(go));
                }

                var template = transform.GetChild(0);
                template.gameObject.SetActive(true);
                button_objects.Add(GetComponentInChildren<Button>());
                current_size = 1;
            }

            button_objects[0].gameObject
                .SetActive(buttons.Count != 0); //we cant delete the last button so we disable it


            for (int i = current_size - 1; i >= target_size; i--)
            {
                //delete excess buttons
                var go = transform.GetChild(i).gameObject;
                this.EndFrame(() => DestroyImmediate(go));
                button_objects.RemoveAt(i);
            }

            var button_prefab = transform.GetChild(0);
            for (int i = current_size; i < target_size; i++)
            {
                //add missing buttons
                var go = Instantiate(button_prefab, transform);
                Debug.Assert(go);

                var button = go.GetComponent<Button>();
                Debug.Assert(button);
                button_objects.Add(button);
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                //set button text
                var text = button_objects[i].GetComponentInChildren<TextMeshProUGUI>();
                Debug.Assert(text);
                text.text = buttons[i];
                button_objects[i].name = "button_" + buttons[i];

                //set button listeners
                var button = button_objects[i];
                button.onClick.RemoveAllListeners();
                var index = i;
                button.onClick.AddListener(() => OnSelectButton(index));
                button.gameObject.SetActive(true);
            }

            selected_index = selected_index.Clamp(0, button_objects.Count - 1);
            selected = button_objects[selected_index];

            foreach (var button in button_objects)
                button.interactable = button != selected;
        }

        bool PrefabModeIsActive()
        {
#if UNITY_EDITOR
            return UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
#else
            return false;
#endif
        }
    }
}