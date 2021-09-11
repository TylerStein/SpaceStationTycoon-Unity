using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SST.UI
{
    public class UINumberInputRow : MonoBehaviour
    {
        public int value = 0;
        public int minValue = int.MinValue;
        public int maxValue = int.MaxValue;
        public int step = 1;

        public Button decrementButton;
        public Button incrementButton;
        public InputField inputField;

        public UnityEvent<int> valueChanged;
        private void Awake() {
            incrementButton.onClick.AddListener(() => SetValue(value + step));
            decrementButton.onClick.AddListener(() => SetValue(value - step));
            inputField.onEndEdit.AddListener((fieldValue) => {
                int parsedValue;
                if (int.TryParse(fieldValue, out parsedValue)) {
                    SetValue(parsedValue, false);
                }
            });
        }

        public void SetValue(int updatedValue, bool updateText = true) {
            value = Mathf.Clamp(updatedValue, minValue, maxValue);
            string stringValue = value.ToString();
            if (inputField.text != stringValue) {
                inputField.SetTextWithoutNotify(stringValue);
            }

            valueChanged.Invoke(value);
        }
    }
}