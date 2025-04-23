using UnityEngine;
using TMPro;

namespace JumpGame
{
    public class IfBlockExecutor : MonoBehaviour
    {
        public TMP_Dropdown conditionDropdown;
        public TMP_Dropdown trueDropdown;
        public GameObject elseContainer;
        public TMP_Dropdown elseDropdown;

        public void Execute(PlayerController player)
        {
            string condition = conditionDropdown.options[conditionDropdown.value].text;
            string trueAction = trueDropdown.options[trueDropdown.value].text;
            string elseAction = (elseContainer.activeSelf && elseDropdown != null)
                ? elseDropdown.options[elseDropdown.value].text
                : null;

            bool conditionMet = EvaluateCondition(player, condition);
            Debug.Log($"▶ 判断：{condition} = {conditionMet}");

            if (conditionMet)
            {
                ExecuteAction(player, trueAction);
            }
            else if (!string.IsNullOrEmpty(elseAction))
            {
                ExecuteAction(player, elseAction);
            }
        }

        private bool EvaluateCondition(PlayerController player, string condition)
        {
            switch (condition)
            {
                case "rock ahead": return player.IsRockAhead();
                case "platform up": return player.IsPlatformAbove();
                case "always true": return true;
                default: return false;
            }
        }

        private void ExecuteAction(PlayerController player, string action)
        {
            switch (action)
            {
                case "jump":
                    Debug.Log("🟢 jump()");
                    player.Jump();
                    break;
                case "say hello":
                    Debug.Log("🟢 say hello");
                    break;
                case "do nothing":
                    Debug.Log("🟡 do nothing");
                    break;
                default:
                    Debug.LogWarning($"⚠ 未知动作：{action}");
                    break;
            }
        }
    }
}
