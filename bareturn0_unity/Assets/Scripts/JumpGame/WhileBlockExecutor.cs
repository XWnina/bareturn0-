using System.Collections;
using UnityEngine;
using TMPro;

namespace JumpGame
{
    public class WhileBlockExecutor : MonoBehaviour
    {
        public TMP_Dropdown conditionDropdown;
        public TMP_Dropdown trueDropdown;
        public Transform ifBlockContainer; // ✅ 存放嵌套的 IfBlock

        public IEnumerator Execute(PlayerController player)
        {
            string condition = conditionDropdown.options[conditionDropdown.value].text;
            string action = trueDropdown.options[trueDropdown.value].text;

            int maxIterations = 50;
            int iteration = 0;

            while (iteration++ < maxIterations && !player.interrupted)
            {
                bool conditionMet = false;
                yield return player.EvaluateConditionAsync(condition, result => conditionMet = result);
                Debug.Log($"🔁 While 条件：{condition} = {conditionMet}");

                if (!conditionMet || player.reachedGoal)
                {
                    Debug.Log("🛑 条件不再满足，退出 while");
                    break;
                }

                // ✅ 先执行嵌套 ifBlock
                foreach (Transform child in ifBlockContainer)
                {
                    IfBlockExecutor nested = child.GetComponent<IfBlockExecutor>();
                    if (nested != null)
                        yield return StartCoroutine(nested.Execute(player));
                }

                // ✅ 最后再执行 dropdown 动作（例如 walk）
                if (!string.IsNullOrEmpty(action))
                    yield return player.ExecuteActionAsync(action);

                yield return null;
            }
        }
    }
}