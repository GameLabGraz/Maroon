using System.Collections.Generic;
using Maroon.Physics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.UI
{
    [ExecuteInEditMode]
    public class QuantityPropertyView : MonoBehaviour
    {
        [SerializeField] private GameObject TextViewPrefab;
        [SerializeField] private GameObject InputViewPrefab;
        [SerializeField] private GameObject sliderViewPrefab;
        [SerializeField] private GameObject toggleViewPrefab;
        [SerializeField] private GameObject vectorViewPrefab;
        [SerializeField] private GameObject containerPrefab;

        [SerializeField] private List<QuantityReferenceValue> quantities = new List<QuantityReferenceValue>();

        private void OnEnable()
        {
            LoadUI();
        }

        private void OnDisable()
        {
            ClearUI();
        }

        private void ClearUI()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        private void LoadUI()
        {
            ClearUI();

            foreach (var quantity in quantities)
            {
                var container = Instantiate(containerPrefab, transform);
                var layout = container.AddComponent<HorizontalLayoutGroup>();
                layout.childControlHeight = layout.childControlWidth = true;
                layout.spacing = 5;

                var label = Instantiate(TextViewPrefab, container.transform);
                label.GetComponent<TextMeshProUGUI>().text = quantity.Value.GetName();

                switch (quantity.Value)
                {
                    case QuantityInt intQuantity:
                        var sliderView = Instantiate(sliderViewPrefab, container.transform);
                        var slider = sliderView.GetComponentInChildren<Slider>();
                        slider.minValue = intQuantity.minValue;
                        slider.maxValue = intQuantity.maxValue;
                        slider.wholeNumbers = true;

                        slider.onValueChanged.AddListener(value =>
                        {
                            intQuantity.Value = (int)value;
                        });

                        break;
                    case QuantityFloat floatQuantity:
                        break;
                    case QuantityVector3 vectorQuantity:
                        break;
                    case QuantityBool boolQuantity:
                        break;
                    case QuantityString stringQuantity:
                        break;
                    default:
                        Debug.LogWarning("QuantityPropertyView::LoadUI:Unsupported Quantity Type!");
                        return;
                }



            }
        }
    }
}
