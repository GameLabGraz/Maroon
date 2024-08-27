using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Maroon.Utils;
using UnityEditor;

namespace Maroon.reusableGui.Experiment.Scripts.Runtime
{
    [Serializable]
    public class ButtonToggleEvent : UnityEvent<int, string>
    {
    }

    public class RadioButton : MonoBehaviour
    {
        public int SelectedIndex { get; private set; }
        [SerializeField] private List<string> buttons;
        public ButtonToggleEvent onSelect;


        private List<Button> _button_objects = new List<Button>();
        private Button _selected;

        // Start is called before the first frame update
        private void Start()
        {
            Refresh();
            OnSelectButton(SelectedIndex);
        }

        public void SetButtons(List<string> b)
        {
            buttons = b;
            Refresh();
        }

        private void OnSelectButton(int index)
        {
            Debug.Assert(_button_objects.IsValidIndex(index),
                $"ButtonToggle: Selected button id {index} is not in buttons list");

            SelectedIndex = index;
            if (_selected)
                _selected.interactable = true;

            _selected = _button_objects[SelectedIndex];
            _selected.interactable = false;

            onSelect.Invoke(SelectedIndex, buttons[SelectedIndex]);
        }

        public bool refreshButtons;

        private void OnValidate()
        {
            if (refreshButtons)
            {
                Refresh();
                refreshButtons = false;
            }
        }

        private void Refresh()
        {
            if (_button_objects == null)
                _button_objects = new List<Button>();
            if (_button_objects.Count == 0) //should not happen
                _button_objects.AddRange(transform.GetComponentsInChildren<Button>());

            var currentSize = transform.childCount;
            var targetSize = Math.Max(buttons.Count, 1); //we need to keep at least one button as a template

            if (_button_objects.Count != currentSize ||
                _button_objects.Any(b => b == null))
            {
                Debug.LogWarning("RadioButton OnValidate: something went wrong, reset");
                foreach (var buttonObject in _button_objects.Skip(1))
                {
                    EditorApplication.delayCall += () => DestroyImmediate(buttonObject.gameObject);
                }
                _button_objects.Clear();

                var template = transform.GetChild(0);
                template.gameObject.SetActive(true);
                _button_objects.Add(GetComponentInChildren<Button>());
                currentSize = 1;
            }

            _button_objects[0].gameObject
                .SetActive(buttons.Count != 0); //we cant delete the last button so we disable it


            for (int i = currentSize - 1; i >= targetSize; i--)
            {
                //delete excess buttons
                var go = transform.GetChild(i).gameObject;
                EditorApplication.delayCall += () => DestroyImmediate(go);
                _button_objects.RemoveAt(i);
            }

            var buttonPrefab = transform.GetChild(0);
            for (int i = currentSize; i < targetSize; i++)
            {
                //add missing buttons
                var go = Instantiate(buttonPrefab, transform);
                Debug.Assert(go);

                var button = go.GetComponent<Button>();
                Debug.Assert(button);
                _button_objects.Add(button);
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                //set button text
                var text = _button_objects[i].GetComponentInChildren<TextMeshProUGUI>();
                Debug.Assert(text);
                text.text = buttons[i];
                _button_objects[i].name = "button_" + buttons[i];

                //set button listeners
                var button = _button_objects[i];
                button.onClick.RemoveAllListeners();
                var index = i;
                button.onClick.AddListener(() => OnSelectButton(index));
                button.gameObject.SetActive(true);
            }

            SelectedIndex = Mathf.Clamp(SelectedIndex, 0, _button_objects.Count - 1);
            _selected = _button_objects[SelectedIndex];

            foreach (var button in _button_objects)
                button.interactable = button != _selected;
        }
    }
}