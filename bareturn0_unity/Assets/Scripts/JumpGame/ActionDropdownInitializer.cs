using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace JumpGame
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class ActionDropdownInitializer : MonoBehaviour
    {
        private TMP_Dropdown _dropdown;

        private static readonly List<string> Actions = new List<string>
        {
            "Action",
            "jump",
            "walk",
            "do nothing"
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

            foreach (string option in Actions)
            {
                newOptions.Add(new TMP_Dropdown.OptionData(option));
            }

            _dropdown.AddOptions(newOptions);
            _dropdown.value = 0;
            _dropdown.RefreshShownValue();
        }
    }
}