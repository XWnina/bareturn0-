using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace JumpGame
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class ConditionDropdownInitializer : MonoBehaviour
    {
        private TMP_Dropdown _dropdown;

        private static readonly List<string> Conditions = new List<string>
        {
            "Condition",
            "platform up",
            "obstacle ahead",
            "always true",
            "is grounded"
            
        };

        void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();

            if (_dropdown == null)
            {
                Debug.LogError("❗找不到 TMP_Dropdown 组件！");
                return;
            }

            _dropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

            foreach (string option in Conditions)
            {
                newOptions.Add(new TMP_Dropdown.OptionData(option));
            }

            _dropdown.AddOptions(newOptions);
            _dropdown.value = 0;
            _dropdown.RefreshShownValue();
        }
    }
}