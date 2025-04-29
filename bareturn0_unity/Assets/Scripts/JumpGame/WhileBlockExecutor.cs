using System.Collections;
using UnityEngine;
using TMPro;

namespace JumpGame
{
    public class WhileBlockExecutor : MonoBehaviour
    {
        public TMP_Dropdown conditionDropdown;
        public TMP_Dropdown trueDropdown;
        public Transform trueContainer;

        public IEnumerator Execute(PlayerController player)
        {
            string condition = conditionDropdown.options[conditionDropdown.value].text;
            string action = trueDropdown.options[trueDropdown.value].text;

            int maxIterations = 50; // 防止死循环
            int iteration = 0;

            while (iteration++ < maxIterations)
            {
                bool conditionMet = false;
                yield return player.EvaluateConditionAsync(condition, result => conditionMet = result);
                Debug.Log($"🔁 While 条件：{condition} = {conditionMet}");

                if (!conditionMet)
                {
                    Debug.Log("🛑 条件不再满足，退出 while");
                    break;
                }

                if (!string.IsNullOrEmpty(action))
                    yield return player.ExecuteActionAsync(action); // 比如 walk

                foreach (Transform child in trueContainer)
                {
                    IfBlockExecutor nested = child.GetComponent<IfBlockExecutor>();
                    if (nested != null)
                        yield return nested.Execute(player);
                }

                yield return null; // 每次循环小等待
            }
        }
    }
}